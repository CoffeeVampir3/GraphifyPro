using System;
using UnityEditor.GraphToolsFoundation.Overdrive;
using UnityEditor.GraphToolsFoundation.Overdrive.BasicModel;
using UnityEditor;
using UnityEditor.Callbacks;
using Vampire.Runtime;

namespace Vampire.Graphify.EditorOnly
{
    public class GraphifyAssetModel : GraphAssetModel
    {
        public RuntimeGraphBlueprint runtimeBlueprint;
        protected override Type GraphModelType => typeof(GraphifyModel);
        
        public static void CreateGraph(MenuCommand menuCommand)
        {
            const string path = "Assets";
            var template = new GraphTemplate<GraphifyStencil>(GraphifyStencil.graphName);
            CommandDispatcher commandDispatcher = null;
            if (EditorWindow.HasOpenInstances<GraphifyWindow>())
            {
                var window = EditorWindow.GetWindow<GraphifyWindow>();
                if (window != null)
                {
                    commandDispatcher = window.CommandDispatcher;
                }
            }

            GraphAssetCreationHelpers<GraphifyAssetModel>.CreateInProjectWindow(template, commandDispatcher, path);
        }

        [OnOpenAsset(1)]
        public static bool OpenGraphAsset(int instanceId, int line)
        {
            var obj = EditorUtility.InstanceIDToObject(instanceId);
            if (obj is not GraphifyAssetModel) return false;
            
            string path = AssetDatabase.GetAssetPath(instanceId);
            var asset = AssetDatabase.LoadAssetAtPath<GraphifyAssetModel>(path);
            if (asset == null)
                return false;

            var window = GraphViewEditorWindow.FindOrCreateGraphWindow<GraphifyWindow>();
            return window != null;

        }
    }
}