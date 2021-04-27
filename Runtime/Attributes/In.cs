namespace Vampire.Runtime
{
    public class In : PortDefinition
    {
        public In(PortCapacity capacity = PortCapacity.Single, 
            Orientation orientation = Orientation.Horizontal)
        {
            Construct(capacity, orientation);
        }
    }
}