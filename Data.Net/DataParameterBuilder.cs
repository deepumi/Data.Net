using System.Data;

namespace Data.Net
{
    internal class DataParameterBuilder
    {
        private readonly DataParameters _dataParameter;
        
        internal DataParameterBuilder(IDbCommand cmd, DataParameters parameters)
        {
            if (parameters == null) return;

            _dataParameter = parameters;

            Add(cmd);
        }

        private void Add(IDbCommand cmd)
        {
            foreach (var item in _dataParameter)
            {
                switch (item)
                {
                    case Parameter param:
                        cmd.Parameters.Add(CreateParameter(cmd, param));
                        break;
                    case IDbDataParameter dataParameter:
                        cmd.Parameters.Add(dataParameter);
                        break;
                }
            }
        }

        private IDbDataParameter CreateParameter(IDbCommand cmd, Parameter parameter)
        {
            var p = cmd.CreateParameter();
            p.ParameterName = parameter.Name;
            p.Value = parameter.Value;

            if (!parameter.IsOutputOrReturn) return p;

            p.DbType = parameter.DbType;
            p.Direction = parameter.Direction;
            return p;
        }

        internal void Update(IDbCommand cmd)
        {
            if (cmd.Parameters == null || _dataParameter == null) return;

            foreach (var item in _dataParameter)
            {
                switch (item)
                {
                    case Parameter parameter:
                        if (!parameter.IsOutputOrReturn) continue;
                        parameter.Name = parameter.Name;
                        parameter.Value = (cmd.Parameters[parameter.Name] as IDbDataParameter)?.Value;
                        break;
                    case IDbDataParameter dataParameter:
                        if (dataParameter.Direction == ParameterDirection.Input) continue;
                        dataParameter.Value = (cmd.Parameters[dataParameter.ParameterName] as IDbDataParameter)?.Value;
                        break;
                }
            }
        }
    }
}
