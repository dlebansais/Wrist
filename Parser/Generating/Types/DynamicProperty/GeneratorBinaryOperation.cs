using System;
using System.Collections.Generic;

namespace Parser
{
    public class GeneratorBinaryOperation : IGeneratorBinaryOperation
    {
        public GeneratorBinaryOperation(IBinaryOperation operation)
        {
            Type = operation.Type;
            Operand1 = GeneratorDynamicProperty.ConvertOperation(operation.Operand1);
            Operand2 = GeneratorDynamicProperty.ConvertOperation(operation.Operand2);
        }

        public DynamicOperationTypes Type { get; set; }
        public IGeneratorDynamicOperation Operand1 { get; set; }
        public IGeneratorDynamicOperation Operand2 { get; set; }

        public bool Connect(IGeneratorDomain domain)
        {
            bool IsConnected = false;

            IsConnected |= Operand1.Connect(domain);
            IsConnected |= Operand2.Connect(domain);

            return IsConnected;
        }

        public void GetUsedObjects(Dictionary<IGeneratorObject, List<IGeneratorObjectProperty>> usedObjectTable)
        {
            Operand1.GetUsedObjects(usedObjectTable);
            Operand2.GetUsedObjects(usedObjectTable);
        }

        public bool GetComposedValue(IGeneratorObject obj, IGeneratorObjectProperty objectProperty, out string ComposedValue)
        {
            string NestedComposedValue1;
            bool IsUsed1 = Operand1.GetComposedValue(obj, objectProperty, out NestedComposedValue1);
            string NestedComposedValue2;
            bool IsUsed2 = Operand2.GetComposedValue(obj, objectProperty, out NestedComposedValue2);

            switch (Type)
            {
                case DynamicOperationTypes.OR:
                    ComposedValue = $"({NestedComposedValue1}) || ({NestedComposedValue2})";
                    break;

                case DynamicOperationTypes.AND:
                    ComposedValue = $"({NestedComposedValue1}) && ({NestedComposedValue2})";
                    break;

                case DynamicOperationTypes.EQUALS:
                    ComposedValue = $"({NestedComposedValue1}) == ({NestedComposedValue2})";
                    break;

                default:
                    throw new InvalidOperationException();
            }

            return IsUsed1 || IsUsed2;
        }

        public override string ToString()
        {
            string Operand1String = Operand1.ToString();
            string Operand2String = Operand2.ToString();
            return $"({Operand1String}) {Type} ({Operand2String})";
        }
    }
}
