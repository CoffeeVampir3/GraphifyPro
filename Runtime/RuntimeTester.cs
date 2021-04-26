using System;
using UnityEngine;

namespace Vampire.Runtime
{
    public class RuntimeTester : MonoBehaviour
    {
        public RuntimeGraphBlueprint blueprint;

        [NonSerialized] 
        private RuntimeGraph rtGraph;
        [NonSerialized]
        public int rootNodeIndex = 0;
        [NonSerialized] 
        private RuntimeNode currentNode;

        public void Awake()
        {
            rtGraph = blueprint.CreateRuntimeGraph();
            currentNode = rtGraph.nodes[rootNodeIndex];
        }
        
        private int currentFrame = 0;
        public void Update()
        {
            if (++currentFrame != 700)
                return;

            currentFrame = 0;
            if (currentNode == null)
                return;
            //Blackboard.
            currentNode = currentNode.Evaluate(rtGraph);
        }
    }
}