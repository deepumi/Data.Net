using System.Text;

namespace Data.Net;

internal sealed class SqlServerQueryBuilder : EntityQueryBuilder
{
    public override string ParameterDelimiter => "@";

    public override SqlResult InsertQuery(EntityMetaData metaData)
    {
        var sb = new StringBuilder("INSERT INTO ");

        sb.Append(metaData.TableName);

        CreateColumnNames(sb, metaData);

        if (metaData.AutoIncrementInfo?.AutoIncrementSetter != null)
        {
            sb.Append("output INSERTED.");
            sb.Append(metaData.AutoIncrementInfo.ColumnName);
        }

        CreateColumnValues(sb, metaData);

        var sql = sb.ToString();

        sb.Clear();

        return new SqlResult(sql, CreateDataParameters(metaData));
    }

    private static void CreateColumnNames(StringBuilder sb, EntityMetaData metaData)
    {
        var comma = string.Empty;

        sb.Append(" (");

        for (var i = 0; i < metaData.PropertiesList.Count; i++)
        {
            if (metaData.IsAutoIncrement(metaData.PropertiesList[i].Name)) continue;

            sb.Append(comma);
            sb.Append(metaData.PropertiesList[i].Name);

            comma = ",";
        }

        sb.Append(") ");
    }

    private void CreateColumnValues(StringBuilder sb, EntityMetaData metaData)
    {
        sb.Append(" VALUES (");

        var comma = string.Empty;

        for (var i = 0; i < metaData.PropertiesList.Count; i++)
        {
            if (metaData.IsAutoIncrement(metaData.PropertiesList[i].Name)) continue;
                
            sb.Append(comma);
            sb.Append(ParameterDelimiter);
            sb.Append(metaData.PropertiesList[i].Name);

            comma = ",";
        }

        sb.Append(") ");
    }
}