using System;
using System.Text;

namespace Data.Net
{
    internal abstract class EntityQueryBuilder : IEntityQueryBuilder
    {
        public abstract string ParameterDelimiter { get; }

        protected internal EntityQueryBuilder() { }

        public abstract SqlResult InsertQuery(EntityMetaData metaData);

        public SqlResult UpdateQuery(EntityMetaData metaData)
        {
            var sb = new StringBuilder("UPDATE " + metaData.TableName + " SET ");

            var comma = string.Empty;

            object keyValue = default;

            string columnName = default;

            for (var i = 0; i < metaData.PropertiesList.Count; i++)
            {
                if (keyValue == null && (metaData.IsAutoIncrement(metaData.PropertiesList[i].Name) || string.Equals(metaData.KeyInfo,
                    metaData.PropertiesList[i].Name, StringComparison.OrdinalIgnoreCase)))
                {
                    columnName = metaData.PropertiesList[i].Name;
                    keyValue = metaData.PropertiesList[i].Value;
                    continue;
                }

                sb.Append(comma);

                sb.Append(' ');

                sb.Append(metaData.PropertiesList[i].Name);

                sb.Append('=');

                sb.Append(ParameterDelimiter);

                sb.Append(metaData.PropertiesList[i].Name);

                comma = ",";
            }

            sb.Append(" WHERE ");

            if (string.IsNullOrEmpty(columnName) || keyValue == null)
                throw new Exception("Update failed, cannot obtain Key or Auto Increment column or value");

            sb.Append(columnName);
            sb.Append("=");
            sb.Append(ParameterDelimiter);
            sb.Append(columnName);

            return new SqlResult(sb.ToString(), CreateDataParameters(metaData, false));
        }

        public SqlResult DeleteQuery(EntityMetaData metaData)
        {
            var sb = new StringBuilder("DELETE FROM " + metaData.TableName);

            DataParameters dp = null;

            for (var i = 0; i < metaData.PropertiesList.Count; i++)
            {
                if (metaData.IsAutoIncrement(metaData.PropertiesList[i].Name) ||
                    string.Equals(metaData.KeyInfo,
                        metaData.PropertiesList[i].Name, StringComparison.OrdinalIgnoreCase))
                {
                    dp = new DataParameters(1)
                    {
                        {metaData.PropertiesList[i].Name, metaData.PropertiesList[i].Value}
                    };

                    sb.Append(" WHERE ");

                    sb.Append(metaData.PropertiesList[i].Name);
                    sb.Append("=");
                    sb.Append(ParameterDelimiter);
                    sb.Append(metaData.PropertiesList[i].Name);

                    break;
                }
            }

            if (dp == null)
                throw new Exception("Unable to perform DELETE operation. Cannot obtain Key or Auto Increment column or value");

            return new SqlResult(sb.ToString(), dp);
        }

        public SqlResult SelectQuery(EntityMetaData metaData)
        {
            var sb = new StringBuilder("SELECT * FROM " + metaData.TableName);

            DataParameters dp = null;

            for (var i = 0; i < metaData.PropertiesList.Count; i++)
            {
                if (metaData.IsAutoIncrement(metaData.PropertiesList[i].Name) || string.Equals(metaData.KeyInfo,
                    metaData.PropertiesList[i].Name, StringComparison.OrdinalIgnoreCase))
                {
                    dp = new DataParameters(1)
                    {
                        { metaData.PropertiesList[i].Name, metaData.PropertiesList[i].Value }
                    };

                    sb.Append("  WHERE ");
                    sb.Append(metaData.PropertiesList[i].Name);
                    sb.Append("=");
                    sb.Append(ParameterDelimiter);
                    sb.Append(metaData.PropertiesList[i].Name);

                    break;
                }
            }

            if (dp == null)
                throw new Exception("Unable to perform SELECT operation. Cannot obtain Key or Auto Increment column or value");

            return new SqlResult(sb.ToString(), dp);
        }

        protected DataParameters CreateDataParameters(EntityMetaData metaData, bool insert = true)
        {
            var dp = new DataParameters(metaData.PropertiesList.Count);

            for (var i = 0; i < metaData.PropertiesList.Count; i++)
            {
                if (insert && (string.IsNullOrEmpty(metaData.PropertiesList[i].Name) || metaData.IsAutoIncrement(metaData.PropertiesList[i].Name))) continue;

                dp.Add(ParameterDelimiter + metaData.PropertiesList[i].Name, metaData.PropertiesList[i].Value);
            }

            return dp;
        }

        public PagedSqlResult PagedModel(string whereClause, string orderByClause, int pageSize = 10, int currentPage = 1)
        {
            var pageIndex = currentPage;

            if (pageIndex <= 0) pageIndex = 1;
            
            pageIndex -= 1;
            
            if (pageSize <= 0) pageSize = 10;

            var startRow = (pageIndex * pageSize) + 1;

            var endRow = startRow + (pageSize - 1);

            var whereCondition = !string.IsNullOrEmpty(whereClause)
                ? whereClause.IndexOf("WHERE", StringComparison.OrdinalIgnoreCase) == -1 ? "WHERE " + whereClause : whereClause
                : string.Empty;

            var orderCondition =  !string.IsNullOrEmpty(orderByClause)
                ? orderByClause.IndexOf("ORDER BY", StringComparison.OrdinalIgnoreCase) == -1 ? "ORDER BY " + orderByClause : orderByClause
                : string.Empty;
                
            return new PagedSqlResult
            {
                CurrentPage = pageIndex,
                PageSize = pageSize,
                StartRow = startRow,
                EndRow = endRow,
                WhereClause = whereCondition,
                OrderByClause = orderCondition
            };
        }
    }
}