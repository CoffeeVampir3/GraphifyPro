namespace Vampire.Runtime
{
    public class RuntimeGraph
    {
        public readonly RuntimeNode[] nodes;
        public readonly object[] values;
        public PropertyDictionary properties;
        internal static RuntimeGraph current;

        //Copies our value array in a way that preserves the allocation wrappers.
        private void DeepCopyValueWrappers(ref object[] origValues)
        {
            for (var index = 0; index < origValues.Length; index++)
            {
                var item = origValues[index];
                if (item is AntiAllocationWrapper wrapper)
                {
                    values[index] = wrapper.CloneWrapper();
                }
            }
        }

        public RuntimeGraph(RuntimeGraphBlueprint blueprint)
        {
            values = new object[blueprint.initializationValues.Length];
            //Shallow copy objects
            blueprint.initializationValues.CopyTo(values, 0);
            //Deep copy wrapper pointers
            DeepCopyValueWrappers(ref blueprint.initializationValues);
            //Copies the local properties using the same shallow/deep wrapper as the values.
            properties = blueprint.localProperties.Copy();
            
            nodes = blueprint.nodes;
        }
    }
}