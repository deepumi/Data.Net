using System;
using System.Data;

namespace Data.Net
{
    internal sealed class AutoIncrementInfo
    {
        internal string ColumnName { get; }
        
        internal Action<object> AutoIncrementSetter { get;}
        
        internal string SequenceName { get; }

        internal IAutoIncrementRetriever AutoIncrementRetriever { get; }

        internal DbType PropertyType { get; }
        
        internal AutoIncrementInfo(string columnName, string sequenceName, IAutoIncrementRetriever retrieve,
            Action<object> action, Type propertyType)
        {
            ColumnName = columnName;
            SequenceName = sequenceName;
            AutoIncrementRetriever = retrieve;
            AutoIncrementSetter = action;
            PropertyType = DbTypeHelper.GetType(propertyType);
        }
    }
}