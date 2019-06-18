using System.Data;

namespace Data.Net
{
    internal sealed class Parameter
    {
        internal string Name { get; }

        internal DbType DbType { get; }

        internal ParameterDirection Direction { get; }

        internal int Size { get; }

        internal object Value { get; private set; }

        internal Parameter(string name, object value)
        {
            Name = name;
            Value = value;
        }

        internal Parameter(string name, ParameterDirection direction, DbType dbType, int size)
        {
            Name = name;
            Direction = direction;
            DbType = dbType;
            Size = size;
        }

        internal void SetValue(object value) => Value = value;
    }
}