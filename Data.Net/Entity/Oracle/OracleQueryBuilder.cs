using System.Data;
using System.Text;

namespace Data.Net;

internal sealed class OracleQueryBuilder : EntityQueryBuilder
{
    private const string InsertSql = "INSERT INTO {0} ({1}) VALUES({2}) {3}";

    public override char ParameterDelimiter => ':';


    public override SqlResult InsertQuery(EntityMetaData metaData)
    {
        var dataParameters = CreateDataParameters(metaData);

        if (dataParameters != null && metaData.AutoIncrementInfo != null && !string.IsNullOrEmpty(metaData.AutoIncrementInfo.ColumnName))
            dataParameters.AddOutPutParameter(string.Concat(ParameterDelimiter, metaData.AutoIncrementInfo.ColumnName), ParameterDirection.Output, metaData.AutoIncrementInfo.PropertyType);

        return new SqlResult(CreateInsertColumnNames(metaData), dataParameters);
    }

    private string CreateInsertColumnNames(EntityMetaData metaData)
    {
        var sb = new StringBuilder();

        var first = true;

        for (var i = 0; i < metaData.PropertiesList.Count; i++)
        {
            var columnName = metaData.PropertiesList[i].Name;

            if (metaData.IsAutoIncrement(metaData.PropertiesList[i].Name))
            {
                if (string.IsNullOrEmpty(metaData.AutoIncrementInfo.SequenceName)) continue;
            }

            sb.Append(first ? "" : ',');
            sb.Append(columnName);

            first = false;
        }

        var columns = sb.ToString();

        sb.Clear();

        var identityInserted = string.Empty;

        if (metaData.AutoIncrementInfo?.AutoIncrementSetter != null)
            identityInserted = string.Concat("RETURNING ", metaData.AutoIncrementInfo.ColumnName, " INTO :", metaData.AutoIncrementInfo.ColumnName);

        first = true;

        for (var i = 0; i < metaData.PropertiesList.Count; i++)
        {
            string paramName;

            if (metaData.IsAutoIncrement(metaData.PropertiesList[i].Name))
            {
                if (metaData.AutoIncrementInfo == null || string.IsNullOrEmpty(metaData.AutoIncrementInfo.SequenceName)) continue;

                paramName = string.Concat(metaData.AutoIncrementInfo?.SequenceName, ".NEXTVAL");
            }
            else
            {
                paramName = string.Concat(ParameterDelimiter, metaData.PropertiesList[i].Name);
            }

            sb.Append(first ? "" : ',');
            sb.Append(paramName);

            first = false;
        }

        var values = sb.ToString();

        sb.Clear();

        return string.Format(InsertSql, metaData.TableName, columns, values, identityInserted);
    }
}