using System.IO;

namespace Parser
{
    public class GeneratorObjectPropertyString : GeneratorObjectProperty, IGeneratorObjectPropertyString
    {
        public GeneratorObjectPropertyString(IObjectPropertyString property, IGeneratorObject obj)
            : base(property, obj)
        {
            MaximumLength = property.MaximumLength;
            Category = property.Category;
        }

        public int MaximumLength { get; private set; }
        public ObjectPropertyStringCategory Category { get; private set; }

        public override bool Connect(IGeneratorDomain domain)
        {
            return false;
        }

        public override void Generate(IGeneratorDomain domain, StreamWriter cSharpWriter)
        {
            cSharpWriter.WriteLine($"        string {CSharpName} {{ get; set; }}");
        }

        public override string ToString()
        {
            return base.ToString() +
                ((MaximumLength != int.MaxValue) ? (", Maximum Length=" + MaximumLength.ToString()) : "") +
                ((Category != ObjectPropertyStringCategory.Normal) ? (", Category=" + Category.ToString()) : "");
        }
    }
}
