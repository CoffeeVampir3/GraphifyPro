namespace Vampire.Graphify.EditorOnly
{
    internal static class PCGConstants
    {
        public const string publicStaticPrefix = "public static ";
        public const string singleSpace = " ";
        public const string blackboardStart =
            @"namespace Vampire.Runtime {"
            + "\n\t" +
            @"public static partial class Properties {"
            + "\n\t\t" +
            @"public sealed class ";

        public const string generatedDisclaimer =
@"/* ~~~~~~~~~~~~~~~~~~Graphify automatically generated file.~~~~~~~~~~~~~~~~~~
These files are generated when you compile a graph with generate static 
graph properties option enabled. 
Deleting this file will make this graph's statically typed properties 
inaccessible and may cause compilation errors if they're being used by 
you or someone else in your project. Delete with caution!
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/";
        
        public const string disclaimerAndCodeStart = generatedDisclaimer + "\n" + blackboardStart;
    }
}