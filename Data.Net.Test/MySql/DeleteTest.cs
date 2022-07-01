using System;
using System.Data;
using MySql.Data.MySqlClient;
using Xunit;

namespace Data.Net.Test.MySql
{
    public sealed class DeleteTest : IDisposable
    {
        private readonly MySqlConnection _database = new MySqlConnection(ConnectionString.MySqlConnectionString);

        private const string StudentTable = nameof(StudentDelete);

        public DeleteTest()
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

            _database.Insert(new StudentDelete
            {
                Name = "Jolly",
                Gender = "Female",
                Age = 20
            });
        }
        
        [Fact]
        public void Delete_Sql()
        {
            var result = _database.Delete(StudentTable, "Id = 1", null); //DELETE FROM StudentTable WHERE Id = 1;

            Assert.True(result == 1);
        }

        [Fact]
        public void Delete_Parameterized_Sql()
        {
            var result = _database.Delete(StudentTable, "Id = @Id AND Name = @Name", new {Id = 1, Name = "Jolly"}); //DELETE FROM {StudentTable} WHERE Id = @Id AND Name = @Name;

            Assert.True(result == 1);
        }
        
        [Fact]
        public void Delete_StoredProcedure_Return_Value()
        {
            var proc = @$"DROP PROCEDURE IF EXISTS DeleteStudent;
                    Create Procedure DeleteStudent (in Id int, out ReturnValue VARCHAR(15))
                    Begin
                        DELETE FROM {StudentTable} WHERE Id = Id;
                        SET ReturnValue = 'DELETED';
                        SELECT ReturnValue;
                    End";

            _database.ExecuteNonQuery(proc); //drop and create procedure

            var dataParameters = new DataParameters
            {
                {"@Id", 1 }
            };
            dataParameters.AddOutPutParameter("@ReturnValue", ParameterDirection.Output, DbType.String, 15);

            _database.ExecuteNonQuery("DeleteStudent", CommandType.StoredProcedure, dataParameters);

            var returnValue = dataParameters.Value<string>("@ReturnValue"); //return output parameter StudentId.

            Assert.True(returnValue == "DELETED");
        }
        
        public void Dispose()
        {
            DropTable();
            _database.Dispose();
        }
        
        private void DropTable() => _database.ExecuteNonQuery($"DROP TABLE IF EXISTS {StudentTable};");
    }
}