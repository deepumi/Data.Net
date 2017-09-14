using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace Data.Net
{
    /// <inheritdoc />
    /// <summary>
    /// Initializes a new <see cref="T:Data.Net.DataParameters" /> class
    /// </summary>
    public sealed class DataParameters : IEnumerable<object>
    {
        private readonly List<object> _parameters = new List<object>();

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <returns></returns>
        public IEnumerator<object> GetEnumerator() => _parameters.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// </summary>
        /// <param name="parameter"></param>
        public void Add(IDbDataParameter parameter)
        {
            if (parameter == null) throw new ArgumentNullException(nameof(parameter));

            _parameters.Add(parameter);
        }

        /// <summary>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void Add(string name, object value)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            _parameters.Add(new Parameter {Name = name, Value = value});
        }

        /// <summary>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="direction"></param>
        /// <param name="dbType"></param>
        /// <param name="size"></param>
        public void Add(string name, ParameterDirection direction = ParameterDirection.Output,
            DbType dbType = DbType.Int32, int size = 0)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            _parameters.Add(new Parameter
            {
                Name = name,
                DbType = dbType,
                Size = size,
                Direction = direction,
                IsOutputOrReturn = direction == ParameterDirection.Output ||
                                   direction == ParameterDirection.ReturnValue ||
                                   direction == ParameterDirection.InputOutput
            });
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public T Value<T>(string name)
        {
            if (name == null) return default(T);

            foreach (var item in _parameters)
            {
                switch (item)
                {
                    case Parameter parameter when parameter.IsOutputOrReturn &&
                                                  parameter.Name.Equals(name,
                                                      StringComparison.OrdinalIgnoreCase):
                        return parameter.Value.ToValue<T>();
                    case IDbDataParameter dataParam when
                                        (dataParam.Direction == ParameterDirection.InputOutput ||
                                         dataParam.Direction == ParameterDirection.Output ||
                                         dataParam.Direction == ParameterDirection.ReturnValue) &&
                                        dataParam.ParameterName.Equals(name,
                                        StringComparison.OrdinalIgnoreCase):
                        return dataParam.Value.ToValue<T>();
                }
            }
            return default(T);
        }
    }
}