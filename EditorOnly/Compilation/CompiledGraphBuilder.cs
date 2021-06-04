using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.GraphToolsFoundation.Overdrive;
using Vampire.Runtime;
using Vampire.Runtime.SignalLinker;
using Debug = UnityEngine.Debug;

namespace Vampire.Graphify.EditorOnly
{
    internal static class CompiledGraphBuilder
    {
        private static readonly Dictionary<IPortModel, short> portModelToValueId = new();
        private static readonly Dictionary<short, object> portIdToInitialValue = new();
        private static readonly Dictionary<short, string> valueIdToName = new();
        private static readonly Dictionary<short, DynamicPortInfo> idToDynamicGroup = new();
        private static readonly Dictionary<IPortModel, short> portToDynamicGroupId = new();
        private static readonly Dictionary<DynamicPortInfo, short> dynamicGroupToValueId = new();
        private static readonly Dictionary<string, short> blackboardBuilder = new();
        private static readonly Dictionary<string, IRuntimeBasePort> fieldNameToBasePort = new();
        private static readonly Dictionary<IVariableDeclarationModel, short> declModelToValId = new();
        private static readonly List<IRuntimeBasePort> allRuntimePorts = new();
        private static Stencil stencil;
        private static short currentPortValueId = -1;
        private static short currentDynamicIdentifier = -1;
        private static void Swap<T>(ref T rhs, ref T lhs)
        {
            T temp = lhs;
            lhs = rhs;
            rhs = temp;
        }

        private static short ResolvePortValueId(IPortModel port)
        {
            if (portModelToValueId.TryGetValue(port, out var id))
                return id;
            return -1;
        }

        private static IPortModel GetToPort(IEdgeModel edge) => edge.ToPort;
        private static IPortModel GetFromPort(IEdgeModel edge) => edge.FromPort;
        
        //Recurs through portaled connections and creates real connections
        private static bool ResolveEndPointFrom(IPortModel targetPort, 
            out short endPointNodeId, out short endPointPortId, 
            Func<IEdgeModel, IPortModel> getPortFunc)
        {
            while (true)
            {
                endPointNodeId = -1;
                endPointPortId = -1;
                var targetNode = targetPort.NodeModel;
                switch (targetNode)
                {
                    case IHasRuntimeNode rtNode:
                        endPointNodeId = rtNode.RuntimeNodeId;
                        endPointPortId = ResolvePortValueId(targetPort);
                        return true;
                    case IVariableNodeModel:
                        //Variable nodes are not valid to traverse to.
                        endPointNodeId = -1;
                        endPointPortId = ResolvePortValueId(targetPort);
                        return true;
                }

                if (targetNode is not IEdgePortalModel toPortalModel) return false;
                var linkedPortals = stencil.GetLinkedPortals(toPortalModel);
                var fromPortalModel = linkedPortals.FirstOrDefault(e => toPortalModel != e);
                var oppositeEdge = fromPortalModel?.GetConnectedEdges().FirstOrDefault();
                targetPort = getPortFunc.Invoke(oppositeEdge);
            }
        }

        private static bool TryBuildLink(IEdgeModel edge, short dynamicPortIndex, 
            bool swapToAndFrom, out Link link)
        {
            if (!ResolveEndPointFrom(edge.FromPort, out var fromNodeIndex, out var fromIndex, GetFromPort))
            {
                Debug.LogError("Graph Builder was unable to build a semantic link between: " + edge.FromPort?.NodeModel + 
                               " on port: " + edge.FromPort + " to " + edge.ToPort?.NodeModel + " on port: " + edge.ToPort);
                link = default;
                return false;
            }

            if (!ResolveEndPointFrom(edge.ToPort, out var toNodeIndex, out var toIndex, GetToPort))
            {
                Debug.LogError("Graph Builder was unable to build a semantic link between: " + edge.ToPort?.NodeModel + 
                               " on port: " + edge.ToPort + " to " + edge.FromPort?.NodeModel + " on port: " + edge.FromPort);
                link = default;
                return false;
            }

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


        private static short ResolveDynamicPortId(short groupId, IPortWithValue valuePort)
        {
            if (!idToDynamicGroup.TryGetValue(groupId, out var dynGroup)) {return -1;}
            if (dynamicGroupToValueId.TryGetValue(dynGroup, out var valueId))
            {
                return valueId;
            }

            var newId = ++currentPortValueId;
            dynamicGroupToValueId.Add(dynGroup, newId);
            portIdToInitialValue.Add(newId, valuePort.GetInitValue());
            return newId;
        }

        private static void AssignSpecialPortIdentifiers(IPortModel portModel, 
            IRuntimeBasePort bp, bool isDynamic)
        {
            short portId;
            //Port is not a value port, it has no value id.
            if (bp is not IPortWithValue valuePort)
            {
                bp.PortId = -1;
                return;
            }

            //If this port is a value port, assign it a new id.
            if (portModelToValueId.TryGetValue(portModel, out var id))
            {
                portId = id;
                bp.PortId = portId;
                return;
            }

            //If the port is dynamic and has a group id
            if (portToDynamicGroupId.TryGetValue(portModel, out var dynamicGroupId))
            {
                //Assign dynamic id if dynamic
                portId = ResolveDynamicPortId(dynamicGroupId, valuePort);
                portModelToValueId.Add(portModel, portId);
                valueIdToName.Add(portId, portModel.UniqueName + portModel.NodeModel.Guid);
                bp.PortId = portId;
                return;
            }

            //If the port is dynamic but is not assigned an id, we don't want it to fall through.
            if (isDynamic)
                return;
            
            portId = ++currentPortValueId;
            portModelToValueId.Add(portModel, portId);
            valueIdToName.Add(portId, portModel.UniqueName + portModel.NodeModel.Guid);
            portIdToInitialValue.Add(portId, valuePort.GetInitValue());
            bp.PortId = portId;
        }

        /// <summary>
        /// Visits an edge and tries to build a link to it from this origin port.
        /// </summary>
        private static void VisitEdgeFromOrigin(IEdgeModel edge,
            IRuntimeBasePort bp, bool originIsToPort, short dynamicPortIndex)
        {
            if (!TryBuildLink(edge, dynamicPortIndex, originIsToPort, out var link))
                return;
            bp.AddLink(link);
        }

        /// <summary>
        /// Visits the given port and all of its edges.
        /// </summary>
        private static void VisitRuntimePort(IPortModel port, 
            IRuntimeBasePort bp, short dynamicPortIndex = -1)
        {
            var edges = port.GetConnectedEdges();
            foreach (var edge in edges)
            {
                bool edgeOriginIsFrom = edge.FromPort != port;
                VisitEdgeFromOrigin(edge, bp, edgeOriginIsFrom, dynamicPortIndex);
            }
        }
        
        /// <summary>
        /// Identifies this variable node, since variable nodes have a singular port but possibly
        /// have multiple instances of themselves which share a value, they all share the same port id.
        /// </summary>
        /// <param name="variableNode"></param>
        private static void IdentifyVariableNodeModel(IVariableNodeModel variableNode)
        {
            var port = variableNode.Ports.FirstOrDefault();
            if (!declModelToValId.TryGetValue(variableNode.VariableDeclarationModel, out var portId))
            {
                portId = ++currentPortValueId;
                declModelToValId.Add(variableNode.VariableDeclarationModel, portId);
                valueIdToName.Add(portId, variableNode.VariableDeclarationModel.GetVariableName());
            }
            portModelToValueId.Add(port!, portId);
        }
        
        /// <summary>
        /// Visits a variable node (the thing you drag in from the blackboard) and assign it
        /// a singular id and value. Which just makes sense, really.
        /// </summary>
        private static void VisitVariableNodeModel(IVariableNodeModel variableNode)
        {
            if (blackboardBuilder.TryGetValue(variableNode.VariableDeclarationModel.DisplayTitle, out _)) return;
            var port = variableNode.Ports.FirstOrDefault();
            portIdToInitialValue.Add(portModelToValueId[port!],
                variableNode.VariableDeclarationModel.InitializationModel.ObjectValue);
            blackboardBuilder.Add(variableNode.VariableDeclarationModel.DisplayTitle, 
                portModelToValueId[port]);
        }
        
        /// <summary>
        /// Identifies (and assigns an id to, if needed) ports on this node.
        /// </summary>
        /// <param name="hasRuntimeNode"></param>
        private static void IdentifyRuntimeNodeModel(IHasRuntimeNode hasRuntimeNode)
        {
            var rtNode = hasRuntimeNode.RuntimeNode;
            var dynamicPorts = hasRuntimeNode.DynamicPortList;
            var portList = hasRuntimeNode.Ports.ToArray();
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
                bp.Editor_Reset();
                fieldNameToBasePort.Add(field.Name, bp);
                allRuntimePorts.Add(bp);
            }
            
            //First pass creates dynamic port filter hash set and assign the dynamic port
            //it's own identifiers.
            foreach (var dynamicPort in dynamicPorts)
            {
                dynamicFieldNames.Add(dynamicPort.fieldName);
                if (!fieldNameToBasePort.TryGetValue(dynamicPort.fieldName, out var bp)) continue;
                foreach (var dynPort in dynamicPort.ports.Values)
                {
                    AssignSpecialPortIdentifiers(dynPort, bp, true);
                }
            }

            //Identify each non-dynamic port.
            foreach (var port in portList)
            {
                if (fieldNameToBasePort.TryGetValue(port.UniqueName, out var bp))
                {
                    AssignSpecialPortIdentifiers(port, bp, false);
                }
            }
        }
        
        /// <summary>
        /// Visits a runtime model which snakes through each port and will visit each
        /// port connection from the perspective of this node.
        /// </summary>
        private static void VisitRuntimeModel(IHasRuntimeNode hasRuntimeNode)
        {
            var rtNode = hasRuntimeNode.RuntimeNode;
            var dynamicPorts = hasRuntimeNode.DynamicPortList;
            var portList = hasRuntimeNode.Ports.ToArray();
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
                fieldNameToBasePort.Add(field.Name, bp);
            }

            //Visit all non-dynamic ports.
            foreach (var port in portList)
            {
                if (fieldNameToBasePort.TryGetValue(port.UniqueName, out var bp))
                {
                    VisitRuntimePort(port, bp);
                }
            }
            
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

        /// <summary>
        /// Iterates through all portId->InitValue relations and creates a flattened array indexed via
        /// the port id.
        /// </summary>
        private static void SetupRuntimePortDataTable(RuntimeGraphBlueprint blueprint)
        {
            for (short i = 0; i < blueprint.initializationValues.Length; i++)
            {
                if (!portIdToInitialValue.TryGetValue(i, out var initialValue)) continue;
                blueprint.initializationNames[i] = valueIdToName[i];
                if (initialValue != null && initialValue.GetType().IsValueType)
                {
                    blueprint.initializationValues[i] = AntiAllocationWrapper.CreateValueTypeWrapper(initialValue);
                }
                else
                {
                    blueprint.initializationValues[i] = initialValue;
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
            if (!(model.AssetModel is GraphifyAssetModel assetModel))
                return;

            var blueprint = assetModel.runtimeBlueprint;
            List<RuntimeNode> runtimeNodes = new();
            portModelToValueId.Clear();
            portIdToInitialValue.Clear();
            idToDynamicGroup.Clear();
            allRuntimePorts.Clear();
            portToDynamicGroupId.Clear();
            dynamicGroupToValueId.Clear();
            blackboardBuilder.Clear();
            declModelToValId.Clear();
            valueIdToName.Clear();
            currentPortValueId = -1;
            currentDynamicIdentifier = -1;
            stencil = model.Stencil;

            var oldInitializationNames = blueprint.initializationNames;

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
                            foreach (var port in dynamicPort.ports.Values)
                            {
                                if (!port.GetConnectedEdges().Any())
                                    continue;

                                if (dynamicId == -1)
                                {
                                    dynamicId = ++currentDynamicIdentifier;
                                    idToDynamicGroup.Add(dynamicId, dynamicPort);
                                }
                                portToDynamicGroupId.Add(port, dynamicId);
                            }
                        }
                        break;
                }
            }
            
            //Identify all runtime nodes.
            foreach (var nodeModel in model.NodeModels)
            {
                switch (nodeModel)
                {
                    case IHasRuntimeNode graphifyNodeModel:
                        IdentifyRuntimeNodeModel(graphifyNodeModel);
                        break;
                    case IVariableNodeModel variableNodeModel:
                        IdentifyVariableNodeModel(variableNodeModel);
                        break;
                }
            }
            
            //Every node model is now indexed, we'll visit each model which will recurse into
            //Node->Ports->Edges->Create Links per Edge
            foreach (var nodeModel in model.NodeModels)
            {
                switch (nodeModel)
                {
                    case IHasRuntimeNode graphifyNodeModel:
                        VisitRuntimeModel(graphifyNodeModel);
                        break;
                    case IVariableNodeModel variableNodeModel:
                        VisitVariableNodeModel(variableNodeModel);
                        break;
                }
            }

            //Finally order the links of all runtime ports.
            foreach (var runtimePort in allRuntimePorts)
            {
                runtimePort.OrderLinks();
            }

            blueprint.nodes = runtimeNodes.ToArray();
            blueprint.initializationValues = new object[currentPortValueId+1];
            blueprint.initializationNames = new string[currentPortValueId+1];
            SetupSpecializedData(blueprint);

            blueprint.localProperties = new PropertyDictionary();
            var typeNameHelper = new Dictionary<string, string>();
            foreach (var decl in model.VariableDeclarations)
            {
                blueprint.localProperties.Add(
                    decl.DisplayTitle, 
                    decl.InitializationModel.ObjectValue,
                    blackboardBuilder);
                typeNameHelper.Add(decl.DisplayTitle, decl.DataType.Resolve().FullName);
            }

            PropertyCodeGenerator.Generate(blueprint, typeNameHelper);
            AssetDatabase.Refresh();
            EditorUtility.SetDirty(assetModel);
            EditorUtility.SetDirty(blueprint);
            AssetDatabase.WriteImportSettingsIfDirty(AssetDatabase.GetAssetPath(blueprint));
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(blueprint));

            BlueprintBuiltSignal builtSig = new BlueprintBuiltSignal(blueprint.GetType(), oldInitializationNames);
            builtSig.Send();
        }
    }
}