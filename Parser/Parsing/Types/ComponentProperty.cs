using System.Collections.Generic;

namespace Parser
{
    public class ComponentProperty : IComponentProperty
    {
        public ComponentProperty(ComponentInfo info)
        {
            SetterName = info.NameSource;
            FixedValueSource = info.FixedValueSource;
            ObjectSource = info.ObjectSource;
            ObjectPropertySource = info.MemberSource;
            ObjectPropertyKey = info.KeySource;
        }

        public IDeclarationSource SetterName { get; private set; }
        public IDeclarationSource FixedValueSource { get; private set; }
        public IDeclarationSource ObjectSource { get; private set; }
        public IDeclarationSource ObjectPropertySource { get; private set; }
        public IDeclarationSource ObjectPropertyKey { get; private set; }

        public static List<IArea> AreaWithCurrentPage { get; } = new List<IArea>();

        private static void ConnectToObject(IDomain domain, IArea currentArea, IDeclarationSource objectSource, IDeclarationSource objectPropertySource, IDeclarationSource objectPropertyKey, ref IObject obj)
        {
            if (objectSource.Name == Object.TranslationObject.Name)
            {
                if (domain.Translation == null)
                    throw new ParsingException(objectSource.Source, $"Object {objectSource.Name} is used but no translation file is specified");

                obj = Object.TranslationObject;
                if (objectPropertySource.Name != ObjectPropertyStringDictionary.StringsProperty.NameSource.Name)
                    throw new ParsingException(objectPropertySource.Source, $"The only valid property for object {objectSource.Name} is {ObjectPropertyStringDictionary.StringsProperty.NameSource.Name}");

                if (objectPropertyKey == null)
                    throw new ParsingException(objectSource.Source, $"For object {objectSource.Name} property {ObjectPropertyStringDictionary.StringsProperty.NameSource.Name} a key is required");

                if (objectPropertyKey.Name != Page.CurrentPage.Name)
                {
                    if (!domain.Translation.KeyList.Contains(objectPropertyKey.Name))
                        throw new ParsingException(objectPropertyKey.Source, $"The translation file doesn't contain key '{objectPropertyKey.Name}'");
                }
                else
                {
                    // Verify it later
                    if (!AreaWithCurrentPage.Contains(currentArea))
                        AreaWithCurrentPage.Add(currentArea);
                }
            }
            else
            {
                foreach (IObject Item in domain.Objects)
                    if (Item.Name == objectSource.Name)
                    {
                        obj = Item;
                        break;
                    }

                if (obj == null)
                    throw new ParsingException(objectSource.Source, $"Unknown object {objectSource.Name}");
            }
        }

        public bool ConnectToResourceOrObject(IDomain domain, IArea currentArea, IObject currentObject, ref IResource resource, ref IObject obj, ref IObjectProperty objectProperty, ref IDeclarationSource objectPropertyKey)
        {
            bool IsConnected = false;

            if (FixedValueSource != null && resource == null)
            {
                foreach (IResource Item in domain.Resources)
                    if (Item.Name == FixedValueSource.Name)
                    {
                        resource = Item;
                        break;
                    }

                if (resource == null)
                    throw new ParsingException(FixedValueSource.Source, $"Unknown static resource {FixedValueSource.Name}");

                IsConnected = true;
            }

            else if ((ObjectSource != null || ObjectPropertySource != null) && (obj == null || objectProperty == null))
            {
                ConnectToObject(domain, currentArea, ObjectSource, ObjectPropertySource, ObjectPropertyKey, ref obj);

                foreach (IObjectProperty Property in obj.Properties)
                    if (Property.NameSource.Name == ObjectPropertySource.Name)
                    {
                        IObjectPropertyStringDictionary AsObjectPropertyStringDictionary;
                        if ((AsObjectPropertyStringDictionary = Property as IObjectPropertyStringDictionary) != null)
                        {
                            if (ObjectPropertyKey == null)
                                throw new ParsingException(ObjectPropertySource.Source, $"{obj.Name}.{ObjectPropertySource.Name} must be used with a key");
                        }
                        else
                        {
                            if (ObjectPropertyKey != null)
                                throw new ParsingException(ObjectPropertySource.Source, $"{obj.Name}.{ObjectPropertySource.Name} cannot be used with a key");
                        }

                        objectProperty = Property;
                        objectPropertyKey = ObjectPropertyKey;
                        break;
                    }

                if (objectProperty == null)
                    throw new ParsingException(ObjectPropertySource.Source, $"Unknown property {ObjectPropertySource.Name} in object {obj.Name}");

                IsConnected = true;
            }

            return IsConnected;
        }

        public bool ConnectToResource(IDomain domain, ref IResource resource)
        {
            bool IsConnected = false;

            if (FixedValueSource != null && resource == null)
            {
                foreach (IResource Item in domain.Resources)
                    if (Item.Name == FixedValueSource.Name)
                    {
                        resource = Item;
                        break;
                    }

                if (resource == null)
                    throw new ParsingException(FixedValueSource.Source, $"Unknown static resource {FixedValueSource.Name}");

                IsConnected = true;
            }

            return IsConnected;
        }

        public bool ConnectToStringList(IDomain domain, IArea currentArea, IObject currentObject, ref IResource resource, ref IObject obj, ref IObjectPropertyStringList objectProperty)
        {
            bool IsConnected = false;

            if (FixedValueSource != null && resource == null)
            {
                foreach (IResource Item in domain.Resources)
                    if (Item.Name == FixedValueSource.Name)
                    {
                        resource = Item;
                        break;
                    }

                if (resource == null)
                    throw new ParsingException(FixedValueSource.Source, $"Unknown static resource {FixedValueSource.Name}");

                IsConnected = true;
            }

            else if ((ObjectSource != null || ObjectPropertySource != null) && (obj == null || objectProperty == null))
            {
                ConnectToObject(domain, currentArea, ObjectSource, ObjectPropertySource, ObjectPropertyKey, ref obj);

                foreach (IObjectProperty Property in obj.Properties)
                    if (Property.NameSource.Name == ObjectPropertySource.Name)
                    {
                        IObjectPropertyStringList AsObjectPropertyStringList;
                        if ((AsObjectPropertyStringList = Property as IObjectPropertyStringList) != null)
                        {
                            objectProperty = AsObjectPropertyStringList;
                            break;
                        }
                        else
                            throw new ParsingException(ObjectPropertySource.Source, $"{obj.Name}.{ObjectPropertySource.Name} must be a string list property");
                    }

                if (objectProperty == null)
                    throw new ParsingException(ObjectPropertySource.Source, $"Unknown property {ObjectPropertySource.Name} in object {obj.Name}");

                IsConnected = true;
            }

            return IsConnected;
        }

        public bool ConnectToObjectIntegerOnly(IDomain domain, IArea currentArea, IObject currentObject, ref IObject obj, ref IObjectPropertyInteger objectProperty)
        {
            bool IsConnected = false;

            if ((ObjectSource != null || ObjectPropertySource != null) && (obj == null || objectProperty == null))
            {
                ConnectToObject(domain, currentArea, ObjectSource, ObjectPropertySource, ObjectPropertyKey, ref obj);

                foreach (IObjectProperty Property in obj.Properties)
                    if (Property.NameSource.Name == ObjectPropertySource.Name)
                    {
                        IObjectPropertyInteger AsObjectPropertyInteger;
                        if ((AsObjectPropertyInteger = Property as IObjectPropertyInteger) != null)
                        {
                            objectProperty = AsObjectPropertyInteger;
                            break;
                        }
                        else
                            throw new ParsingException(ObjectPropertySource.Source, $"{obj.Name}.{ObjectPropertySource.Name} must be an integer property");
                    }

                if (objectProperty == null)
                    throw new ParsingException(ObjectPropertySource.Source, $"Unknown property {ObjectPropertySource.Name} in object {obj.Name}");

                IsConnected = true;
            }

            return IsConnected;
        }

        public bool ConnectToObjectBooleanOnly(IDomain domain, IArea currentArea, IObject currentObject, ref IObject obj, ref IObjectPropertyBoolean objectProperty)
        {
            bool IsConnected = false;

            if ((ObjectSource != null || ObjectPropertySource != null) && (obj == null || objectProperty == null))
            {
                ConnectToObject(domain, currentArea, ObjectSource, ObjectPropertySource, ObjectPropertyKey, ref obj);

                foreach (IObjectProperty Property in obj.Properties)
                    if (Property.NameSource.Name == ObjectPropertySource.Name)
                    {
                        IObjectPropertyBoolean AsObjectPropertyBoolean;
                        if ((AsObjectPropertyBoolean = Property as IObjectPropertyBoolean) != null)
                        {
                            objectProperty = AsObjectPropertyBoolean;
                            break;
                        }
                        else
                            throw new ParsingException(ObjectPropertySource.Source, $"{obj.Name}.{ObjectPropertySource.Name} must be a boolean property");
                    }

                if (objectProperty == null)
                    throw new ParsingException(ObjectPropertySource.Source, $"Unknown property {ObjectPropertySource.Name} in object {obj.Name}");

                IsConnected = true;
            }

            return IsConnected;
        }

        public bool ConnectToObjectStringOnly(IDomain domain, IArea currentArea, IObject currentObject, ref IObject obj, ref IObjectPropertyString objectProperty)
        {
            bool IsConnected = false;

            if ((ObjectSource != null || ObjectPropertySource != null) && (obj == null || objectProperty == null))
            {
                ConnectToObject(domain, currentArea, ObjectSource, ObjectPropertySource, ObjectPropertyKey, ref obj);

                foreach (IObjectProperty Property in obj.Properties)
                    if (Property.NameSource.Name == ObjectPropertySource.Name)
                    {
                        IObjectPropertyString AsObjectPropertyString;
                        if ((AsObjectPropertyString = Property as IObjectPropertyString) != null)
                        {
                            objectProperty = AsObjectPropertyString;
                            break;
                        }
                        else
                            throw new ParsingException(ObjectPropertySource.Source, $"{obj.Name}.{ObjectPropertySource.Name} must be a string property");
                    }

                if (objectProperty == null)
                    throw new ParsingException(ObjectPropertySource.Source, $"Unknown property {ObjectPropertySource.Name} in object {obj.Name}");

                IsConnected = true;
            }

            return IsConnected;
        }

        public bool ConnectToObjectStringListOnly(IDomain domain, IArea currentArea, IObject currentObject, ref IObject obj, ref IObjectPropertyStringList objectProperty)
        {
            bool IsConnected = false;

            if ((ObjectSource != null || ObjectPropertySource != null) && (obj == null || objectProperty == null))
            {
                ConnectToObject(domain, currentArea, ObjectSource, ObjectPropertySource, ObjectPropertyKey, ref obj);

                foreach (IObjectProperty Property in obj.Properties)
                    if (Property.NameSource.Name == ObjectPropertySource.Name)
                    {
                        IObjectPropertyStringList AsObjectPropertyStringList;
                        if ((AsObjectPropertyStringList = Property as IObjectPropertyStringList) != null)
                        {
                            objectProperty = AsObjectPropertyStringList;
                            break;
                        }
                        else
                            throw new ParsingException(ObjectPropertySource.Source, $"{obj.Name}.{ObjectPropertySource.Name} must be a string list property");
                    }

                if (objectProperty == null)
                    throw new ParsingException(ObjectPropertySource.Source, $"Unknown property {ObjectPropertySource.Name} in object {obj.Name}");

                IsConnected = true;
            }

            return IsConnected;
        }

        public bool ConnectToObjectItemOnly(IDomain domain, IArea currentArea, IObject currentObject, ref IObject obj, ref IObjectPropertyItem objectProperty, ref IObject ItemObject)
        {
            bool IsConnected = false;

            if ((ObjectSource != null || ObjectPropertySource != null) && (obj == null || objectProperty == null))
            {
                ConnectToObject(domain, currentArea, ObjectSource, ObjectPropertySource, ObjectPropertyKey, ref obj);

                foreach (IObjectProperty Property in obj.Properties)
                    if (Property.NameSource.Name == ObjectPropertySource.Name)
                    {
                        IObjectPropertyItem AsObjectPropertyItem;
                        if ((AsObjectPropertyItem = Property as IObjectPropertyItem) != null)
                        {
                            objectProperty = AsObjectPropertyItem;
                            break;
                        }
                        else
                            throw new ParsingException(ObjectPropertySource.Source, $"{obj.Name}.{ObjectPropertySource.Name} must be an item property");
                    }

                if (objectProperty == null)
                    throw new ParsingException(ObjectPropertySource.Source, $"Unknown property {ObjectPropertySource.Name} in object {obj.Name}");

                foreach (IObject Item in domain.Objects)
                    if (Item.Name == objectProperty.ObjectSource.Name)
                    {
                        ItemObject = Item;
                        break;
                    }

                if (ItemObject == null)
                    throw new ParsingException(ObjectPropertySource.Source, $"Unknown object {objectProperty.ObjectSource.Name} for item {ObjectPropertySource.Name}");

                IsConnected = true;
            }

            return IsConnected;
        }

        public bool ConnectToObjectItemListOnly(IDomain domain, IArea currentArea, IObject currentObject, ref IObject obj, ref IObjectPropertyItemList objectProperty, ref IObject ItemObject)
        {
            bool IsConnected = false;

            if ((ObjectSource != null || ObjectPropertySource != null) && (obj == null || objectProperty == null))
            {
                ConnectToObject(domain, currentArea, ObjectSource, ObjectPropertySource, ObjectPropertyKey, ref obj);

                foreach (IObjectProperty Property in obj.Properties)
                    if (Property.NameSource.Name == ObjectPropertySource.Name)
                    {
                        IObjectPropertyItemList AsObjectPropertyItemList;
                        if ((AsObjectPropertyItemList = Property as IObjectPropertyItemList) != null)
                        {
                            objectProperty = AsObjectPropertyItemList;
                            break;
                        }
                        else
                            throw new ParsingException(ObjectPropertySource.Source, $"{obj.Name}.{ObjectPropertySource.Name} must be an item list property");
                    }

                if (objectProperty == null)
                    throw new ParsingException(ObjectPropertySource.Source, $"Unknown property {ObjectPropertySource.Name} in object {obj.Name}");

                foreach (IObject Item in domain.Objects)
                    if (Item.Name == objectProperty.ObjectSource.Name)
                    {
                        ItemObject = Item;
                        break;
                    }

                if (ItemObject == null)
                    throw new ParsingException(ObjectPropertySource.Source, $"Unknown object {objectProperty.ObjectSource.Name} for item list {ObjectPropertySource.Name}");

                IsConnected = true;
            }

            return IsConnected;
        }

        public bool ConnectToObjectIndexOnly(IDomain domain, IArea currentArea, IObject currentObject, ref IObject obj, ref IObjectPropertyIndex objectProperty)
        {
            bool IsConnected = false;

            if ((ObjectSource != null || ObjectPropertySource != null) && (obj == null || objectProperty == null))
            {
                ConnectToObject(domain, currentArea, ObjectSource, ObjectPropertySource, ObjectPropertyKey, ref obj);

                foreach (IObjectProperty Property in obj.Properties)
                    if (Property.NameSource.Name == ObjectPropertySource.Name)
                    {
                        IObjectPropertyIndex AsObjectPropertyIndex;
                        if ((AsObjectPropertyIndex = Property as IObjectPropertyIndex) != null)
                        {
                            objectProperty = AsObjectPropertyIndex;
                            break;
                        }
                        else
                            throw new ParsingException(ObjectPropertySource.Source, $"{obj.Name}.{ObjectPropertySource.Name} must be an integer, state or boolean property");
                    }

                if (objectProperty == null)
                    throw new ParsingException(ObjectPropertySource.Source, $"Unknown property {ObjectPropertySource.Name} in object {obj.Name}");

                IsConnected = true;
            }

            return IsConnected;
        }
    }
}
