namespace Vampire.Runtime
{
    /// <summary>
    /// Used by the Graphify Code Generator, allows us to have unsafe implementations
    /// that are not user facing.
    /// </summary>
    public interface IUnsafePropertyDictionary
    {
        T Unsafe_GetDirect<T>(string key);
        void Unsafe_SetDirect(string key, object o);
        void Unsafe_SetWrapped<T>(string key, T wrappedItem);
    }
}