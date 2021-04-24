using UnityEditor.GraphToolsFoundation.Overdrive;
using UnityEditor.GraphToolsFoundation.Overdrive.BasicModel;
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

        /// <inheritdoc />
        public override IBlackboardGraphModel CreateBlackboardGraphModel(IGraphAssetModel graphAssetModel)
        {
            return null;
        }
        
        public override ISearcherDatabaseProvider GetSearcherDatabaseProvider()
        {
            return m_SearcherDatabaseProvider ??= new GraphifySearcherDatabaseProvider(this);
        }
    }
}