﻿using System;

 namespace Data.Net.Generator
{
    internal sealed class AutoIncrementInfo
    {
        internal string ColumName { get; }
        
        internal Action<object> AutoIncrementActionSetter { get;}
        
        internal string SequenceName { get; }
        
        internal AutoIncrementInfo(string columName, string sequenceName, Action<object> action)
        {
            ColumName = columName;
            AutoIncrementActionSetter = action;
            SequenceName = sequenceName;
        }
    }
}