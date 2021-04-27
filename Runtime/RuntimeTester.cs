using UnityEngine;

namespace Vampire.Runtime
{
    public class RuntimeTester : MonoBehaviour
    {
        public RuntimeGraphBlueprint blueprint;
        
        private int rootNodeIndex = 0;
        private RuntimeGraph rtGraph;
        private RuntimeNode currentNode;
        private RuntimeNode nextNode = null;
        private Context rootContext;

        public void Awake()
        {
            rtGraph = blueprint.CreateRuntimeGraph();
            currentNode = rtGraph.nodes[rootNodeIndex];
            rootContext = new Context(rtGraph);
        }
        
        private int currentFrame = 0;
        public void Update()
        {
            if (++currentFrame != 700)
                return;

            currentFrame = 0;
            if (currentNode == null)
                return;
            CurrentEvaluation.currentGraph = rtGraph;
            nextNode = currentNode.Evaluate(rootContext);
            if (nextNode == null && rootContext.Count() > 0)
            {
                nextNode = rootContext.Pop();
            }

            currentNode = nextNode;
        }
    }
}