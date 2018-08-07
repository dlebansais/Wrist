using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Parser
{
    public interface IObjectPropertyCollection : IList<IObjectProperty>
    {
        ReadOnlyCollection<IObjectProperty> AsReadOnly();
    }
}
