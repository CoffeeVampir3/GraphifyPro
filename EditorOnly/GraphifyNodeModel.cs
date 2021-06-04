using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.GraphToolsFoundation.Overdrive;
using UnityEditor.GraphToolsFoundation.Overdrive.BasicModel;
using UnityEngine;
using Vampire.Graphify.EditorOnly.StaticBridge;
using Vampire.Runtime;
using PortCapacity = UnityEditor.GraphToolsFoundation.Overdrive.PortCapacity;

namespace Vampire.Graphify.EditorOnly
{
    [Serializable]
    public class GraphifyNodeModel : NodeModel, IRenamable, IHasRuntimeNode
    {
        [SerializeField, HideInInspector]
        protected string nodeName;
        [SerializeReference, ShowInNodeInspector]
        public RuntimeNode runtimeNode;
        [SerializeField, HideInInspector]
        protected InfoCollection<PortInfo, PortDefinition> portInfo = new();
        [SerializeField, HideInInspector]
        protected InfoCollection<DynamicPortInfo, DynamicRange> dynamicPorts = new();
        
        public RuntimeNode RuntimeNode => runtimeNode;
        public short RuntimeNodeId => runtimeNode?.nodeId ?? -1;
        
        public IReadOnlyList<DynamicPortInfo> DynamicPortList => dynamicPorts.InfoList;
        public override string Title => nodeName;
        public void ResizeDynamicPort(DynamicPortInfo targetPortInfo, int by)
        {
            var oldSize = targetPortInfo.currentSize;
            targetPortInfo.currentSize += by;
            targetPortInfo.currentSize = 
                Mathf.Clamp(targetPortInfo.currentSize, targetPortInfo.minSize, targetPortInfo.maxSize);

            if (oldSize == targetPortInfo.currentSize)
                return;
            
            //Not to be confused with OnDefineNode
            DefineNode();
        }

        public void Rename(string newName)
        {
            nodeName = newName;
        }
        
        //Copies the runtime node when duplicating.
        public override void OnDuplicateNode(INodeModel sourceNode)
        {
            if (sourceNode is GraphifyNodeModel m)
            {
                //Creates a carbon copy of the original nodes runtime data.
                m.runtimeNode = Activator.CreateInstance(runtimeNode.GetType()) as RuntimeNode;
                EditorUtility.CopySerializedManagedFieldsOnly(runtimeNode, m.runtimeNode);
            }
            base.OnDuplicateNode(sourceNode);
        }

        private readonly HashSet<string> definedFieldNames = new();
        private readonly HashSet<string> definedDynamicNames = new();
        protected override void OnDefineNode()
        {
            definedFieldNames.Clear();
            definedDynamicNames.Clear();

            m_Capabilities.Add(UnityEditor.GraphToolsFoundation.Overdrive.Capabilities.Renamable);
            if (string.IsNullOrWhiteSpace(nodeName))
            {
                nodeName = ObjectNames.NicifyVariableName(runtimeNode.GetType().Name);
            }

            foreach (var item in DynamicPortList)
            {
                item.ports = new OrderedPorts();
            }

            if (runtimeNode == null)
            {
                Debug.Log("Attempted to construct node with no runtime data!" + nodeName);
                return;
            }

            runtimeNode.GetDefinedPortFields(out var fields, out var defs);
            for (int i = 0; i < fields.Length; i++)
            {
                CreatePort(fields[i], defs[i]);
            }

            portInfo.RemoveUnused(definedFieldNames);
            dynamicPorts.RemoveUnused(definedDynamicNames);
            
            base.OnDefineNode();
        }

        #region Ports

        protected virtual IPortModel CreatePort(FieldInfo field,
            PortDefinition portDef)
        {
            var dynPort = ResolveDynamicPort(field);
            if (dynPort != null)
            {
                return CreateDynamicPort(field, portDef, dynPort);
            }
            var handle = TypeSerializer.GenerateTypeHandle(field.FieldType);
            return CreateSinglePort(field.Name, handle, portDef);
        }
        
        private DynamicPortInfo ResolveDynamicPort(FieldInfo field)
        {
            if (!typeof(IRuntimeDynamicBasePort).IsAssignableFrom(field.FieldType))
            {
                return null;
            }
            var dynDef = field.GetCustomAttribute<DynamicRange>();

            definedDynamicNames.Add(field.Name);
            if (!dynamicPorts.TryGetInfo(field.Name, out var dynPort))
            {
                dynPort = dynamicPorts.AddOrUpdateInfo(field.Name, dynDef, 
                    def => new DynamicPortInfo(field.Name, def));
            }

            return dynPort;
        }

        protected IPortModel CreateDynamicPort(FieldInfo field,
            PortDefinition portDef, DynamicPortInfo dynPortInfo)
        {
            if (dynPortInfo.currentSize <= 0)
                return null;
            
            var handle = TypeSerializer.GenerateTypeHandle(field.FieldType);

            var first = CreateSinglePort(field.Name + "0", handle, portDef, dynPortInfo);
            for (int i = 1; i < dynPortInfo.currentSize; i++)
            {
                CreateSinglePort(field.Name + i, handle, portDef, dynPortInfo);
            }
            return first;
        }
        
        /// <summary>
        /// Generates an actual port model instance and configures the
        /// port info, defined field names, and dynamic port info.
        /// The created port is automagically added to the node via addin/out.
        /// </summary>
        protected IPortModel CreateSinglePort(string portName, TypeHandle type,
            PortDefinition portDef, DynamicPortInfo dynPortInfo = null)
        {
            var portModel = portDef switch
            {
                In => AddInputPort(portName, PortType.Data,
                    type, null, portDef.orientation.ToUnity(), PortModelOptions.NoEmbeddedConstant),
                Out => AddOutputPort(portName, PortType.Data,
                    type, null, portDef.orientation.ToUnity(), PortModelOptions.NoEmbeddedConstant),
                _ => throw new ArgumentOutOfRangeException()
            };
            definedFieldNames.Add(portName);
            portInfo.AddOrUpdateInfo(
                portModel.UniqueName, portDef, 
                pDef => new PortInfo(pDef));

            dynPortInfo?.ports.Add(portModel);
            return portModel;
        }

        /// <summary>
        /// IPortModel defers the the INodeModel to find it's capacity, this will
        /// return the known capacity for this port, or it'll fallback to the unity default
        /// if none is known.
        /// </summary>
        public override PortCapacity GetPortCapacity(IPortModel portModel)
        {
            if (!portInfo.TryGetInfo(portModel, out var info))
                return base.GetPortCapacity(portModel);
            return info.portCapacity.ToUnity();
        }
        
        #endregion
    }
}