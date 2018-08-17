using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Parser
{
    public interface IDynamicPropertyCollection : IList<IDynamicProperty>
    {
        ReadOnlyCollection<IDynamicProperty> AsReadOnly();
    }
}
