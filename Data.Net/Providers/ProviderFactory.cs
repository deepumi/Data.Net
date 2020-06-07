using System;

namespace Data.Net.Providers
{
    internal static class ProviderFactory
    {
        internal static DbProvider GetDbProvider(string typeName)
        {
            return typeName switch
            {
                "Oracle.DataAccess.Client.OracleConnection" => new OracleProvider(),
                "Oracle.ManagedDataAccess.Client.OracleConnection" => new OracleProvider(),
                "System.Data.SqlClient.SqlConnection" => new MsSqlProvider(),
                "Microsoft.Data.SqlClient.SqlConnection" => new MsSqlProvider(),
                "MySql.Data.MySqlClient.MySqlConnection" => new MySqlProvider(),
                "Npgsql.NpgsqlConnection" => new NgpSqlProvider(),
                _ => throw new Exception("provider does not support!")
            };
        }
    }
}