using System;
using UnityEngine;

namespace Vampire.Runtime
{
    [Serializable]
    public class GraphifyNode : Attribute
    {
        public readonly string path;
        public readonly Type graphifyBlueprintType;

        public GraphifyNode(Type blueprintType, string searchPath)
        {
            if (!typeof(RuntimeGraphBlueprint).IsAssignableFrom(blueprintType))
            {
                Debug.LogError("Node with path: " + searchPath + " attempted to register a blueprint type that does not derive RuntimeGraphBlueprint!");
                return;
            }
            graphifyBlueprintType = blueprintType;
            path = searchPath;
        }
    }
}