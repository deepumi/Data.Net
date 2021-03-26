namespace Data.Net
{
    interface IEntityQueryBuilder
    {
        string ParameterDelimiter { get; }

        SqlResult InsertQuery(EntityMetaData metaData);

        SqlResult UpdateQuery(EntityMetaData metaData);

        SqlResult DeleteQuery(EntityMetaData metaData);
    }
}