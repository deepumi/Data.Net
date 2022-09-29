using System;
using System.Data;

namespace Data.Net;

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

        var dataParameter = result.DataParameters.OutputParameter[_query.ParameterDelimiter + metaData.AutoIncrementInfo.ColumnName];

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

    internal override PaginationResult<TEntity> PagedQuery<TEntity>(Database db, string sql, string whereClause, object parameters = null, string orderByClause = null, int currentPage = 1, int pageSize = 10)
    {
        var pagedQuery = _query.PagedModel(whereClause, orderByClause, pageSize, currentPage);

        const string countSql = "SELECT COUNT(*) FROM ({0} {1} {2})";

        var scalarValue = db.ExecuteScalar(string.Format(countSql, sql, pagedQuery.WhereClause, pagedQuery.OrderByClause), parameters: parameters);

        var recordCount = scalarValue != null ? Convert.ToInt32(scalarValue.ToString()) : 0;

        if (recordCount <= 0) return null;

        const string pagedSql = @"SELECT * 
                                    FROM
                                      (SELECT rownum AS rnum,a.*
                                       FROM
                                         ({0} {1} {2}) a
                                       WHERE rownum <= {3})
                                    WHERE rnum >= {4}";

        var pagedSqlFormatted = string.Format(pagedSql, sql, pagedQuery.WhereClause, pagedQuery.OrderByClause, pagedQuery.EndRow.ToString(), pagedQuery.StartRow.ToString());

        var result = db.Query<TEntity>(pagedSqlFormatted, parameters: parameters);

        return new PaginationResult<TEntity>(result, new PaginationInfo(recordCount, pagedQuery.CurrentPage, pagedQuery.PageSize));
    }
}