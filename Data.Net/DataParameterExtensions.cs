using System.Collections.Generic;
using System.ComponentModel;

namespace Data.Net
{
    internal static class DataParameterExtensions
    {
        internal static DataParameters ToDataParameters(this object parameters, string parameterDelimiter)
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
                        dataParameters.Add(AddParameterDelimiter(item.Key, parameterDelimiter), item.Value);
                    }

                    return dataParameters;

                case IEnumerable<KeyValuePair<string, object>> kvp:

                    if (kvp is IList<KeyValuePair<string, object>> list)
                    {
                        dataParameters = new DataParameters(list.Count);

                        for (var i = 0; i < list.Count; i++)
                        {
                            dataParameters.Add(AddParameterDelimiter(list[i].Key, parameterDelimiter), list[i].Value);
                        }

                        return dataParameters;
                    }

                    dataParameters = new DataParameters();

                    foreach (var item in kvp)
                    {
                        dataParameters.Add(AddParameterDelimiter(item.Key, parameterDelimiter), item.Value);
                    }

                    return dataParameters;

              default:

                    var properties = TypeDescriptor.GetProperties(parameters);

                    dataParameters = new DataParameters(properties.Count);

                    for (var i = 0; i < properties.Count; i++)
                    {
                        var obj = properties[i].GetValue(parameters);

                        dataParameters.Add(AddParameterDelimiter(properties[i].Name, parameterDelimiter), obj);
                    }

                    return dataParameters;
            }
        }

        private static string AddParameterDelimiter(string key, string parameterDelimiter)
        {
            if (key == null) return string.Empty;

            return key[0] >= 65 && key[0] <= 90 // A-Z
                          || key[0] >= 97 && key[0] <= 122 // a-z
                ? parameterDelimiter + key
                : key;
        }
    }
}