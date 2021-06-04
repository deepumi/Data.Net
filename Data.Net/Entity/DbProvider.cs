namespace Data.Net
{
    internal abstract class DbProvider
    {
        protected internal string ParameterDelimiter { get; }

        protected DbProvider() { }

        internal DbProvider(string parameterDelimiter)
        {
            ParameterDelimiter = parameterDelimiter;
        }

        internal abstract TEntity Insert<TEntity>(TEntity entity, Database db) where TEntity : class;

        internal abstract TEntity Update<TEntity>(TEntity entity, Database db) where TEntity : class;

        internal abstract bool Delete<TEntity>(TEntity entity, Database db) where TEntity : class;

        internal abstract TEntity Get<TEntity>(TEntity entity, Database db) where TEntity : class;

        internal virtual PaginationResult<TEntity> PagedQuery<TEntity>(Database db, string sql, string whereClause, string orderByClause, int pageSize = 10, int currentPage = 1)
        {
            return default;
        }
    }
}