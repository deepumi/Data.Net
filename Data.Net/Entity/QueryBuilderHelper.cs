using System.Data;

namespace Data.Net
{
    internal static class QueryBuilderHelper
    {
        internal static TEntity Get<TEntity>(this IEntityQueryBuilder query, TEntity entity, Database db)
        {
            var result = query.SelectQuery(new EntityMetaData(entity));

            if (result == null) return entity;

            return db.QuerySingle<TEntity>(result.Query, CommandType.Text, result.DataParameters);
        }
        
        internal static TEntity Update<TEntity>(this IEntityQueryBuilder query, TEntity entity, Database db)
        {
            var result = query.UpdateQuery(new EntityMetaData(entity));

            if (result == null) return entity;

            db.ExecuteNonQuery(result.Query, CommandType.Text, result.DataParameters);

            return entity;
        }
        
        internal static bool Delete<TEntity>(this IEntityQueryBuilder query, TEntity entity, Database db)
        {
            var result = query.DeleteQuery(new EntityMetaData(entity));

            if (result == null) return false;

            var ok = db.ExecuteNonQuery(result.Query, CommandType.Text, result.DataParameters);

            return ok == 1;
        }
    }
}