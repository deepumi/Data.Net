using System;
using System.Data;
using Data.Net.Generator;

namespace Data.Net.Providers
{
    internal sealed class OracleProvider : DbProvider
    {
        protected internal override string PrameterDelimiter => ":";

        internal override TEntity Insert<TEntity>(TEntity entity, Database db)
        {
            var metaData = GetEntityMetaData(entity);

            var queryBuilder = new OracleInsertQueryBuilder(metaData);

            var result = queryBuilder.BuildInsertQuery();

            if (result == null) return entity;
            
            db.ExecuteNonQuery(result.Query, CommandType.Text, result.DataParameters);

            if (metaData.AutoIncrementInfo?.AutoIncrementSetter == null) return entity; // || metaData.AutoIncrementInfo.AutoIncrementRetriever == null

            var dataPrameter = result.DataParameters[PrameterDelimiter + metaData.AutoIncrementInfo.ColumName];

            if (dataPrameter?.Value == null || dataPrameter.Value == DBNull.Value) return entity;
            
            if (metaData.AutoIncrementInfo.AutoIncrementRetriever != null)
            {
                metaData.AutoIncrementInfo.AutoIncrementRetriever.Retrieve(dataPrameter.Value);
            }
            else
            {
                metaData.AutoIncrementInfo.AutoIncrementSetter(dataPrameter.Value); //call action setter
            }

            return entity;
        }
    }
}