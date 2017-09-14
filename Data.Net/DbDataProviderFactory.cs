namespace Data.Net
{
    internal static class DbDataProviderFactory
    {
        internal static DbDataProvider GetDbDataProvider(string typeName)
        {
            switch (typeName)
            {
                case "Oracle.DataAccess.Client.OracleConnection":
                    return new OracleDataProvider();
                case "System.Data.SqlClient.SqlConnection":
                    return new SqlDataProvider();
                case "MySql.Data.MySqlClient.MySqlConnection":
                    return new MySqlDataProvider();
                default:
                    return new DefaultDataProvider();
            }
        }
    }
    internal abstract class DbDataProvider
    {
    }

    internal sealed class DefaultDataProvider : DbDataProvider
    {
    }

    internal sealed class MySqlDataProvider : DbDataProvider
    {
    }

    internal sealed class OracleDataProvider : DbDataProvider
    {
    }

    internal sealed class SqlDataProvider : DbDataProvider
    {
    }
}