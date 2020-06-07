using Data.Net.Generator;

namespace Data.Net.Providers
{
    internal abstract class DbProvider
    {
        internal abstract TEntity Insert<TEntity>(TEntity entity, Database db) where TEntity : class;
        
        protected static EntityMetaData GetEntityMetaData<TEntity>(TEntity entity) where TEntity : class
        {
            return new EntityMetaData(entity);
        }
    }
}
