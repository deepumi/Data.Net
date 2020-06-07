using System;
using System.Data;
using System.Data.SqlClient;

namespace Data.Net.PerformanceTest
{
    internal class AdoNetInsertSqlPerfTest : BasePerfTest
    {
        internal User Insert(User user)
        {
            using (var db = new SqlConnection(ConnectionString))
            {
                using (var cmd = new SqlCommand("INSERT INTO Users_Test (FirstName,LastName) output INSERTED.UserId VALUES(@FirstName,@LastName)", db))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@FirstName", user.FirstName));
                    cmd.Parameters.Add(new SqlParameter("@LastName", user.LastName));

                    if (db.State != ConnectionState.Open) db.Open();

                    var scalar = cmd.ExecuteScalar();

                    if (scalar != null && DBNull.Value != scalar)
                    {
                        user.UserId = (int) scalar;
                    }
                }
            }

            return user;
        }
    }
}