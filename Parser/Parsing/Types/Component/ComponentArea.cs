namespace Parser
{
    public class ComponentArea : Component, IComponentArea
    {
        public ComponentArea(IDeclarationSource source, string xamlName, IDeclarationSource areaSource)
            : base(source, xamlName)
        {
            AreaSource = areaSource;
        }

        public IDeclarationSource AreaSource { get; private set; }
        public IArea Area { get; private set; }

        public override bool Connect(IDomain domain, IArea rootArea, IArea currentArea, IObject currentObject)
        {
            bool IsConnected = false;

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

            return IsConnected;
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
