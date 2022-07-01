using System;
using System.Data;

namespace Data.Net
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class BaseOutputParameter : IOutputParameter
    {
        /// <summary>
        /// Returns database output or return parameter to <see cref="IDbDataParameter"/>.
        /// </summary>
        /// <param name="name">Output parameter name</param>
        /// <returns></returns>
        public IDbDataParameter this[string name] => GetDbParameter(name);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public abstract IDbDataParameter this[int index] { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected abstract IDbDataParameter GetDbParameter(string name);


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public T Value<T>(string name)
        {
            var parameter = GetDbParameter(name);

            return parameter?.Value == null ? default : parameter.Value.ToValue<T>();
        }

        /// <summary>
        /// Returns specified output value as a Boolean.
        /// </summary>
        /// <param name="name">Name of the output parameter</param>
        /// <returns></returns>
        public bool GetBoolean(string name)
        {
            var value = GetDbParameter(name)?.Value?.ToString();

            return value != null &&
                   (value.Equals("true", StringComparison.OrdinalIgnoreCase) ||
                   value == "1" ||
                   value.Equals("yes", StringComparison.OrdinalIgnoreCase) ||
                   value.Equals("y", StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Returns specified output value as a DateTime.
        /// </summary>
        /// <param name="name">Name of the output parameter</param>
        /// <returns></returns>
        public DateTime? GetDateTime(string name)
        {
            var value = GetDbParameter(name)?.Value?.ToString();

            if (value != null && DateTime.TryParse(value, out var dateTime))
                return dateTime;

            return null;
        }

        /// <summary>
        /// Returns specified output value as a Decimal.
        /// </summary>
        /// <param name="name">Name of the output parameter</param>
        /// <returns></returns>
        public decimal GetDecimal(string name)
        {
            var value = GetDbParameter(name)?.Value?.ToString();

            return value != null && decimal.TryParse(value, out var result) ? result : default;
        }

        /// <summary>
        /// Returns specified output value as a Double.
        /// </summary>
        /// <param name="name">Name of the output parameter</param>
        /// <returns></returns>
        public double GetDouble(string name)
        {
            var value = GetDbParameter(name)?.Value?.ToString();

            return value != null && double.TryParse(value, out var result) ? result : default;
        }

        /// <summary>
        /// Returns specified output value as a Float.
        /// </summary>
        /// <param name="name">Name of the output parameter</param>
        /// <returns></returns>
        public float GetFloat(string name)
        {
            var value = GetDbParameter(name)?.Value?.ToString();

            return value != null && float.TryParse(value, out var result) ? result : default;
        }

        /// <summary>
        /// Returns specified output value as a Guid.
        /// </summary>
        /// <param name="name">Name of the output parameter</param>
        /// <returns></returns>
        public Guid? GetGuid(string name)
        {
            var value = GetDbParameter(name)?.Value?.ToString();

            return value != null && Guid.TryParse(value, out var result) ? result : default(Guid?);
        }

        /// <summary>
        /// Returns specified output value as a Short.
        /// </summary>
        /// <param name="name">Name of the output parameter</param>
        /// <returns></returns>
        public short GetInt16(string name)
        {
            var value = GetDbParameter(name)?.Value?.ToString();

            return value != null && short.TryParse(value, out var result) ? result : default;
        }

        /// <summary>
        /// Returns specified output value as a Integer.
        /// </summary>
        /// <param name="name">Name of the output parameter</param>
        /// <returns></returns>
        public int GetInt32(string name)
        {
            var value = GetDbParameter(name)?.Value?.ToString();

            return value != null && int.TryParse(value, out var result) ? result : default;
        }

        /// <summary>
        /// Returns specified output value as a Long.
        /// </summary>
        /// <param name="name">Name of the output parameter</param>
        /// <returns></returns>
        public long GetInt64(string name)
        {
            var value = GetDbParameter(name)?.Value?.ToString();

            return value != null && long.TryParse(value, out var result) ? result : default;
        }

        /// <summary>
        /// Returns specified output value as a String.
        /// </summary>
        /// <param name="name">Name of the output parameter</param>
        /// <returns></returns>
        public string GetString(string name) => GetDbParameter(name)?.Value?.ToString() ?? string.Empty;

        /// <summary>
        /// Returns specified output value as a Object.
        /// </summary>
        /// <param name="name">Name of the output parameter</param>
        /// <returns></returns>
        public object GetObject(string name) => GetDbParameter(name)?.Value;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        protected static bool IsOutPutOrReturnParameter(ParameterDirection direction)
            => direction != 0 && direction != ParameterDirection.Input;
    }
}