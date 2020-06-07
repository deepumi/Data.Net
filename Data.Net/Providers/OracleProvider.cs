using System;
using System.Data;
using Data.Net.Generator;

namespace Data.Net.Providers
{
    internal sealed class OracleProvider : DbProvider
    {
        internal override TEntity Insert<TEntity>(TEntity entity, Database db)
        {
            var metaData = GetEntityMetaData(entity);

            var queryBuilder = new OracleInsertQueryBuilder(metaData);

            var result = queryBuilder.BuildInsertQuery();

            if (result == null) return entity;
            
            db.ExecuteNonQuery(result.Query, CommandType.Text, result.DataParameters);

            if (result.AutoIncrementSetter == null) return entity;

            var dataPrameter = result.DataParameters[":" + metaData.AutoIncrementInfo.ColumName];

            if (dataPrameter?.Value != null && dataPrameter.Value != DBNull.Value)
                result.AutoIncrementSetter(dataPrameter.Value);

            return entity;
        }
    }
}