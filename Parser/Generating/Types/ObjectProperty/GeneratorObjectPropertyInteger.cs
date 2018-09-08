using System.IO;

namespace Parser
{
    public class GeneratorObjectPropertyInteger : GeneratorObjectPropertyIndex, IGeneratorObjectPropertyInteger
    {
        public static GeneratorObjectPropertyInteger NavigationIndexProperty = new GeneratorObjectPropertyInteger(ObjectPropertyInteger.NavigationIndexProperty, null);

        public GeneratorObjectPropertyInteger(IObjectPropertyInteger property, IGeneratorObject obj)
            : base(property, obj)
        {
            if (property == ObjectPropertyInteger.NavigationIndexProperty)
                GeneratorObjectPropertyMap.Add(property, this);
        }

        public override bool Connect(IGeneratorDomain domain)
        {
            return false;
        }

        public override void Generate(IGeneratorDomain domain, StreamWriter cSharpWriter)
        {
            GenerateDeclaration(domain, cSharpWriter, "int");
        }
    }
}
