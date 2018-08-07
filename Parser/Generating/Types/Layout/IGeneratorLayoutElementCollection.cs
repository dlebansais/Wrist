using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Parser
{
    public interface IGeneratorLayoutElementCollection : IList<IGeneratorLayoutElement>
    {
        ReadOnlyCollection<IGeneratorLayoutElement> AsReadOnly();
    }
}
