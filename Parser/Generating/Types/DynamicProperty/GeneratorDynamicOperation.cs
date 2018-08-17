using System;

namespace Parser
{
    public class GeneratorDynamicOperation : IGeneratorDynamicOperation
    {
        public GeneratorDynamicOperation(DynamicOperation operation)
        {
            Type = operation.Type;
            Operand1 = GeneratorDynamicOperation.Convert(operation.Operand1);
            Operand2 = GeneratorDynamicOperation.Convert(operation.Operand2);
        }

        public DynamicOperationTypes Type { get; set; }
        public IGeneratorDynamicOperation Operand1 { get; set; }
        public IGeneratorDynamicOperation Operand2 { get; set; }

        public static IGeneratorDynamicOperation Convert(IDynamicOperation operand)
        {
            if (operand is DynamicOperation AsOperation)
                return new GeneratorDynamicOperation(AsOperation);
            else if (operand is PropertyValueOperation AsPropertyValue)
                return new GeneratorPropertyValueOperation(AsPropertyValue);
            else if (operand == null)
                return null;
            else
                throw new InvalidCastException();
        }
    }
}
