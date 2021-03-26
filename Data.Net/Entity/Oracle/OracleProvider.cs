using System;
using System.Data;

namespace Data.Net
{
    internal sealed class OracleProvider : DbProvider
    {
        private readonly IEntityQueryBuilder _query;
         
        internal OracleProvider(IEntityQueryBuilder query) : base(query.ParameterDelimiter) => _query = query;

        internal override TEntity Delete<TEntity>(TEntity entity, Database db)
        {
            var result = _query.DeleteQuery(CreateMetaData(entity));

            if (result == null) return entity;

            db.ExecuteNonQuery(result.Query, CommandType.Text, result.DataParameters);

            return entity;
        }

        internal override TEntity Insert<TEntity>(TEntity entity, Database db)
        {
            var metaData = CreateMetaData(entity);

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

        internal override TEntity Update<TEntity>(TEntity entity, Database db)
        {
            var result = _query.UpdateQuery(CreateMetaData(entity));

            if (result == null) return entity;

            db.ExecuteNonQuery(result.Query, CommandType.Text, result.DataParameters);

            return entity;
        }
    }
}