namespace Data.Net
{
    internal abstract class DbProvider
    {
        protected internal string ParameterDelimiter { get;}

        protected DbProvider() { }

        internal DbProvider(string parameterDelimiter)
        {
            ParameterDelimiter = parameterDelimiter;
        }
        
        internal abstract TEntity Insert<TEntity>(TEntity entity, Database db) where TEntity : class;

        internal abstract TEntity Update<TEntity>(TEntity entity, Database db) where TEntity : class;

        internal abstract TEntity Delete<TEntity>(TEntity entity, Database db) where TEntity : class;

        protected static EntityMetaData CreateMetaData<TEntity>(TEntity entity) where TEntity : class
        {
            return new EntityMetaData(entity);
        }
    }
}