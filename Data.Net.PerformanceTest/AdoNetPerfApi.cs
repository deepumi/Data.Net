using System.Data;
using System.Data.SqlClient;

namespace Data.Net.PerformanceTest
{
    internal sealed class AdoNetPerfApi : BasePerfTest
    {
        internal void ExecuteScalarTest()
        {
            using (var db = new SqlConnection(ConnectionString))
            {
                for (int i = 0; i < MaxLimit; i++)
                {
                    using (var cmd = new SqlCommand("SELECT FirstName From Users_Test Where Email = @Email", db))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Add(new SqlParameter("Email", "deepumi@gmail.com"));

                        if (db.State != ConnectionState.Open) db.Open();

                        var scalar = cmd.ExecuteScalar();

                        var result = scalar != null ? scalar.ToString() : string.Empty;
                    }
                }
            }
        }
    }
}