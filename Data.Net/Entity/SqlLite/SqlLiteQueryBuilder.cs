using System.Text;

namespace Data.Net;

internal sealed class SqlLiteQueryBuilder : EntityQueryBuilder
{
    private const string Sql = "INSERT INTO {0} ({1}) VALUES({2}); {3}";

    public override char ParameterDelimiter => '@';

    public override SqlResult InsertQuery(EntityMetaData metaData)
    {
        var dataParameters = CreateDataParameters(metaData);

        var result = CreateInsertColumnNames(metaData);

        return new SqlResult(result, dataParameters);
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
            identityInserted = "select last_insert_rowid();";

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