using System;

namespace Vampire.Runtime.SignalLinker
{
    public class BlueprintBuiltSignal : Signal<BlueprintBuiltSignal>
    {
        public readonly Type graphBlueprintType;
        public readonly string[] oldInitializationNames;
        public BlueprintBuiltSignal(Type graphBlueprintType, string[] oldInitializationNames)
        {
            this.graphBlueprintType = graphBlueprintType;
            this.oldInitializationNames = oldInitializationNames;
        }
    }
}