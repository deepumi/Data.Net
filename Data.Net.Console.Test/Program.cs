using System;
using Oracle.ManagedDataAccess.Client;

namespace Data.Net.Console.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var conn = "";

            var message = new LogMessage
            {
                Id = 2,
                Message = "Updated this",
                Detail = "This is just an update",
                CreateDate = DateTime.Now
            };

            try
            {
                using (var db = new Database(new OracleConnection(conn)))
                {
                    var result = db.Delete(message);
                }
            }
            catch (Exception exp)
            {
                System.Console.WriteLine(exp.ToString());
            }
        }
    }

    [TableName("CAREER_BOT_ENTITIES_EXCEPTIONS")]
    public sealed class LogMessage
    {
        [Column(Name = "ID"), OracleSequence(SequenceName = "SEQ_CAREER_BOT_ENTITIES_EXCEPTIONS")]
        public decimal Id { get; set; }

        [Column(Name = "MESSAGE")]
        public string Message { get; set; }

        [Column(Name = "DETAIL")]
        public string Detail { get; set; }

        [Column(Name = "CREATE_DATE")]
        public DateTime CreateDate { get; set; }
    }
}