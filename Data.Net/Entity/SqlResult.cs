namespace Data.Net;

internal sealed class SqlResult
{
    internal readonly string Query;

    internal readonly DataParameters DataParameters;

    internal SqlResult(string query, DataParameters dataParameters)
    {
        Query = query;
        DataParameters = dataParameters;
    }
}