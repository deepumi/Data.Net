﻿using System;
 using System.Data;

 namespace Data.Net.Generator
{
    internal sealed class AutoIncrementInfo
    {
        internal string ColumName { get; }
        
        internal Action<object> AutoIncrementSetter { get;}
        
        internal string SequenceName { get; }

        internal IAutoIncrementRetriever AutoIncrementRetriever { get; }

        internal DbType PropertyType { get; }
        
        internal AutoIncrementInfo(string columName, string sequenceName, IAutoIncrementRetriever retrieve,
            Action<object> action, Type propertyType)
        {
            ColumName = columName;
            SequenceName = sequenceName;
            AutoIncrementRetriever = retrieve;
            AutoIncrementSetter = action;
            PropertyType = DbTypeHelper.GetType(propertyType);
        }
    }
}