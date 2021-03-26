﻿using System.Data;

namespace Data.Net
{
    internal sealed class SqlLiteProvider : DbProvider
    {
        private readonly IEntityQueryBuilder _query;

        public SqlLiteProvider(IEntityQueryBuilder query) : base(query.ParameterDelimiter) => _query = query;

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

        internal override TEntity Update<TEntity>(TEntity entity, Database db)
        {
            var result = _query.UpdateQuery(CreateMetaData(entity));

            if (result == null) return entity;

            db.ExecuteNonQuery(result.Query, CommandType.Text, result.DataParameters);

            return entity;
        }
    }
}