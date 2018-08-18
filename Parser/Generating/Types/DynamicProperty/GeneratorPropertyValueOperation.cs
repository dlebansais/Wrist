using System.Collections.Generic;

namespace Parser
{
    public class GeneratorPropertyValueOperation : IGeneratorPropertyValueOperation
    {
        public GeneratorPropertyValueOperation(IPropertyValueOperation operation)
        {
            ValueKey = operation.ValueKey;
            BaseOperation = operation;
        }

        private IPropertyValueOperation BaseOperation;

        public IGeneratorObject ValueObject { get; private set; }
        public IGeneratorObjectProperty ValueObjectProperty { get; private set; }
        public IDeclarationSource ValueKey { get; private set; }

        public bool Connect(IGeneratorDomain domain)
        {
            bool IsConnected = false;

            if (ValueObject == null)
            {
                IsConnected = true;

                if (GeneratorObject.GeneratorObjectMap.ContainsKey(BaseOperation.ValueObject))
                    ValueObject = GeneratorObject.GeneratorObjectMap[BaseOperation.ValueObject];
            }

            if (ValueObjectProperty == null)
            {
                IsConnected = true;

                if (GeneratorObjectProperty.GeneratorObjectPropertyMap.ContainsKey(BaseOperation.ValueObjectProperty))
                    ValueObjectProperty = GeneratorObjectProperty.GeneratorObjectPropertyMap[BaseOperation.ValueObjectProperty];
            }

            return IsConnected;
        }

        public void GetUsedObjects(Dictionary<IGeneratorObject, List<IGeneratorObjectProperty>> usedObjectTable)
        {
            if (!usedObjectTable.ContainsKey(ValueObject))
                usedObjectTable.Add(ValueObject, new List<IGeneratorObjectProperty>());

            if (!usedObjectTable[ValueObject].Contains(ValueObjectProperty))
                usedObjectTable[ValueObject].Add(ValueObjectProperty);
        }

        public bool GetComposedValue(IGeneratorObject obj, IGeneratorObjectProperty objectProperty, out string ComposedValue)
        {
            bool IsUsed = (obj == ValueObject) && (objectProperty == ValueObjectProperty);

            ComposedValue = $"Page.{ValueObject.CSharpName}.{ValueObjectProperty.CSharpName}";

            return IsUsed;
        }

        public override string ToString()
        {
            return $"{ValueObject.CSharpName}.{ValueObjectProperty.CSharpName}";
        }
    }
}
