namespace Vampire.Runtime
{
    public class Out : PortDefinition
    {
        public Out(PortCapacity capacity = PortCapacity.Single,
            Orientation orientation = Orientation.Horizontal)
        {
            Construct(capacity, orientation);
        }
    }
}