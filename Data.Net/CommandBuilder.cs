using System;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Data.Net
{
    internal sealed class CommandBuilder : IDisposable
    {
        private readonly DataParameters _parameters;

        internal readonly DbCommand Command;

        //Sync constructor
        internal CommandBuilder(string sql, IDbConnection connection, IDbTransaction transaction, DbProvider dbProvider,
            DataParameters parameters = null, CommandType commandType = CommandType.Text) : this(sql, connection, dbProvider,
                parameters, commandType)
        {
            if (connection.State != ConnectionState.Open) connection.Open();

            if (transaction != null) Command.Transaction = transaction as DbTransaction;
        }

        // Async constructor
        internal CommandBuilder(string sql, IDbConnection connection, DbProvider dbProvider,
           DataParameters parameters = null, CommandType commandType = CommandType.Text)
        {
            Command = connection.CreateCommand() as DbCommand;
            Command.CommandText = sql ?? throw new ArgumentNullException(nameof(sql));
            Command.CommandType = commandType;
            Command.Connection = connection as DbConnection;

            if (dbProvider is OracleProvider)
            {
                var bindByName = Command.GetType().GetProperty("BindByName", BindingFlags.Public | BindingFlags.Instance);
                bindByName?.SetValue(Command, true, null);
            }

            if (parameters == null) return;
            
            _parameters = parameters;

            AddParam();
        }

        internal Task OpenAsync(CancellationToken token) => Command.Connection.OpenAsync(token);

        private void AddParam()
        {
            foreach (var item in _parameters)
            {
                switch (item)
                {
                    case Parameter param:
                        Command.Parameters.Add(BuildParameter(param));
                        break;
                    case IDbDataParameter dataParameter:
                        Command.Parameters.Add(dataParameter);
                        break;
                }
            }
        }

        private IDbDataParameter BuildParameter(Parameter parameter)
        {
            var p = Command.CreateParameter();
            p.ParameterName = parameter.Name;
            p.Value = parameter.Value ?? DBNull.Value;

            if (parameter.Direction == 0 || parameter.Direction == ParameterDirection.Input) return p;

            p.DbType = parameter.DbType;
            p.Direction = parameter.Direction;
            p.Size = parameter.Size;

            parameter.DbParameter = p;

            return p;
        }

        public void Dispose() => Command?.Dispose();
    }
}