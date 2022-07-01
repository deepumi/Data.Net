using System;

namespace Data.Net
{
    /// <summary>
    /// Attribute class for set the table name.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class TableNameAttribute : Attribute
    {
        /// <summary>
        /// Get table name
        /// </summary>
        public string TableName { get; }

        /// <summary>
        /// Initialize a new TableNameAttribute instance. 
        /// </summary>
        /// <param name="tableName">The name of the table</param>
        public TableNameAttribute(string tableName)
        {
            TableName = tableName;
        }
    }
}