namespace Data.Net;

internal sealed class PagedSqlResult
{
    internal string WhereClause { get; set; }

    internal string OrderByClause { get; set; }

    internal int PageSize { get; set; }

    internal int CurrentPage { get; set; }

    internal int StartRow { get; set; }

    internal int EndRow { get; set; }
}