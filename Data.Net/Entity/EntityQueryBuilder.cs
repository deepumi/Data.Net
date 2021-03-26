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
                if (keyValue == null && (IsAutoIncrement(metaData.AutoIncrementInfo, metaData.PropertiesList[i].Name) || metaData.KeyInfo != null))
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
            {
                throw new Exception("Update failed, cannot obtain Key or Auto Increment column or value");
            }

            sb.Append(columnName);
            sb.Append("=");
            sb.Append(ParameterDelimiter);
            sb.Append(columnName);

            return new SqlResult(sb.ToString(), CreateDataParameters(metaData, false));
        }

        public SqlResult DeleteQuery(EntityMetaData metaData)
        {
            var sb = new StringBuilder("DELETE FROM " + metaData.TableName);

            object keyValue = default;

            string columnName = default;

            var dp = new DataParameters(1);

            for (var i = 0; i < metaData.PropertiesList.Count; i++)
            {
                if (keyValue == null && (IsAutoIncrement(metaData.AutoIncrementInfo, metaData.PropertiesList[i].Name) || metaData.KeyInfo != null))
                {
                    columnName = metaData.PropertiesList[i].Name;
                    keyValue = metaData.PropertiesList[i].Value;
                    dp.Add(columnName, keyValue);
                }
            }

            sb.Append("  WHERE ");

            if (string.IsNullOrEmpty(columnName) || keyValue == null)
            {
                throw new Exception("Unable to perform DELETE operation. Cannot obtain Key or Auto Increment column or value");
            }

            sb.Append(columnName);
            sb.Append("=");
            sb.Append(ParameterDelimiter);
            sb.Append(columnName);

            return new SqlResult(sb.ToString(), dp);
        }

        protected internal static bool IsAutoIncrement(AutoIncrementInfo autoInfo, string key) => autoInfo != null &&
                                                      key.Equals(autoInfo.ColumnName, StringComparison.OrdinalIgnoreCase);

        protected DataParameters CreateDataParameters(EntityMetaData metaData, bool insert = true)
        {
            var dp = new DataParameters(metaData.PropertiesList.Count);

            for (var i = 0; i < metaData.PropertiesList.Count; i++)
            {
                if (insert && (metaData.PropertiesList[i].Name == null || IsAutoIncrement(metaData.AutoIncrementInfo, metaData.PropertiesList[i].Name))) continue;

                dp.Add(ParameterDelimiter + metaData.PropertiesList[i].Name, metaData.PropertiesList[i].Value);
            }

            return dp;
        }
    }
}