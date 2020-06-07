using System.Data.SqlClient;

namespace Data.Net.PerformanceTest
{
    internal class InsertMsSqlDataNet : BasePerfTest
    {
        internal User Insert(User user)
        {
            using (var db = new Database(new SqlConnection(ConnectionString)))
            {
                return db.Insert(user);
            }
        }
    }
}