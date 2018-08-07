using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Parser
{
    public interface IComponentCollection : IList<IComponent>
    {
        ReadOnlyCollection<IComponent> AsReadOnly();
    }
}
