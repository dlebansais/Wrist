using System;
using System.Collections.Generic;
using System.IO;

namespace Parser
{
    public abstract class GeneratorObjectProperty : IGeneratorObjectProperty
    {
        public static Dictionary<IObjectProperty, IGeneratorObjectProperty> GeneratorObjectPropertyMap { get; } = new Dictionary<IObjectProperty, IGeneratorObjectProperty>();

        public static IGeneratorObjectProperty Convert(IObjectProperty property, IGeneratorObject obj)
        {
            IGeneratorObjectProperty Result;

            if (property is IObjectPropertyInteger AsObjectPropertyInteger)
                Result = new GeneratorObjectPropertyInteger(AsObjectPropertyInteger, obj);
            else if (property is IObjectPropertyBoolean AsObjectPropertyBoolean)
                Result = new GeneratorObjectPropertyBoolean(AsObjectPropertyBoolean, obj);
            else if (property is IObjectPropertyString AsObjectPropertyString)
                Result = new GeneratorObjectPropertyString(AsObjectPropertyString, obj);
            else if (property is IObjectPropertyReadonlyString AsObjectPropertyReadonlyString)
                Result = new GeneratorObjectPropertyReadonlyString(AsObjectPropertyReadonlyString, obj);
            else if (property is IObjectPropertyStringDictionary AsObjectPropertyStringDictionary)
                Result = new GeneratorObjectPropertyStringDictionary(AsObjectPropertyStringDictionary, obj);
            else if (property is IObjectPropertyStringList AsObjectPropertyStringList)
                Result = new GeneratorObjectPropertyStringList(AsObjectPropertyStringList, obj);
            else if (property is IObjectPropertyEnum AsObjectPropertyEnum)
                Result = new GeneratorObjectPropertyEnum(AsObjectPropertyEnum, obj);
            else if (property is IObjectPropertyItem AsObjectPropertyItem)
                Result = new GeneratorObjectPropertyItem(AsObjectPropertyItem, obj);
            else if (property is IObjectPropertyItemList AsObjectPropertyItemList)
                Result = new GeneratorObjectPropertyItemList(AsObjectPropertyItemList, obj);
            else
                throw new InvalidOperationException();

            GeneratorObjectPropertyMap.Add(property, Result);
            return Result;
        }

        public GeneratorObjectProperty(IObjectProperty property, IGeneratorObject obj)
        {
            NameSource = property.NameSource;
            CSharpName = property.CSharpName;
            IsRead = property.IsRead;
            IsWrite = property.IsWrite;
            Object = obj;
        }

        public IDeclarationSource NameSource { get; private set; }
        public string CSharpName { get; private set; }
        public bool IsRead { get; private set; }
        public bool IsWrite { get; private set; }
        public IGeneratorObject Object { get; private set; }

        public abstract bool Connect(IGeneratorDomain domain);

        public override string ToString()
        {
            string Access = (IsRead ? "R" : "") + (IsWrite ? "W" : "");
            return $"{Object.CSharpName}.{NameSource.Name} {Access} property";
        }

        public abstract void Generate(IGeneratorDomain domain, StreamWriter cSharpWriter);

        protected void GenerateDeclaration(IGeneratorDomain domain, StreamWriter cSharpWriter, string typeName)
        {
            string Indentation = "        ";

            if (IsRead && IsWrite)
                cSharpWriter.WriteLine($"{Indentation}{typeName} {CSharpName} {{ get; set; }}");
            else if (IsRead)
                cSharpWriter.WriteLine($"{Indentation}{typeName} {CSharpName} {{ get; }}");
            else if (IsWrite)
                cSharpWriter.WriteLine($"{Indentation}{typeName} {CSharpName} {{ set; }}");
        }
    }
}
