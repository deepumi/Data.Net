﻿using System;
using System.Data;
using System.Data.Common;
using Data.Net.Providers;

namespace Data.Net
{
    internal sealed class CommandBuilder : IDisposable
    {
        private readonly DataParameters _parameters;

        internal readonly DbCommand Command;

        internal CommandBuilder(string sql, IDbConnection connection, IDbTransaction transaction, DbProvider dbProvider,
            DataParameters parameters = null, CommandType commandType = CommandType.Text)
        {
            Command = connection.CreateCommand() as DbCommand;
            Command.CommandText = sql ?? throw new ArgumentNullException(nameof(sql));
            Command.CommandType = commandType;
            Command.Connection = connection as DbConnection;

            if (dbProvider is OracleProvider)
            {
                var bindByName = Command.GetType().GetProperty("BindByName");
                bindByName?.SetValue(Command, true, null);
            }

            if (connection.State != ConnectionState.Open) connection.Open();

            if (transaction != null) Command.Transaction = transaction as DbTransaction;

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