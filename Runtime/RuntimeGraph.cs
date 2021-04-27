using System.Collections.Generic;

namespace Vampire.Runtime
{
    public class RuntimeGraph
    {
        public readonly RuntimeNode[] nodes;
        public readonly object[] values;
        public readonly Dictionary<string, object> localBlackboard = null;

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
            localBlackboard = blueprint.serializedBlackboard != null ? 
                blueprint.serializedBlackboard.Deserialize() : 
                new Dictionary<string, object>();
            
            nodes = blueprint.nodes;
        }
    }
}