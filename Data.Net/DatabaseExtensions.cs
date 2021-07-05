using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Data.Net
{
    /// <summary>
    /// 
    /// </summary>
    public static class DatabaseExtensions
    {
        /// <summary>
        /// Executes a Transact-SQL statement and returns the number of rows affected by an insert,update or delete.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="sql">Transact-SQL statement</param>
        /// <param name="commandType">One of the command type of <see cref="CommandType" /></param>
        /// <param name="parameters">Parameter collection of <see cref="DataParameters" /> class.</param>
        /// <returns></returns>
        public static int ExecuteNonQuery(this IDbConnection conn, string sql,
            CommandType commandType = CommandType.Text,
            object parameters = null) =>
            new Database(conn).ExecuteNonQuery(sql, commandType, parameters);

        /// <summary>
        /// Asynchronous version of Executes a Transact-SQL statement and returns the number of rows affected by an insert,update or delete.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="sql">Transact-SQL statement</param>
        /// <param name="commandType">One of the command type of <see cref="CommandType" /></param>
        /// <param name="parameters">Parameter collection of <see cref="DataParameters" /> class.</param>
        /// <param name="token">Cancellation token <see cref="CancellationToken" /></param>
        /// <returns></returns>
        public static Task<int> ExecuteNonQueryAsync(this IDbConnection conn, string sql,
            CommandType commandType = CommandType.Text,
            object parameters = null, CancellationToken token = default) =>
            new Database(conn).ExecuteNonQueryAsync(sql, commandType, parameters, token);

        /// <summary>
        /// Executes a Transact-SQL statement and returns a single value from the first row/column.
        /// </summary>
        /// <typeparam name="T">The return value type</typeparam>
        /// <param name="conn"></param>
        /// <param name="sql">Transact-SQL statement</param>
        /// <param name="commandType">One of the command type of <see cref="CommandType" /></param>
        /// <param name="parameters">Parameter collection of <see cref="DataParameters" /> class.</param>
        /// <returns></returns>
        public static T ExecuteScalar<T>(this IDbConnection conn, string sql,
            CommandType commandType = CommandType.Text,
            object parameters = null) =>
            new Database(conn).ExecuteScalar<T>(sql, commandType, parameters);

        /// <summary>
        /// Executes a Transact-SQL statement and returns a single value from the first row/column.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="sql">Transact-SQL statement</param>
        /// <param name="commandType">One of the command type of <see cref="CommandType" /></param>
        /// <param name="parameters">Parameter collection of <see cref="DataParameters" /> class.</param>
        /// <returns></returns>
        public static object ExecuteScalar(this IDbConnection conn, string sql,
            CommandType commandType = CommandType.Text,
            object parameters = null) =>
            new Database(conn).ExecuteScalar(sql, commandType, parameters);

        /// <summary>
        /// Asynchronous version of Executes a Transact-SQL statement and returns a single value from the first row/column.
        /// </summary>
        /// <typeparam name="T">The return value type</typeparam>
        /// <param name="conn"></param>
        /// <param name="sql">Transact-SQL statement</param>
        /// <param name="commandType">One of the command type of <see cref="CommandType" /></param>
        /// <param name="parameters">Parameter collection of <see cref="DataParameters" /> class.</param>
        /// <param name="token">Cancellation token <see cref="CancellationToken" /></param>
        /// <returns></returns>
        public static Task<T> ExecuteScalarAsync<T>(this IDbConnection conn, string sql,
          CommandType commandType = CommandType.Text,
          object parameters = null, CancellationToken token = default) =>
          new Database(conn).ExecuteScalarAsync<T>(sql, commandType, parameters, token);

        /// <summary>
        /// Execute a Transact-SQL statement and return a collection of <see cref="IDataReader" />
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="sql">Transact-SQL statement</param>
        /// <param name="commandType">One of the command type of <see cref="CommandType" /></param>
        /// <param name="parameters">Parameter collection of <see cref="DataParameters" /> class.</param>
        /// <param name="behavior">One of the command behavior <see cref="CommandBehavior" /></param>
        /// <returns></returns>
        public static IDataReader ExecuteReader(this IDbConnection conn, string sql,
            CommandType commandType = CommandType.Text,
            object parameters = null, CommandBehavior behavior = CommandBehavior.CloseConnection) =>
            new Database(conn).ExecuteReader(sql, commandType, parameters, behavior);

        /// <summary>
        /// Asynchronous version of Execute a Transact-SQL statement and return a collection of <see cref="IDataReader" />
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="sql">Transact-SQL statement</param>
        /// <param name="commandType">One of the command type of <see cref="CommandType" /></param>
        /// <param name="parameters">Parameter collection of <see cref="DataParameters" /> class.</param>
        /// <param name="behavior">One of the command behavior <see cref="CommandBehavior" /></param>
        /// <param name="token">Cancellation token <see cref="CancellationToken" /></param>
        /// <returns></returns>
        public static Task<IDataReader> ExecuteReaderAsync(this IDbConnection conn, string sql,
            CommandType commandType = CommandType.Text,
            object parameters = null, CommandBehavior behavior = CommandBehavior.CloseConnection,
            CancellationToken token = default) =>
            new Database(conn).ExecuteReaderAsync(sql, commandType, parameters, behavior, token);

        /// <summary>
        /// Execute a Transact-SQL statement and return a collection of <see cref="List{T}" />
        /// </summary>
        /// <typeparam name="T">The return object type</typeparam>
        /// <param name="conn"></param>
        /// <param name="sql">Transact-SQL statement</param>
        /// <param name="commandType">One of the command type of <see cref="CommandType" /></param>
        /// <param name="parameters">Parameter collection of <see cref="DataParameters" /> class.</param>
        /// <param name="behavior">One of the command behavior <see cref="CommandBehavior" /></param>
        /// <returns></returns>
        public static List<T> Query<T>(this IDbConnection conn, string sql, CommandType commandType = CommandType.Text,
            object parameters = null,
            CommandBehavior behavior = CommandBehavior.CloseConnection) =>
            new Database(conn).Query<T>(sql, commandType, parameters, behavior);

        /// <summary>
        /// Execute a Transact-SQL statement and return a single object
        /// </summary>
        /// <typeparam name="T">The return object type</typeparam>
        /// <param name="conn"></param>
        /// <param name="sql">Transact-SQL statement</param>
        /// <param name="commandType">One of the command type of <see cref="CommandType" /></param>
        /// <param name="parameters">Parameter collection of <see cref="DataParameters" /> class.</param>
        /// <param name="behavior">One of the command behavior <see cref="CommandBehavior" /></param>
        /// <returns></returns>
        public static T QuerySingle<T>(this IDbConnection conn, string sql, CommandType commandType = CommandType.Text,
            object parameters = null,
            CommandBehavior behavior = CommandBehavior.CloseConnection) =>
            new Database(conn).QuerySingle<T>(sql, commandType, parameters, behavior);

        /// <summary>
        /// Asynchronous version of Execute a Transact-SQL statement and return a single object
        /// </summary>
        /// <typeparam name="T">The return object type</typeparam>
        /// <param name="conn"></param>
        /// <param name="sql">Transact-SQL statement</param>
        /// <param name="commandType">One of the command type of <see cref="CommandType" /></param>
        /// <param name="parameters">Parameter collection of <see cref="DataParameters" /> class.</param>
        /// <param name="behavior">One of the command behavior <see cref="CommandBehavior" /></param>
        /// <param name="token">Cancellation token <see cref="CancellationToken" /></param>
        /// <returns></returns>
        public static Task<T> QuerySingleAsync<T>(this IDbConnection conn, string sql,
            CommandType commandType = CommandType.Text, object parameters = null,
            CommandBehavior behavior = CommandBehavior.CloseConnection, CancellationToken token = default) =>
            new Database(conn).QuerySingleAsync<T>(sql, commandType, parameters, behavior, token);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <param name="whereClause"></param>
        /// <param name="orderByClause"></param>
        /// <param name="pageSize"></param>
        /// <param name="currentPage"></param>
        /// <returns></returns>
        public static PaginationResult<TEntity> PagedQuery<TEntity>(this IDbConnection conn, string sql, string whereClause = default, 
            string orderByClause = default, int pageSize = 10, int currentPage = 1) => new Database(conn).PagedQuery<TEntity>(sql, whereClause, orderByClause, pageSize, currentPage);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="conn"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static TEntity Insert<TEntity>(this IDbConnection conn, TEntity entity) where TEntity : class =>
            new Database(conn).Insert(entity);
            
        /// <summary>
        /// 
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="tableName"></param>
        /// <param name="setStatement"></param>
        /// <param name="whereClause"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static int Update(this IDbConnection conn, string tableName, string setStatement, string whereClause, object parameters) =>
            new Database(conn).Update(tableName,setStatement, whereClause,parameters);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="tableName"></param>
        /// <param name="whereClause"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static int Delete(this IDbConnection conn, string tableName, string whereClause, object parameters) => new Database(conn).Delete(tableName,whereClause,parameters);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="entity"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static bool Delete<TEntity>(this IDbConnection conn, TEntity entity) where TEntity : class => new Database(conn).Delete(entity);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="conn"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static TEntity Get<TEntity>(this IDbConnection conn, TEntity entity) where TEntity : class =>
         new Database(conn).Get(entity);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="conn"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static TEntity Update<TEntity>(this IDbConnection conn, TEntity entity) where TEntity : class =>
         new Database(conn).Update(entity);
    }
}