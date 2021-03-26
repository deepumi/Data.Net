using System;

namespace Data.Net
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class OracleSequenceAttribute : AutoIncrementAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        public string SequenceName { get; set; }
    }
}