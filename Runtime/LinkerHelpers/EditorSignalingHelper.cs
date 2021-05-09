using System.Diagnostics;
using Vampire.Runtime.SignalLinker;

namespace Vampire.Runtime
{
    public static class EditorSignalingHelper
    {
        [Conditional("UNITY_EDITOR")]
        public static void Editor_RegisterBuildWatcher(this GraphEvaluator evaluator)
        {
            BlueprintBuiltSignal.RegisterListener(evaluator.Editor_ObserveBuildChanges);
        }
        
        [Conditional("UNITY_EDITOR")]
        public static void Editor_SendNodeVisitedSignal(this GraphEvaluator evaluator, short visitedNodeId)
        {
            VisitNodeIdSignal visitNodeSig = new VisitNodeIdSignal(visitedNodeId, VisitNodeIdSignal.activeNodeCssClass);
            visitNodeSig.Send();
        }
    }
}