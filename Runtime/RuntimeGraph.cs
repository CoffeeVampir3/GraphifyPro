using System.Collections.Generic;

namespace Vampire.Runtime
{
    public class RuntimeGraph
    {
        public readonly RuntimeNode[] nodes;
        public readonly object[] values;
        public readonly Dictionary<string, object> localBlackboard = null;

        public RuntimeGraph(RuntimeGraphBlueprint blueprint)
        {
            values = new object[blueprint.initializationValues.Length];
            blueprint.initializationValues.CopyTo(values, 0);
            localBlackboard = blueprint.serializedBlackboard != null ? 
                blueprint.serializedBlackboard.Deserialize() : 
                new Dictionary<string, object>();
            
            nodes = blueprint.nodes;
        }
    }
}