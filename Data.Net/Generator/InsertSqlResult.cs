﻿ namespace Data.Net.Generator
{
    internal sealed class InsertSqlResult
    {
        internal string Query { get; }

        internal DataParameters DataParameters { get; }

        internal InsertSqlResult(string query, DataParameters dataParameters)
        {
            Query = query;
            DataParameters = dataParameters;
        }
    }
}