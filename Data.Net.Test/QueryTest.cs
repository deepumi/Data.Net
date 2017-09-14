using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Data.Net.MsSql.Test
{
    [TestClass]
    public class QueryTest : TestBase,IDisposable
    {
        private readonly Database _db =  new Database();
 

        [TestMethod]
        public void Query_TimeSpan()
        {
            var q = _db.QuerySingle<TimeSpan>("SELECT cast(getdate() as time)");
            Assert.IsTrue(q != null);
        }

        [TestMethod]
        public void Query_Decimal()
        {
            var q = _db.QuerySingle<decimal>("select 50.1d");
            Assert.IsTrue(q == 50.1M);
        }

        [TestMethod]
        public void Query_Guid()
        {
            var q = _db.QuerySingle<Guid>("select NewId()");
            Assert.IsTrue(q != Guid.Empty);
        }

        [TestMethod]
        public void Query_Bit()
        {
            var q = _db.QuerySingle<bool>("SELECT cast(1 as bit)");
            Assert.IsTrue(q);
        }

        [TestMethod]
        public void Query_With_No_Parameter()
        {
            var q = _db.Query<User>("SELECT Email,LastName FROM Users_Test");
            Assert.IsTrue(q.Count > 0);
        }

        [TestMethod]
        public void Query_With_DataParameter()
        {
            var dataParameter = new DataParameters
            {
                { "@Email", "user1@domain.com" }
            };
            var q = _db.Query<User>("SELECT * FROM Users_Test Where Email = @Email", CommandType.Text, dataParameter);
            Assert.IsTrue(q.Count > 0);
        }

        [TestMethod]
        public void Query_With_SqlParameter()
        {
            var sqlParamterList = new DataParameters
            {
                new SqlParameter("@Email", "user@domain.com")
            };
            var q = _db.Query<User>("SELECT * FROM Users_Test Where Email = @Email", CommandType.Text, sqlParamterList);
            Assert.IsTrue(q.Count > 0);
        }

        [TestMethod]
        public void Query_With_Anonymus_Parameter()
        {
            var q = _db.Query<User>("SELECT * FROM Users_Test Where Email = @Email", CommandType.Text, new DataParameters {{"Email", "user1@domain.com" } });
            Assert.IsTrue(q.Count > 0);
        }

        [TestMethod]
        public void Query_With_Multiple_Output_DataParameter()
        {
            var dataParameter = new DataParameters()
            {
                { "@Email", "user1@domain.com" },
                { "@RecordCount"},
                { "@AnotherOutParameter"}
            };
            var q = _db.Query<User>("GetUsers_TEST", CommandType.StoredProcedure, dataParameter);
            Assert.IsTrue(q.Count > 0 && dataParameter.Value<int>("@RecordCount") > 0 &&
                          dataParameter.Value<int>("@AnotherOutParameter") > 0);
        }

        [TestMethod]
        public void Query_With_Multiple_Output_DbDataParameter()
        {
            var dataParameter = new DataParameters
            {
                new SqlParameter("@Email", "user1@domain.com"),
                new SqlParameter {ParameterName = "@RecordCount",Direction = ParameterDirection.Output,SqlDbType = SqlDbType.Int},
                new SqlParameter {ParameterName = "@AnotherOutParameter",Direction = ParameterDirection.Output,SqlDbType = SqlDbType.Int}
            };
            var q = _db.Query<User>("GetUsers_TEST", CommandType.StoredProcedure, dataParameter);
            Assert.IsTrue(q.Count > 0 && dataParameter.Value<int>("@RecordCount") > 0 &&
                          dataParameter.Value<int>("@AnotherOutParameter") > 0);
        }

        [TestMethod]
        public void Query_With_Return_Value_DataParameter()
        {
            var dataParameter = new DataParameters
            {
                { "@Email", "user1@domain.com" },
                { "@ReturnVal",ParameterDirection.ReturnValue}
            };
            var q = _db.Query<User>("GetUsersWithReturn_TEST", CommandType.StoredProcedure, dataParameter);
            Assert.IsTrue(q.Count > 0 && dataParameter.Value<int>("@ReturnVal") > 0);
        }

        [TestMethod]
        public void Query_With_Multiple_Output_And_Return_Value()
        {
            var dataParameter = new DataParameters
            {
                { "@Email", "user1@domain.com" },
                { "@RecordCount"},
                { "@AnotherOutParameter"},
                { "@ReturnVal",ParameterDirection.ReturnValue}
            };

            var q = _db.Query<User>("GetUsers_With_Output_Return_TEST", CommandType.StoredProcedure, dataParameter);
            Assert.IsTrue(q.Count > 0 && dataParameter.Value<int>("@RecordCount") > 0 &&
                          dataParameter.Value<int>("@AnotherOutParameter") > 0 &&
                          dataParameter.Value<int>("@ReturnVal") > 0);
        }

        [TestMethod]
        public void Query_With_Multiple_Output_ParameterMap()
        {
            var dataParameter = new DataParameters
            {
                { "@Email", "user1@domain.com" },
                { "@RecordCount" },
                { "@AnotherOutParameter" }
            };

            var q = _db.Query<User>("GetUsers_TEST", CommandType.StoredProcedure, dataParameter);
            Assert.IsTrue(q.Count > 0 && dataParameter.Value<int>("@RecordCount") > 0 &&
                          dataParameter.Value<int>("@AnotherOutParameter") > 0);
        }

        public void Dispose()
        {
            _db?.Dispose();
        }
    }
}
