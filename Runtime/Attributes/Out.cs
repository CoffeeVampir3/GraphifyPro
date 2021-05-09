namespace Vampire.Runtime
{
    public class Out : PortDefinition, IShowInNodeInspector
    {
        public Out(PortCapacity capacity = PortCapacity.Single,
            Orientation orientation = Orientation.Horizontal,
            bool showBackingValue = true)
        {
            Construct(capacity, orientation, showBackingValue);
        }
    }
}