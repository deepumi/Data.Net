using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Net.Console.Test
{
    [TableName("CAREER_BOT_ENTITIES_EXCEPTIONS")]
    public sealed class LogMessage
    {
        public string Message { get; set; }

        [AutoIncrement(SequenceName = "SEQ_CAREER_BOT_ENTITIES_EXCEPTIONS")]
        public decimal Id { get; set; }

        public string Detail { get; set; }

        [Column(Name = "CREATE_DATE")]
        public DateTime CreateDate { get; set; }
    }

    [TableName("studentinsert")]
    public sealed class StudentBase
    {
        [AutoIncrement(SequenceName = "StudentTable_Sequence")]
        public long Id { get; set; }

        public string Name { get; set; }

        public string Gender { get; set; }

        public decimal Age { get; set; }
    }


    [TableName("ADMIN_USERS")]
    public sealed class AdminUser
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
