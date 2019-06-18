using System;
using System.Globalization;

namespace Data.Net
{
    internal static class Converter
    {
        internal static T ToValue<T>(this object value)
        {
            if (value == null || DBNull.Value == value) return default(T);

            return (T) Convert.ChangeType(value, Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T), CultureInfo.InvariantCulture);
        }
    }
}