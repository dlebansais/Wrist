using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Parser
{
    public interface IGeneratorObjectPropertyCollection : IList<IGeneratorObjectProperty>
    {
        ReadOnlyCollection<IGeneratorObjectProperty> AsReadOnly();
    }
}
