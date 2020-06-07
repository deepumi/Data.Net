using System.Data;
using System.Text;

namespace Data.Net.Generator
{
    internal sealed class OracleInsertQueryBuilder : BaseInsertQueryBuilder
    {
        private const string Sql = "INSERT INTO {0} ({1}) VALUES({2}) {3}";

        protected override string PrameterDelimiter => ":";
        
        internal OracleInsertQueryBuilder(EntityMetaData metaData) : base(metaData) { }

        protected internal override InsertSqlResult BuildInsertQuery()
        {
            var dataParameters = CreateDataParameters();

            if (dataParameters != null && MetaData.AutoIncrementInfo != null && !string.IsNullOrEmpty(MetaData.AutoIncrementInfo.ColumName))
                dataParameters.Add(PrameterDelimiter + MetaData.AutoIncrementInfo.ColumName, ParameterDirection.Output, DbType.Decimal, int.MaxValue);

            return new InsertSqlResult(CreateInsertColumNames(), dataParameters, MetaData.AutoIncrementInfo?.AutoIncrementActionSetter);
        }

        private string CreateInsertColumNames()
        {
            var sb = new StringBuilder();

            var comma = string.Empty;

            for (var i = 0; i < MetaData.PropertiesList.Count; i++)
            {
                var columnName = MetaData.PropertiesList[i].Name;
                
                if (IsAutoIncrement(MetaData.PropertiesList[i].Name))
                {
                    if (string.IsNullOrEmpty(MetaData.AutoIncrementInfo.SequenceName)) continue;
                }

                sb.Append(comma + columnName);

                comma = ",";
            }

            var columns = sb.ToString();
            
            sb.Clear();

            var identityInserted = string.Empty;

            if (MetaData.AutoIncrementInfo?.AutoIncrementActionSetter != null)
                identityInserted = $"RETURNING {MetaData.AutoIncrementInfo.ColumName} INTO :{MetaData.AutoIncrementInfo.ColumName}";

            comma = string.Empty;    

            for (var i = 0; i < MetaData.PropertiesList.Count; i++)
            {
                string paramName;
                
                if (IsAutoIncrement(MetaData.PropertiesList[i].Name))
                {
                    if (MetaData.AutoIncrementInfo == null || string.IsNullOrEmpty(MetaData.AutoIncrementInfo.SequenceName)) continue;
                    
                    paramName = MetaData.AutoIncrementInfo?.SequenceName + ".NEXTVAL";
                }
                else
                {
                    paramName = PrameterDelimiter + MetaData.PropertiesList[i].Name;
                }

                sb.Append(comma + paramName);

                comma = ",";
            }

            var values = sb.ToString();

            sb.Clear();

            return string.Format(Sql, MetaData.TableName, columns, values, identityInserted);
        }
    }
    
    // internal sealed class OracleInsertQueryBuilder : BaseInsertQueryBuilder
    // {
    //     private const string Sql = "INSERT INTO {0} ({1}) VALUES({2}) {3}";
    //
    //     protected override string PrameterDelimiter => ":";
    //     
    //     internal OracleInsertQueryBuilder(EntityMetaData metaData) : base(metaData) { }
    //
    //     protected internal override InsertSqlResult BuildInsertQuery()
    //     {
    //         var dataParameters = CreateDataParameters();
    //
    //         if (dataParameters != null && MetaData.AutoIncrementInfo != null && !string.IsNullOrEmpty(MetaData.AutoIncrementInfo.ColumName))
    //             dataParameters.Add(PrameterDelimiter + MetaData.AutoIncrementInfo.ColumName, ParameterDirection.Output, DbType.Decimal, int.MaxValue);
    //
    //         return new InsertSqlResult(CreateInsertColumNames(), dataParameters, MetaData.AutoIncrementInfo?.AutoIncrementActionSetter);
    //     }
    //
    //     private string CreateInsertColumNames()
    //     {
    //         var sb = new StringBuilder();
    //
    //         var comma = string.Empty;
    //
    //         for (var i = 0; i < MetaData.PropertiesList.Count; i++)
    //         {
    //             if (IsAutoIncrement(MetaData.PropertiesList[i].Name)) continue;
    //
    //             sb.Append(comma + MetaData.PropertiesList[i].Name);
    //
    //             comma = ",";
    //         }
    //
    //         var columns = sb.ToString();
    //         
    //         sb.Clear();
    //
    //         var identityInserted = string.Empty;
    //
    //         if (MetaData.AutoIncrementInfo?.AutoIncrementActionSetter != null)
    //             identityInserted = $"RETURNING {MetaData.AutoIncrementInfo.ColumName} INTO :{MetaData.AutoIncrementInfo.ColumName}";
    //
    //         comma = string.Empty;    
    //
    //         for (var i = 0; i < MetaData.PropertiesList.Count; i++)
    //         {
    //             if (IsAutoIncrement(MetaData.PropertiesList[i].Name)) continue;
    //
    //             var paramName = PrameterDelimiter + MetaData.PropertiesList[i].Name;
    //
    //             sb.Append(comma + paramName);
    //
    //             comma = ",";
    //         }
    //
    //         var values = sb.ToString();
    //
    //         sb.Clear();
    //
    //         return string.Format(Sql, MetaData.TableName, columns, values, identityInserted);
    //     }
    // }
}