namespace Vampire.Runtime
{
    public interface IPortWithValue
    {
        internal object GetInitValue();
        object GetValueDirect(Link link, RuntimeGraph graph);

        public bool TryGetValueAs<SomeType>(Link link, RuntimeGraph graph, out SomeType val);
    }

    public interface IPortWithValue<T> : IPortWithValue
    {
        public bool TryGetValue(Link link, RuntimeGraph graph, out T val);
    }
}