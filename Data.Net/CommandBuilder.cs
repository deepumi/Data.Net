using System;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Data.Net;

internal sealed class CommandBuilder : IDisposable
{
    internal readonly DbCommand Command;

    internal CommandBuilder(string sql, IDbConnection connection, IDbTransaction transaction, DbProvider dbProvider,
        DataParameters parameters = null, CommandType commandType = CommandType.Text) : this(sql, connection, dbProvider,
        parameters, commandType)
    {
        if (connection.State != ConnectionState.Open) connection.Open();

        if (transaction != null) Command.Transaction = transaction as DbTransaction;
    }

    internal CommandBuilder(string sql, IDbConnection connection, DbProvider dbProvider,
        DataParameters parameters = null, CommandType commandType = CommandType.Text)
    {
        if (string.IsNullOrEmpty(sql)) ThrowHelper.Throw(sql);

        Command = connection.CreateCommand() as DbCommand;

        if (Command == null) return;

        Command.CommandText = sql;
        Command.CommandType = commandType;
        Command.Connection = connection as DbConnection;

        if (dbProvider is OracleProvider)
        {
            var bindByName = Command.GetType()
                .GetProperty("BindByName", BindingFlags.Public | BindingFlags.Instance);
            bindByName?.SetValue(Command, true, null);
        }

        if (parameters == null) return;

        AddParam(parameters);
    }

    internal Task OpenAsync(CancellationToken token) => Command?.Connection?.OpenAsync(token);

    private void AddParam(DataParameters parameters)
    {
        if (parameters.DbParameters?.Count > 0)
        {
            for (var i = 0; i < parameters.DbParameters.Count; i++)
            {
                Command.Parameters.Add(parameters.DbParameters[i]);
            }
            return;
        }

        foreach (var item in parameters)
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

        if (parameter.Direction is 0 or ParameterDirection.Input) return p;

        p.DbType = parameter.DbType;
        p.Direction = parameter.Direction;
        p.Size = parameter.Size;

        parameter.DbParameter = p;

        return p;
    }

    public void Dispose() => Command?.Dispose();
}