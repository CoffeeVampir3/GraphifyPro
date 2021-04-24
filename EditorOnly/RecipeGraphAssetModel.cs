using System;
using UnityEditor.GraphToolsFoundation.Overdrive;
using UnityEditor.GraphToolsFoundation.Overdrive.BasicModel;
using UnityEditor;
using UnityEditor.Callbacks;
using Vampire.Graphify.Runtime;

namespace Vampire.Graphify.EditorOnly
{
    public class RecipeGraphAssetModel : GraphAssetModel
    {
        public RuntimeGraphBlueprint runtimeBlueprint;
        protected override Type GraphModelType => typeof(RecipeGraphModel);
        
        [MenuItem("Assets/Create/Recipe")]
        public static void CreateGraph(MenuCommand menuCommand)
        {
            const string path = "Assets";
            var template = new GraphTemplate<RecipeStencil>(RecipeStencil.graphName);
            CommandDispatcher commandDispatcher = null;
            if (EditorWindow.HasOpenInstances<RecipeGraphWindow>())
            {
                var window = EditorWindow.GetWindow<RecipeGraphWindow>();
                if (window != null)
                {
                    commandDispatcher = window.CommandDispatcher;
                }
            }

            GraphAssetCreationHelpers<RecipeGraphAssetModel>.CreateInProjectWindow(template, commandDispatcher, path);
        }

        [OnOpenAsset(1)]
        public static bool OpenGraphAsset(int instanceId, int line)
        {
            var obj = EditorUtility.InstanceIDToObject(instanceId);
            if (obj is not RecipeGraphAssetModel) return false;
            
            string path = AssetDatabase.GetAssetPath(instanceId);
            var asset = AssetDatabase.LoadAssetAtPath<RecipeGraphAssetModel>(path);
            if (asset == null)
                return false;

            var window = GraphViewEditorWindow.FindOrCreateGraphWindow<RecipeGraphWindow>();
            return window != null;

        }
    }
}