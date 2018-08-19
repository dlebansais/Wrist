using System.IO;

namespace Parser
{
    public class GeneratorObjectPropertyItemList : GeneratorObjectProperty, IGeneratorObjectPropertyItemList
    {
        public GeneratorObjectPropertyItemList(IObjectPropertyItemList property, IGeneratorObject obj)
            : base(property, obj)
        {
            BaseProperty = property;
        }

        private IObjectPropertyItemList BaseProperty;

        public IGeneratorObject NestedObject { get; private set; }

        public override bool Connect(IGeneratorDomain domain)
        {
            bool IsConnected = false;

            if (NestedObject == null)
            {
                IsConnected = true;

                if (GeneratorObject.GeneratorObjectMap.ContainsKey(BaseProperty.NestedObject))
                    NestedObject = GeneratorObject.GeneratorObjectMap[BaseProperty.NestedObject];
            }

            return IsConnected;
        }

        public override void Generate(IGeneratorDomain domain, StreamWriter cSharpWriter)
        {
            GenerateDeclaration(domain, cSharpWriter, $"ObservableCollection<I{NestedObject.CSharpName}>");
        }
    }
}
