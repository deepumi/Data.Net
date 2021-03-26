namespace Data.Net
{
    internal sealed class SqlResult
    {
        public string Query { get; }

        public DataParameters DataParameters { get; }

        internal SqlResult(string query, DataParameters dataParameters)
        {
            Query = query;
            DataParameters = dataParameters;
        }
    }
}