namespace Vampire.Binding
{
    public class PropertyValue
    {
        public object initialValue;
        public string lookupKey;

        public PropertyValue(object initialValue, string lookupKey)
        {
            this.initialValue = initialValue;
            this.lookupKey = lookupKey;
        }
    }
}