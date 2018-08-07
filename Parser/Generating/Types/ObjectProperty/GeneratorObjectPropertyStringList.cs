using System.IO;

namespace Parser
{
    public class GeneratorObjectPropertyStringList : GeneratorObjectProperty, IGeneratorObjectPropertyStringList
    {
        public GeneratorObjectPropertyStringList(IObjectPropertyStringList property, IGeneratorObject obj)
            : base(property, obj)
        {
        }

        public override bool Connect(IGeneratorDomain domain)
        {
            return false;
        }

        public override void Generate(IGeneratorDomain domain, StreamWriter cSharpWriter)
        {
            cSharpWriter.WriteLine($"        List<string> {CSharpName} {{ get; }}");
        }
    }
}
