using System.Collections.Generic;
using System.Data.SQLite;
using Xunit;

namespace Data.Net.Test.SqlLite
{
    public sealed class ReaderTest : BaseSqlLite
    {
        private const string PersonTable = nameof(PersonReader);

        public ReaderTest() : base(PersonTable, new SQLiteConnection(SqlLiteConnectionString))
        {
            Connection.Insert(new PersonReader
            {
                Name = "Jon",
                Email = "Jon@email.com"
            });

            Connection.Insert(new PersonReader
            {
                Name = "Jolly",
                Email = "Jolly@email.com"
            });
        }

        [Fact]
        public void Reader_List_Test()
        {
            var list = new List<PersonReader>();

            using (var reader = Connection.ExecuteReader($"SELECT * FROM {PersonTable}"))
            {
                while (reader.Read())
                {
                    list.Add(new PersonReader
                    {
                        Id = reader.GetInt64(0),
                        Name = reader.GetString(1),
                        Email = reader.GetString(2),
                    });
                }
            }

            Assert.True(list.Count == 2);
        }

        [Fact]
        public void Reader_Single_Test()
        {
            PersonReader person = null;

            using (var reader = Connection.ExecuteReader($"SELECT Id,Name,Email FROM {PersonTable} Where Id = @Id",
                parameters: new {Id = 1}))
            {
                if (reader.Read())
                {
                    person = new PersonReader
                    {
                        Id = reader.GetInt64(0),
                        Name = reader.GetString(1),
                        Email = reader.GetString(2),
                    };
                }
            }

            Assert.True(person?.Name == "Jon");
        }
    }
}