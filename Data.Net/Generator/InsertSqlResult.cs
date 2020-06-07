﻿using System;

 namespace Data.Net.Generator
{
    internal sealed class InsertSqlResult
    {
        internal Action<object> AutoIncrementSetter { get;}

        internal string Query { get; }

        internal DataParameters DataParameters { get; }

        internal InsertSqlResult(string query, DataParameters dataParameters, Action<object> autoIncrementSetter = null)
        {
            Query = query;
            DataParameters = dataParameters;
            AutoIncrementSetter = autoIncrementSetter;
        }
    }
}