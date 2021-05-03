using System;

namespace Vampire.Runtime.SignalLinker
{
    public class BlueprintBuiltSignal : Signal<BlueprintBuiltSignal>
    {
        public readonly Type graphBlueprintType;
        public BlueprintBuiltSignal(Type graphBlueprintType)
        {
            this.graphBlueprintType = graphBlueprintType;
        }
    }
}