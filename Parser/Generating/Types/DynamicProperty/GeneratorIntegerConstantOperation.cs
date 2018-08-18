using System.Collections.Generic;

namespace Parser
{
    public class GeneratorIntegerConstantOperation : IGeneratorIntegerConstantOperation
    {
        public GeneratorIntegerConstantOperation(IIntegerConstantOperation operation)
        {
            Value = operation.Value;
        }

        public int Value { get; private set; }

        public bool Connect(IGeneratorDomain domain)
        {
            return false;
        }

        public void GetUsedObjects(Dictionary<IGeneratorObject, List<IGeneratorObjectProperty>> usedObjectTable)
        {
        }

        public bool GetComposedValue(IGeneratorObject obj, IGeneratorObjectProperty objectProperty, out string ComposedValue)
        {
            ComposedValue = Value.ToString();
            return false;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
