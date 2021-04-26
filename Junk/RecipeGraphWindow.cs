using System.Collections.Generic;
using UnityEditor;
using UnityEditor.GraphToolsFoundation.Overdrive;
using UnityEditor.GraphToolsFoundation.Overdrive.BasicModel;
using UnityEngine;
using UnityEngine.UIElements;
using Vampire.Binding;

namespace Vampire.Graphify.EditorOnly
{
    public class RecipeGraphWindow : GraphViewEditorWindow
    {
        [SerializeField]
        public StyleSheet graphifyStyler;
        
        [InitializeOnLoadMethod]
        static void RegisterTool()
        {
            ShortcutHelper.RegisterDefaultShortcuts<RecipeGraphWindow>(RecipeStencil.toolName);
        }

        [MenuItem("GTF Samples/Recipe Editor", false)]
        public static void ShowRecipeGraphWindow()
        {
            FindOrCreateGraphWindow<RecipeGraphWindow>();
        }
        
        //TODO::(Z) Matching variables used for the hack as described in update.
        private Unity.Properties.UI.PropertyElement sidePanelTarget;
        private INodeModel prevTarget = null;
        private bool CreatedBlackboard => (rootVisualElement.Q<CustomBlackboard>() != null);
        protected override void OnEnable()
        {
            base.OnEnable();
            EditorToolName = "Recipe Editor";
            rootVisualElement.styleSheets.Add(graphifyStyler);
            
            sidePanelTarget = 
                m_SidePanel.Q("sidePanelInspector") as Unity.Properties.UI.PropertyElement;
            
            rootVisualElement.schedule.Execute(TryCreateBlackboard).StartingIn(10);
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
            TryCreateBlackboard();
        }

        private void TryCreateBlackboard()
        {
            if (CreatedBlackboard) return;
            var mew = m_GraphView?.GraphModel?.AssetModel;
            if (mew is not RecipeGraphAssetModel graphifyAssetModel) return;
            CustomBlackboard blackboard = new CustomBlackboard(graphifyAssetModel.blackboardData);
            rootVisualElement.Q("graphContainer").Add(blackboard);
            blackboard.BringToFront();
        }

        protected override GraphView CreateGraphView()
        {
            return new RecipeGraphView(this, CommandDispatcher);
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
                var template = new GraphTemplate<RecipeStencil>(RecipeStencil.graphName);
                return AddNewGraphButton<RecipeGraphAssetModel>(template);
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