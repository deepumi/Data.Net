using System;

namespace Data.Net.Providers
{
    internal static class ProviderFactory
    {
        internal static DbProvider GetDbProvider(string type)
        {
            return type switch
            {
                "OracleConnection" => new OracleProvider(),
                "SqlConnection" => new SqlServerProvider(),
                "MySqlConnection" => new MySqlProvider(),
                "NpgsqlConnection" => new NgpSqlProvider(),
                "SQLiteConnection" => new SqlLiteProvider(),
                "SqliteConnection" => new SqlLiteProvider(),
                _ => throw new Exception("provider does not support!")
            };
        }
    }
}