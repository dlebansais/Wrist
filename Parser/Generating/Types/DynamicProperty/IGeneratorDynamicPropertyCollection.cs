using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Parser
{
    public interface IGeneratorDynamicPropertyCollection : IList<IGeneratorDynamicProperty>
    {
        ReadOnlyCollection<IGeneratorDynamicProperty> AsReadOnly();
    }
}
