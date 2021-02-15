using Microsoft.Data.Sqlite;
using Xunit;

namespace Data.Net.Test.SqlLite
{
    public sealed class UpdateTest : BaseSqlLite
    {
        private const string PersonTable = nameof(PersonUpdate);

        public UpdateTest()  : base(PersonTable, new SqliteConnection(SqlLiteConnectionString))
        {
        }

        private void CreateRecord()
        {
            Connection.Insert(new PersonUpdate
            {
                Name = "Jolly",
                Email = "Jolly@email.com"
            });    
        }
        
        [Fact]
        public void Update_Sql()
        {
            CreateRecord();
            
            var result = Connection.Update(PersonTable, "Name = 'Jolly', Email = 'Jolly@email.com'", "Id = 1", null);

            Assert.True(result == 1);
        }

        [Fact]
        public void Update_Parameterized_Sql()
        {
            CreateRecord();
                
            var result = Connection.Update(PersonTable, "Name = @Name, Email = @Email", "Id = @Id",
                new
                {
                    Name = "Jolly", Email = "Jolly@email.com", Id = 1
                }); 

            Assert.True(result == 1);
        }
    }
}