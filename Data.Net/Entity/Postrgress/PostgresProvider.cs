using System;
using System.Data;

namespace Data.Net
{
    internal sealed class PostgresProvider : DbProvider
    {
        private readonly IEntityQueryBuilder _query;

        internal PostgresProvider(IEntityQueryBuilder query) : base(query.ParameterDelimiter) => _query = query;

        internal override bool Delete<TEntity>(TEntity entity, Database db) => _query.Delete(entity, db);

        internal override TEntity Insert<TEntity>(TEntity entity, Database db)
        {
            var metaData = new EntityMetaData(entity);

            var result = _query.InsertQuery(metaData);

            if (result == null) return entity;

            if (metaData.AutoIncrementInfo?.AutoIncrementSetter != null)
            {
                var identityValue = db.ExecuteScalar(result.Query, CommandType.Text, result.DataParameters);

                if (identityValue != null && identityValue != DBNull.Value)
                    metaData.AutoIncrementInfo.AutoIncrementSetter(identityValue);
            }
            else
            {
                db.ExecuteNonQuery(result.Query, CommandType.Text, result.DataParameters);
            }

            return entity;
        }

        internal override TEntity Update<TEntity>(TEntity entity, Database db) => _query.Update(entity, db);

        internal override TEntity Get<TEntity>(TEntity entity, Database db) => _query.Get(entity, db);
    }
}