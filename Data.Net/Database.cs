using System;
using System.Collections.Generic;
using System.Data;

namespace Data.Net
{
    /// <inheritdoc />
    /// <summary>
    ///     Generic database class for handling all db operations for any ADO.Net provider.
    /// </summary>
    public sealed class Database : IDisposable
    {
        private readonly IDbConnection _connection;

        private readonly DbDataProvider _dbDataProvider;

        private DataParameterBuilder _dataParameterBuilder;

        private IDbTransaction _transaction;

        private IsolationLevel _isolationLevel = IsolationLevel.Unspecified;

        private bool _isTransaction;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Database" /> class.
        /// </summary>
        /// <param name="connection">Instance of <see cref="IDbConnection" /></param>
        public Database(IDbConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(_connection));
            _dbDataProvider = DbDataProviderFactory.GetDbDataProvider(_connection.GetType().FullName);
        }

#if NET461 || NET462 || NET47
        
        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Data.Net.Database" /> class, ensure you have default connection string in the config file. 
        /// </summary>
        public Database() : this(DbConnectionFactory.Connection) {}

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Data.Net.Database" /> class.
        /// </summary>
        /// <param name="connectionStringName">The connection string name from web/app config</param>
        public Database(string connectionStringName) : this(DbConnectionFactory.OpenConnection(connectionStringName ?? throw new ArgumentNullException(nameof(connectionStringName)))) {}

#endif

        /// <inheritdoc />
        /// <summary>
        ///     Dispose the database class
        /// </summary>
        public void Dispose()
        {
            _transaction?.Dispose();
            _transaction = null;
            _connection?.Dispose();
        }

        /// <summary>
        ///     Executes a Transact-SQL statement and returns the number of rows affected by an insert,update or delete.
        /// </summary>
        /// <param name="sql">Transact-SQL statement</param>
        /// <param name="commandType">One of the command type of <see cref="CommandType" /></param>
        /// <param name="parameters">Parameter collection of <see cref="DataParameters" /> class.</param>
        /// <returns></returns>
        public int ExecuteNonQuery(string sql, CommandType commandType = CommandType.Text, DataParameters parameters = null) =>
            ExecuteQuery<int>(sql, false, commandType, parameters);

        /// <summary>
        ///     Executes a Transact-SQL statement and returns a single value from the first row/column.
        /// </summary>
        /// <typeparam name="T">The return value type</typeparam>
        /// <param name="sql">Transact-SQL statement</param>
        /// <param name="commandType">One of the command type of <see cref="CommandType" /></param>
        /// <param name="parameters">Parameter collection of <see cref="DataParameters" /> class.</param>
        /// <returns></returns>
        public T ExecuteScalar<T>(string sql, CommandType commandType = CommandType.Text, DataParameters parameters = null) =>
            ExecuteQuery<T>(sql, true, commandType, parameters);

        /// <summary>
        ///     Execute a Transact-SQL statement and return a collection of <see cref="IDataReader" />
        /// </summary>
        /// <param name="sql">Transact-SQL statement</param>
        /// <param name="commandType">One of the command type of <see cref="CommandType" /></param>
        /// <param name="parameters">Parameter collection of <see cref="DataParameters" /> class.</param>
        /// <param name="behavior">One of the command behavior <see cref="CommandBehavior" /></param>
        /// <returns></returns>
        public IDataReader ExecuteReader(string sql, CommandType commandType = CommandType.Text,
            DataParameters parameters = null, CommandBehavior behavior = CommandBehavior.CloseConnection)
        {
            using (var cmd = CreateCommand(sql, parameters, commandType))
            {
                var reader = cmd.ExecuteReader(behavior);
                _dataParameterBuilder.Update(cmd);
                return reader;
            }
        }

        /// <summary>
        ///     Execute a Transact-SQL statement and return a collection of <see cref="List{T}" />
        /// </summary>
        /// <typeparam name="T">The return object type</typeparam>
        /// <param name="sql">Transact-SQL statement</param>
        /// <param name="commandType">One of the command type of <see cref="CommandType" /></param>
        /// <param name="parameters">Parameter collection of <see cref="DataParameters" /> class.</param>
        /// <param name="behavior">One of the command behavior <see cref="CommandBehavior" /></param>
        /// <returns></returns>
        public List<T> Query<T>(string sql, CommandType commandType = CommandType.Text, DataParameters parameters = null,
            CommandBehavior behavior = CommandBehavior.CloseConnection)
        {
            var list = default(List<T>);

            using (var cmd = CreateCommand(sql, parameters, commandType))
            {
                using (var reader = cmd.ExecuteReader(behavior))
                {
                    var row = new DataRowReader();

                    while (reader.Read())
                    {
                        if (list == default(List<T>)) list = new List<T>();

                        var obj = row.ReaderToType<T>(reader);
                        if (obj == null) continue;
                        list.Add(obj);
                    }
                    row.Flush();
                }
                _dataParameterBuilder.Update(cmd);
            }
            return list?.Count == 0 ? null : list;
        }

        /// <summary>
        ///     Execute a Transact-SQL statement and return a single object
        /// </summary>
        /// <typeparam name="T">The return object type</typeparam>
        /// <param name="sql">Transact-SQL statement</param>
        /// <param name="commandType">One of the command type of <see cref="CommandType" /></param>
        /// <param name="parameters">Parameter collection of <see cref="DataParameters" /> class.</param>
        /// <param name="behavior">One of the command behavior <see cref="CommandBehavior" /></param>
        /// <returns></returns>
        public T QuerySingle<T>(string sql, CommandType commandType = CommandType.Text, DataParameters parameters = null,
            CommandBehavior behavior = CommandBehavior.CloseConnection)
        {
            var result = default(T);
            using (var cmd = CreateCommand(sql, parameters, commandType))
            {
                using (var reader = cmd.ExecuteReader(behavior))
                {
                    var row = new DataRowReader();
                    if (reader.Read())
                    {
                        result = row.ReaderToType<T>(reader);
                        row.Flush();
                    }
                }
                _dataParameterBuilder.Update(cmd);
            }
            return result;
        }

        /// <summary>
        ///     Commit a transaction
        /// </summary>
        public void CommitTransaction() => _transaction?.Commit();

        /// <summary>
        ///     Rollback a transaction
        /// </summary>
        public void RollbackTransaction() => _transaction?.Rollback();

        /// <summary>
        ///     Start a transaction
        /// </summary>
        public void BeginTransaction() => BeginTransaction(_isolationLevel);

        /// <summary>
        ///     Start a transaction with isolation level
        /// </summary>
        /// <param name="isolationLevel">One of the isolation level <see cref="IsolationLevel" /></param>
        public void BeginTransaction(IsolationLevel isolationLevel)
        {
            _isTransaction = true;
            _isolationLevel = isolationLevel;
        }

        private T ExecuteQuery<T>(string sql, bool isScalar, CommandType commandType = CommandType.Text,
            DataParameters parameters = null)
        {
            using (var cmd = CreateCommand(sql, parameters, commandType))
            {
                var result = isScalar ? cmd.ExecuteScalar() : cmd.ExecuteNonQuery();
                _dataParameterBuilder.Update(cmd);
                return result.ToValue<T>();
            }
        }

        private IDbCommand CreateCommand(string sql, DataParameters parameters = null,
            CommandType commandType = CommandType.Text)
        {
            var cmd = _connection.CreateCommand();
            cmd.CommandText = sql ?? throw new ArgumentNullException(nameof(sql));
            cmd.CommandType = commandType;
            cmd.Connection = _connection;

            if (_dbDataProvider is OracleDataProvider)
            {
                var bindByName = cmd.GetType().GetProperty("BindByName");
                bindByName?.SetValue(cmd, true, null);
            }

            _dataParameterBuilder = new DataParameterBuilder(cmd, parameters);

            if (_connection.State != ConnectionState.Open) _connection.Open();

            CreateTransaction(cmd);

            return cmd;
        }

        private void CreateTransaction(IDbCommand cmd)
        {
            if (!_isTransaction || _connection.State != ConnectionState.Open) return;

            if (_transaction == null)
                _transaction = _isolationLevel == IsolationLevel.Unspecified
                    ? _connection.BeginTransaction()
                    : _connection.BeginTransaction(_isolationLevel);

            cmd.Transaction = _transaction;
        }
    }   
}