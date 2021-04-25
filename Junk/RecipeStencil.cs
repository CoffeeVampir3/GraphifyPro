using System;
using Unity.Burst.CompilerServices;
using UnityEditor.GraphToolsFoundation.Overdrive;
using UnityEditor.GraphToolsFoundation.Overdrive.BasicModel;
using UnityEngine;
using Vampire.Graphify.EditorOnly.Search;

namespace Vampire.Graphify.EditorOnly
{
    public class RecipeStencil : Stencil
    {
        public static string toolName = "Recipe Editor";
        public override string ToolName => toolName;

        public static readonly string graphName = "Recipe";
        public override IGraphProcessingErrorModel CreateProcessingErrorModel(GraphProcessingError error)
        {
            if (error.SourceNode != null && !error.SourceNode.Destroyed)
            {
                return new GraphProcessingErrorModel(error);
            }

            return null;
        }

        public override Type GetConstantNodeValueType(TypeHandle typeHandle)
        {
            return typeHandle.Resolve();
        }

        public override IConstant CreateConstantValue(TypeHandle constantTypeHandle)
        {
            Debug.Log("Constant: " + constantTypeHandle.Name);
            var nodeType = TypeToConstantMapper.GetConstantNodeType(constantTypeHandle);
            if (nodeType == null)
            {
                Debug.LogError("Node type was null! " + constantTypeHandle.Name);
                return null;
            }

            if (Activator.CreateInstance(nodeType) is not IConstant instance) return null;
            
            instance.ObjectValue = instance.DefaultValue;
            return instance;
        }

        /// <inheritdoc />
        public override IBlackboardGraphModel CreateBlackboardGraphModel(IGraphAssetModel graphAssetModel)
        {
            return new RecipeBlackboardGraphModel(graphAssetModel);
        }
        
        public override ISearcherDatabaseProvider GetSearcherDatabaseProvider()
        {
            return m_SearcherDatabaseProvider ??= new GraphifySearcherDatabaseProvider(this);
        }
    }
}