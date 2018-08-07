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
            IObjectPropertyInteger AsObjectPropertyInteger;
            IObjectPropertyBoolean AsObjectPropertyBoolean;
            IObjectPropertyString AsObjectPropertyString;
            IObjectPropertyReadonlyString AsObjectPropertyReadonlyString;
            IObjectPropertyStringDictionary AsObjectPropertyStringDictionary;
            IObjectPropertyStringList AsObjectPropertyStringList;
            IObjectPropertyState AsObjectPropertyState;
            IObjectPropertyItem AsObjectPropertyItem;
            IObjectPropertyItemList AsObjectPropertyItemList;

            if ((AsObjectPropertyInteger = property as IObjectPropertyInteger) != null)
                Result = new GeneratorObjectPropertyInteger(AsObjectPropertyInteger, obj);
            else if ((AsObjectPropertyBoolean = property as IObjectPropertyBoolean) != null)
                Result = new GeneratorObjectPropertyBoolean(AsObjectPropertyBoolean, obj);
            else if ((AsObjectPropertyString = property as IObjectPropertyString) != null)
                Result = new GeneratorObjectPropertyString(AsObjectPropertyString, obj);
            else if ((AsObjectPropertyReadonlyString = property as IObjectPropertyReadonlyString) != null)
                Result = new GeneratorObjectPropertyReadonlyString(AsObjectPropertyReadonlyString, obj);
            else if ((AsObjectPropertyStringDictionary = property as IObjectPropertyStringDictionary) != null)
                Result = new GeneratorObjectPropertyStringDictionary(AsObjectPropertyStringDictionary, obj);
            else if ((AsObjectPropertyStringList = property as IObjectPropertyStringList) != null)
                Result = new GeneratorObjectPropertyStringList(AsObjectPropertyStringList, obj);
            else if ((AsObjectPropertyState = property as IObjectPropertyState) != null)
                Result = new GeneratorObjectPropertyState(AsObjectPropertyState, obj);
            else if ((AsObjectPropertyItem = property as IObjectPropertyItem) != null)
                Result = new GeneratorObjectPropertyItem(AsObjectPropertyItem, obj);
            else if ((AsObjectPropertyItemList = property as IObjectPropertyItemList) != null)
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
