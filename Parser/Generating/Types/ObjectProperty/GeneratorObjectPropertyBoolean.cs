using System.IO;

namespace Parser
{
    public class GeneratorObjectPropertyBoolean : GeneratorObjectPropertyIndex, IGeneratorObjectPropertyBoolean
    {
        public GeneratorObjectPropertyBoolean(IObjectPropertyBoolean property, IGeneratorObject obj)
            : base(property, obj)
        {
            IsClosingPopup = property.IsClosingPopup;
        }

        public bool IsClosingPopup { get; private set; }

        public override bool Connect(IGeneratorDomain domain)
        {
            return false;
        }

        public override void Generate(IGeneratorDomain domain, StreamWriter cSharpWriter)
        {
            GenerateDeclaration(domain, cSharpWriter, "bool");

            if (IsClosingPopup)
            {
                string Indentation = "        ";
                cSharpWriter.WriteLine($"{Indentation}void OnPopupClosed_{CSharpName}();");
            }
        }
    }
}
