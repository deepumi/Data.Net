using System;

namespace Data.Net
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class AutoIncrementAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public string SequenceName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public AutoIncrementAttribute()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sequenceName"></param>
        public AutoIncrementAttribute(string sequenceName)
        {
            SequenceName = sequenceName;
        }
    }
}