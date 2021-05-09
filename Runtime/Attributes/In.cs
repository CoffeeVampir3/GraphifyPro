namespace Vampire.Runtime
{
    public class In : PortDefinition, IShowInNodeInspector
    {
        public In(PortCapacity capacity = PortCapacity.Single, 
            Orientation orientation = Orientation.Horizontal,
            bool showBackingValue = true)
        {
            Construct(capacity, orientation, showBackingValue);
        }
    }
}