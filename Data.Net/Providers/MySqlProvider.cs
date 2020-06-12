using System.Data;
using Data.Net.Generator;

namespace Data.Net.Providers
{
    internal sealed class MySqlProvider : DbProvider
    {
        internal override TEntity Insert<TEntity>(TEntity entity, Database db)
        {
            var metaData = GetEntityMetaData(entity);

            var queryBuilder = new MySqlInsertQueryBuilder(metaData);

            var result = queryBuilder.BuildInsertQuery();

            if (result == null) return entity;

            if (metaData.AutoIncrementInfo?.AutoIncrementSetter != null)
            {
                var identity = db.ExecuteScalar(result.Query, CommandType.Text, result.DataParameters);

                metaData.AutoIncrementInfo.AutoIncrementSetter(identity);
            }
            else
            {
                db.ExecuteNonQuery(result.Query, CommandType.Text, result.DataParameters);
            }
            
            return entity;
        }
    }
}