using System.IO;

namespace Parser
{
    public class GeneratorObjectPropertyState : GeneratorObjectPropertyIndex, IGeneratorObjectPropertyState
    {
        public GeneratorObjectPropertyState(IObjectPropertyState property, IGeneratorObject obj)
            : base(property, obj)
        {
        }

        public override bool Connect(IGeneratorDomain domain)
        {
            return false;
        }

        public override void Generate(IGeneratorDomain domain, StreamWriter cSharpWriter)
        {
            cSharpWriter.WriteLine($"        {Object.CSharpName}States {CSharpName} {{ get; set; }}");
        }
    }
}
