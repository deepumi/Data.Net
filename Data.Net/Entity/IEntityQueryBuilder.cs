namespace Data.Net;

internal interface IEntityQueryBuilder
{
    char ParameterDelimiter { get; }

    SqlResult InsertQuery(EntityMetaData metaData);

    SqlResult UpdateQuery(EntityMetaData metaData);

    SqlResult DeleteQuery(EntityMetaData metaData);

    SqlResult SelectQuery(EntityMetaData metaData);

    PagedSqlResult PagedModel(string whereClause, string orderByClause, int pageSize = 10, int currentPage = 1);
}