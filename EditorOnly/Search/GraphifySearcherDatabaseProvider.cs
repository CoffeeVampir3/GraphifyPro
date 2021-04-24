using System.Collections.Generic;
using UnityEditor.GraphToolsFoundation.Overdrive;
using UnityEditor.Searcher;

namespace Vampire.Graphify.EditorOnly.Search
{
    public class GraphifySearcherDatabaseProvider : ISearcherDatabaseProvider
    {
        private readonly Stencil m_Stencil;
        private List<SearcherDatabaseBase> m_GraphElementsSearcherDatabases;

        public GraphifySearcherDatabaseProvider(Stencil stencil)
        {
            m_Stencil = stencil;
        }

        public List<SearcherDatabaseBase> GetGraphElementsSearcherDatabases(IGraphModel graphModel)
        {
            return m_GraphElementsSearcherDatabases ??= new List<SearcherDatabaseBase>
            {
                new GraphifyElementDatabase(m_Stencil, graphModel)
                    .AddGraphifyNodes()
                    .AddStickyNote()
                    .Build()
            };
        }

        public List<SearcherDatabase> GetVariableTypesSearcherDatabases()
            => new();

        public List<SearcherDatabaseBase> GetGraphVariablesSearcherDatabases(IGraphModel graphModel)
            => new();

        public List<SearcherDatabaseBase> GetDynamicSearcherDatabases(IPortModel portModel)
            => new();

        public List<SearcherDatabaseBase> GetDynamicSearcherDatabases(IEnumerable<IPortModel> portModel)
            => new();
    }
}