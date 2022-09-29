using System.Text;

namespace Data.Net;

internal sealed class PostgresQueryBuilder : EntityQueryBuilder
{
    private const string Sql = "INSERT INTO {0} ({1}) VALUES({2}) {3}";

    public override char ParameterDelimiter => '@';

    public override SqlResult InsertQuery(EntityMetaData metaData)
    {
        return new SqlResult(CreateInsertColumnNames(metaData), CreateDataParameters(metaData));
    }

    private string CreateInsertColumnNames(EntityMetaData metaData)
    {
        var sb = new StringBuilder();

        var first = true;

        for (var i = 0; i < metaData.PropertiesList.Count; i++)
        {
            if (metaData.IsAutoIncrement(metaData.PropertiesList[i].Name)) continue;

            sb.Append(first ? "" : ',');
            sb.Append(metaData.PropertiesList[i].Name);

            first = false;
        }

        var columns = sb.ToString();

        sb.Clear();

        var identityInserted = string.Empty;

        if (metaData.AutoIncrementInfo?.AutoIncrementSetter != null)
            identityInserted = "RETURNING " + metaData.AutoIncrementInfo.ColumnName;

        first = true;

        for (var i = 0; i < metaData.PropertiesList.Count; i++)
        {
            if (metaData.IsAutoIncrement(metaData.PropertiesList[i].Name)) continue;

            var paramName = ParameterDelimiter + metaData.PropertiesList[i].Name;

            sb.Append(first ? "" : ',');
            sb.Append(paramName);

            first = false;
        }

        var values = sb.ToString();

        sb.Clear();

        return string.Format(Sql, metaData.TableName, columns, values, identityInserted);
    }
}