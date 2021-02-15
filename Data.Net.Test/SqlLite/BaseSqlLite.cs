using System;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace Data.Net.Test.SqlLite
{
    public abstract class BaseSqlLite : IDisposable
    {
        internal static readonly string SqlLiteConnectionString = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build()["SQLLiteDatabasePath"];

        protected readonly IDbConnection Connection;

        private readonly string _tableName;
        
        protected BaseSqlLite(string tableName, IDbConnection connection)
        {
            Connection = connection;
            
            _tableName = tableName;
            
            DropTable();

            var table = @$"CREATE TABLE {_tableName} (
            Id    INTEGER,
            Name    TEXT,
            Email    Text,
            PRIMARY KEY(Id AUTOINCREMENT))";

            Connection.ExecuteNonQuery(table);
        }

        public void Dispose()
        {
            DropTable();
            
            Connection?.Dispose();
        }
        
        private void DropTable() => Connection.ExecuteNonQuery($"drop table if exists {_tableName}");
    }
}