using System.Text;

namespace Data.Net.Generator
{
    internal sealed class NgpSqlInsertQueryBuilder : BaseInsertQueryBuilder
    {
        private const string Sql = "INSERT INTO {0} ({1}) VALUES({2}) {3}";
        
        internal NgpSqlInsertQueryBuilder(EntityMetaData metaData) : base(metaData) {}
        
        protected internal override InsertSqlResult BuildInsertQuery()
        {
            return new InsertSqlResult(CreateInsertColumNames(), CreateDataParameters(), MetaData.AutoIncrementInfo?.AutoIncrementActionSetter);
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
            
            if (MetaData.AutoIncrementInfo?.AutoIncrementActionSetter != null)
                identityInserted = "RETURNING "+ MetaData.AutoIncrementInfo.ColumName;

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

            return string.Format(Sql, MetaData.TableName, columns, values, identityInserted);
        }
    }
}