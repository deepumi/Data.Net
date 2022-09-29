using System;
using System.Collections.Generic;
using System.Data;

namespace Data.Net;

internal static class DbTypeHelper
{
    private static readonly Dictionary<Type, DbType> _types = new(9)
    {
        [typeof(short)] = DbType.Int16,
        [typeof(ushort)] = DbType.UInt16,
        [typeof(int)] = DbType.Int32,
        [typeof(uint)] = DbType.UInt32,
        [typeof(long)] = DbType.Int64,
        [typeof(ulong)] = DbType.UInt64,
        [typeof(double)] = DbType.Double,
        [typeof(decimal)] = DbType.Decimal,
        [typeof(object)] = DbType.Object
    };

    internal static DbType GetType(Type type) => _types.ContainsKey(type) ? _types[type] : default;
}