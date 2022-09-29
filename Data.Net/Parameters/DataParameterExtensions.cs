using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

namespace Data.Net;

/// <summary>
/// 
/// </summary>
public static class DataParameterExtensions
{
    /// <summary>
    /// Convert <see cref="IDbDataParameter"/> to DataParameters.
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public static DataParameters ToDbParameters(this IEnumerable<IDbDataParameter> parameters)
    {
        return parameters == null ? default : new DataParameters(parameters);
    }

    internal static DataParameters ToDbParameters(this object parameters, char parameterDelimiter)
    {
        DataParameters dataParameters;

        switch (parameters)
        {
            case null:
                return null;

            case DataParameters dp:
                return dp;

            case IDictionary<string, object> dict:

                dataParameters = new DataParameters(dict.Count);

                foreach (var item in dict)
                {
                    dataParameters.Add(AddDelimiter(item.Key, parameterDelimiter), item.Value);
                }

                return dataParameters;

            case IEnumerable<KeyValuePair<string, object>> kvp:

                if (kvp is IList<KeyValuePair<string, object>> list)
                {
                    dataParameters = new DataParameters(list.Count);

                    for (var i = 0; i < list.Count; i++)
                    {
                        dataParameters.Add(AddDelimiter(list[i].Key, parameterDelimiter), list[i].Value);
                    }

                    return dataParameters;
                }

                dataParameters = new DataParameters();

                foreach (var item in kvp)
                {
                    dataParameters.Add(AddDelimiter(item.Key, parameterDelimiter), item.Value);
                }

                return dataParameters;

            case IEnumerable<IDbDataParameter> kvp:

                return new DataParameters(kvp);

            default:

                var properties = TypeDescriptor.GetProperties(parameters);

                dataParameters = new DataParameters(properties.Count);

                for (var i = 0; i < properties.Count; i++)
                {
                    var obj = properties[i].GetValue(parameters);

                    dataParameters.Add(AddDelimiter(properties[i].Name, parameterDelimiter), obj);
                }

                return dataParameters;
        }
    }

    private static string AddDelimiter(string key, char parameterDelimiter)
    {
        if (key == null) return string.Empty;

        return key[0] >= 65 && key[0] <= 90 // A-Z
               || key[0] >= 97 && key[0] <= 122 // a-z
            ? parameterDelimiter + key
            : key;
    }
}