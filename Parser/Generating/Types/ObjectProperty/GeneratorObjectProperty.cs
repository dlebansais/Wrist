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
            Object = obj;
        }

        public IDeclarationSource NameSource { get; private set; }
        public string CSharpName { get; private set; }
        public IGeneratorObject Object { get; private set; }

        public abstract bool Connect(IGeneratorDomain domain);

        public override string ToString()
        {
            return $"{Object.CSharpName}.{NameSource.Name} property";
        }

        public abstract void Generate(IGeneratorDomain domain, StreamWriter cSharpWriter);
    }
}
