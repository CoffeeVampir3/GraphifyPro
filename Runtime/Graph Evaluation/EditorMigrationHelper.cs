using System.Collections.Generic;
using Vampire.Runtime.SignalLinker;
using Debug = UnityEngine.Debug;

namespace Vampire.Runtime
{
    /// <summary>
    /// Helper class that gets compiled out in builds, supports the EditorSignalingHelper allowing
    /// the editor change watcher to receive updates in editor but get compiled out in build.
    /// </summary>
    internal static class EditorMigrationHelper
    {
        internal static void Editor_ObserveBuildChanges(this GraphEvaluator evaluator, BlueprintBuiltSignal sig)
        {
            if (evaluator.blueprint.GetType() != sig.graphBlueprintType) return;
            //Rebuilds the runtime graph from all new data.
            var oldGraph = evaluator.rtGraph;
            evaluator.rtGraph = evaluator.blueprint.CreateRuntimeGraph();
            evaluator.rtGraph.Editor_MigrateToNewDataset(evaluator.blueprint, sig.oldInitializationNames, oldGraph);
            evaluator.currentNode = Editor_ReplaceRebuiltNode(evaluator, evaluator.currentNode);
            evaluator.nextNode = Editor_ReplaceRebuiltNode(evaluator, evaluator.nextNode);
            evaluator.rootContext = new Context(evaluator.rootContext, evaluator.rtGraph, evaluator.Editor_ReplaceRebuiltNode);
        }
        
        private static RuntimeNode Editor_ReplaceRebuiltNode(this GraphEvaluator evaluator, RuntimeNode existing)
        {
            return evaluator.currentNode.nodeId > evaluator.rtGraph.nodes.Length ? 
                evaluator.rtGraph.nodes[evaluator.rootNodeIndex] : evaluator.rtGraph.nodes[existing.nodeId];
        }
        
        private static void Editor_MigrateToNewDataset(this RuntimeGraph migratingTo, RuntimeGraphBlueprint blueprint, string[] oldNames, RuntimeGraph oldGraph)
        {
            if (oldNames == null)
                return;

            if (oldGraph.values.Length != oldNames.Length)
            {
                Debug.Log("Migration failed because the old names did not match the value table length.");
                return;
            }
            
            HashSet<string> oldNamesHashed = new();
            foreach (var item in oldNames)
            {
                oldNamesHashed.Add(item);
            }

            Dictionary<string, object> migrationDict = new();
            for (int i = 0; i < oldGraph.values.Length; i++)
            {
                migrationDict.Add(oldNames[i], oldGraph.values[i]);
            }

            for (var i = 0; i < blueprint.initializationNames.Length; i++)
            {
                var item = blueprint.initializationNames[i];
                //Allows updating new properties but not change old node values keeping the
                //sim in tact. This is obvious kinda fragile but extremely edge-casey.
                if (item.Length < 26)
                    continue;
                if (oldNamesHashed.Contains(item))
                {
                    migratingTo.values[i] = migrationDict[item];
                }
            }

            //TODO:: Possibly inadequate? Added/new properties shouldn't need to be migrated because
            //it would require a change of code? The value-keyed properties are already added.
            migratingTo.properties = oldGraph.properties;

            if (RuntimeGraph.current == oldGraph)
                RuntimeGraph.current = migratingTo;
        }
    }
}