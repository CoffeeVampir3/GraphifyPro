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

        public void Testing_SetCurrentGraphContext()
        {
            RuntimeGraph.current = rtGraph;
        }

        public void Initialize()
        {
            rtGraph = blueprint.CreateRuntimeGraph();
            currentNode = rtGraph.nodes[rootNodeIndex];
            rootContext = new Context(rtGraph);

            //TODO:: Temp
            #if UNITY_EDITOR
            BlueprintBuiltSignal.RegisterListener(Editor_ObserveBuildChanges);
            VisitNodeIdSignal visitNodeSig = new VisitNodeIdSignal(currentNode.nodeId, VisitNodeIdSignal.activeNodeCssClass);
            visitNodeSig.Send();
            #endif
        }
        
        private RuntimeNode Editor_ReplaceRebuiltNode(RuntimeNode existing)
        {
            return currentNode.nodeId > rtGraph.nodes.Length ? 
                rtGraph.nodes[rootNodeIndex] : rtGraph.nodes[existing.nodeId];
        }

        public void Editor_ObserveBuildChanges(BlueprintBuiltSignal sig)
        {
            if (blueprint.GetType() != sig.graphBlueprintType) return;
            //Rebuilds the runtime graph from all new data.
            rtGraph = blueprint.CreateRuntimeGraph();
            currentNode = Editor_ReplaceRebuiltNode(currentNode);
            nextNode = Editor_ReplaceRebuiltNode(nextNode);
            rootContext.currentGraph = rtGraph;
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