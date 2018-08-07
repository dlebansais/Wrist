using System.IO;

namespace Parser
{
    public class GeneratorObjectPropertyReadonlyString : GeneratorObjectProperty, IGeneratorObjectPropertyReadonlyString
    {
        public GeneratorObjectPropertyReadonlyString(IObjectPropertyReadonlyString property, IGeneratorObject obj)
            : base(property, obj)
        {
        }

        public override bool Connect(IGeneratorDomain domain)
        {
            return false;
        }

        public override void Generate(IGeneratorDomain domain, StreamWriter cSharpWriter)
        {
            cSharpWriter.WriteLine($"        string {CSharpName} {{ get; }}");
        }
    }
}
