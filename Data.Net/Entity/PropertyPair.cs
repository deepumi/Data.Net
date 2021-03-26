namespace Data.Net
{
    internal sealed class PropertyPair
    {
        internal readonly string Name;

        internal readonly object Value;

        internal PropertyPair(string name, object value)
        {
            Name = name;
            Value = value;
        }
    }
}