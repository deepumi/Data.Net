using System;
using System.Data;

namespace Data.Net
{
    /// <summary>
    /// 
    /// </summary>
    public interface IOutputParameter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        IDbDataParameter this[int index] { get; }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        IDbDataParameter this[string name] { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool GetBoolean(string name);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        DateTime? GetDateTime(string name);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        decimal GetDecimal(string name);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        double GetDouble(string name);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        float GetFloat(string name);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Guid? GetGuid(string name);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        short GetInt16(string name);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        int GetInt32(string name);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        long GetInt64(string name);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        string GetString(string name);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        object GetObject(string name);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T Value<T>(string name);
    }
}