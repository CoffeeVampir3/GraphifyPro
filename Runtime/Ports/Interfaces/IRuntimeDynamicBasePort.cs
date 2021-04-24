using System.Collections.Generic;

namespace Vampire.Graphify.Runtime
{
    public interface IRuntimeDynamicBasePort : IRuntimeBasePort
    {
        List<Link> this[int index]
        {
            get;
        }
    }
}