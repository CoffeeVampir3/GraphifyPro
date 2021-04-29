using System.Collections.Generic;
using UnityEditor;
using UnityEditor.GraphToolsFoundation.Overdrive;
using UnityEditor.GraphToolsFoundation.Overdrive.BasicModel;
using UnityEngine;
using UnityEngine.UIElements;

namespace Vampire.Graphify.EditorOnly
{
    public class GraphifyWindow : GraphViewEditorWindow
    {
        [SerializeField]
        public StyleSheet graphifyStyler;
        
        [InitializeOnLoadMethod]
        static void RegisterTool()
        {
            ShortcutHelper.RegisterDefaultShortcuts<GraphifyWindow>(GraphifyStencil.toolName);
        }

        [MenuItem("GTF Samples/Recipe Editor", false)]
        public static void ShowRecipeGraphWindow()
        {
            FindOrCreateGraphWindow<GraphifyWindow>();
        }
        
        //TODO::(Z) Matching variables used for the hack as described in update.
        private Unity.Properties.UI.PropertyElement sidePanelTarget;
        private INodeModel prevTarget = null;
        protected override void OnEnable()
        {
            base.OnEnable();
            EditorToolName = "Graphify Pro";
            rootVisualElement.styleSheets.Add(graphifyStyler);
            
            sidePanelTarget = 
                m_SidePanel.Q("sidePanelInspector") as Unity.Properties.UI.PropertyElement;
        }
        
        protected override void Update()
        {
            base.Update();
            //TODO::(Z) NOTICE! Hack, this is likely to break in new versions!
            //We're comparing the current property panel target to the previous one
            //If they're different we clear and reset the target which seems to solve
            //the issue of the side panel not correctly updating.
            if (!sidePanelTarget.TryGetTarget<INodeModel>(out var currentTarget) 
                || currentTarget == prevTarget) return;
            
            prevTarget = currentTarget;
            sidePanelTarget.ClearTarget();
            sidePanelTarget.SetTarget(currentTarget);
        }

        protected override GraphView CreateGraphView()
        {
            return new GraphifyView(this, CommandDispatcher);
        }
        
        protected override void RegisterCommandHandlers()
        {
            base.RegisterCommandHandlers();

            CommandDispatcher.RegisterCommandHandler<ResizeDynamicPortCommand>(ResizeDynamicPortCommand.DefaultHandler);
            CommandDispatcher.RegisterCommandHandler<BuildAllEditorCommand>(CompiledGraphBuilder.Build);
        }

        private class RecipeOnboardingProvider : OnboardingProvider
        {
            public override VisualElement CreateOnboardingElements(
                CommandDispatcher commandDispatcher)
            {
                var template = new GraphTemplate<GraphifyStencil>(GraphifyStencil.graphName);
                return AddNewGraphButton<GraphifyAssetModel>(template);
            }
        }

        protected override BlankPage CreateBlankPage()
        {
            var onboardingProviders = new List<OnboardingProvider> 
                {new RecipeOnboardingProvider()};

            return new BlankPage(CommandDispatcher, onboardingProviders);
        }
    }
}