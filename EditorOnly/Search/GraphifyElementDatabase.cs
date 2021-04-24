using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.GraphToolsFoundation.Overdrive;
using UnityEditor.Searcher;
using UnityEngine;
using Vampire.Graphify.Runtime;

namespace Vampire.Graphify.EditorOnly.Search
{
    public class GraphifyElementDatabase
    {
        const string k_Sticky = "Sticky Note";
        private readonly List<SearcherItem> items;
        private readonly Stencil stencil;
        private IGraphModel graphModel;
        
        public GraphifyElementDatabase(Stencil stencil, IGraphModel graphModel)
        {
            this.stencil = stencil;
            this.items = new List<SearcherItem>();
            this.graphModel = graphModel;
        }

        public GraphifyElementDatabase AddGraphifyNodes()
        {
            var types = TypeCache.GetTypesWithAttribute<GraphifyNode>();
            foreach (var type in types)
            {
                var attributes = type.GetCustomAttributes<GraphifyNode>().ToList();
                if (!attributes.Any())
                    continue;

                foreach (var attribute in attributes)
                {
                    var name = attribute.path.Split('/').Last();
                    var path = attribute.path.Remove(attribute.path.LastIndexOf('/') + 1);
                    
                    //The graph node model searcher item is special, can't use our own.
                    //So we're bootstrapping it.
                    
                    var genType = typeof(GraphifyNodeModel);
                    var node = new GraphNodeModelSearcherItem(
                        new NodeSearcherItemData(genType),
                        data =>
                        {
                            return data.CreateNode(genType, name, obj =>
                            {
                                if (obj is not GraphifyNodeModel gfm) return;
                                if (Activator.CreateInstance(type) is not RuntimeNode rtNode) return;
                                gfm.runtimeNode = rtNode;
                            });
                        },
                        name
                    );
                    items.AddAtPath(node, path);
                }
            }

            return this;
        }

        public GraphifyElementDatabase AddStickyNote()
        {
            var node = new GraphNodeModelSearcherItem(
                new TagSearcherItemData(CommonSearcherTags.StickyNote),
                data =>
                {
                    var rect = new Rect(data.Position, StickyNote.defaultSize);
                    var dataLocalGraphModel = data.GraphModel;
                    return dataLocalGraphModel.CreateStickyNote(rect, data.SpawnFlags);
                },
                k_Sticky
            );
            items.AddAtPath(node);

            return this;
        }
        
        public SearcherDatabase Build()
        {
            Recurse(items);

            return SearcherDatabase.Create(items, null);

            static void Recurse(IEnumerable<SearcherItem> searcherItems)
            {
                foreach (var searcherItem in searcherItems.Where(searcherItem => searcherItem.HasChildren))
                {
                    Recurse(searcherItem.Children);
                }
            }
        }
    }
}