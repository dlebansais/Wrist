namespace Parser
{
    public class ObjectPropertyItem : ObjectProperty, IObjectPropertyItem
    {
        public ObjectPropertyItem(IDeclarationSource nameSource, string cSharpName, IDeclarationSource objectSource)
            : base(nameSource, cSharpName, false)
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
                    throw new ParsingException(NameSource.Source, $"Object {ObjectSource.Name} not found");
            }

            return IsConnected;
        }
    }
}
