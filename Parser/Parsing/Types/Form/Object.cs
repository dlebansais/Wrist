﻿using System.Collections.Generic;

namespace Parser
{
    public class Object : IObject
    {
        public static Object TranslationObject = new Object("translation");

        private Object(string name)
        {
            Name = name;

            List<IObjectProperty> LocalProperties = new List<IObjectProperty>() { ObjectPropertyStringDictionary.StringsProperty };
            Properties = LocalProperties.AsReadOnly();
        }

        public Object(string name, string cSharpName, bool isGlobal, IObjectPropertyCollection properties, List<IObjectEvent> events)
        {
            Name = name;
            CSharpName = cSharpName;
            IsGlobal = isGlobal;
            Properties = properties.AsReadOnly();
            Events = events.AsReadOnly();
        }

        public string Name { get; private set; }
        public string CSharpName { get; private set; }
        public bool IsGlobal { get; private set; }
        public IReadOnlyCollection<IObjectProperty> Properties { get; private set; }
        public IReadOnlyCollection<IObjectEvent> Events { get; private set; }
        public bool IsUsed { get; private set; }

        public bool Connect(IDomain domain)
        {
            bool IsConnected = false;

            foreach (IObjectProperty Property in Properties)
                IsConnected |= Property.Connect(domain);

            foreach (IObjectEvent Event in Events)
                IsConnected |= Event.Connect(domain);

            return IsConnected;
        }

        public void SetIsUsed()
        {
            IsUsed = true;
        }

        public override string ToString()
        {
            return $"{GetType().Name} '{Name}'";
        }
    }
}
