using System;
using System.Data;
using MySql.Data.MySqlClient;
using Xunit;

namespace Data.Net.Test.MySql
{
    public class UpdateTest : IDisposable
    {
        private readonly MySqlConnection _database = new MySqlConnection(ConnectionString.MySqlConnectionString);

        private const string StudentTable = nameof(StudentUpdate);

        public UpdateTest()
        {
           var table = @$"CREATE TABLE {StudentTable}
                        (
                            Id INT NOT NULL AUTO_INCREMENT,
                            Name VARCHAR(50) NOT NULL,
                            Gender VARCHAR(50) NOT NULL,
                            Age INT NOT NULL,
                            PRIMARY KEY (Id)
                         )";

            _database.ExecuteNonQuery(table);

            _database.Insert(new StudentUpdate
            {
                Name = "Jolly",
                Gender = "Female",
                Age = 20
            });
        }

        [Fact]
        public void Update_Sql()
        {
            var result = _database.Update(StudentTable, "Name = 'Jolly', Gender = 'Female',Age = 20", "Id = 1", null);

            Assert.True(result == 1);
        }

        [Fact]
        public void Update_Parameterized_Sql()
        {
            var result = _database.Update(StudentTable, "Name = @Name, Gender = @Gender,Age = @Age", "Id = @Id",
                new
                {
                    Name = "Jolly", Gender = "Female", Age = 25, Id = 1
                }); //$"UPDATE {StudentTable} SET Name = @Name, Gender = @Gender,Age = @Age WHERE Id = @Id"

            Assert.True(result == 1);
        }

        [Fact]
        public void Update_StoredProcedure_Return_Value()
        {
            var proc =
                @$" DROP PROCEDURE IF EXISTS UpdateStudent;    
                    Create Procedure UpdateStudent (in Name Varchar(50), in Age int, in Gender Varchar(50), in Id int, out ReturnValue int)
                    Begin
                        UPDATE {StudentTable} SET Name = Name, Gender = Gender,Age = Age WHERE Id = Id;
                        SELECT Id INTO ReturnValue From {StudentTable} Where Name = Name;
                    End";

            _database.ExecuteNonQuery(proc);

            var dbParameters = new DataParameters
            {
                {"@Name", "Jon"},
                {"@Gender", "Male"},
                {"@Age", 25},
                {"@Id", 1},
            };
            dbParameters.AddOutPutParameter("@ReturnValue");

            var result = _database.ExecuteNonQuery("UpdateStudent", CommandType.StoredProcedure, dbParameters);

            var returnValue = dbParameters.Value<int>("@ReturnValue"); //return output parameter StudentId.

            Assert.True(result == 1);

            Assert.True(returnValue == 1);
        }

        public void Dispose()
        {
            DropTable();
            _database.Dispose();
        }
        
        private void DropTable() => _database.ExecuteNonQuery($"DROP TABLE IF EXISTS {StudentTable};");
    }
}