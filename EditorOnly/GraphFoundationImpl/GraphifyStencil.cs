using System;
using UnityEditor.GraphToolsFoundation.Overdrive;
using UnityEditor.GraphToolsFoundation.Overdrive.BasicModel;
using Vampire.Graphify.EditorOnly.Search;
using PortCapacity = UnityEditor.GraphToolsFoundation.Overdrive.PortCapacity;

namespace Vampire.Graphify.EditorOnly
{
    public class GraphifyStencil : Stencil
    {
        public static string toolName = "Graphify Pro";
        public override string ToolName => toolName;

        public static readonly string graphName = "Graphify Pro";
        public override IGraphProcessingErrorModel CreateProcessingErrorModel(GraphProcessingError error)
        {
            return null;
        }
        
        public override Type GetConstantNodeValueType(TypeHandle typeHandle)
        {
            return typeHandle.Resolve();
        }

        public override IConstant CreateConstantValue(TypeHandle constantTypeHandle)
        {
            IConstant instance;
            var nodeType = TypeToConstantMapper.GetConstantNodeType(constantTypeHandle);
            if (nodeType == null)
            {
                instance = new AnyConstant {ObjectValue = new object()};
            }
            else
            {
                if (Activator.CreateInstance(nodeType) is not IConstant constantVal)
                {
                    return null;
                }

                instance = constantVal;
                instance.ObjectValue = instance.DefaultValue;
            }
            
            return instance;
        }

        /// <summary>
        /// All ports have a default capacity of single unless they explicitly declare a capacity.
        /// </summary>
        public override bool GetPortCapacity(IPortModel portModel, out PortCapacity capacity)
        {
            capacity = PortCapacity.Single;
            return true;
        }

        public override IToolbarProvider GetToolbarProvider()
        {
            return m_ToolbarProvider ??= new CustomToolbarProvider();
        }

        public override IBlackboardGraphModel CreateBlackboardGraphModel(IGraphAssetModel graphAssetModel)
        {
            return new GraphifyBlackboardModel(graphAssetModel);
        }

        public override ISearcherDatabaseProvider GetSearcherDatabaseProvider()
        {
            return m_SearcherDatabaseProvider ??= new GraphifySearcherDatabaseProvider(this);
        }
    }
}