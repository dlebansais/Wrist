using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Parser
{
    public interface ILayoutElementCollection : IList<ILayoutElement>
    {
        ReadOnlyCollection<ILayoutElement> AsReadOnly();
        ILayoutElementCollection GetClone();
    }
}
