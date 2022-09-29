using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using MySql.Data.MySqlClient;
using Npgsql;
using Oracle.ManagedDataAccess.Client;

namespace Data.Net.Console.Test
{
    class Program
    {
        private static string AddDelimiter(string key, char parameterDelimiter)
        {
            if (key == null) return string.Empty;

            return key[0] >= 65 && key[0] <= 90 // A-Z
                   || key[0] >= 97 && key[0] <= 122 // a-z
                ? parameterDelimiter + key
                : key;
        }

        static void Main(string[] args)
        {
            var s1 = AddDelimiter("SomeKye", '@');

            var sb = new StringBuilder();

            CreateColumnValues(sb, new List<string>
            {
                "Column1",
                "Column2",
                "Column3"
            }, false);

            var f = sb.ToString();

            sb.Clear();

            CreateColumnValues1(sb, new List<string>
            {
                "Column1",
                "Column2",
                "Column3"
            }, false);

            var s = sb.ToString();
        }

        private static void CreateColumnValues1(StringBuilder sb, List<string> metaData, bool isAutoIncrement)
        {
            sb.Append(" VALUES (");

            bool first = true;

            for (var i = 0; i < metaData.Count; i++)
            {
                if (isAutoIncrement)
                {
                    isAutoIncrement = false;
                    continue;

                }

                sb.Append(first ? "" : ',');
                sb.Append("@");
                sb.Append(metaData[i]);

                first = false;
            }

            sb.Append(") ");
        }

        private static void CreateColumnValues(StringBuilder sb, List<string> metaData, bool isAutoIncrement)
        {
            sb.Append(" VALUES (");

            var first = true;

            for (var i = 0; i < metaData.Count; i++)
            {
                if (isAutoIncrement)
                {
                    isAutoIncrement = false;
                    continue;

                }

                sb.Append(first ? "" : ',');
                sb.Append("@");
                sb.Append(metaData[i]);

                first = false;
            }

            sb.Append(") ");
        }

        void Test()
        {
            try
            {
                using (var sql = new SqlConnection(""))
                {
                    using var dp = new List<SqlParameter>(2)
                    {
                        new SqlParameter("@PageIndex", "1"),
                        new SqlParameter("@RecordCount",SqlDbType.Int) {Direction = ParameterDirection.Output}
                    }.ToDbParameters();

                    using (var r = sql.ExecuteReader("GetLogs", CommandType.StoredProcedure, dp))
                    {
                        while (r.Read())
                        {
                            var x = r.GetString(1);
                        }
                    }
                    
                    var dp1 = new DataParameters(1)
                    {
                        { "@Email", "jon@gmail.com" }
                    };

                    var tes = sql.PagedQuery<ExceptionLog>("SELECT * from ExceptionLog",
                        whereClause: "Email = @Email AND Email != ''",
                        parameters: dp1,
                        orderByClause: "ExceptionId ASC",
                        currentPage: 0,
                        pageSize: 10);

                    var e = dp.OutputParameter.GetInt32("@RecordCount");
                }

                // var student = db.Get(new AdminUser { AdminUserId = 109 });
                var message = new LogMessage
                {
                    Message = "Test Message in QA",
                    Detail = "Test detail message in QA",
                    CreateDate = DateTime.Now
                };

                // var result = db.Insert(message);
            }
            catch (Exception exp)
            {
                System.Console.WriteLine(exp.ToString());
            }
        }

        private static void NpgsqlPagination()
        {
            using var pg = new NpgsqlConnection("");

            var s = pg.PagedQuery<Test>("select * from users", pageSize: 2, currentPage: 5);
        }

        private static void MySqlPagination()
        {
            using var mysql = new MySqlConnection("");

            var t = mysql.PagedQuery<Test>("SELECT * FROM `users`", currentPage: 5, pageSize: 2);
        }

        private void SqlPagination()
        {
            var sq = new Database(new SqlConnection("Data Source=SQLEXPRESS;Initial Catalog=DataNet;Integrated Security=True"));

            var tes = sq.PagedQuery<Test>("SELECT * from Test", pageSize: 4, currentPage: 0);
        }

        private void OraclePagination()
        {
            using var db = new OracleConnection("");

            var orderBy = @"CASE
                WHEN modify_date IS NOT NULL AND modify_date > Create_date THEN modify_date
                ELSE Create_date
                END DESC";

            var pageInfo = db.PagedQuery<ApiException>(@"SELECT * From ApiException", whereClause: "UPPER(STATUS) = UPPER('ACTIVE')",
                orderByClause: orderBy, pageSize: 10, currentPage: 1);

        }
    }


    [TableName("API_EXCEPTIONS")]
    public class ApiException
    {
        public string GUID { get; set; }

        public string MACHINE_NAME { get; set; }

        public DateTime CREATE_DATE { get; set; }
    }

    [TableName("Users")]
    public class Test
    {
        [AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; }
    }

    public class ExceptionLog
    {
        [AutoIncrement]
        public int ExceptionId { get; set; }

        public string ExceptionMessage { get; set; }
    }
}