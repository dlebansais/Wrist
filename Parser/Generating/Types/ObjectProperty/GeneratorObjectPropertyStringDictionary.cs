using System.IO;

namespace Parser
{
    public class GeneratorObjectPropertyStringDictionary : GeneratorObjectProperty, IGeneratorObjectPropertyStringDictionary
    {
        public static GeneratorObjectPropertyStringDictionary StringsProperty = new GeneratorObjectPropertyStringDictionary(ObjectPropertyStringDictionary.StringsProperty, null);

        public GeneratorObjectPropertyStringDictionary(IObjectPropertyStringDictionary property, IGeneratorObject obj)
            : base(property, obj)
        {
            if (property == ObjectPropertyStringDictionary.StringsProperty)
                GeneratorObjectPropertyMap.Add(property, this);
        }

        public override bool Connect(IGeneratorDomain domain)
        {
            return false;
        }

        public override void Generate(IGeneratorDomain domain, StreamWriter cSharpWriter)
        {
            cSharpWriter.WriteLine($"        Dictionary<string, string> {CSharpName} {{ get; }}");
        }
    }
}
