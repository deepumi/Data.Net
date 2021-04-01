namespace Data.Net.Test
{
    internal static class ConnectionString
    {
        internal static readonly string SqlServerConnectionString = AppSettings.Config["ConnectionStrings:SqlServerConnectionString"];

        internal static readonly string OracleConnectionString = AppSettings.Config["ConnectionStrings:OracleConnectionString"];

        internal static readonly string PostgresSqlConnectionString = "";

        internal static readonly string MySqlConnectionString = "";
    }
}