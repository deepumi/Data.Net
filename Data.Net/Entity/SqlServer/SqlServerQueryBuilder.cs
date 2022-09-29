using System.Text;

namespace Data.Net;

internal sealed class SqlServerQueryBuilder : EntityQueryBuilder
{
    public override char ParameterDelimiter => '@';

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
        var first = true;

        sb.Append(" (");

        for (var i = 0; i < metaData.PropertiesList.Count; i++)
        {
            if (metaData.IsAutoIncrement(metaData.PropertiesList[i].Name)) continue;

            sb.Append(first ? "" : ',');
            sb.Append(metaData.PropertiesList[i].Name);

            first = false;
        }

        sb.Append(") ");
    }

    private void CreateColumnValues(StringBuilder sb, EntityMetaData metaData)
    {
        sb.Append(" VALUES (");

        var first = true;

        for (var i = 0; i < metaData.PropertiesList.Count; i++)
        {
            if (metaData.IsAutoIncrement(metaData.PropertiesList[i].Name)) continue;

            sb.Append(first ? "" : ',');
            sb.Append(ParameterDelimiter);
            sb.Append(metaData.PropertiesList[i].Name);

            first = false;
        }

        sb.Append(") ");
    }
}