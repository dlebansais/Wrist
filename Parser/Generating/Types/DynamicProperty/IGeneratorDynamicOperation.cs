using System.Collections.Generic;

namespace Parser
{
    public interface IGeneratorDynamicOperation
    {
        bool Connect(IGeneratorDomain domain);
        void GetUsedObjects(Dictionary<IGeneratorObject, List<IGeneratorObjectProperty>> usedObjectTable);
        bool GetComposedValue(IGeneratorObject obj, IGeneratorObjectProperty objectProperty, out string ComposedValue);
    }
}
