using System;

namespace Vampire.Runtime
{
    [Serializable]
    public class GraphEvaluator
    {
        public RuntimeGraphBlueprint blueprint;
        
        private int rootNodeIndex = 0;
        private RuntimeGraph rtGraph;
        private RuntimeNode currentNode;
        private RuntimeNode nextNode = null;
        private Context rootContext;

        public void Initialize()
        {
            rtGraph = blueprint.CreateRuntimeGraph();
            currentNode = rtGraph.nodes[rootNodeIndex];
            rootContext = new Context(rtGraph);
        }

        public bool Step()
        {
            if (currentNode == null)
                return false;
            CurrentEvaluation.currentGraph = rtGraph;
            nextNode = currentNode.Evaluate(rootContext);
            if (nextNode == null && rootContext.Count() > 0)
            {
                nextNode = rootContext.Pop();
            }

            currentNode = nextNode;
            return true;
        }
    }
}