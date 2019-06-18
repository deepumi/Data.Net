using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;

namespace Data.Net
{
    internal sealed class DataRowReader<T>
    {
        private const BindingFlags Flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase;

        private readonly int _fieldCount;

        private readonly Func<T> _instance = Activator.CreateInstance<T>;//Expression.Lambda<Func<T>>(Expression.New(typeof(T))).Compile();

        private readonly Type _type = typeof(T);

        private readonly Dictionary<string, Action<T, object>> _setter;

        internal DataRowReader(int fieldCount)
        {
            _fieldCount = fieldCount;
            _setter = new Dictionary<string, Action<T, object>>(fieldCount, StringComparer.OrdinalIgnoreCase);
        }

        internal T ReaderToType(IDataReader reader)
        {
            var instance =  _instance();

            var ok = false;

            for (var i = 0; i < _fieldCount; i++)
            {
                if (reader.IsDBNull(i) || reader.GetValue(i) == DBNull.Value ||
                    !Set(instance, reader[i], reader.GetName(i))) continue;

                if (!ok) ok = true;
            }

            return ok ? instance : default(T);
        }

        private bool Set(T instance, object value, string propertyName)
        {
            if (_setter.ContainsKey(propertyName))
            {
                _setter[propertyName](instance, value);
                return true;
            }

            var p = _type.GetProperty(propertyName, Flags);

            if (p == null) return false;

            var typeParameter = Expression.Parameter(typeof(T));
            var valueParameter = Expression.Parameter(typeof(object));

            var setter = Expression.Lambda<Action<T, object>>(
                Expression.Assign(
                    Expression.Property(
                        Expression.Convert(typeParameter, _type), p),
                    Expression.Convert(valueParameter, p.PropertyType)),
                typeParameter, valueParameter);

            _setter.Add(propertyName, setter.Compile());

            _setter[propertyName](instance, value);

            return true;
        }

        internal void Clear() => _setter.Clear();
    }
}