using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.GraphToolsFoundation.Overdrive;
using UnityEngine;
using Vampire.Graphify.Runtime;

namespace Vampire.Graphify.EditorOnly
{
    public static class CompiledGraphBuilder
    {
        private static readonly Dictionary<IPortModel, short> portModelToId = new();
        private static readonly Dictionary<short, IPortWithValue> idToValuePort = new();
        private static readonly Dictionary<DynamicPortInfo, short> dynamicPortToId = new();
        private static readonly List<IRuntimeBasePort> allRuntimePorts = new();
        private static void Swap<T>(ref T rhs, ref T lhs)
        {
            T temp = lhs;
            lhs = rhs;
            rhs = temp;
        }
        private static bool TryBuildLink(IEdgeModel edge, short dynamicPortIndex, 
            bool swapToAndFrom, out Link link)
        {
            var fromPort = edge.FromPort;
            var toPort = edge.ToPort;
            if (fromPort?.NodeModel is not IHasRuntimeNode runtimeFrom ||
                toPort?.NodeModel is not IHasRuntimeNode runtimeTo) 
            {
                Debug.LogError("Graph Builder was unable to build a semantic link between: " + fromPort?.NodeModel + 
                               " on port: " + fromPort + " " + toPort?.NodeModel + " on port: " + toPort);
                link = default;
                return false;
            }
            
            short fromNodeIndex = runtimeFrom.RuntimeNodeId;
            short toNodeIndex = runtimeTo.RuntimeNodeId;
            short fromIndex = portModelToId[fromPort];
            short toIndex = portModelToId[toPort];
            
            //Runtime nodes expect to always be the "owner" of the from port, so if the
            //case is that this node actually owns the to port, we swap the from<->to
            if (swapToAndFrom)
            {
                Swap(ref fromIndex, ref toIndex);
                Swap(ref fromNodeIndex, ref toNodeIndex);
            }
            
            link = new Link {
                toNodeIndex = toNodeIndex,
                fromPortIndex = fromIndex,
                toPortIndex = toIndex,
                dynamicPortId = dynamicPortIndex
            };
            return true;
        }

        private static void AssignSpecializedPortIdentifiers(short portId, IRuntimeBasePort bp)
        {
            switch (bp)
            {
                case IPortWithValue valuePort:
                    if (idToValuePort.ContainsKey(portId))
                        return;
                    idToValuePort.Add(portId, valuePort);
                    break;
            }
        }
        
        private static void AssignSpecializedPortIdentifiers(IPortModel portModel, 
            IRuntimeBasePort bp)
        {
            AssignSpecializedPortIdentifiers(portModelToId[portModel], bp);
        }

        private static void VisitEdgeFromOrigin(IEdgeModel edge,
            IRuntimeBasePort bp, bool originIsToPort, short dynamicPortIndex)
        {
            if (!TryBuildLink(edge, dynamicPortIndex, originIsToPort, out var link))
                return;
            bp.AddLink(link);
        }

        private static void VisitRuntimePort(IPortModel port, 
            IRuntimeBasePort bp, short dynamicPortIndex = -1)
        {
            var edges = port.GetConnectedEdges();
            foreach (var edge in edges)
            {
                bool edgeOriginIsFrom = edge.FromPort != port;
                AssignSpecializedPortIdentifiers(port, bp);
                VisitEdgeFromOrigin(edge, bp, edgeOriginIsFrom, dynamicPortIndex);
            }
        }

        private static readonly Dictionary<string, IRuntimeBasePort> fieldNameToBasePort = new();
        private static void VisitRuntimeModel(IHasRuntimeNode nodeNode)
        {
            var rtNode = nodeNode.RuntimeNode;
            var dynamicPorts = nodeNode.DynamicPortList;
            var portList = nodeNode.Ports;
            var dynamicFieldNames = new HashSet<string>();
            fieldNameToBasePort.Clear();

            //Iterate over all defined fields, 
            rtNode.GetDefinedPortFields(out var fields, out _);
            foreach (var field in fields)
            {
                var bp = field.GetValue(rtNode) as IRuntimeBasePort;
                if (bp == null)
                {
                    continue;
                }
                bp.ClearLinks();
                fieldNameToBasePort.Add(field.Name, bp);
                allRuntimePorts.Add(bp);
            }

            //Broken into passes so we can avoid high complexity iterations.
            
            //First pass creates dynamic port filter hash set and assign the dynamic port
            //it's own identifiers.
            foreach (var dynamicPort in dynamicPorts)
            {
                dynamicFieldNames.Add(dynamicPort.fieldName);
                if (!fieldNameToBasePort.TryGetValue(dynamicPort.fieldName, out var bp)) continue;
                if (dynamicPortToId.TryGetValue(dynamicPort, out var id))
                {
                    AssignSpecializedPortIdentifiers(id, bp);
                }
            }

            //Second pass visits all non-dynamic ports
            foreach (var port in portList)
            {
                if (dynamicFieldNames.Contains(port.UniqueName)) continue;
                if (fieldNameToBasePort.TryGetValue(port.UniqueName, out var bp))
                {
                    VisitRuntimePort(port, bp);
                }
            }
            
            //Third pass visits all dynamic ports
            //We iterate dynamic ports in a separate pass so we can sequence them correctly.
            foreach (var dynamicPort in dynamicPorts)
            {
                if (!fieldNameToBasePort.TryGetValue(dynamicPort.fieldName, out var bp)) continue;
                //Dynamic port index, this works because dynamic ports are already ordered.
                short i = -1;
                foreach (var port in dynamicPort.ports)
                {
                    VisitRuntimePort(port.Value, bp, ++i);
                }
            }
        }

        //No need to clear, since these types are consistent. We can keep these cached 4ever
        private static readonly Dictionary<Type, Type> typeToWrapperType = new();
        private static AntiAllocationWrapper CreateValueTypeWrapper(object value)
        {
            var valueType = value.GetType();
            if (!typeToWrapperType.TryGetValue(valueType, out var wrapperType))
            {
                wrapperType = typeof(AntiAllocationWrapper<>).MakeGenericType(valueType);
                typeToWrapperType.Add(valueType, wrapperType);
            }

            if (Activator.CreateInstance(wrapperType) is not AntiAllocationWrapper wrappedValue)
            {
                Debug.LogError("System was unable to create a valid allocation wrapper for type: " + valueType + 
                               " this is a critical bug, please report this.");
                return null;
            }
            wrappedValue.SetValue(value);
            return wrappedValue;
        }
        
        /// <summary>
        /// Copies all ports with initial values into the blueprint's value initialization table.
        /// </summary>
        private static void SetupRuntimePortDataTable(RuntimeGraphBlueprint blueprint)
        {
            for (short i = 0; i < blueprint.initializationValues.Length; i++)
            {
                if (!idToValuePort.TryGetValue(i, out var valuePort)) continue;

                var value = valuePort.GetInitValue();
                if (value != null && value.GetType().IsValueType)
                {
                    blueprint.initializationValues[i] = CreateValueTypeWrapper(value);
                }
                else
                {
                    blueprint.initializationValues[i] = value;
                }
            }
        }

        private static void SetupSpecializedData(RuntimeGraphBlueprint blueprint)
        {
            SetupRuntimePortDataTable(blueprint);
        }
        
        public static void Build(GraphToolState graphToolState, BuildAllEditorCommand command)
        {
            var model = graphToolState.WindowState.GraphModel;
            if (!(model.AssetModel is RecipeGraphAssetModel assetModel))
                return;

            var blueprint = assetModel.runtimeBlueprint;
            List<RuntimeNode> runtimeNodes = new();
            short currentPortId = -1;
            portModelToId.Clear();
            idToValuePort.Clear();
            dynamicPortToId.Clear();
            allRuntimePorts.Clear();

            //First loop over node models, assign each one a unique id so we
            //can flat map them to an array. We also flatten the dynamic port hierarchy
            foreach (var nodeModel in model.NodeModels)
            {
                switch (nodeModel)
                {
                    case IHasRuntimeNode graphifyNodeModel:
                        graphifyNodeModel.RuntimeNode.nodeId = (short)runtimeNodes.Count;
                        runtimeNodes.Add(graphifyNodeModel.RuntimeNode);
                        
                        //This sets the id for every dynamic actual port instance under
                        //a dynamic port to use the same id. (Effectively, we reduce dynamic ports
                        //to a combined single logical port.)
                        foreach (var dynamicPort in graphifyNodeModel.DynamicPortList)
                        {
                            //We only create a new Id if there's actually a used port on this
                            //dynamic.
                            short dynamicId = -1;
                            foreach (var port in dynamicPort.ports)
                            {
                                if (!port.Value.GetConnectedEdges().Any())
                                    continue;
                                if (dynamicId == -1)
                                {
                                    dynamicId = ++currentPortId;
                                    dynamicPortToId.Add(dynamicPort, dynamicId);
                                }

                                portModelToId.Add(port.Value, dynamicId);
                            }
                        }
                        break;
                }
            }

            //Assign every port with > 0 connections an id
            foreach (var port in model.GetPortModels())
            {
                if (!port.GetConnectedEdges().Any() || portModelToId.ContainsKey(port))
                    continue;
                portModelToId.Add(port, ++currentPortId);
            }

            //Every node model is now indexed, visit each model.
            foreach (var nodeModel in model.NodeModels)
            {
                switch (nodeModel)
                {
                    case IHasRuntimeNode graphifyNodeModel:
                        VisitRuntimeModel(graphifyNodeModel);
                        break;
                }
            }

            //Finally order the links of all runtime ports.
            foreach (var runtimePort in allRuntimePorts)
            {
                runtimePort.OrderLinks();
            }

            blueprint.nodes = runtimeNodes.ToArray();
            blueprint.initializationValues = new object[currentPortId+1];
            SetupSpecializedData(blueprint);
            
            EditorUtility.SetDirty(blueprint);
        }
    }
}