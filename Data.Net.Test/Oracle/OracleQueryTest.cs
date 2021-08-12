using System.Data;
using Oracle.ManagedDataAccess.Client;
using Xunit;

namespace Data.Net.Test.Oracle
{
    public sealed class OracleQueryTest
    {
        private const string StudentTable = nameof(StudentInsert);

        private const string SequenceName = OracleSequenceHelper.SequenceName;

        private static readonly string SchemaName = AppSettings.Config["OracleSchemaName"];

        [Fact]
        public void Insert_Entity_Return_Sequence_Value()
        {
            using IDbConnection connection = new OracleConnection(ConnectionString.OracleConnectionString);

            try
            {
                CleanUpAndCreate(connection, StudentTable, SequenceName);

                var result = connection.Insert(new StudentInsert
                {
                    Name = "Jon",
                    Gender = "Male",
                    Age = 20
                });

                Assert.True(result.Id == 1);
            }
            finally
            {
                CleanUp(connection, StudentTable, SequenceName);
            }
        }

        [Fact]
        public void Insert_Raw_Sql_Parameterized()
        {
            using IDbConnection connection = new OracleConnection(ConnectionString.OracleConnectionString);

            try
            {
                CleanUpAndCreate(connection, StudentTable, SequenceName);

                var result = connection.ExecuteNonQuery(
                    $"INSERT INTO {StudentTable} (Name,Gender,Age)  VALUES(:Name,:Gender,:Age)",
                    parameters: new DataParameters {{":Name", "Jon"}, {":Gender", "Male"}, {":Age", "25"}});

                Assert.True(result == 1);
            }
            finally
            {
                CleanUp(connection, StudentTable, SequenceName);
            }
        }

        [Fact]
        public void Insert_Execute_Procedure()
        {
            var procedureName = $"{SchemaName}.Student_Insert_Proc";

            using IDbConnection connection = new OracleConnection(ConnectionString.OracleConnectionString);

            try
            {
                CleanUpAndCreate(connection, StudentTable, SequenceName);

                var proc = @$"CREATE OR REPLACE PROCEDURE {procedureName}(
                               pName VARCHAR2, 
                               pGender VARCHAR2,
                               pAge NUMBER,
                               pId OUT NUMBER)
                        AS
                        BEGIN
                            INSERT INTO {StudentTable} (Id,Name,Gender,Age) VALUES ({SequenceName}.NEXTVAL,pName,pGender,pAge);

                            SELECT {SequenceName}.CURRVAL INTO pId FROM DUAL;
                        END;";

                connection.ExecuteNonQuery(proc); //Create proc

                var dp = new DataParameters
                {
                    {"pName", "Jon"},
                    {"pGender", "Male"},
                    {"pAge", "25"},
                    new OracleParameter("pId", OracleDbType.Decimal, ParameterDirection.Output)
                };

                connection.ExecuteNonQuery($"{procedureName}", CommandType.StoredProcedure, dp);

                var id = dp.OutputParameter["pId"]?.Value;

                Assert.True(id != null && id.ToString() == "1");

                connection.ExecuteNonQuery($"DROP PROCEDURE {procedureName}");
            }
            finally
            {
                CleanUp(connection, StudentTable, SequenceName);
            }
        }

        [Fact]
        public void Update_Entity()
        {
            using IDbConnection connection = new OracleConnection(ConnectionString.OracleConnectionString);

            try
            {
                CleanUpAndCreate(connection, StudentTable, SequenceName);

                //insert some data.
                var insertResult = connection.Insert(new StudentInsert
                {
                    Name = "Jon",
                    Gender = "Male",
                    Age = 20
                });

                insertResult.Name = "Rambo"; //update Name property.

                connection.Update(insertResult); //update entity.

                //get entity by Sequence Value or Key attribute value
                var getEntity = connection.Get(new StudentInsert {Id = insertResult.Id});

                //Compare entity Name.
                Assert.True(getEntity.Name == "Rambo");
            }
            finally
            {
                CleanUp(connection, StudentTable, SequenceName);
            }
        }

        [Fact]
        public void Get_Entity()
        {
            using IDbConnection connection = new OracleConnection(ConnectionString.OracleConnectionString);

            try
            {
                CleanUpAndCreate(connection, StudentTable, SequenceName);

                //insert some data.
                var insertResult = connection.Insert(new StudentInsert
                {
                    Name = "Jon",
                    Gender = "Male",
                    Age = 20
                });

                var getResult = connection.Get(new StudentInsert
                    {Id = insertResult.Id}); //get entity by Sequence Value or Key attribute value

                Assert.True(getResult.Name == "Jon" && getResult.Id == 1 && getResult.Age == 20); //Compare entity Name.
            }
            finally
            {
                CleanUp(connection, StudentTable, SequenceName);
            }
        }

        [Fact]
        public void Delete_Entity()
        {
            using IDbConnection connection = new OracleConnection(ConnectionString.OracleConnectionString);

            try
            {
                CleanUpAndCreate(connection, StudentTable, SequenceName);

                //insert some data.
                var insertResult = connection.Insert(new StudentInsert
                {
                    Name = "Jon",
                    Gender = "Male",
                    Age = 20
                });

                var success = connection.Delete(new StudentInsert
                    {Id = insertResult.Id}); //delete entity by Sequence Value or Key attribute value

                Assert.True(success); //Compare entity Name.
            }
            finally
            {
                CleanUp(connection, StudentTable, SequenceName);
            }
        }

        private static void CleanUp(IDbConnection connection, string tableName, string sequenceName)
        {
            var dropTable = $@"BEGIN
                              EXECUTE IMMEDIATE 'DROP TABLE {tableName}';
                            EXCEPTION
                              WHEN OTHERS THEN
                                NULL;
                            END;";

            var dropSequence = $@"BEGIN
                                  EXECUTE IMMEDIATE 'DROP SEQUENCE {sequenceName}';
                                EXCEPTION
                                  WHEN OTHERS THEN
                                    NULL;
                                END;";

            connection.ExecuteNonQuery(dropTable);

            connection.ExecuteNonQuery(dropSequence);
        }

        private static void CreateTableAndSequence(IDbConnection connection, string tableName, string sequenceName)
        {
            connection.ExecuteNonQuery(@$"CREATE TABLE {tableName}
                        (
                            ID NUMBER(10),
                            Name VARCHAR(50) NOT NULL,
                            Gender VARCHAR(50) NOT NULL,
                            Age NUMBER NOT NULL    
                         )");

            connection.ExecuteNonQuery(@$"CREATE SEQUENCE {sequenceName}
                           MINVALUE 1
                           MAXVALUE 1000
                           START WITH 1
                           INCREMENT BY 1
                           CACHE 5");
        }

        private static void CleanUpAndCreate(IDbConnection connection, string tableName, string sequenceName)
        {
            CleanUp(connection, tableName, sequenceName);
            CreateTableAndSequence(connection, tableName, sequenceName);
        }
    }
}