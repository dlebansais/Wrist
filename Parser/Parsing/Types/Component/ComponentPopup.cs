namespace Parser
{
    public class ComponentPopup : Component, IComponentPopup
    {
        public ComponentPopup(IDeclarationSource source, string xamlName, IComponentProperty sourceProperty, IDeclarationSource areaSource)
            : base(source, xamlName)
        {
            SourceProperty = sourceProperty;
            AreaSource = areaSource;
        }

        public IComponentProperty SourceProperty { get; private set; }
        public IResource SourceResource { get; private set; }
        public IDeclarationSource AreaSource { get; private set; }
        public IArea Area { get; private set; }

        public override bool Connect(IDomain domain, IArea rootArea, IArea currentArea, IObject currentObject)
        {
            bool IsConnected = false;

            ConnectSource(domain, ref IsConnected);
            ConnectArea(domain, ref IsConnected);

            return IsConnected;
        }

        private void ConnectSource(IDomain domain, ref bool IsConnected)
        {
            IResource Resource = SourceResource;
            IsConnected |= SourceProperty.ConnectToResource(domain, ref Resource);
            SourceResource = Resource;
        }

        private void ConnectArea(IDomain domain, ref bool IsConnected)
        {
            if (Area == null)
            {
                foreach (IArea Item in domain.Areas)
                    if (Item.Name == AreaSource.Name)
                    {
                        Area = Item;
                        break;
                    }

                if (Area == null)
                    throw new ParsingException(117, Source.Source, $"Unknown area '{AreaSource.Name}'.");

                IsConnected = true;
            }
        }

        public override bool IsReferencing(IArea other)
        {
            if (Area == other)
                return true;

            else if (other.IsReferencedBy(Area))
                return true;

            else
                return false;
        }
    }
}
