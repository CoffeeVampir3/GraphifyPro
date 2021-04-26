using System.Collections.Generic;

namespace Vampire.Runtime
{
    public interface IRuntimeDynamicBasePort : IRuntimeBasePort
    {
        List<Link> this[int index]
        {
            get;
        }
    }
}