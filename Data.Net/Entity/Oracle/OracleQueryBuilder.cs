using System.Data;
using System.Text;

namespace Data.Net
{
    internal sealed class OracleQueryBuilder : EntityQueryBuilder
    {
        private const string Sql = "INSERT INTO {0} ({1}) VALUES({2}) {3}";

        public override string ParameterDelimiter => ":";
        
        public override SqlResult InsertQuery(EntityMetaData metaData)
        {
            var dataParameters = CreateDataParameters(metaData);

            if (dataParameters != null && metaData.AutoIncrementInfo != null && !string.IsNullOrEmpty(metaData.AutoIncrementInfo.ColumnName))
                dataParameters.Add(ParameterDelimiter + metaData.AutoIncrementInfo.ColumnName, ParameterDirection.Output, metaData.AutoIncrementInfo.PropertyType);

            return new SqlResult(CreateInsertColumnNames(metaData), dataParameters);
        }

        private string CreateInsertColumnNames(EntityMetaData metaData)
        {
            var sb = new StringBuilder();

            var comma = string.Empty;

            for (var i = 0; i < metaData.PropertiesList.Count; i++)
            {
                var columnName = metaData.PropertiesList[i].Name;
                
                if (IsAutoIncrement(metaData.AutoIncrementInfo, metaData.PropertiesList[i].Name))
                {
                    if (string.IsNullOrEmpty(metaData.AutoIncrementInfo.SequenceName)) continue;
                }

                sb.Append(comma + columnName);

                comma = ",";
            }

            var columns = sb.ToString();
            
            sb.Clear();

            var identityInserted = string.Empty;

            if (metaData.AutoIncrementInfo?.AutoIncrementSetter != null)
                identityInserted = $"RETURNING {metaData.AutoIncrementInfo.ColumnName} INTO :{metaData.AutoIncrementInfo.ColumnName}";

            comma = string.Empty;    

            for (var i = 0; i < metaData.PropertiesList.Count; i++)
            {
                string paramName;
                
                if (IsAutoIncrement(metaData.AutoIncrementInfo, metaData.PropertiesList[i].Name))
                {
                    if (metaData.AutoIncrementInfo == null || string.IsNullOrEmpty(metaData.AutoIncrementInfo.SequenceName)) continue;
                    
                    paramName = metaData.AutoIncrementInfo?.SequenceName + ".NEXTVAL";
                }
                else
                {
                    paramName = ParameterDelimiter + metaData.PropertiesList[i].Name;
                }

                sb.Append(comma + paramName);

                comma = ",";
            }

            var values = sb.ToString();

            sb.Clear();

            return string.Format(Sql, metaData.TableName, columns, values, identityInserted);
        }
    }
}