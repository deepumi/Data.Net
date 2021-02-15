using System.Data;
using Oracle.ManagedDataAccess.Client;
using Xunit;

namespace Data.Net.Test.Oracle
{
    public sealed class InsertTest : BaseOracle
    {
        private const string StudentTable = nameof(StudentInsert);

        private const string SchemaName = "PORTALOWNER";
        
        public InsertTest() : base(StudentTable) {}
        
        [Fact]
        public void Insert_POCO_With_Identity_Return()
        {
            var result = Connection.Insert(new StudentInsert
            {
                Name = "Jon",
                Gender = "Male",
                Age = 20
            });

            Assert.True(result.Id == 1);
        }
        
        
        [Fact]
        public void Insert_Parameterized_Sql()
        {
            var result = Connection.ExecuteNonQuery($"INSERT INTO {StudentTable} (Name,Gender,Age)  VALUES(:Name,:Gender,:Age)", parameters: new DataParameters { { ":Name", "Jon" }, { ":Gender", "Male" }, { ":Age", 25 } });

            Assert.True(result == 1);
        }

        [Fact]
        public void ProcedureInsert()
        {
            var proc = @$"CREATE OR REPLACE PROCEDURE {SchemaName}.Student_Insert_Proc(
                               pName VARCHAR2, 
                               pGender VARCHAR2,
                               pAge NUMBER,
                               pId OUT NUMBER)
                        AS
                        BEGIN
                            INSERT INTO {StudentTable} (Id,Name,Gender,Age) VALUES ({SequenceName}.NEXTVAL,pName,pGender,pAge);

                            SELECT {SequenceName}.CURRVAL INTO pId FROM DUAL;
                        END;";

            var re = Connection.ExecuteNonQuery(proc);

            var dp = new DataParameters
            {
                {"pName", "Jon"},
                {"pGender", "Male"},
                {"pAge", 25},
                new OracleParameter("pId", OracleDbType.Decimal, ParameterDirection.Output)
            };

            Connection.ExecuteNonQuery("PORTALOWNER.Student_Insert_Proc", CommandType.StoredProcedure, dp);

            var id = dp["pId"]?.Value;
            
        }
    }
}