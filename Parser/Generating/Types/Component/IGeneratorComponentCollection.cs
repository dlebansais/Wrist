using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Parser
{
    public interface IGeneratorComponentCollection : IList<IGeneratorComponent>
    {
        ReadOnlyCollection<IGeneratorComponent> AsReadOnly();
    }
}
