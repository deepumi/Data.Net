using System.Data;

namespace Data.Net
{
    internal sealed class SqlLiteProvider : DbProvider
    {
        private readonly IEntityQueryBuilder _query;

        public SqlLiteProvider(IEntityQueryBuilder query) : base(query.ParameterDelimiter) => _query = query;

        internal override bool Delete<TEntity>(TEntity entity, Database db) => _query.Delete(entity, db);

        internal override TEntity Insert<TEntity>(TEntity entity, Database db)
        {
            var metaData = new EntityMetaData(entity);

            var result = _query.InsertQuery(metaData);

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

        internal override TEntity Update<TEntity>(TEntity entity, Database db) => _query.Update(entity, db);

        internal override TEntity Get<TEntity>(TEntity entity, Database db) => _query.Get(entity, db);
    }
}