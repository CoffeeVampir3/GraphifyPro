using System;
using System.Collections.Generic;
using System.Linq;

namespace Vampire.Graphify.EditorOnly
{
    internal static class Generics
    {
        /// <summary>
        /// Unfucks a generic type name into a system readable type.
        /// </summary>
        public static string Unfuck( Type t ) {
            if (!t.IsGenericType) return t.FullName;
            var genericArgs = t.GetGenericArguments().ToList();

            return Unfuck( t, genericArgs );
        }

        private static string Unfuck( Type t, List<Type> availableArguments ) {
            if (!t.IsGenericType) return t.FullName;

            string value = t.FullName;
            if( value.IndexOf("`", StringComparison.Ordinal) > -1 ) {
                value = value.Substring( 0, value.IndexOf( "`", StringComparison.Ordinal) );
            }

            if( t.DeclaringType != null ) {
                // This is a nested type, build the nesting type first
                value = Unfuck( t.DeclaringType, availableArguments ) + "+" + value;
            }

            // Build the type arguments (if any)
            string argString = "";
            var thisTypeArgs = t.GetGenericArguments();
            for( int i = 0; i < thisTypeArgs.Length && availableArguments.Count > 0; i++ ) {
                if( i != 0 ) argString += ", ";

                argString += Unfuck( availableArguments[0]);
                availableArguments.RemoveAt( 0 );
            }

            // If there are type arguments, add them with < >
            if( argString.Length > 0 ) {
                value += "<" + argString + ">";
            }

            return value;
        }
    }
}