using System.Data.SQLite;
using Xunit;

namespace Data.Net.Test.SqlLite
{
    public sealed class QueryTest :  BaseSqlLite
    {
        private const string PersonTable = nameof(PersonQuery);

        public QueryTest() : base(PersonTable, new SQLiteConnection(SqlLiteConnectionString))
        {
            Connection.Insert(new PersonQuery
            {
                Name = "Jon",
                Email = "Jon@email.com"
            });
            
            Connection.Insert(new PersonQuery
            {
                Name = "Jolly",
                Email = "Jolly@email.com"
            });
        }

        [Fact] 
        public void Query_Sql_Test()
        {
            var result = Connection.Query<PersonQuery>($"SELECT * FROM {PersonTable}");
            
            Assert.True(result.Count == 2);
        }
        
        [Fact]
        public void Single_Sql_Test()
        {
            var result = Connection.QuerySingle<PersonQuery>($"SELECT Id,Name FROM {PersonTable} Where Id = @Id",
                parameters: new {Id = 1});
            
            Assert.True(result.Name == "Jon");
        }
    }
}