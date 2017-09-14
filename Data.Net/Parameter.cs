using System.Data;

namespace Data.Net
{
    internal sealed class Parameter
    {
        internal string Name { get; set; }

        internal object Value { get; set; }

        internal DbType DbType { get; set; }

        internal ParameterDirection Direction { get; set; }

        internal int Size { get; set; }

        internal bool IsOutputOrReturn { get; set; }
    }
}