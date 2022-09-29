using System;
using System.Collections.Generic;
using System.Data;

namespace Data.Net;

/// <summary>
/// 
/// </summary>
internal sealed class OutputParameter : BaseOutputParameter
{
    private readonly List<object> _parameters;

    internal OutputParameter(List<object> parameters)
    {
        _parameters = parameters;
    }

    /// <summary>
    /// Returns database output or return parameter to <see cref="IDbDataParameter"/>.  
    /// </summary>
    /// <param name="index">Array index</param>
    /// <returns></returns>
    public override IDbDataParameter this[int index]
    {
        get
        {
            if (index < 0 || index > _parameters.Count - 1) return default;

            return _parameters[index] switch
            {
                Parameter p when IsOutPutOrReturnParameter(p.Direction) => p.DbParameter,
                IDbDataParameter dp when IsOutPutOrReturnParameter(dp.Direction) => dp,
                _ => default
            };
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    protected override IDbDataParameter GetDbParameter(string name)
    {
        if (name == null) return default;

        for (var i = _parameters.Count - 1; i >= 0; i--)
        {
            switch (_parameters[i])
            {
                case Parameter p when IsOutPutOrReturnParameter(p.Direction) &&
                                      string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase):
                    return p.DbParameter;

                case IDbDataParameter dp when IsOutPutOrReturnParameter(dp.Direction) &&
                                              string.Equals(dp.ParameterName, name, StringComparison.OrdinalIgnoreCase):
                    return dp;
            }
        }

        return default;
    }
}