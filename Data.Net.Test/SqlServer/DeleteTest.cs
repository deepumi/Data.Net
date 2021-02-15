using System;
using System.Data;
using System.Data.SqlClient;
using Xunit;

namespace Data.Net.Test.SqlServer
{
    public class DeleteTest : IDisposable
    {
        private readonly SqlConnection _database = new SqlConnection(ConnectionString.SqlServerConnectionString);

        private const string StudentTable = nameof(StudentDelete);

        public DeleteTest()
        {
           var table = @$"CREATE TABLE {StudentTable}
                        (
                            Id INT IDENTITY(1,1) PRIMARY KEY,
                            Name VARCHAR(50) NOT NULL,
                            Gender VARCHAR(50) NOT NULL,
                            Age INT NOT NULL
                         )";

            _database.ExecuteNonQuery(table);

            _database.Insert(new StudentDelete
            {
                Name = "Jolly",
                Gender = "Female",
                Age = 25
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
            var proc = @$"Create Procedure DeleteStudent (@Id int, @ReturnValue VARCHAR(15) OUTPUT)
                    As
                    Begin
                        DELETE FROM {StudentTable} WHERE Id = @Id
                        SET @ReturnValue = 'DELETED'
                    End";

            _database.ExecuteNonQuery(proc); //create temp procedure

            var dataParameters = new DataParameters
            {
                {"@Id", 1},
                {"@ReturnValue", ParameterDirection.Output, DbType.String, 15}
            };

            _database.ExecuteNonQuery("DeleteStudent", CommandType.StoredProcedure, dataParameters);

            var returnValue = dataParameters.Value<string>("@ReturnValue"); //return output parameter StudentId.

            Assert.True(returnValue == "DELETED");
            
            _database.ExecuteNonQuery("DROP PROC DeleteStudent");
        }

        public void Dispose()
        {
            DropTable();
            _database.Dispose();
        }
        
        private void DropTable()
        {
            var tableExist = @$"IF OBJECT_ID('{StudentTable}', 'U') IS NOT NULL 
                                DROP TABLE {StudentTable};";
            
            _database.ExecuteNonQuery(tableExist);
        }
    }
}