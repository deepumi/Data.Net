using System;

namespace Data.Net
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class TableNameAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public string TableName { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        public TableNameAttribute(string tableName)
        {
            TableName = tableName;
        }
    }
}