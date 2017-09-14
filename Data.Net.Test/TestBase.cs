using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Data.Net.MsSql.Test
{
    [TestClass]
    public abstract class TestBase
    {
        [AssemblyCleanup]
        public static void Cleanup()
        {
            using (var db = new Database())
            {
               db.ExecuteNonQuery("DELETE FROM UserRoles_Test");
               db.ExecuteNonQuery("DELETE FROM Users_Test");
            }
        }
         
        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext tc)
        {
            const string sqlUser = @"INSERT INTO [Users_Test] (FirstName,LastName,Email,CreateDate) output INSERTED.UserId 
                                 VALUES (@FirstName,@LastName,@Email,@CreateDate) ";
            const string sqlUserRoles = "INSERT INTO UserRoles_Test(UserId,Role) VALUES (@UserId,@Role)";

            using (var db = new Database())
            {
                try
                {
                    db.BeginTransaction();

                    var parameters = new DataParameters
                    {
                        { "@FirstName", "User First Name" },
                        { "@LastName", "User Last Name" },
                        { "@Email", "user@domain.com" },
                        { "@CreateDate", DateTime.Now}
                    };

                    var userId = db.ExecuteScalar<int>(sqlUser, parameters: parameters);

                    db.ExecuteNonQuery(sqlUserRoles, parameters: new DataParameters { { "UserId", userId }, { "Role", "User" } });

                    db.ExecuteNonQuery(sqlUserRoles, parameters: new DataParameters { { "UserId", userId }, { "Role", "Admin" } });

                    db.CommitTransaction();
                }
                catch (Exception exp)
                {
                    db.RollbackTransaction();
                    Assert.Fail(exp.ToString());
                }
            }
        }
    }
}

