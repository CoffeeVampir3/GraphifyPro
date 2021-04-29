using UnityEditor.GraphToolsFoundation.Overdrive;
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
            return null;
        }

        public override ISearcherDatabaseProvider GetSearcherDatabaseProvider()
        {
            return m_SearcherDatabaseProvider ??= new GraphifySearcherDatabaseProvider(this);
        }
    }
}