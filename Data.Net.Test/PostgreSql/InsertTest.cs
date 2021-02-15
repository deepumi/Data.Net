using System;
using Npgsql;
using Xunit;

namespace Data.Net.Test.PostgreSql
{
    public sealed class PostgresSqlTest : IDisposable
    {
        private readonly Database _database = new Database(new NpgsqlConnection(ConnectionString.PostgresSqlConnectionString));

        private const string StudentTable = nameof(StudentInsert);

        public PostgresSqlTest()
        {
            var table = @$"CREATE TABLE {StudentTable}
                        (
                            Id INT GENERATED ALWAYS AS IDENTITY,
                            Name VARCHAR(50) NOT NULL,
                            Gender VARCHAR(50) NOT NULL,
                            Age INT NOT NULL    
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

            _database.Insert(student);

            Assert.True(student.StudentId == 1);
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
            var result = _database.ExecuteScalar($"INSERT INTO {StudentTable} (Name,Gender,Age) VALUES(@Name,@Gender,@Age) RETURNING Id", parameters: new { Name = "Jon", Gender = "Male", Age = 25 });

            Assert.True((int) result == 1);
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

        private void DropTable()
        {
            _database.ExecuteNonQuery($"DROP TABLE IF EXISTS {StudentTable}");
        }
    }
}