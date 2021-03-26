using System;

namespace Data.Net
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public sealed class ColumnAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public bool IgnoreColumn { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
    }
}