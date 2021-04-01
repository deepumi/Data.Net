using System.Text;

namespace Data.Net
{
    internal sealed class MySqlQueryBuilder : EntityQueryBuilder
    {
        private const string Sql = "INSERT INTO {0} ({1}) VALUES({2}); {3}";

        public override string ParameterDelimiter => "@";

        public override SqlResult InsertQuery(EntityMetaData metaData)
        {
            var dataParameters = CreateDataParameters(metaData);

            var result = CreateInsertColumnNames(metaData);

            return new SqlResult(result, dataParameters);
        }

        private string CreateInsertColumnNames(EntityMetaData metaData)
        {
            var sb = new StringBuilder();

            var comma = string.Empty;

            for (var i = 0; i < metaData.PropertiesList.Count; i++)
            {
                if (metaData.IsAutoIncrement(metaData.PropertiesList[i].Name)) continue;

                sb.Append(comma + metaData.PropertiesList[i].Name);

                comma = ",";
            }

            var columns = sb.ToString();

            sb.Clear();

            var identityInserted = string.Empty;

            if (metaData.AutoIncrementInfo?.AutoIncrementSetter != null)
                identityInserted = "SELECT LAST_INSERT_ID();";

            comma = string.Empty;

            for (var i = 0; i < metaData.PropertiesList.Count; i++)
            {
                if (metaData.IsAutoIncrement(metaData.PropertiesList[i].Name)) continue;

                var paramName = ParameterDelimiter + metaData.PropertiesList[i].Name;

                sb.Append(comma + paramName);

                comma = ",";
            }

            var values = sb.ToString();

            sb.Clear();

            return string.Format(Sql, metaData.TableName, columns, values, identityInserted);
        }
    }
}