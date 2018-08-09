﻿namespace Parser
{
    public class ComponentContainer : Component, IComponentContainer
    {
        public ComponentContainer(IDeclarationSource source, string xamlName, IComponentProperty itemProperty, IDeclarationSource areaSource)
            : base(source, xamlName)
        {
            ItemProperty = itemProperty;
            AreaSource = areaSource;
        }

        public IComponentProperty ItemProperty { get; private set; }
        public IObject ItemObject { get; private set; }
        public IObjectPropertyItem ItemObjectProperty { get; private set; }
        public IObject ItemNestedObject { get; private set; }
        public IDeclarationSource AreaSource { get; private set; }
        public IArea ItemNestedArea { get; private set; }

        public override bool Connect(IDomain domain, IArea rootArea, IObject currentObject)
        {
            bool IsConnected = false;

            ConnectItem(domain, currentObject, ref IsConnected);
            ConnectArea(domain, ref IsConnected);

            return IsConnected;
        }

        private void ConnectItem(IDomain domain, IObject currentObject, ref bool IsConnected)
        {
            IObject Object = ItemObject;
            IObjectPropertyItem ObjectProperty = ItemObjectProperty;
            IObject NestedObject = ItemNestedObject;
            IsConnected = ItemProperty.ConnectToObjectItemOnly(domain, currentObject, ref Object, ref ObjectProperty, ref NestedObject);
            ItemObject = Object;
            ItemObjectProperty = ObjectProperty;
            ItemNestedObject = NestedObject;
        }

        private void ConnectArea(IDomain domain, ref bool IsConnected)
        {
            if (ItemNestedArea == null)
            {
                foreach (IArea Item in domain.Areas)
                    if (Item.Name == AreaSource.Name)
                    {
                        ItemNestedArea = Item;
                        break;
                    }

                if (ItemNestedArea == null)
                    throw new ParsingException(Source.Source, $"Unknown area {AreaSource}");

                ItemNestedArea.SetCurrentObject(Source, ItemNestedObject);

                IsConnected = true;
            }
        }
    }
}
