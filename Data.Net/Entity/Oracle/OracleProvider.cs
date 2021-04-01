using System;
using System.Data;

namespace Data.Net
{
    internal sealed class OracleProvider : DbProvider
    {
        private readonly IEntityQueryBuilder _query;
         
        internal OracleProvider(IEntityQueryBuilder query) : base(query.ParameterDelimiter) => _query = query;

        internal override bool Delete<TEntity>(TEntity entity, Database db) => _query.Delete(entity, db);

        internal override TEntity Insert<TEntity>(TEntity entity, Database db)
        {
            var metaData = new EntityMetaData(entity);

            var result = _query.InsertQuery(metaData);

            if (result == null) return entity;
            
            db.ExecuteNonQuery(result.Query, CommandType.Text, result.DataParameters);

            if (metaData.AutoIncrementInfo?.AutoIncrementSetter == null) return entity; // || metaData.AutoIncrementInfo.AutoIncrementRetriever == null

            var dataParameter = result.DataParameters[_query.ParameterDelimiter + metaData.AutoIncrementInfo.ColumnName];

            if (dataParameter?.Value == null || dataParameter.Value == DBNull.Value) return entity;
            
            if (metaData.AutoIncrementInfo.AutoIncrementRetriever != null)
            {
                metaData.AutoIncrementInfo.AutoIncrementRetriever.Retrieve(dataParameter.Value);
            }
            else
            {
                metaData.AutoIncrementInfo.AutoIncrementSetter(dataParameter.Value); //call action setter
            }

            return entity;
        }

        internal override TEntity Update<TEntity>(TEntity entity, Database db) => _query.Update(entity, db);

        internal override TEntity Get<TEntity>(TEntity entity, Database db) => _query.Get(entity, db);
    }
}