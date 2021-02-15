using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace Data.Net.Test.Oracle
{
    public abstract class BaseOracle : IDisposable
    {
        protected readonly IDbConnection Connection = new OracleConnection(ConnectionString.OracleConnectionString);

        private readonly string _tableName;

        internal const string SequenceName = "StudentTable_Sequence";

        protected BaseOracle(string tableName)
        {
            _tableName = tableName;

            DropTable();

            Connection.ExecuteNonQuery(@$"CREATE TABLE {_tableName}
                        (
                            ID NUMBER(10),
                            Name VARCHAR(50) NOT NULL,
                            Gender VARCHAR(50) NOT NULL,
                            Age NUMBER NOT NULL    
                         )");

            Connection.ExecuteNonQuery(@$"CREATE SEQUENCE {SequenceName}
                           MINVALUE 1
                           MAXVALUE 1000
                           START WITH 1
                           INCREMENT BY 1
                           CACHE 5");
        }

        public void Dispose()
        {
            DropTable();
            Connection.Dispose();
        }

        private void DropTable()
        {
            var dropTable = $@"BEGIN
                              EXECUTE IMMEDIATE 'DROP TABLE {_tableName}';
                            EXCEPTION
                              WHEN OTHERS THEN
                                NULL;
                            END;";
            
            var dropSequence = $@"BEGIN
                                  EXECUTE IMMEDIATE 'DROP SEQUENCE {SequenceName}';
                                EXCEPTION
                                  WHEN OTHERS THEN
                                    NULL;
                                END;";
                
            Connection.ExecuteNonQuery(dropTable);
            
            Connection.ExecuteNonQuery(dropSequence);
        }
    }
}