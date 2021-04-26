namespace Vampire.Runtime
{
    public interface IPortWithValue
    {
        internal object GetInitValue();
        object GetValueDirect(Link link, RuntimeGraph graph);

        public bool TryGetValueAs<SomeType>(Link link, out SomeType val);
    }

    public interface IPortWithValue<T> : IPortWithValue
    {
        public T LocalValue { get; set; }

        public bool TryGetValue(Link link, out T val);
    }
}