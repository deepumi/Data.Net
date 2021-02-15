using Xunit;
using System.Data.SQLite;

namespace Data.Net.Test.SqlLite
{
    public sealed class DeleteTest : BaseSqlLite
    {
        private const string PersonTable = nameof(PersonDelete);

        public DeleteTest() : base(PersonTable, new SQLiteConnection(SqlLiteConnectionString))
        {
            Connection.Insert(new PersonDelete
            {
                Name = "Jon",
                Email = "Jon@email.com"
            });
        }

        [Fact]
        public void Delete_Sql()
        {
            var result = Connection.Delete(PersonTable, "Id = 1", null);

            Assert.True(result == 1);
        }

        [Fact]
        public void Delete_Parameterized_Sql()
        {
            var result = Connection.Delete(PersonTable, "Email = @Email", new {Email = "Jon@email.com"});

            Assert.True(result == 1);
        }
    }
}