using System;
using System.Data;

namespace Data.Net
{
    public interface IOutputParameter
    {
        IDbDataParameter this[int index] { get; }
        IDbDataParameter this[string name] { get; }

        bool GetBoolean(string name);
        DateTime? GetDateTime(string name);
        decimal GetDecimal(string name);
        double GetDouble(string name);
        float GetFloat(string name);
        Guid? GetGuid(string name);
        short GetInt16(string name);
        int GetInt32(string name);
        long GetInt64(string name);
        string GetString(string name);
        object GetValue(string name);
        T Value<T>(string name);
    }
}