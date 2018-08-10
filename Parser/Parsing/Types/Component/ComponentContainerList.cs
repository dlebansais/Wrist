namespace Parser
{
    public class ComponentContainerList : Component, IComponentContainerList
    {
        public ComponentContainerList(IDeclarationSource source, string xamlName, IComponentProperty itemProperty, IDeclarationSource areaSource)
            : base(source, xamlName)
        {
            ItemProperty = itemProperty;
            AreaSource = areaSource;
        }

        public IComponentProperty ItemProperty { get; private set; }
        public IObject ItemObject { get; private set; }
        public IObjectPropertyItemList ItemObjectProperty { get; private set; }
        public IObject ItemNestedObject { get; private set; }
        public IDeclarationSource AreaSource { get; private set; }
        public IArea ItemNestedArea { get; private set; }

        public override bool Connect(IDomain domain, IArea rootArea, IArea currentArea, IObject currentObject)
        {
            bool IsConnected = false;

            ConnectItem(domain, currentArea, currentObject, ref IsConnected);
            ConnectArea(domain, ref IsConnected);

            return IsConnected;
        }

        private void ConnectItem(IDomain domain, IArea currentArea, IObject currentObject, ref bool IsConnected)
        {
            IObject Object = ItemObject;
            IObjectPropertyItemList ObjectProperty = ItemObjectProperty;
            IObject NestedObject = ItemNestedObject;
            IsConnected = ItemProperty.ConnectToObjectItemListOnly(domain, currentArea, currentObject, ref Object, ref ObjectProperty, ref NestedObject);
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
                    throw new ParsingException(AreaSource.Source, $"Unknown area {AreaSource}");

                ItemNestedArea.SetCurrentObject(AreaSource, ItemNestedObject);

                IsConnected = true;
            }
        }
    }
}
