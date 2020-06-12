using System;
using System.Data;
using Data.Net.Generator;

namespace Data.Net.Providers
{
    internal class NgpSqlProvider : DbProvider
    {
        internal override TEntity Insert<TEntity>(TEntity entity, Database db)
        {
            var metaData = GetEntityMetaData(entity);

            var queryBuilder = new NgpSqlInsertQueryBuilder(metaData); //_builder.BuildInsertQuery(metaData);

            var result = queryBuilder.BuildInsertQuery();

            if (result == null) return entity;
            
            if (metaData.AutoIncrementInfo?.AutoIncrementSetter != null)
            {
                var identityValue = db.ExecuteScalar(result.Query, CommandType.Text, result.DataParameters);

                if(identityValue != null && identityValue != DBNull.Value)
                    metaData.AutoIncrementInfo.AutoIncrementSetter(identityValue);
            }
            else
            {
                db.ExecuteNonQuery(result.Query, CommandType.Text, result.DataParameters);
            }
            
            return entity;
        }
    }
}