namespace Vampire.Runtime
{
    /// <summary>
    /// A pointer to a value keyed property within the graph value array.
    /// </summary>
    public class ValueKeyedProperty
    {
        public readonly short valueKey;
        public ValueKeyedProperty(short valueKey)
        {
            this.valueKey = valueKey;
        }
    }
}