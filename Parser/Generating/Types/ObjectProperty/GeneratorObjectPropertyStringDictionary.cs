﻿using System.IO;

namespace Parser
{
    public class GeneratorObjectPropertyStringDictionary : GeneratorObjectProperty, IGeneratorObjectPropertyStringDictionary
    {
        public GeneratorObjectPropertyStringDictionary(IObjectPropertyStringDictionary property, IGeneratorObject obj)
            : base(property, obj)
        {
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
