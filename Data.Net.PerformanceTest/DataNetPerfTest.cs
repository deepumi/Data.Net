using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Data.Net.PerformanceTest
{
    internal sealed class DataNetPerfTest : BasePerfTest
    {
        internal void ExecuteScalarTest()
        {
            using (var db = new Database(new SqlConnection(ConnectionString)))
            {
                for (int i = 1; i < MaxLimit; i++)
                {
                    var result = db.ExecuteScalar<string>("SELECT FirstName From Users_Test Where Email = @Email"
                        , CommandType.Text, new DataParameters(1) { { "Email", "email@gmail.com" } });
                }
            }
        }

        internal async Task ExecuteScalarTestAsync()
        {
            using (var db = new Database(new SqlConnection(ConnectionString)))
            {
                for (int i = 1; i < MaxLimit; i++)
                {
                   var x =   await db.ExecuteScalarAsync<string>("SELECT FirstName From Users_Test Where Email = @Email"
                        , CommandType.Text, new DataParameters(1) { { "Email", "email@gmail.com" } });

                    var y = db.ExecuteScalar<string>("SELECT FirstName From Users_Test Where Email = @Email"
                        , CommandType.Text, new DataParameters(1) { { "Email", "email@gmail.com" } });


                }
            }
        }

        internal void QueryMultipleTest()
        {
            using (var db = new Database(new SqlConnection(ConnectionString)))
            {
                for (int i = 1; i < MaxLimit; i++)
                {
                    var x = db.Query<User>("SELECT FirstName,LastName,Email From Users_Test"
                        , CommandType.Text,null, CommandBehavior.Default);
                }
            }
        }

        internal void QuerySingleTest()
        {
            using (var db = new Database(new SqlConnection(ConnectionString)))
            {
                for (int i = 1; i < MaxLimit; i++)
                {
                    var x = db.QuerySingle<string>("SELECT FirstName From Users_Test Where Email = @Email"
                        , CommandType.Text, new DataParameters(1) { { "Email", "email@gmail.com" } },CommandBehavior.SingleRow);
                }
            }
        }
    }
}
