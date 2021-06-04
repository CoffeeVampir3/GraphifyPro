using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.GraphToolsFoundation.Overdrive;
using UnityEditor.GraphToolsFoundation.Overdrive.BasicModel;
using UnityEngine;
using UnityEngine.UIElements;
using Vampire.Runtime;
using Vampire.Runtime.SignalLinker;
using PropertyElement = Unity.Properties.UI.PropertyElement;

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

        private static bool SidePanelAttribFilter(IEnumerable<Attribute> attribs)
        {
            bool isShown = false;
            foreach (var item in attribs)
            {
                if (item is IShowInNodeInspector)
                    isShown = true;
                if (item is not PortDefinition pd) continue;
                if (!pd.showBackingValue)
                    return false;
            }
            
            return isShown;
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
            
            Debug.Assert(sidePanelTarget != null, nameof(sidePanelTarget) + " -- side panel was null?");
            var handler = new PropertyElement.AttributeFilterHandler(SidePanelAttribFilter);
            sidePanelTarget.SetAttributeFilter(handler);
            sidePanelTarget.AddToClassList("sidePanelStyler");
            
            VisitNodeIdSignal.RegisterListener(ListenForNodeVisitedSignal);
            EditorApplication.playModeStateChanged += OnPlaymodeChanged;
        }

        protected override void OnDisable()
        {
            VisitNodeIdSignal.UnregisterListener(ListenForNodeVisitedSignal);
            EditorApplication.playModeStateChanged -= OnPlaymodeChanged;
        }

        private void OnPlaymodeChanged(PlayModeStateChange change)
        {
            switch (change)
            {
                case PlayModeStateChange.EnteredEditMode:
                {
                    var nodes = this.m_GraphView.Nodes;
                    foreach (var node in nodes)
                    {
                        node.RemoveFromClassList(VisitNodeIdSignal.activeNodeCssClass);
                    }
                    break;
                }
            }
        }

        //TODO:: Temp
        private void ListenForNodeVisitedSignal(VisitNodeIdSignal sig)
        {
            var nodes = this.m_GraphView.Nodes;
            foreach (var node in nodes)
            {
                if (node is not DynamicNodeUI rtNode)
                    continue;
                if (rtNode.NodeModel is not IHasRuntimeNode rtNodeModel)
                    continue;
                if (rtNodeModel.RuntimeNodeId == sig.nodeId)
                {
                    node.AddToClassList(sig.addedCssUponVisit);
                }
                else
                {
                    node.RemoveFromClassList(sig.addedCssUponVisit);
                }
            }
        }
        
        protected override void Update()
        {
            base.Update();
            //TODO::(Z) NOTICE! Hack, this is likely to break in new versions!
            //We're comparing the current property panel target to the previous one
            //If they're different we clear and reset the target which seems to solve
            //the issue of the side panel not correctly updating.
            if (!sidePanelTarget.TryGetTarget<INodeModel>(out var currentTarget)
                || currentTarget == prevTarget)
            {
                prevTarget = currentTarget;
                return;
            }
            
            prevTarget = currentTarget;
            sidePanelTarget.ClearTarget();
            sidePanelTarget.SetTarget(currentTarget);

            //Combined with the styles set in GraphStyles.css we're able to hide
            //noisy garbage created by the side panel inspector.
            var rtNodeItem = m_SidePanel.Q("runtimeNode");
            if (rtNodeItem == null)
                return;
            
            var toggle = rtNodeItem.Q<Toggle>();
            toggle?.SetValueWithoutNotify(true);
            var contentContainer = rtNodeItem.Q("unity-content");
            contentContainer.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);

            //Hides foldouts with no items.
            foreach(var item in rtNodeItem.Query<Foldout>().ToList())
            {
                item.style.display = item.contentContainer.childCount == 0 
                    ? new StyleEnum<DisplayStyle>(DisplayStyle.None) : 
                    new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
            }
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