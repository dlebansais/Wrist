using System.IO;

namespace Parser
{
    public class GeneratorObjectPropertyEnum : GeneratorObjectPropertyIndex, IGeneratorObjectPropertyEnum
    {
        public GeneratorObjectPropertyEnum(IObjectPropertyEnum property, IGeneratorObject obj)
            : base(property, obj)
        {
        }

        public override bool Connect(IGeneratorDomain domain)
        {
            return false;
        }

        public override void Generate(IGeneratorDomain domain, StreamWriter cSharpWriter)
        {
            GenerateDeclaration(domain, cSharpWriter, CSharpName);
        }
    }
}
