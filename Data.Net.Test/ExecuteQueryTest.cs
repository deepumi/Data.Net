using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Data.Net.MsSql.Test
{
    [TestClass]
    public class ExecuteQueryTest : TestBase
    {
        [TestMethod]
        public void Execute_Query_Get_Single_Value()
        {
            using (var db = new Database())
            {
                var firstName = db.ExecuteScalar<string>("SELECT FirstName From Users_Test Where Email = @Email"
                                ,parameters:new DataParameters(1){{"Email", "user@domain.com" } });
                Assert.IsTrue(!string.IsNullOrWhiteSpace(firstName));
            }
        }

        [TestMethod]
        public async Task Execute_Query_Get_Single_Value_Async()
        {
            using (var db = new Database())
            {
                var firstName = await db.ExecuteScalarAsync<string>("SELECT FirstName From Users_Test Where Email = @Email"
                                , parameters: new DataParameters(1) { { "Email", "user@domain.com" } });

                Assert.IsTrue(!string.IsNullOrWhiteSpace(firstName));
            }
        }

        [TestMethod]
        public void Execute_Query_Insert_With_Transaction()
        {
            const string sqlUser = @"INSERT INTO [Users_Test] (FirstName,LastName,Email,CreateDate) output INSERTED.UserId 
                                 VALUES (@FirstName,@LastName,@Email,@CreateDate) ";
            const string sqlRoles = "INSERT INTO UserRoles_Test(UserId,Role) VALUES (@UserId,@Role)";

            using (var db = new Database(new SqlConnection(ConfigurationManager.ConnectionStrings["DemoDb"].ConnectionString), true))
            {
                try
                {
                    for (var i = 1; i < 10; i++)
                    {
                        var parameters = new DataParameters
                        {
                            { "@FirstName", $"User First Name {i}" },
                            { "@LastName", $"User Last Name {i}" },
                            { "@Email", $"user{i}@domain.com" },
                            { "@CreateDate", DateTime.Now}
                        };

                        var userId = db.ExecuteScalar<int>(sqlUser, parameters: parameters);

                        db.ExecuteNonQuery(sqlRoles, parameters: new DataParameters{ {"UserId",userId}, {"Role","User"} });

                        db.ExecuteNonQuery(sqlRoles, parameters: new DataParameters { { "UserId", userId }, { "Role", "Admin" } });
                    }

                    db.CommitTransaction();
                }
                catch (Exception exp)
                {
                    db.RollbackTransaction();
                    Assert.Fail(exp.ToString());
                }
            }
        }

        [TestMethod]
        public void Execute_Query_Update()
        {
            const string sqlUser =
                @"UPDATE [Users_Test] SET FirstName = 'First Name updated' Where Email = @Email";

            using (var db = new Database())
            {
                var result = db.ExecuteNonQuery(sqlUser, parameters: new DataParameters { { "Email", "user@domain.com" } });
                Assert.IsTrue(result == 1);
            }
        }
    }
}
