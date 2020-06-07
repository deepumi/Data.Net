using System.Data;
using Data.Net.Generator;

namespace Data.Net.Providers
{
    internal sealed class MsSqlProvider : DbProvider
    {
        internal override TEntity Insert<TEntity>(TEntity entity, Database db)
        {
            var metaData = GetEntityMetaData(entity);

            var queryBuilder = new MsInsertQueryBuilder(metaData); //_builder.BuildInsertQuery(metaData);

            var result = queryBuilder.BuildInsertQuery();
            
            if (result == null) return entity;
            
            if (result.AutoIncrementSetter != null)
            {
                var identity = db.ExecuteScalar(result.Query, CommandType.Text, result.DataParameters);

                result.AutoIncrementSetter(identity);
            }
            else
            {
                db.ExecuteNonQuery(result.Query, CommandType.Text, result.DataParameters);
            }
            
            return entity;
        }
    }
}