using System;
using System.Data;

namespace Data.Net
{
    internal sealed class KeyInfo
    {
        internal string ColumnName { get; }
        
        internal DbType PropertyType { get; }
        
        internal KeyInfo(string columnName, Type propertyType)
        {
            ColumnName = columnName;
            PropertyType = DbTypeHelper.GetType(propertyType);
        }
    }
}