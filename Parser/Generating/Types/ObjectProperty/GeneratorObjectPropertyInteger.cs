using System.IO;

namespace Parser
{
    public class GeneratorObjectPropertyInteger : GeneratorObjectPropertyIndex, IGeneratorObjectPropertyInteger
    {
        public GeneratorObjectPropertyInteger(IObjectPropertyInteger property, IGeneratorObject obj)
            : base(property, obj)
        {
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
