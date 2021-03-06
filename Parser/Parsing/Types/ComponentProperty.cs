﻿using System.Collections.Generic;

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

        public static Dictionary<IArea, IDeclarationSource> AreaWithCurrentPage { get; } = new Dictionary<IArea, IDeclarationSource>();

        private static void ConnectToObject(IDomain domain, IArea currentArea, IDeclarationSource objectSource, IDeclarationSource objectPropertySource, IDeclarationSource objectPropertyKey, ref IObject obj)
        {
            if (objectSource.Name == Object.TranslationObject.Name)
            {
                if (domain.Translation == null)
                    throw new ParsingException(11, objectSource.Source, $"Object '{objectSource.Name}' is used but no translation file is specified.");

                obj = Object.TranslationObject;
                if (objectPropertySource.Name != ObjectPropertyStringDictionary.StringsProperty.NameSource.Name)
                    throw new ParsingException(137, objectPropertySource.Source, $"The only valid property for object '{objectSource.Name}' is '{ObjectPropertyStringDictionary.StringsProperty.NameSource.Name}'.");

                if (objectPropertyKey == null)
                    throw new ParsingException(138, objectSource.Source, $"For object '{objectSource.Name}' property '{objectPropertySource.Name}' a key is required.");

                if (objectPropertyKey.Name != Page.CurrentPage.Name)
                {
                    if (!domain.Translation.KeyList.Contains(objectPropertyKey.Name))
                        throw new ParsingException(139, objectPropertyKey.Source, $"The translation file doesn't contain key '{objectPropertyKey.Name}'.");

                    if (!domain.Translation.UsedKeyList.Contains(objectPropertyKey.Name))
                        domain.Translation.UsedKeyList.Add(objectPropertyKey.Name);
                }
                else
                {
                    // Verify it later
                    if (!AreaWithCurrentPage.ContainsKey(currentArea))
                        AreaWithCurrentPage.Add(currentArea, objectPropertyKey);
                }
            }

            else if (objectSource.Name == Object.ApplicationObject.Name)
            {
                obj = Object.ApplicationObject;
                if (objectPropertySource.Name != ObjectPropertyItemList.NavigationHistoryProperty.NameSource.Name && objectPropertySource.Name != ObjectPropertyInteger.NavigationIndexProperty.NameSource.Name)
                    throw new ParsingException(137, objectPropertySource.Source, $"The only valid property for object '{objectSource.Name}' is '{ObjectPropertyItemList.NavigationHistoryProperty.NameSource.Name}' or '{ObjectPropertyInteger.NavigationIndexProperty.NameSource.Name}'.");

                if (objectPropertyKey != null)
                    throw new ParsingException(0, objectSource.Source, $"For object '{objectSource.Name}' property '{objectPropertySource.Name}' there must be no key.");
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
                    throw new ParsingException(140, objectSource.Source, $"Unknown object '{objectSource.Name}'.");

                obj.SetIsUsed();
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
                    throw new ParsingException(142, FixedValueSource.Source, $"Unknown static resource '{FixedValueSource.Name}'.");

                IsConnected = true;
            }

            else if ((ObjectSource != null || ObjectPropertySource != null) && (obj == null || objectProperty == null))
            {
                ConnectToObject(domain, currentArea, ObjectSource, ObjectPropertySource, ObjectPropertyKey, ref obj);

                foreach (IObjectProperty Property in obj.Properties)
                    if (Property.NameSource.Name == ObjectPropertySource.Name)
                    {
                        if (Property is IObjectPropertyStringDictionary AsObjectPropertyStringDictionary)
                        {
                            if (ObjectPropertyKey == null)
                                throw new ParsingException(143, ObjectPropertySource.Source, $"'{obj.Name}.{ObjectPropertySource.Name}' must be used with a key.");
                        }
                        else
                        {
                            if (ObjectPropertyKey != null)
                                throw new ParsingException(144, ObjectPropertySource.Source, $"'{obj.Name}.{ObjectPropertySource.Name}' cannot be used with a key.");
                        }

                        objectProperty = Property;
                        objectPropertyKey = ObjectPropertyKey;
                        break;
                    }

                if (objectProperty == null)
                    throw new ParsingException(141, ObjectPropertySource.Source, $"Unknown property '{ObjectPropertySource.Name}' in object '{obj.Name}'.");

                IsConnected = true;
            }

            return IsConnected;
        }

        public bool ConnectToResourceOnly(IDomain domain, ref IResource resource)
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
                    throw new ParsingException(142, FixedValueSource.Source, $"Unknown static resource '{FixedValueSource.Name}'.");

                IsConnected = true;
            }

            return IsConnected;
        }

        public bool ConnectToObjectOnly(IDomain domain, IArea currentArea, IObject currentObject, ref IObject obj, ref IObjectProperty objectProperty, ref IDeclarationSource objectPropertyKey)
        {
            bool IsConnected = false;

            if ((ObjectSource != null || ObjectPropertySource != null) && (obj == null || objectProperty == null))
            {
                ConnectToObject(domain, currentArea, ObjectSource, ObjectPropertySource, ObjectPropertyKey, ref obj);

                foreach (IObjectProperty Property in obj.Properties)
                    if (Property.NameSource.Name == ObjectPropertySource.Name)
                    {
                        if (Property is IObjectPropertyStringDictionary AsObjectPropertyStringDictionary)
                        {
                            if (ObjectPropertyKey == null)
                                throw new ParsingException(143, ObjectPropertySource.Source, $"'{obj.Name}.{ObjectPropertySource.Name}' must be used with a key.");
                        }
                        else
                        {
                            if (ObjectPropertyKey != null)
                                throw new ParsingException(144, ObjectPropertySource.Source, $"'{obj.Name}.{ObjectPropertySource.Name}' cannot be used with a key.");
                        }

                        objectProperty = Property;
                        objectPropertyKey = ObjectPropertyKey;
                        break;
                    }

                if (objectProperty == null)
                    throw new ParsingException(141, ObjectPropertySource.Source, $"Unknown property '{ObjectPropertySource.Name}' in object '{obj.Name}'.");

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
                    throw new ParsingException(142, FixedValueSource.Source, $"Unknown static resource '{FixedValueSource.Name}'.");

                IsConnected = true;
            }

            else if ((ObjectSource != null || ObjectPropertySource != null) && (obj == null || objectProperty == null))
            {
                ConnectToObject(domain, currentArea, ObjectSource, ObjectPropertySource, ObjectPropertyKey, ref obj);

                foreach (IObjectProperty Property in obj.Properties)
                    if (Property.NameSource.Name == ObjectPropertySource.Name)
                        if (Property is IObjectPropertyStringList AsObjectPropertyStringList)
                        {
                            objectProperty = AsObjectPropertyStringList;
                            break;
                        }
                        else
                            throw new ParsingException(145, ObjectPropertySource.Source, $"'{obj.Name}.{ObjectPropertySource.Name}' must be a string list property.");

                if (objectProperty == null)
                    throw new ParsingException(141, ObjectPropertySource.Source, $"Unknown property '{ObjectPropertySource.Name}' in object '{obj.Name}'.");

                IsConnected = true;
            }

            return IsConnected;
        }

        public bool ConnectToObjectInteger(IDomain domain, IArea currentArea, IObject currentObject, ref IObject obj, ref IObjectPropertyInteger objectProperty)
        {
            bool IsConnected = false;

            if ((ObjectSource != null || ObjectPropertySource != null) && (obj == null || objectProperty == null))
            {
                ConnectToObject(domain, currentArea, ObjectSource, ObjectPropertySource, ObjectPropertyKey, ref obj);

                foreach (IObjectProperty Property in obj.Properties)
                    if (Property.NameSource.Name == ObjectPropertySource.Name)
                        if (Property is IObjectPropertyInteger AsObjectPropertyInteger)
                        {
                            objectProperty = AsObjectPropertyInteger;
                            break;
                        }
                        else
                            throw new ParsingException(146, ObjectPropertySource.Source, $"'{obj.Name}.{ObjectPropertySource.Name}' must be an integer property.");

                if (objectProperty == null)
                    throw new ParsingException(141, ObjectPropertySource.Source, $"Unknown property '{ObjectPropertySource.Name}' in object '{obj.Name}'.");

                IsConnected = true;
            }

            return IsConnected;
        }

        public bool ConnectToObjectBoolean(IDomain domain, IArea currentArea, IObject currentObject, ref IObject obj, ref IObjectPropertyBoolean objectProperty)
        {
            bool IsConnected = false;

            if ((ObjectSource != null || ObjectPropertySource != null) && (obj == null || objectProperty == null))
            {
                ConnectToObject(domain, currentArea, ObjectSource, ObjectPropertySource, ObjectPropertyKey, ref obj);

                foreach (IObjectProperty Property in obj.Properties)
                    if (Property.NameSource.Name == ObjectPropertySource.Name)
                        if (Property is IObjectPropertyBoolean AsObjectPropertyBoolean)
                        {
                            objectProperty = AsObjectPropertyBoolean;
                            break;
                        }
                        else
                            throw new ParsingException(147, ObjectPropertySource.Source, $"'{obj.Name}.{ObjectPropertySource.Name}' must be a boolean property.");

                if (objectProperty == null)
                    throw new ParsingException(141, ObjectPropertySource.Source, $"Unknown property '{ObjectPropertySource.Name}' in object '{obj.Name}'.");

                IsConnected = true;
            }

            return IsConnected;
        }

        public bool ConnectToObjectString(IDomain domain, IArea currentArea, IObject currentObject, ref IObject obj, ref IObjectPropertyString objectProperty)
        {
            bool IsConnected = false;

            if ((ObjectSource != null || ObjectPropertySource != null) && (obj == null || objectProperty == null))
            {
                ConnectToObject(domain, currentArea, ObjectSource, ObjectPropertySource, ObjectPropertyKey, ref obj);

                foreach (IObjectProperty Property in obj.Properties)
                    if (Property.NameSource.Name == ObjectPropertySource.Name)
                        if (Property is IObjectPropertyString AsObjectPropertyString)
                        {
                            objectProperty = AsObjectPropertyString;
                            break;
                        }
                        else
                            throw new ParsingException(148, ObjectPropertySource.Source, $"'{obj.Name}.{ObjectPropertySource.Name}' must be a string property.");

                if (objectProperty == null)
                    throw new ParsingException(141, ObjectPropertySource.Source, $"Unknown property '{ObjectPropertySource.Name}' in object '{obj.Name}'.");

                IsConnected = true;
            }

            return IsConnected;
        }

        public bool ConnectToObjectReadonlyString(IDomain domain, IArea currentArea, IObject currentObject, ref IObject obj, ref IObjectPropertyReadonlyString objectProperty)
        {
            bool IsConnected = false;

            if ((ObjectSource != null || ObjectPropertySource != null) && (obj == null || objectProperty == null))
            {
                ConnectToObject(domain, currentArea, ObjectSource, ObjectPropertySource, ObjectPropertyKey, ref obj);

                foreach (IObjectProperty Property in obj.Properties)
                    if (Property.NameSource.Name == ObjectPropertySource.Name)
                        if (Property is IObjectPropertyReadonlyString AsObjectPropertyString)
                        {
                            objectProperty = AsObjectPropertyString;
                            break;
                        }
                        else
                            throw new ParsingException(0, ObjectPropertySource.Source, $"'{obj.Name}.{ObjectPropertySource.Name}' must be a readonly string property.");

                if (objectProperty == null)
                    throw new ParsingException(141, ObjectPropertySource.Source, $"Unknown property '{ObjectPropertySource.Name}' in object '{obj.Name}'.");

                IsConnected = true;
            }

            return IsConnected;
        }

        public bool ConnectToObjectStringList(IDomain domain, IArea currentArea, IObject currentObject, ref IObject obj, ref IObjectPropertyStringList objectProperty)
        {
            bool IsConnected = false;

            if ((ObjectSource != null || ObjectPropertySource != null) && (obj == null || objectProperty == null))
            {
                ConnectToObject(domain, currentArea, ObjectSource, ObjectPropertySource, ObjectPropertyKey, ref obj);

                foreach (IObjectProperty Property in obj.Properties)
                    if (Property.NameSource.Name == ObjectPropertySource.Name)
                        if (Property is IObjectPropertyStringList AsObjectPropertyStringList)
                        {
                            objectProperty = AsObjectPropertyStringList;
                            break;
                        }
                        else
                            throw new ParsingException(145, ObjectPropertySource.Source, $"'{obj.Name}.{ObjectPropertySource.Name}' must be a string list property.");

                if (objectProperty == null)
                    throw new ParsingException(141, ObjectPropertySource.Source, $"Unknown property '{ObjectPropertySource.Name}' in object '{obj.Name}'.");

                IsConnected = true;
            }

            return IsConnected;
        }

        public bool ConnectToObjectItem(IDomain domain, IArea currentArea, IObject currentObject, ref IObject obj, ref IObjectPropertyItem objectProperty, ref IObject ItemObject)
        {
            bool IsConnected = false;

            if ((ObjectSource != null || ObjectPropertySource != null) && (obj == null || objectProperty == null))
            {
                ConnectToObject(domain, currentArea, ObjectSource, ObjectPropertySource, ObjectPropertyKey, ref obj);

                foreach (IObjectProperty Property in obj.Properties)
                    if (Property.NameSource.Name == ObjectPropertySource.Name)
                        if (Property is IObjectPropertyItem AsObjectPropertyItem)
                        {
                            objectProperty = AsObjectPropertyItem;
                            break;
                        }
                        else
                            throw new ParsingException(149, ObjectPropertySource.Source, $"'{obj.Name}.{ObjectPropertySource.Name}' must be an item property.");

                if (objectProperty == null)
                    throw new ParsingException(141, ObjectPropertySource.Source, $"Unknown property '{ObjectPropertySource.Name}' in object '{obj.Name}'.");

                foreach (IObject Item in domain.Objects)
                    if (Item.Name == objectProperty.ObjectSource.Name)
                    {
                        ItemObject = Item;
                        break;
                    }

                if (ItemObject == null)
                    throw new ParsingException(150, ObjectPropertySource.Source, $"Unknown object '{objectProperty.ObjectSource.Name}' for item '{ObjectPropertySource.Name}'.");

                IsConnected = true;
            }

            return IsConnected;
        }

        public bool ConnectToObjectItemList(IDomain domain, IArea currentArea, IObject currentObject, ref IObject obj, ref IObjectPropertyItemList objectProperty, ref IObject ItemObject)
        {
            bool IsConnected = false;

            if ((ObjectSource != null || ObjectPropertySource != null) && (obj == null || objectProperty == null))
            {
                ConnectToObject(domain, currentArea, ObjectSource, ObjectPropertySource, ObjectPropertyKey, ref obj);

                foreach (IObjectProperty Property in obj.Properties)
                    if (Property.NameSource.Name == ObjectPropertySource.Name)
                        if (Property is IObjectPropertyItemList AsObjectPropertyItemList)
                        {
                            objectProperty = AsObjectPropertyItemList;
                            break;
                        }
                        else
                            throw new ParsingException(151, ObjectPropertySource.Source, $"'{obj.Name}.{ObjectPropertySource.Name}' must be an item list property.");

                if (objectProperty == null)
                    throw new ParsingException(141, ObjectPropertySource.Source, $"Unknown property '{ObjectPropertySource.Name}' in object '{obj.Name}'.");

                foreach (IObject Item in domain.Objects)
                    if (Item.Name == objectProperty.ObjectSource.Name)
                    {
                        ItemObject = Item;
                        break;
                    }

                if (ItemObject == null)
                    throw new ParsingException(152, ObjectPropertySource.Source, $"Unknown object '{objectProperty.ObjectSource.Name}' for item list '{ObjectPropertySource.Name}'.");

                IsConnected = true;
            }

            return IsConnected;
        }

        public bool ConnectToObjectIndex(IDomain domain, IArea currentArea, IObject currentObject, ref IObject obj, ref IObjectPropertyIndex objectProperty)
        {
            bool IsConnected = false;

            if ((ObjectSource != null || ObjectPropertySource != null) && (obj == null || objectProperty == null))
            {
                ConnectToObject(domain, currentArea, ObjectSource, ObjectPropertySource, ObjectPropertyKey, ref obj);

                foreach (IObjectProperty Property in obj.Properties)
                    if (Property.NameSource.Name == ObjectPropertySource.Name)
                        if (Property is IObjectPropertyIndex AsObjectPropertyIndex)
                        {
                            objectProperty = AsObjectPropertyIndex;
                            break;
                        }
                        else
                            throw new ParsingException(153, ObjectPropertySource.Source, $"'{obj.Name}.{ObjectPropertySource.Name}' must be an integer, enum or boolean property.");

                if (objectProperty == null)
                    throw new ParsingException(141, ObjectPropertySource.Source, $"Unknown property '{ObjectPropertySource.Name}' in object '{obj.Name}'.");

                IsConnected = true;
            }

            return IsConnected;
        }
    }
}
