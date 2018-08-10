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
            else if (property is IObjectPropertyState AsObjectPropertyState)
                Result = new GeneratorObjectPropertyState(AsObjectPropertyState, obj);
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
            IsEncrypted = property.IsEncrypted;
        }

        public IDeclarationSource NameSource { get; private set; }
        public string CSharpName { get; private set; }
        public IGeneratorObject Object { get; private set; }
        public bool IsEncrypted { get; private set; }

        public abstract bool Connect(IGeneratorDomain domain);

        public override string ToString()
        {
            string Encrypted = IsEncrypted ? ", Encrypted" : "";
            return $"{Object.CSharpName}.{NameSource.Name} property{Encrypted}";
        }

        public abstract void Generate(IGeneratorDomain domain, StreamWriter cSharpWriter);
    }
}
