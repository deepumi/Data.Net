using System;

namespace Data.Net
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class AutoIncrementAttribute : Attribute
    {
    }
    
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class OracleSequenceAttribute : AutoIncrementAttribute
    {
        public string SequenceName { get; set; }
    }
}