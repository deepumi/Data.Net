using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace Data.Net
{
    /// <inheritdoc />
    /// <summary>
    /// Initializes a new <see cref="DataParameters" /> class
    /// </summary>
    public sealed class DataParameters : IEnumerable<object>
    {
        private const int DefaultCapacity = 4;

        private readonly List<object> _parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataParameters"/> class that is empty and has the initial capacity as 4.
        /// </summary>
        public DataParameters() : this(DefaultCapacity) { }

        /// <summary>
        /// Intialize a new instance of the <see cref="DataParameters"/> class that is empty and has the specified initial capacity.
        /// </summary>
        /// <param name="capacity">The number of required parameters can initially store.</param>
        public DataParameters(int capacity)
        {
            if (capacity <= 0) capacity = DefaultCapacity;

            _parameters = new List<object>(capacity);
        }

        /// <summary>
        /// Returns database output or return parameter to <see cref="IDbDataParameter"/>.  
        /// </summary>
        /// <param name="index">Array index</param>
        /// <returns></returns>
        public IDbDataParameter this[int index]
        {
            get
            {
                if (index < 0 || index > _parameters.Count - 1) return default;

                return _parameters[index] switch
                {
                    Parameter p when IsOutPutOrReturnParameter(p.Direction) => p.DbParameter,
                    IDbDataParameter dp when IsOutPutOrReturnParameter(dp.Direction) => dp,
                    _ => default
                };
            }
        }

        /// <summary>
        /// Returns database output or return parameter to <see cref="IDbDataParameter"/>.
        /// </summary>
        /// <param name="name">Output parameter name</param>
        /// <returns></returns>
        public IDbDataParameter this[string name] => GetDbParameter(name);

        /// <summary>
        /// Add IDbDataParameter type object. <see cref="IDbDataParameter"/>
        /// </summary>
        /// <param name="parameter"></param>
        public void Add(IDbDataParameter parameter)
        {
            if (parameter == null) Throw(nameof(parameter));

            _parameters.Add(parameter);
        }

        /// <summary>
        /// Add input parameter name and value.
        /// </summary>
        /// <param name="name">Sql parameter name</param>
        /// <param name="value">Value of the parameter</param>
        public void Add(string name, object value)
        {
            if (name == null) Throw(nameof(name));

            _parameters.Add(new Parameter(name, value));
        }

        /// <summary>
        /// Add parameter name and specify parameter direction. Default is Output parameter.
        /// </summary>
        /// <param name="name">Name of the parameter</param>
        /// <param name="direction">Parameter direction one of the value of <see cref="ParameterDirection"/></param>
        /// <param name="dbType">Parameter type <see cref="DbType"/></param>
        /// <param name="size">Parameter size. Datatype size.</param>
        public void Add(string name, ParameterDirection direction = ParameterDirection.Output,
            DbType dbType = DbType.Int32, int size = 0)
        {
            if (name == null) Throw(nameof(name));

            _parameters.Add(new Parameter(name, direction, dbType, size));
        }

        /// <summary>
        /// Returns database output or return parameter value.
        /// </summary>
        /// <typeparam name="T">Type of the parameter</typeparam>
        /// <param name="name">Name of the parameter</param>
        /// <returns></returns>
        public T Value<T>(string name)
        {
            var parameter = GetDbParameter(name);

            return parameter?.Value == null ? default : parameter.Value.ToValue<T>();
        }
        
        private IDbDataParameter GetDbParameter(string name)
        {
            if (name == null) return default;

            for (var i = _parameters.Count - 1; i >= 0; i--)
            {
                switch (_parameters[i])
                {
                    case Parameter p when IsOutPutOrReturnParameter(p.Direction) && 
                                          string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase):
                        return p.DbParameter;

                    case IDbDataParameter dp when IsOutPutOrReturnParameter(dp.Direction) && 
                                                  string.Equals(dp.ParameterName, name, StringComparison.OrdinalIgnoreCase):
                        return dp;
                }
            }
 
            return default;
        }

        /// <inheritdoc />
        /// <summary>
        /// Return enumerator object.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<object> GetEnumerator() => _parameters.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
         
        private static void Throw(string parameter) => throw new ArgumentNullException(parameter);
        
        private static bool IsOutPutOrReturnParameter(ParameterDirection direction)
            => direction != 0 && direction != ParameterDirection.Input;
    }
}