using System;
using Vampire.Runtime.SignalLinker;

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

            //TODO:: Temp
            #if UNITY_EDITOR
            VisitNodeIdSignal visitNodeSig = new VisitNodeIdSignal(currentNode.nodeId, VisitNodeIdSignal.activeNodeCssClass);
            visitNodeSig.Send();
            #endif
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
            
            #if UNITY_EDITOR
            //TODO:: Temporary code.
            if (nextNode != null)
            {
                VisitNodeIdSignal visitNodeSig = new VisitNodeIdSignal(nextNode.nodeId, VisitNodeIdSignal.activeNodeCssClass);
                visitNodeSig.Send();
            }
            #endif

            currentNode = nextNode;
            return true;
        }
    }
}