using System;
using Oracle.ManagedDataAccess.Client;

namespace Data.Net.Console.Test
{
    class Program
    {
        static void  Main(string[] args)
        {
            var conn = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(Host=172.16.0.150)(Port=1521)))(CONNECT_DATA=(SERVICE_NAME=cosqa)(SERVER = DEDICATED))));User Id=portalowner;Password=pmanager2001;";

            try
            {
                using var db = new OracleConnection(conn);

                var student = db.Get(new AdminUser { AdminUserId = 109 });
            }
            catch (Exception exp)
            {
                System.Console.WriteLine(exp.ToString());
            }

            //var message = new LogMessage
            //{
            //    Message = "Inser this",
            //    Detail = "This is just an update",
            //    CreateDate = DateTime.Now
            //};

            //try
            //{
            //    using var db = new Database(new OracleConnection(conn));
            //    // var result = db.Insert(message);

            //    // var query = db.QuerySingle<LogMessage>("SELECT * FROM CAREER_BOT_ENTITIES_EXCEPTIONS WHERE Id = :Id", parameters: new DataParameters(1)
            //    // {
            //    //     {"Id", 1}
            //    // });

            //    var getResult = db.Get(new LogMessage { Id = 1 });
            //}
            //catch (Exception exp)
            //{
            //    System.Console.WriteLine(exp.ToString());
            //}
        }
    }

    [TableName("CAREER_BOT_ENTITIES_EXCEPTIONS")]
    public sealed class LogMessage
    {
        // [OracleSequence(SequenceName = "SEQ_CAREER_BOT_ENTITIES_EXCEPTIONS")]
        // public decimal Id { get; set; }

        public string Message { get; set; }
        
        [Key]
        public decimal Id { get; set; }

        public string Detail { get; set; }

        [Column(Name = "CREATE_DATE")]
        public DateTime CreateDate { get; set; }
    }

    [TableName("studentinsert")]
    public class StudentBase
    {
        [OracleSequence(SequenceName = "StudentTable_Sequence")]
        public long Id { get; set; }

        public string Name { get; set; }

        public string Gender { get; set; }

        public decimal Age { get; set; }
    }


    [TableName("ADMIN_USERS")]
    public class AdminUser
    {
        [Key]
        [Column(Name = "ADMIN_USER_ID")]
        public long AdminUserId { get; set; }

        [Column(Name = "USER_NAME")]
        public string UserName { get; set; }

        [Column(Name = "LAST_LOGON_DATE")]
        public DateTime LastLoginDate { get; set; }
    }
}