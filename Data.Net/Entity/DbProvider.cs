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
    }
}