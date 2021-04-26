using System;
using UnityEditor.GraphToolsFoundation.Overdrive;
using UnityEngine;
using Vampire.Graphify;

namespace Vampire.Runtime
{
    [Serializable]
    public class RuntimeNode
    {
        [SerializeField, HideInInspector] 
        internal short nodeId = -1;
        [SerializeField]
        [DynamicPortDefinition(2, 4)]
        [In(PortCapacity.Multi)]
        public ValuePort<int> inPort = new();
        [DynamicPortDefinition(2, 4)]
        [SerializeField]
        [Out(PortCapacity.Multi)]
        public DynamicValuePort<string> outPort = new();
        [SerializeField]
        [Out]
        public ValuePort<AnimationCurve> smug = new();

        public RuntimeNode Evaluate(RuntimeGraph graph)
        {
            Debug.Log("Evaluating id: " + nodeId);
            foreach (var link in outPort.links)
            {
                if (outPort.TryGetValueAs<int>(link, graph, out var val))
                {
                    var connectedId = outPort.GetConnectedNode(link, graph).nodeId;
                    Debug.Log("In port value of " + connectedId + " is: " + val);
                }
            }
            
            foreach (var link in inPort.links)
            {
                if (inPort.TryGetValueAs<string>(link, graph, out var val))
                {
                    var connectedId = inPort.GetConnectedNode(link, graph).nodeId;
                    Debug.Log("Out port value of " + connectedId + " is: " + val);
                }
            }

            if (outPort.links.Count == 0)
            {
                Debug.Log("No linko!");
                return null;
            }

            RuntimeNode someConnection = null;
            foreach (var link in outPort.links)
            {
                someConnection = outPort.GetConnectedNode(link, graph);
            }
            
            return someConnection;
        }
    }
}