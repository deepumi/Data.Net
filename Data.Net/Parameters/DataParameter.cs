using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace Data.Net
{
    /// <summary>
    /// Initializes a new <see cref="DataParameters" /> class
    /// </summary>
    public sealed class DataParameters : IEnumerable<object>, IDisposable
    {
        private const int DefaultCapacity = 4;

        private readonly List<object> _parameters;

        private bool _disposed;

        internal readonly IList<IDbDataParameter> DbParameters;

        /// <summary>
        /// Holds output or return command parameters name and values.
        /// </summary>
        public IOutputParameter OutputParameter { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataParameters"/> class that is empty and has the initial capacity as 4.
        /// </summary>
        public DataParameters() : this(DefaultCapacity) { }

        /// <summary>
        /// Initialize a new instance of the <see cref="DataParameters"/> class that is empty and has the specified initial capacity.
        /// </summary>
        /// <param name="capacity">The number of required parameters can initially store.</param>
        public DataParameters(int capacity)
        {
            if (capacity <= 0) capacity = DefaultCapacity;

            _parameters = new List<object>(capacity);

            OutputParameter = new OutputParameter(_parameters);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        public DataParameters(IEnumerable<IDbDataParameter> parameters)
        {
            DbParameters = new List<IDbDataParameter>(parameters);
            OutputParameter = new OutputDbParameter(DbParameters);
        }

        /// <summary>
        /// Add IDbDataParameter type object. <see cref="IDbDataParameter"/>
        /// </summary>
        /// <param name="parameter"></param>
        public void Add(IDbDataParameter parameter)
        {
            if (parameter == null) ThrowHelper.Throw(nameof(parameter));

            _parameters.Add(parameter);
        }

        /// <summary>
        /// Add input parameter name and value.
        /// </summary>
        /// <param name="name">Sql parameter name</param>
        /// <param name="value">Value of the parameter</param>
        public void Add(string name, object value)
        {
            if (name == null) ThrowHelper.Throw(nameof(name));

            _parameters.Add(new Parameter(name, value));
        }

        /// <summary>
        /// Add parameter name and specify parameter direction. Default is Output parameter.
        /// </summary>
        /// <param name="name">Name of the parameter</param>
        /// <param name="direction">Parameter direction one of the value of <see cref="ParameterDirection"/></param>
        /// <param name="dbType">Parameter type <see cref="DbType"/></param>
        /// <param name="size">Parameter size. Datatype size.</param>
        public void AddOutPutParameter(string name, ParameterDirection direction = ParameterDirection.Output,
            DbType dbType = DbType.Int32, int size = 0)
        {
            if (name == null) ThrowHelper.Throw(nameof(name));

            _parameters.Add(new Parameter(name, direction, dbType, size));
        }

        /// <summary>
        /// Returns database output or return parameter value.
        /// </summary>
        /// <typeparam name="T">Type of the parameter</typeparam>
        /// <param name="name">Name of the parameter</param>
        /// <returns></returns>
        public T Value<T>(string name) => OutputParameter.Value<T>(name);

        /// <inheritdoc />
        /// <summary>
        /// Return enumerator object.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<object> GetEnumerator() => _parameters.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;
            
            _parameters?.Clear();
            DbParameters?.Clear();
            _disposed = true;
        }
    }
}