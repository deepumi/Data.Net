using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Dynamic;
using Oracle.ManagedDataAccess.Client;

namespace Data.Net.Console.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var conn = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(Host=172.16.0.150)(Port=1521)))(CONNECT_DATA=(SERVICE_NAME=cosqa)(SERVER = DEDICATED))));User Id=portalowner;Password=pmanager2001;";

            try
            {
                
                var sq = new Database(new SqlConnection("Data Source=HD-FS8T403\\SQLEXPRESS;Initial Catalog=DataNet;Integrated Security=True"));
                var tes = sq.PagedQuery<Test>("SELECT * from Test",null,"ORDER BY Id DESC",10,3);
                
                
                Program p = new Program();
                
                var s = p.BuildPagingQueryPair("SELECT * FROM Test","Id","","Id");
                
                
                using var db = new Database(new OracleConnection(conn));

                var orderBy = @"CASE
                WHEN modify_date IS NOT NULL AND modify_date > Create_date THEN modify_date
                ELSE Create_date
                END DESC";
                
                var pageInfo = db.PagedQuery<ApiException>(@"SELECT * FROM PORTALOWNER.API_EXCEPTIONS", whereClause: "UPPER(STATUS) = UPPER('ACTIVE')",
                    orderByClause: orderBy, pageSize: 2, currentPage: 2);

                // var student = db.Get(new AdminUser { AdminUserId = 109 });

                var message = new LogMessage
                {
                    Message = "Test Message in QA",
                    Detail = "Test detail message in QA",
                    CreateDate = DateTime.Now
                };

                // var result = db.Insert(message);
            }
            catch (Exception exp)
            {
                System.Console.WriteLine(exp.ToString());
            }

        }

        private string PrimaryKeyField;
        private string TableName;
        
        private dynamic BuildPagingQueryPair(string sql = "", string primaryKeyField = "", string whereClause = "", string orderByClause = "", string columns = "*", int pageSize = 20,
            int currentPage = 1)
        {
            var countSQL = string.IsNullOrEmpty(sql) ? string.Format("SELECT COUNT({0}) FROM {1}", PrimaryKeyField, TableName)
                : string.Format("SELECT COUNT({0}) FROM ({1}) AS PagedTable", primaryKeyField, sql);
            var orderByClauseFragment = orderByClause;
            if(string.IsNullOrEmpty(orderByClauseFragment))
            {
                orderByClauseFragment = string.IsNullOrEmpty(primaryKeyField) ? PrimaryKeyField : primaryKeyField;
            }
            var whereClauseFragment = ReadifyWhereClause(whereClause);
            var query = string.Empty;
            if(string.IsNullOrEmpty(sql))
            {
                query = string.Format("SELECT {0} FROM (SELECT ROW_NUMBER() OVER (ORDER BY {1}) AS Row, {0} FROM {2} {3}) AS Paged ", columns, orderByClauseFragment, TableName, 
                    whereClauseFragment);
            }
            else
            {
                query = string.Format("SELECT {0} FROM (SELECT ROW_NUMBER() OVER (ORDER BY {1}) AS Row, {0} FROM ({2}) AS PagedTable {3}) AS Paged ", columns, orderByClauseFragment, sql, 
                    whereClauseFragment);
            }
            var pageStart = (currentPage - 1) * pageSize;
            query += string.Format(" WHERE Row > {0} AND Row <={1}", pageStart, (pageStart + pageSize));
            countSQL += whereClauseFragment;
            dynamic toReturn = new ExpandoObject();
            toReturn.MainQuery = query;
            toReturn.CountQuery = countSQL;
            return toReturn;
        }
        
        private string ReadifyWhereClause(string rawWhereClause)
        {
            return ReadifyClause(rawWhereClause, "WHERE");
        }


        /// <summary>
        /// Readifies the orderby clause specified. If a non-empty/whitespace string is specified, it will make sure it's prefixed with " ORDER BY" including a prefix space.
        /// </summary>
        /// <param name="rawOrderByClause">The raw order by clause.</param>
        /// <returns>
        /// processed rawOrderByClause which will guaranteed contain " ORDER BY" including prefix space.
        /// </returns>
        private string ReadifyOrderByClause(string rawOrderByClause)
        {
            return ReadifyClause(rawOrderByClause, "ORDER BY");
        }


        /// <summary>
        /// Readifies the where clause specified. If a non-empty/whitespace string is specified, it will make sure it's prefixed with the specified operator including a prefix space.
        /// </summary>
        /// <param name="rawClause">The raw clause.</param>
        /// <param name="op">The operator, e.g. "WHERE" or "ORDER BY".</param>
        /// <returns>
        /// processed rawClause which will guaranteed start with op including prefix space.
        /// </returns>
        private string ReadifyClause(string rawClause, string op)
        {
            var toReturn = string.Empty;
            if(rawClause == null)
            {
                return toReturn;
            }
            toReturn = rawClause.Trim();
            if(!string.IsNullOrWhiteSpace(toReturn))
            {
                if(toReturn.StartsWith(op, StringComparison.OrdinalIgnoreCase))
                {
                    toReturn = " " + toReturn;
                }
                else
                {
                    toReturn = string.Format(" {0} {1}", op, toReturn);
                }
            }
            return toReturn;
        }

    }

    
    [TableName("API_EXCEPTIONS")]
    public class ApiException
    {
        public string GUID { get; set; }

        public string MACHINE_NAME { get; set; }

        public DateTime CREATE_DATE { get; set; }
    }

    public class Test
    {
        [AutoIncrement]
        public int Id { get; set; }
        
        public string Name { get; set; }
    }
}