using System;
using System.Data;

namespace Data.Net;

internal sealed class SqlServerProvider : DbProvider
{
    private readonly IEntityQueryBuilder _query;

    public SqlServerProvider(IEntityQueryBuilder query) : base(query.ParameterDelimiter) => _query = query;

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

    internal override PaginationResult<TEntity> PagedQuery<TEntity>(Database db, string sql, string whereClause, object parameters = null, string orderByClause = null, int currentPage = 1, int pageSize = 10)
    {
        if (string.IsNullOrEmpty(orderByClause)) orderByClause = "ORDER BY (SELECT 0)"; //set default order.

        var pagedQuery = _query.PagedModel(whereClause, orderByClause, pageSize, currentPage);

        const string countSql = "SELECT COUNT(*) FROM ({0} {1}) as T";

        var scalarValue = db.ExecuteScalar(string.Format(countSql, sql, pagedQuery.WhereClause), parameters: parameters);

        var recordCount = scalarValue != null ? Convert.ToInt32(scalarValue.ToString()) : 0;

        if (recordCount <= 0) return null;

        const string pagedSql = @"SELECT  *
                                        FROM    (SELECT ROW_NUMBER() OVER ({0} ) AS RowNum, *
                                                  FROM ({1} {2}) AS A
                                                ) AS Rows
                                        WHERE   RowNum between {3} AND {4} 
                                        ORDER BY RowNum";

        var pagedSqlFormatted = string.Format(pagedSql, pagedQuery.OrderByClause, sql, pagedQuery.WhereClause, pagedQuery.StartRow.ToString(), pagedQuery.EndRow.ToString());

        var result = db.Query<TEntity>(pagedSqlFormatted, parameters: parameters);

        return new PaginationResult<TEntity>(result, new PaginationInfo(recordCount, pagedQuery.CurrentPage, pagedQuery.PageSize));
    }
}