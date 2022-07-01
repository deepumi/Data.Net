using System;
using System.Collections.Generic;
using System.Data;

namespace Data.Net
{
    internal sealed class OutputDbParameter : BaseOutputParameter
    {
        private readonly IList<IDbDataParameter> _parameters;

        internal OutputDbParameter(IList<IDbDataParameter> dbDataParameters)
        {
            _parameters = dbDataParameters;
        }

        public override IDbDataParameter this[int index]
        {
            get
            {
                if (index < 0 || index > _parameters.Count - 1) return default;

                return _parameters[index] switch
                {
                    { } dp when IsOutPutOrReturnParameter(dp.Direction) => dp,
                    _ => default
                };
            }
        }

        protected override IDbDataParameter GetDbParameter(string name)
        {
            if (name == null) return default;

            for (var i = _parameters.Count - 1; i >= 0; i--)
            {
                var dp = _parameters[i];

                if (IsOutPutOrReturnParameter(dp.Direction) &&
                   string.Equals(dp.ParameterName, name, StringComparison.OrdinalIgnoreCase))
                {
                    return dp;
                }
            }

            return default;
        }
    }
}