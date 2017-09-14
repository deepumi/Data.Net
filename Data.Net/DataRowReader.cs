using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Reflection;

namespace Data.Net
{
    internal sealed class DataRowReader
    {
        private readonly Dictionary<string, int> _dbColumnDictionary = new Dictionary<string, int>(StringComparer.Ordinal);

        internal T ReaderToType<T>(IDataReader reader)
        {
            var notNull = false;
            var obj = Activator.CreateInstance<T>();

            if (typeof(T).IsValueType || typeof(T) == typeof(string))
            {
                obj = (T) reader.GetValue(0);
                notNull = true;
            }
            else
            {
                var properties = obj.GetType().GetProperties();

                if (_dbColumnDictionary.Count == 0) CreateColumn(properties, reader);

                foreach (var p in properties)
                {
                    if (!_dbColumnDictionary.ContainsKey(p.Name) || reader.IsDBNull(_dbColumnDictionary[p.Name]))
                        continue;

                    p.SetValue(obj,
                        Convert.ChangeType(reader[p.Name], Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType,
                            CultureInfo.InvariantCulture), null);

                    if (!notNull) notNull = true;
                }
            }
            return notNull ? obj : default(T);
        }

        internal void Flush() => _dbColumnDictionary.Clear();

        private void CreateColumn(IEnumerable<PropertyInfo> properties, IDataRecord reader)
        {
            foreach (var prop in properties)
            {
                for (var i = 0; i < reader.FieldCount; i++)
                {
                    if (reader.GetName(i).Equals(prop.Name, StringComparison.OrdinalIgnoreCase))
                        _dbColumnDictionary.Add(prop.Name, i);
                }
            }
        }
    }
}