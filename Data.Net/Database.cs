using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Data.Net
{
    /// <inheritdoc />
    /// <summary>
    /// Generic database class for handling all db operations for any ADO.Net provider.
    /// </summary>
    public sealed class Database : IDisposable
    {
        private readonly IDbConnection _connection;

        private readonly IDbTransaction _transaction;

        private readonly DbProvider _dbProvider;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Database" /> class.
        /// </summary>
        /// <param name="connection">Instance of <see cref="IDbConnection" /></param>
        public Database(IDbConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(_connection));
            _dbProvider = DbProviderFactory.GetDbProvider(_connection.GetType().Name);
        }

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Data.Net.Database" /> class with Ado.Net Transaction object <see cref="T:System.Data.IDbTransaction" />.
        /// </summary>
        /// <param name="connection">Instance of <see cref="T:System.Data.IDbConnection" /></param>
        /// <param name="useTransaction">true, for creating db transaction</param>
        /// <param name="isolationLevel">Specify one of the isolation level <seealso cref="T:System.Data.IsolationLevel" /> </param>
        public Database(IDbConnection connection, bool useTransaction, IsolationLevel isolationLevel = IsolationLevel.Unspecified) : this(connection)
        {
            if (!useTransaction) return;

            if (_connection.State != ConnectionState.Open) _connection.Open();

            _transaction = _connection.BeginTransaction(isolationLevel);
        }

#if NET461 || NET462 || NET47 || NET471 || NET472 || NET48
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

        /// <summary>
        /// Executes a Transact-SQL statement and returns the number of rows affected by an insert,update or delete.
        /// </summary>
        /// <param name="sql">Transact-SQL statement</param>
        /// <param name="commandType">One of the command type of <see cref="CommandType" /></param>
        /// <param name="parameters">Parameter collection of <see cref="DataParameters" /> class.</param>
        /// <returns></returns>
        public int ExecuteNonQuery(string sql, CommandType commandType = CommandType.Text, object parameters = null)
        {
            using var builder = new CommandBuilder(sql, _connection, _transaction, _dbProvider, parameters.ToDataParameters(_dbProvider.ParameterDelimiter), commandType);
            
            var result = builder.Command.ExecuteNonQuery();
            
            return result.ToValue<int>();
        }

        /// <summary>
        /// Asynchronous version of Executes a Transact-SQL statement and returns the number of rows affected by an insert,update or delete.
        /// </summary>
        /// <param name="sql">Transact-SQL statement</param>
        /// <param name="commandType">One of the command type of <see cref="CommandType" /></param>
        /// <param name="parameters">Parameter collection of <see cref="DataParameters" /> class.</param>
        /// <param name="token">Cancellation token <see cref="CancellationToken" /></param>
        /// <returns></returns>
        public async Task<int> ExecuteNonQueryAsync(string sql, CommandType commandType = CommandType.Text, object parameters = null, CancellationToken token = default)
        {
            using var builder = new CommandBuilder(sql, _connection, _dbProvider, parameters.ToDataParameters(_dbProvider.ParameterDelimiter), commandType);
            
            if (builder.Command.Connection.State != ConnectionState.Open) await builder.OpenAsync(token);

            if (_transaction != null) builder.Command.Transaction = _transaction as DbTransaction;

            var result = await builder.Command.ExecuteNonQueryAsync(token);

            return result.ToValue<int>();
        }

        /// <summary>
        /// Executes a Transact-SQL statement and returns a single value from the first row/column.
        /// </summary>
        /// <typeparam name="T">The return value type</typeparam>
        /// <param name="sql">Transact-SQL statement</param>
        /// <param name="commandType">One of the command type of <see cref="CommandType" /></param>
        /// <param name="parameters">Parameter collection of <see cref="DataParameters" /> class.</param>
        /// <returns></returns>
        public T ExecuteScalar<T>(string sql, CommandType commandType = CommandType.Text, object parameters = null)
        {
            var result = ExecuteScalar(sql, commandType, parameters.ToDataParameters(_dbProvider.ParameterDelimiter));

            return result.ToValue<T>();
        }

        /// <summary>
        /// Executes a Transact-SQL statement and returns a single value from the first row/column.
        /// </summary>
        /// <param name="sql">Transact-SQL statement</param>
        /// <param name="commandType">One of the command type of <see cref="CommandType" /></param>
        /// <param name="parameters">Parameter collection of <see cref="DataParameters" /> class.</param>
        /// <returns></returns>
        public object ExecuteScalar(string sql, CommandType commandType = CommandType.Text, object parameters = null)
        {
            using var builder = new CommandBuilder(sql, _connection, _transaction, _dbProvider, parameters.ToDataParameters(_dbProvider.ParameterDelimiter), commandType);

            return builder.Command.ExecuteScalar();
        }

        /// <summary>
        /// Asynchronous version of Executes a Transact-SQL statement and returns a single value from the first row/column.
        /// </summary>
        /// <typeparam name="T">The return value type</typeparam>
        /// <param name="sql">Transact-SQL statement</param>
        /// <param name="commandType">One of the command type of <see cref="CommandType" /></param>
        /// <param name="parameters">Parameter collection of <see cref="DataParameters" /> class.</param>
        /// <param name="token">Cancellation token <see cref="CancellationToken" /></param>
        /// <returns></returns>
        public async Task<T> ExecuteScalarAsync<T>(string sql, CommandType commandType = CommandType.Text, object parameters = null, CancellationToken token = default)
        {
            using var builder = new CommandBuilder(sql, _connection, _dbProvider, parameters.ToDataParameters(_dbProvider.ParameterDelimiter), commandType);

            if (builder.Command.Connection.State != ConnectionState.Open) await builder.OpenAsync(token);

            if (_transaction != null) builder.Command.Transaction = _transaction as DbTransaction;

            var result = await builder.Command.ExecuteScalarAsync(token);

            return result.ToValue<T>();
        }

        /// <summary>
        /// Execute a Transact-SQL statement and return a collection of <see cref="IDataReader" />
        /// </summary>
        /// <param name="sql">Transact-SQL statement</param>
        /// <param name="commandType">One of the command type of <see cref="CommandType" /></param>
        /// <param name="parameters">Parameter collection of <see cref="DataParameters" /> class.</param>
        /// <param name="behavior">One of the command behavior <see cref="CommandBehavior" /></param>
        /// <returns></returns>
        public IDataReader ExecuteReader(string sql, CommandType commandType = CommandType.Text,
            object parameters = null, CommandBehavior behavior = CommandBehavior.CloseConnection)
        {
            using var builder = new CommandBuilder(sql, _connection, _transaction, _dbProvider, parameters.ToDataParameters(_dbProvider.ParameterDelimiter), commandType);

            return builder.Command.ExecuteReader(behavior);
        }

        /// <summary>
        /// Asynchronous version of Execute a Transact-SQL statement and return a collection of <see cref="IDataReader" />
        /// </summary>
        /// <param name="sql">Transact-SQL statement</param>
        /// <param name="commandType">One of the command type of <see cref="CommandType" /></param>
        /// <param name="parameters">Parameter collection of <see cref="DataParameters" /> class.</param>
        /// <param name="behavior">One of the command behavior <see cref="CommandBehavior" /></param>
        /// <param name="token">Cancellation token <see cref="CancellationToken" /></param>
        /// <returns></returns>
        public async Task<IDataReader> ExecuteReaderAsync(string sql, CommandType commandType = CommandType.Text,
            object parameters = null, CommandBehavior behavior = CommandBehavior.CloseConnection, CancellationToken token = default)
        {
            using var builder = new CommandBuilder(sql, _connection, _dbProvider, parameters.ToDataParameters(_dbProvider.ParameterDelimiter), commandType);

            if (builder.Command.Connection.State != ConnectionState.Open) await builder.OpenAsync(token);

            if (_transaction != null) builder.Command.Transaction = _transaction as DbTransaction;

            var reader = await builder.Command.ExecuteReaderAsync(behavior, token);

            return reader;
        }

        /// <summary>
        /// Execute a Transact-SQL statement and return a collection of <see cref="List{T}" />
        /// </summary>
        /// <typeparam name="T">The return object type</typeparam>
        /// <param name="sql">Transact-SQL statement</param>
        /// <param name="commandType">One of the command type of <see cref="CommandType" /></param>
        /// <param name="parameters">Parameter collection of <see cref="DataParameters" /> class.</param>
        /// <param name="behavior">One of the command behavior <see cref="CommandBehavior" /></param>
        /// <returns></returns>
        public List<T> Query<T>(string sql, CommandType commandType = CommandType.Text, object parameters = null,
            CommandBehavior behavior = CommandBehavior.CloseConnection)
        {
            var list = default(List<T>);

            using var builder = new CommandBuilder(sql, _connection, _transaction, _dbProvider, parameters.ToDataParameters(_dbProvider.ParameterDelimiter), commandType);
            
            using var reader = builder.Command.ExecuteReader(behavior);
            
            var row = new DataRowReader<T>(reader.FieldCount);

            while (reader.Read())
            {
                var obj = row.ReaderToType(reader);

                if (obj == null) continue;

                list ??= new List<T>();

                list.Add(obj);
            }
            
            row.Clear();
            
            return list;
        }

        /// <summary>
        /// Execute a Transact-SQL statement and return a single object
        /// </summary>
        /// <typeparam name="T">The return object type</typeparam>
        /// <param name="sql">Transact-SQL statement</param>
        /// <param name="commandType">One of the command type of <see cref="CommandType" /></param>
        /// <param name="parameters">Parameter collection of <see cref="DataParameters" /> class.</param>
        /// <param name="behavior">One of the command behavior <see cref="CommandBehavior" /></param>
        /// <returns></returns>
        public T QuerySingle<T>(string sql, CommandType commandType = CommandType.Text, object parameters = null,
            CommandBehavior behavior = CommandBehavior.CloseConnection)
        {
            var result = default(T);

            using var builder = new CommandBuilder(sql, _connection, _transaction, _dbProvider, parameters.ToDataParameters(_dbProvider.ParameterDelimiter), commandType);
            
            using var reader = builder.Command.ExecuteReader(behavior);
            
            if (!reader.Read()) return result;
            
            if (typeof(T).IsValueType || typeof(T) == typeof(string))
                return reader.IsDBNull(0) ? default : reader.GetValue(0).ToValue<T>();

            var row = new DataRowReader<T>(reader.FieldCount);

            result = row.ReaderToType(reader);

            row.Clear();

            return result;
        }

        /// <summary>
        /// Asynchronous version of Execute a Transact-SQL statement and return a single object
        /// </summary>
        /// <typeparam name="T">The return object type</typeparam>
        /// <param name="sql">Transact-SQL statement</param>
        /// <param name="commandType">One of the command type of <see cref="CommandType" /></param>
        /// <param name="parameters">Parameter collection of <see cref="DataParameters" /> class.</param>
        /// <param name="behavior">One of the command behavior <see cref="CommandBehavior" /></param>
        /// <param name="token">Cancellation token <see cref="CancellationToken" /></param>
        /// <returns></returns>
        public async Task<T> QuerySingleAsync<T>(string sql, CommandType commandType = CommandType.Text, object parameters = null,
            CommandBehavior behavior = CommandBehavior.CloseConnection, CancellationToken token = default)
        {
            var result = default(T);

            using var builder = new CommandBuilder(sql, _connection, _dbProvider, parameters.ToDataParameters(_dbProvider.ParameterDelimiter), commandType);
            
            if (builder.Command.Connection.State != ConnectionState.Open) await builder.OpenAsync(token);

            if (_transaction != null) builder.Command.Transaction = _transaction as DbTransaction;

            using var reader = await builder.Command.ExecuteReaderAsync(behavior, token);

            if (!await reader.ReadAsync(token)) return result;
                
            if (typeof(T).IsValueType || typeof(T) == typeof(string))
                return await reader.IsDBNullAsync(0, token) ? default : reader.GetValue(0).ToValue<T>();

            var row = new DataRowReader<T>(reader.FieldCount);

            result = row.ReaderToType(reader);

            row.Clear();

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public TEntity Insert<TEntity>(TEntity entity) where TEntity : class => _dbProvider.Insert(entity, this);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public TEntity Update<TEntity>(TEntity entity) where TEntity : class => _dbProvider.Update(entity, this);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Delete<TEntity>(TEntity entity) where TEntity : class => _dbProvider.Delete(entity, this);
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public TEntity Get<TEntity>(TEntity entity) where TEntity : class => _dbProvider.Get(entity, this);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="setStatement"></param>
        /// <param name="whereClause"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public int Update(string tableName, string setStatement, string whereClause, object parameters)
        {
            var setStatementString = setStatement.IndexOf("SET", StringComparison.OrdinalIgnoreCase) == -1
                ? "SET " + setStatement
                : setStatement;

            var whereCondition = whereClause.IndexOf("where", StringComparison.OrdinalIgnoreCase) == -1
                ? "WHERE " + whereClause
                : whereClause;

            var sql = $"UPDATE {tableName} {setStatementString} {whereCondition}";
            
            return ExecuteNonQuery(sql, CommandType.Text, parameters);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="whereClause"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public int Delete(string tableName, string whereClause, object parameters)
        {
            var whereCondition = whereClause.IndexOf("where", StringComparison.OrdinalIgnoreCase) == -1
                ? "WHERE " + whereClause
                : whereClause;

            var sql = $"DELETE FROM {tableName} {whereCondition}";
            
            return ExecuteNonQuery(sql, CommandType.Text, parameters);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="sql"></param>
        /// <param name="whereClause"></param>
        /// <param name="parameters"></param>
        /// <param name="orderByClause"></param>
        /// <param name="currentPage"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public PaginationResult<TEntity> PagedQuery<TEntity>(string sql, string whereClause = default, object parameters = null, string orderByClause = default, int currentPage = 1, int pageSize = 10) =>
            _dbProvider.PagedQuery<TEntity>(this, sql, whereClause, parameters, orderByClause, currentPage, pageSize);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public Task<int> DeleteAsync(string sql, object parameters) => ExecuteNonQueryAsync(sql, CommandType.Text, parameters.ToDataParameters(_dbProvider.ParameterDelimiter));

        /// <summary>
        /// Commit a transaction
        /// </summary>
        public void CommitTransaction() => _transaction?.Commit();

        /// <summary>
        /// Rollback a transaction
        /// </summary>
        public void RollbackTransaction() => _transaction?.Rollback();
        
        /// <inheritdoc />
        /// <summary>
        ///  Dispose Database connection and transaction objects.
        /// </summary>
        public void Dispose()
        {
            _transaction?.Dispose();
            _connection?.Close();
            _connection?.Dispose();
        }
    }
}