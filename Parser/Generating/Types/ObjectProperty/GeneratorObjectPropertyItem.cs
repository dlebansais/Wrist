using System.IO;

namespace Parser
{
    public class GeneratorObjectPropertyItem : GeneratorObjectProperty, IGeneratorObjectPropertyItem
    {
        public GeneratorObjectPropertyItem(IObjectPropertyItem property, IGeneratorObject obj)
            : base(property, obj)
        {
            BaseProperty = property;
        }

        private IObjectPropertyItem BaseProperty;

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
            GenerateDeclaration(domain, cSharpWriter, $"I{NestedObject.CSharpName}");
        }
    }
}
