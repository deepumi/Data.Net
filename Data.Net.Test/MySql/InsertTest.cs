using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using Xunit;

namespace Data.Net.Test.MySql
{
    public class InsertTest : IDisposable
    {
        private readonly Database _database = new Database(new MySqlConnection(ConnectionString.MySqlConnectionString));

        private const string StudentTable = nameof(StudentInsert);

        public InsertTest()
        {
            var table = @$"CREATE TEMPORARY TABLE {StudentTable}
                        (
                            Id INT NOT NULL AUTO_INCREMENT,
                            Name VARCHAR(50) NOT NULL,
                            Gender VARCHAR(50) NOT NULL,
                            Age INT NOT NULL,
                            PRIMARY KEY (Id)
                         )";

            _database.ExecuteNonQuery(table);
        }

        [Fact]
        public void Insert_POCO_With_Identity_Return()
        {
            var student = new StudentInsert
            {
                Name = "Jon",
                Gender = "Male",
                Age = 25
            };

            var result = _database.Insert(student);

            Assert.True(result.StudentId == 1);
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
            var result = _database.ExecuteNonQuery($"INSERT INTO {StudentTable} (Name,Gender,Age) VALUES(@Name,@Gender,@Age)", parameters: new DataParameters { { "@Name", "Jon" }, { "@Gender", "Male" }, { "@Age", 25 } });

            Assert.True(result == 1);
        }


        [Fact]
        public void Insert_AnonymusParameters_Sql()
        {
            var result = _database.ExecuteNonQuery($"INSERT INTO {StudentTable} (Name,Gender,Age) VALUES(@Name,@Gender,@Age)", parameters: new { Name = "Jon", Gender = "Male", Age = 25 });

            Assert.True(result == 1);
        }

        [Fact]
        public void Insert_With_Return_Sequence_Value()
        {
            var parameterDictionary = new Dictionary<string, object>(3)
            {
                ["Name"] = "Jon",
                ["Gender"] = "Male",
                ["Age"] = 25
            };
            
            var result = _database.ExecuteScalar($"INSERT INTO {StudentTable} (Name,Gender,Age) VALUES(@Name,@Gender,@Age); SELECT LAST_INSERT_ID();", parameters: parameterDictionary);

            Assert.True((ulong)result == 1);
        }

        //[Fact]
        //public void Function_ReturnValue_AnonymusParameters()
        //{
        //    var proc = @"CREATE FUNCTION pg_temp.AddNumber(id int)
        //                RETURNS int AS 'SELECT $1 + 1' LANGUAGE sql IMMUTABLE;";

        //    _database.ExecuteNonQuery(proc);

        //    var result = (int) _database.ExecuteScalar("SELECT pg_temp.AddNumber(@id)", parameters: new { id = 50 });

        //    Assert.True(result == 51);
        //}

        public void Dispose()
        {
            DropTable();
            _database.Dispose();
        }
        
        private void DropTable() => _database.ExecuteNonQuery($"DROP TABLE IF EXISTS {StudentTable};");
    }
}
