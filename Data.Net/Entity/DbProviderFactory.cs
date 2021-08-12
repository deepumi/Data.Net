using System;

namespace Data.Net
{
    internal static class DbProviderFactory
    {
        internal static DbProvider GetDbProvider(string type)
        {
            return type switch
            {
                "OracleConnection" => new OracleProvider(new OracleQueryBuilder()),
                "SqlConnection" => new SqlServerProvider(new SqlServerQueryBuilder()),
                "MySqlConnection" => new MySqlProvider(new MySqlQueryBuilder()),
                "NpgsqlConnection" => new PostgresProvider(new PostgresQueryBuilder()),
                "SQLiteConnection" => new SqlLiteProvider(new SqlLiteQueryBuilder()),
                "SqliteConnection" => new SqlLiteProvider(new SqlLiteQueryBuilder()),
                _ => throw new Exception("provider does not support!")
            };
        }
    }
}