namespace Data.Net
{
    internal static class DbProviderFactory
    {
        internal static DbProvider GetDbProvider(string type)
        {
            var result = Get(type);

            if(result == null) ThrowHelper.ThrowException("Provider does not support!");

            DbProvider Get(string type)
            {
                return type switch
                {
                    "OracleConnection" => new OracleProvider(new OracleQueryBuilder()),
                    "SqlConnection" => new SqlServerProvider(new SqlServerQueryBuilder()),
                    "MySqlConnection" => new MySqlProvider(new MySqlQueryBuilder()),
                    "NpgsqlConnection" => new PostgresProvider(new PostgresQueryBuilder()),
                    "SQLiteConnection" => new SqlLiteProvider(new SqlLiteQueryBuilder()),
                    "SqliteConnection" => new SqlLiteProvider(new SqlLiteQueryBuilder()),
                    _ => default
                };
            }

            return result;
        }
    }
}