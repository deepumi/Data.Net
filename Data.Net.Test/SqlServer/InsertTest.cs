using System;
using System.Data;
using System.Data.SqlClient;
using Xunit;

namespace Data.Net.Test.SqlServer
{
    public sealed class InsertTest : IDisposable
    {
        private readonly Database _database = new Database(new SqlConnection(ConnectionString.SqlServerConnectionString));

        private const string StudentTable = nameof(StudentInsert);

        public InsertTest() => CreateTable();

        [Fact]
        public void Insert_Model_With_Identity_Return()
        {
            var student = new StudentInsert
            {
                Name = "Jon",
                Gender = "Male",
                Age = 25
            };

            _database.Insert(student);

            Assert.True(student.Id == 1);
        }

        [Fact]
        public void Insert_Sql()
        {
            var result = _database.ExecuteNonQuery($"INSERT INTO {StudentTable} (Name,Gender,Age) VALUES('Jon','Male',25)");

            Assert.True(result == 1);
        }

        [Fact]
        public void Insert_Parameterized_Sql()
        {
            var result = _database.ExecuteNonQuery($"INSERT INTO {StudentTable}  VALUES(@Name,@Gender,@Age)", parameters: new DataParameters { { "@Name", "Jon" }, { "@Gender", "Male" }, { "@Age", 25 } });

            Assert.True(result == 1);
        }

        [Fact]
        public void Insert_StoredProcedure_AnonymusParameters()
        {
            var proc = @$"Create Procedure #InsertStudent(@Name Varchar(50), @Age int, @Gender Varchar(50))
                    As
                    Begin
                        Insert Into {StudentTable} Values(@Name, @Gender, @Age)
                    End";

            _database.ExecuteNonQuery(proc); //create temp procedure!

            var result = _database.ExecuteNonQuery("#InsertStudent", CommandType.StoredProcedure, new { Name = "Jon", Gender = "Male", Age = 25 });

            Assert.True(result == 1);
        }

        [Fact]
        public void Insert_StoredProcedure_Return_Identity_Value()
        {
            var proc = @$"Create Procedure #InsertStudent (@Name Varchar(50), @Age int, @Gender Varchar(50), @StudentId int output)
                    As
                    Begin
                        Insert Into {StudentTable} Values(@Name, @Gender, @Age)
                        SET @StudentId = SCOPE_IDENTITY()
                        RETURN @StudentId
                    End";

            _database.ExecuteNonQuery(proc); //create temp procedure!

            var dataParameters = new DataParameters { { "@Name", "Jon" }, { "@Gender", "Male" }, { "@Age", 25 }, "@StudentId" };

            var result = _database.ExecuteNonQuery("#InsertStudent", CommandType.StoredProcedure, dataParameters);

            var studentId = dataParameters.Value<int>("@StudentId"); //return output parameter StudentId.

            //another way to get Identity return value.
            //object studentId =  dataParameters[3]?.Value; //OR dataParameters["@StudentId"]?.Value; 
            Assert.True(result == 1);

            Assert.True(studentId > 0);
        }

        private void DropTable()
        {
            var tableExist = @$"IF OBJECT_ID('{StudentTable}', 'U') IS NOT NULL 
                                DROP TABLE {StudentTable};";
            
            _database.ExecuteNonQuery(tableExist);
        }

        private void CreateTable()
        {
            DropTable();
            var table = @$"CREATE TABLE {StudentTable}
                        (
                            Id INT IDENTITY(1,1) PRIMARY KEY,
                            Name VARCHAR(50) NOT NULL,
                            Gender VARCHAR(50) NOT NULL,
                            Age INT NOT NULL    
                         )";

            _database.ExecuteNonQuery(table);
        }
        
        public void Dispose()
        {
            DropTable();
            _database.Dispose();
        }
    }
}