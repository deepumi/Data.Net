using System;
using Massive;

namespace Data.Net.PerformanceTest
{
    [TableName("Users_Test")] //SQLSERVER
    public sealed class User
    {
        [AutoIncrement]
        public int UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        [Column(IgnoreColumn = true)]
        public string Email { get; set; }
    }

    [TableName("Persons")] //MYSQL
    public sealed class Person
    {
        [AutoIncrement]
        public ulong PersonId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }

    [TableName("teams")]  //ORACLE
    public sealed class Teams
    {
        [OracleSequence(SequenceName = "teams_seq")] //trigger with //[AutoIncrement]
        public decimal Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }

    [TableName("account")]  //NPGSQL
    public sealed class Account
    {
        [Column(IgnoreColumn = true)]
        public string IgnoredColume { get; set; }

        [AutoIncrement, Column(Name = "user_id")]
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}