using System;

namespace Data.Net
{
    internal static class Converter
    {
        internal static T ToValue<T>(this object value)
        {
            if (value == null || DBNull.Value == value) return default;

            return (T)value;
        }
    }
}