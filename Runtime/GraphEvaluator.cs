using System;

namespace Vampire.Runtime
{
    [Serializable]
    public class GraphEvaluator
    {
        public RuntimeGraphBlueprint blueprint;
        
        internal int rootNodeIndex = 0;
        internal RuntimeGraph rtGraph;
        internal RuntimeNode currentNode;
        internal RuntimeNode nextNode = null;
        internal Context rootContext;

        internal void SetGraphContext()
            => RuntimeGraph.current = rtGraph;
        
        public void Initialize()
        {
            rtGraph = blueprint.CreateRuntimeGraph();
            currentNode = rtGraph.nodes[rootNodeIndex];
            rootContext = new Context(rtGraph);

            this.Editor_RegisterBuildWatcher();
            this.Editor_SendNodeVisitedSignal(currentNode.nodeId);
        }

        public bool Step()
        {
            if (currentNode == null)
                return false;
            
            RuntimeGraph.current = rtGraph;
            nextNode = currentNode.Evaluate(rootContext);
            if (nextNode == null && rootContext.Count() > 0)
            {
                nextNode = rootContext.Pop();
            }

            if (nextNode != null)
                this.Editor_SendNodeVisitedSignal(currentNode.nodeId);
            
            currentNode = nextNode;
            return true;
        }
    }
}