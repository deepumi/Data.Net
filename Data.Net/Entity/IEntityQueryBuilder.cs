namespace Data.Net;

internal interface IEntityQueryBuilder
{
    string ParameterDelimiter { get; }

    SqlResult InsertQuery(EntityMetaData metaData);

    SqlResult UpdateQuery(EntityMetaData metaData);

    SqlResult DeleteQuery(EntityMetaData metaData);

    SqlResult SelectQuery(EntityMetaData metaData);

    PagedSqlResult PagedModel(string whereClause, string orderByClause, int pageSize = 10, int currentPage = 1);
}

internal class PagedSqlResult
{
    internal string WhereClause { get; set; }

    internal string OrderByClause { get; set; }

    internal int PageSize { get; set; }

    internal int CurrentPage { get; set; }

    internal int StartRow { get; set; }

    internal int EndRow { get; set; }
}