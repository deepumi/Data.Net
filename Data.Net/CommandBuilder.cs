using System;
using System.Data;

namespace Data.Net
{
    internal sealed class CommandBuilder : IDisposable
    {
        private readonly DataParameters _parameters;

        private bool _hasOutPutParam;

        internal readonly IDbCommand Command;

        internal CommandBuilder(string sql, IDbConnection connection, IDbTransaction transaction, bool oracleProvider,
            DataParameters parameters = null, CommandType commandType = CommandType.Text)
        {
            Command = connection.CreateCommand();
            Command.CommandText = sql ?? throw new ArgumentNullException(nameof(sql));
            Command.CommandType = commandType;
            Command.Connection = connection;

            if (oracleProvider)
            {
                var bindByName = Command.GetType().GetProperty("BindByName");
                bindByName?.SetValue(Command, true, null);
            }

            if (connection.State != ConnectionState.Open) connection.Open();

            if (transaction != null) Command.Transaction = transaction;

            if (parameters == null) return;

            _parameters = parameters;

            AddParam();
        }

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

                        if (!_hasOutPutParam && dataParameter.Direction != 0 && dataParameter.Direction != ParameterDirection.Input) _hasOutPutParam = true;

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

            if (!_hasOutPutParam) _hasOutPutParam = true;

            p.DbType = parameter.DbType;
            p.Direction = parameter.Direction;
            p.Size = parameter.Size;
            return p;
        }

        internal void UpdateDataParameter()
        {
            if (!_hasOutPutParam || _parameters == null || Command.Parameters == null) return;

            foreach (var item in _parameters)
            {
                switch (item)
                {
                    case Parameter p when p.Direction != 0 && p.Direction != ParameterDirection.Input:
                        p.SetValue((Command.Parameters[p.Name] as IDbDataParameter)?.Value);
                        break;
                    case IDbDataParameter dp when dp.Direction != 0 && dp.Direction != ParameterDirection.Input:
                        dp.Value = (Command.Parameters[dp.ParameterName] as IDbDataParameter)?.Value;
                        break;
                }
            }
        }

        public void Dispose() => Command?.Dispose();
    }
}