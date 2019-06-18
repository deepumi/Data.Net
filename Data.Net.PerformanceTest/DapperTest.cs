using System.Data.SqlClient;
using Dapper;

namespace Data.Net.PerformanceTest
{
    internal sealed class DapperTest : BasePerfTest
    {
        internal void QuerySingleTest()
        {
            using (var db = new SqlConnection(ConnectionString))
            {
                for (int i = 1; i < MaxLimit; i++)
                {
                    db.QuerySingle<string>("SELECT FirstName From Users_Test Where Email = @Email",
                        new {Email = "email@gmail.com"});
                }
            }
        }

        internal void QueryMultipleTest()
        {
            using (var db = new SqlConnection(ConnectionString))
            {
                for (int i = 1; i < MaxLimit; i++)
                {
                    var x = db.Query("SELECT FirstName,LastName,Email From Users_Test");
                }
            }
        }
    }
}
