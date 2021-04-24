namespace Vampire.Graphify.Runtime
{
    public interface IHasNodeConnections
    {
        RuntimeNode GetConnectedNode(Link link, RuntimeGraph graph);
    }
}