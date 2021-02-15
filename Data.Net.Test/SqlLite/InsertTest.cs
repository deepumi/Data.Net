using Microsoft.Data.Sqlite;
using Xunit;

namespace Data.Net.Test.SqlLite
{
    public sealed class InsertTest : BaseSqlLite
    {
        private const string PersonTable = nameof(PersonInsert);

        public InsertTest() : base(PersonTable, new SqliteConnection(SqlLiteConnectionString))
        {
        }

        [Fact]
        public void Insert_POCO_With_Identity_Return()
        {
            var result = Connection.Insert(new PersonInsert {Name = "Jon", Email = "jon@email.com"});

            Assert.True(result.Id == 1);
        }

        [Fact]
        public void Insert_Sql()
        {
            var result = Connection.ExecuteNonQuery($"INSERT INTO {PersonTable} (Name,Email) VALUES('Jon','jon@email.com')");

            Assert.True(result == 1);
        }
        
        [Fact]
        public void Insert_Parameterized_Sql()
        {
            var result = Connection.ExecuteNonQuery($"INSERT INTO {PersonTable} (Name,Email)  VALUES(@Name,@Email)",
                parameters: new {Name = "Jon", Email = "Jon@email.com"});

            Assert.True(result == 1);
        }
    }
}