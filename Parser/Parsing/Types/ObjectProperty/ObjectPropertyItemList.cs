﻿namespace Parser
{
    public class ObjectPropertyItemList : ObjectProperty, IObjectPropertyItemList
    {
        public static ObjectPropertyItemList NavigationHistoryProperty = new ObjectPropertyItemList("navigation history", "NavigationHistory");

        private ObjectPropertyItemList(string name, string cSharpName)
            : base(new DeclarationSource(name, null), cSharpName, true, false)
        {
        }

        public ObjectPropertyItemList(IDeclarationSource nameSource, string cSharpName, IDeclarationSource objectSource)
            : base(nameSource, cSharpName, true, false)
        {
            ObjectSource = objectSource;
        }

        public IDeclarationSource ObjectSource { get; private set; }
        public IObject NestedObject { get; private set; }

        public override bool Connect(IDomain domain)
        {
            bool IsConnected = false;

            if (NestedObject == null)
            {
                foreach (IObject Item in domain.Objects)
                    if (Item.Name == ObjectSource.Name)
                    {
                        NestedObject = Item;
                        break;
                    }

                if (NestedObject == null)
                    throw new ParsingException(175, ObjectSource.Source, $"Object '{ObjectSource.Name}' not found.");
            }

            return IsConnected;
        }
    }
}
