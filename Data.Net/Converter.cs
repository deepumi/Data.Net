using System;
using System.Globalization;

namespace Data.Net
{
    internal static class Converter
    {
        internal static T ToValue<T>(this object value)
        {
            if (value == null || DBNull.Value == value) return default(T);

            var type = Nullable.GetUnderlyingType(typeof(T));

            return (T) Convert.ChangeType(value, type ?? typeof(T), CultureInfo.InvariantCulture);
        }
    }
}