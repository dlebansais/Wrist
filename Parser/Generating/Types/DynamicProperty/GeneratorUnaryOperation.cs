using System;
using System.Collections.Generic;

namespace Parser
{
    public class GeneratorUnaryOperation : IGeneratorUnaryOperation
    {
        public GeneratorUnaryOperation(IUnaryOperation operation)
        {
            Type = operation.Type;
            Operand = GeneratorDynamicProperty.ConvertOperation(operation.Operand);
        }

        public DynamicOperationTypes Type { get; set; }
        public IGeneratorDynamicOperation Operand { get; set; }

        public bool Connect(IGeneratorDomain domain)
        {
            bool IsConnected = false;

            IsConnected |= Operand.Connect(domain);

            return IsConnected;
        }

        public void GetUsedObjects(Dictionary<IGeneratorObject, List<IGeneratorObjectProperty>> usedObjectTable)
        {
            Operand.GetUsedObjects(usedObjectTable);
        }

        public bool GetComposedValue(IGeneratorObject obj, IGeneratorObjectProperty objectProperty, out string ComposedValue)
        {
            string NestedComposedValue;
            bool IsUsed = Operand.GetComposedValue(obj, objectProperty, out NestedComposedValue);

            switch (Type)
            {
                case DynamicOperationTypes.NOT:
                    ComposedValue = $"!({NestedComposedValue})";
                    break;

                case DynamicOperationTypes.IS_EMPTY:
                    ComposedValue = $"string.IsNullOrEmpty({NestedComposedValue})";
                    break;

                default:
                    throw new InvalidOperationException();
            }

            return IsUsed;
        }

        public override string ToString()
        {
            string OperandString = Operand.ToString();
            return $"{Type} ({OperandString})";
        }
    }
}
