using System;

namespace Data.Net
{
    internal static class DbProviderFactory
    {
        private static readonly DbProvider _oracle = new OracleProvider(new OracleQueryBuilder());

        private static readonly DbProvider _sqlServer = new SqlServerProvider(new SqlServerQueryBuilder());

        private static readonly DbProvider _postgres = new PostgresProvider(new PostgresQueryBuilder());

        private static readonly DbProvider _mySql = new MySqlProvider(new MySqlQueryBuilder());

        private static readonly DbProvider _sqlLite = new SqlLiteProvider(new SqlLiteQueryBuilder());

        internal static DbProvider GetDbProvider(string type)
        {
            return type switch
            {
                "OracleConnection" => _oracle,
                "SqlConnection" => _sqlServer,
                "MySqlConnection" => _mySql,
                "NpgsqlConnection" => _postgres,
                "SQLiteConnection" => _sqlLite,
                "SqliteConnection" => _sqlLite,
                _ => throw new Exception("provider does not support!")
            };
        }
    }
}