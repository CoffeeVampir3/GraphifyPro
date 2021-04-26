namespace Vampire.Runtime
{
    public class RuntimeGraph
    {
        public readonly RuntimeNode[] nodes;
        public object[] values;

        public RuntimeGraph(RuntimeGraphBlueprint blueprint)
        {
            values = new object[blueprint.initializationValues.Length];
            blueprint.initializationValues.CopyTo(values, 0);
            nodes = blueprint.nodes;
        }
    }
}