using System;
using System.Collections.Generic;
using System.IO;

namespace Parser
{
    public class GeneratorDynamicProperty : IGeneratorDynamicProperty
    {
        public static Dictionary<IDynamicProperty, IGeneratorDynamicProperty> GeneratorDynamicPropertyMap { get; } = new Dictionary<IDynamicProperty, IGeneratorDynamicProperty>();

        public GeneratorDynamicProperty(IDynamicProperty dynamicProperty)
        {
            Source = dynamicProperty.Source;
            CSharpName = dynamicProperty.CSharpName;
            Result = dynamicProperty.Result;
            RootOperation = ConvertOperation(dynamicProperty.RootOperation);

            GeneratorDynamicPropertyMap.Add(dynamicProperty, this);
        }

        public IDeclarationSource Source { get; private set; }
        public string CSharpName { get; private set; }
        public DynamicOperationResults Result { get; private set; }
        public IGeneratorDynamicOperation RootOperation { get; private set; }

        public static IGeneratorDynamicOperation ConvertOperation(IDynamicOperation operand)
        {
            if (operand is IUnaryOperation AsUnary)
                return new GeneratorUnaryOperation(AsUnary);
            else if (operand is IBinaryOperation AsBinary)
                return new GeneratorBinaryOperation(AsBinary);
            else if (operand is PropertyValueOperation AsPropertyValue)
                return new GeneratorPropertyValueOperation(AsPropertyValue);
            else if (operand is IntegerConstantOperation AsIntegerConstant)
                return new GeneratorIntegerConstantOperation(AsIntegerConstant);
            else
                throw new InvalidCastException();
        }

        public bool Connect(IGeneratorDomain domain)
        {
            return RootOperation.Connect(domain);
        }

        public void GetUsedObjects(Dictionary<IGeneratorObject, List<IGeneratorObjectProperty>> usedObjectTable)
        {
            RootOperation.GetUsedObjects(usedObjectTable);
        }

        public bool Generate(IGeneratorObject obj, IGeneratorObjectProperty objectProperty, StreamWriter cSharpWriter)
        {
            string ComposedValue;
            if (RootOperation.GetComposedValue(obj, objectProperty, out ComposedValue))
            {
                cSharpWriter.WriteLine($"        public bool {CSharpName} {{ get {{ return {ComposedValue}; }} }}");
                return true;
            }
            else
                return false;
        }

        public void GenerateNotification(IGeneratorObject obj, IGeneratorObjectProperty objectProperty, StreamWriter cSharpWriter)
        {
            string ComposedValue;
            if (RootOperation.GetComposedValue(obj, objectProperty, out ComposedValue))
                cSharpWriter.WriteLine($"                NotifyPropertyChanged(nameof({CSharpName}));");
        }
    }
}
