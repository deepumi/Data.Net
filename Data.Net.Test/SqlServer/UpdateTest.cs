using System;
using System.Data;
using System.Data.SqlClient;
using Xunit;

namespace Data.Net.Test.SqlServer
{
    public sealed class UpdateTest : IDisposable
    {
        private readonly SqlConnection _database = new SqlConnection(ConnectionString.SqlServerConnectionString);

        private const string StudentTable = nameof(StudentUpdate);

        public UpdateTest()
        {
            Initialize();
        }

        private void Initialize()
        {
            DropTable(); //Drop table if exists!
            
            var table = @$"CREATE TABLE {StudentTable}
                        (
                            Id INT IDENTITY(1,1) PRIMARY KEY,
                            Name VARCHAR(50) NOT NULL,
                            Gender VARCHAR(50) NOT NULL,
                            Age INT NOT NULL
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
                    Name = "Jolly", 
                    Gender = "Female",
                    Age = 25, 
                    Id = 1
                }); 

            Assert.True(result == 1);
        }

        [Fact]
        public void Update_StoredProcedure_Return_Value()
        {
            var proc =
                @$"Create Procedure UpdateStudent (@Name Varchar(50), @Age int, @Gender Varchar(50), @Id int, @ReturnValue int output)
                    As
                    Begin
                        UPDATE {StudentTable} SET Name = @Name, Gender = @Gender,Age = @Age WHERE Id = @Id
                        SELECT @ReturnValue = Id From {StudentTable} Where Name = @Name
                        RETURN @ReturnValue
                    End";

            _database.ExecuteNonQuery(proc); //create temp procedure

            var dataParameters = new DataParameters
            {
                {"@Name", "Jon"},
                {"@Gender", "Male"},
                {"@Age", 25},
                {"@Id", 1}
            };
            dataParameters.AddOutPutParameter("@ReturnValue");

            var result = _database.ExecuteNonQuery("UpdateStudent", CommandType.StoredProcedure, dataParameters);

            var returnValue = dataParameters.Value<int>("@ReturnValue"); //return output parameter StudentId.

            Assert.True(result == 1);

            Assert.True(returnValue == 1);
            
            _database.ExecuteNonQuery("DROP PROC UpdateStudent");
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