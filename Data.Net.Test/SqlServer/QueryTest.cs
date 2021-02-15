using System;
using System.Data;
using System.Data.SqlClient;
using Xunit;

namespace Data.Net.Test.SqlServer
{
    public sealed class QueryTest: IDisposable
    {
        private readonly SqlConnection _connection = new SqlConnection(ConnectionString.SqlServerConnectionString);

        private const string StudentTable = nameof(StudentQuery);

        public QueryTest()
        {
            DropTable();
            
            var table = @$"CREATE TABLE {StudentTable}
                        (
                            Id INT IDENTITY(1,1) PRIMARY KEY,
                            Name VARCHAR(50) NOT NULL,
                            Gender Varchar(15) NOT NULL,
                            Age INT NOT NULL    
                         )";

            _connection.ExecuteNonQuery(table);

            _connection.Insert(new StudentQuery
            {
                Name = "Jon",
                Gender = "Male",
                Age = 25
            });
            
            _connection.Insert(new StudentQuery
            {
                Name = "Jolly",
                Gender = "Female",
                Age = 20
            });
        }

        [Fact] 
        public void Query_Sql_Test()
        {
            var result = _connection.Query<StudentQuery>($"SELECT Id,Name FROM {StudentTable}");
            
            Assert.True(result.Count == 2);
        }
        
        [Fact]
        public void Single_Sql_Test()
        {
            var result = _connection.QuerySingle<StudentQuery>($"SELECT Id,Name FROM {StudentTable} Where Id = @Id",
                parameters: new {Id = 1});
            
            Assert.True(result.Name == "Jon");
        }
        
        [Fact] public void Query_Proc_Test()
        {
            var proc = @$"Create Procedure GetStudents
                    As
                    Begin
                        SELECT * FROM {StudentTable}
                    End";

            _connection.ExecuteNonQuery(proc); //create temp procedure!
            
            var result = _connection.Query<StudentQuery>("GetStudents",CommandType.StoredProcedure);
            
            Assert.True(result.Count == 2);

            _connection.ExecuteNonQuery("DROP PROC GetStudents");
        }
        
        [Fact] 
        public void Single_Proc_Test()
        {
            var proc = @$"Create Procedure GetStudentSingle(@Id int)
                    As
                    Begin
                        SELECT * FROM {StudentTable} WHERE Id = @Id
                    End";

            _connection.ExecuteNonQuery(proc); //create temp procedure!

            var result = _connection.QuerySingle<StudentQuery>("GetStudentSingle", CommandType.StoredProcedure, new {Id = 1});
            
            Assert.True(result.Name == "Jon");
            
            _connection.ExecuteNonQuery("DROP PROC GetStudentSingle");
        }
        
        public void Dispose()
        {
            DropTable();
            _connection.Dispose();
        }
        
        private void DropTable()
        {
            var tableExist = @$"IF OBJECT_ID('{StudentTable}', 'U') IS NOT NULL 
                                DROP TABLE {StudentTable};";
            
            _connection.ExecuteNonQuery(tableExist);
        }
    }
}