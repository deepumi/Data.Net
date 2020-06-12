using System.Text;

namespace Data.Net.Generator
{
    internal sealed class MsInsertQueryBuilder : BaseInsertQueryBuilder
    {
        private const string Sql = "INSERT INTO {0} ({1}) {2} VALUES({3})";
        
        internal MsInsertQueryBuilder(EntityMetaData metaData) : base(metaData) {}
        
        protected internal override InsertSqlResult BuildInsertQuery()
        {
            var dataParameters = CreateDataParameters();

            var result = CreateInsertColumNames();

            return new InsertSqlResult(result, dataParameters);
        }

        private string CreateInsertColumNames()
        {
            var sb = new StringBuilder();

            var comma = string.Empty;

            for (var i = 0; i < MetaData.PropertiesList.Count; i++)
            {
                if (IsAutoIncrement(MetaData.PropertiesList[i].Name)) continue;

                sb.Append(comma + MetaData.PropertiesList[i].Name);

                comma = ",";
            }

            var columns = sb.ToString();
            
            sb.Clear();

            var identityInserted = string.Empty;
            
            if (MetaData.AutoIncrementInfo?.AutoIncrementSetter != null)
                identityInserted = " output INSERTED." + MetaData.AutoIncrementInfo.ColumName;

            comma = string.Empty;

            for (var i = 0; i < MetaData.PropertiesList.Count; i++)
            {
                if (IsAutoIncrement(MetaData.PropertiesList[i].Name)) continue;

                var paramName = "@" + MetaData.PropertiesList[i].Name;

                sb.Append(comma + paramName);

                comma = ",";
            }

            var values = sb.ToString();

            sb.Clear();
            
            return string.Format(Sql, MetaData.TableName, columns, identityInserted, values);
        }
    }
}