namespace Vampire.Runtime.SignalLinker
{
    public class VisitNodeIdSignal : Signal<VisitNodeIdSignal>
    {
        public readonly short nodeId;
        public readonly string addedCssUponVisit;
        public static readonly string activeNodeCssClass = "activeNode";

        public VisitNodeIdSignal(short nodeId, string addedCssUponVisit)
        {
            this.nodeId = nodeId;
            this.addedCssUponVisit = addedCssUponVisit;
        }
    }
}