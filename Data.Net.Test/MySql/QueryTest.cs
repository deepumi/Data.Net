using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using Xunit;

namespace Data.Net.Test.MySql
{
    public class QueryTest: IDisposable
    {
        private readonly MySqlConnection _database = new MySqlConnection(ConnectionString.MySqlConnectionString);

        private const string StudentTable = nameof(StudentQuery);

        public QueryTest()
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

            _database.Insert(new StudentQuery
            {
                Name = "Jolly",
                Gender = "Female",
                Age = 20
            });
            
            _database.Insert(new StudentQuery
            {
                Name = "Jon",
                Gender = "Male",
                Age = 25
            });
        }
        
        [Fact] 
        public void Query_Sql_Test()
        {
            var result = _database.Query<StudentQuery>($"SELECT Id,Name FROM {StudentTable}");
            
            Assert.True(result.Count == 2);
        }
        
        [Fact]
        public void Single_Sql_Test()
        {
            var result = _database.QuerySingle<StudentQuery>($"SELECT Id,Name FROM {StudentTable} Where Id = @Id",
                parameters: new {Id = 2});
            
            Assert.True(result.Name == "Jon");
        }

        [Fact]
        public void Query_StoredProcedure()
        {
            var proc =
                @$" DROP PROCEDURE IF EXISTS GetStudents;    
                    Create Procedure GetStudents()
                    Begin
                         SELECT * FROM {StudentTable};
                    End";

            _database.ExecuteNonQuery(proc);

            var result = _database.Query<StudentQuery>("GetStudents", CommandType.StoredProcedure);
 
            Assert.True(result.Count == 2);
        }
        
        public void Dispose()
        {
            DropTable();
            _database.Dispose();
        }
        
        private void DropTable() => _database.ExecuteNonQuery($"DROP TABLE IF EXISTS {StudentTable};");
    }
}