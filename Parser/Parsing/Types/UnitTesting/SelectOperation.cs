namespace Parser
{
    public class SelectOperation : TestingOperation, ISelectOperation
    {
        public SelectOperation(IDeclarationSource pageName, IDeclarationSource areaName, IDeclarationSource componentName, int index)
            : base(pageName, areaName, componentName)
        {
            Index = index;
        }

        public int Index { get; private set; }

        public override bool Connect(IDomain domain)
        {
            bool IsConnected = base.Connect(domain);

            if (IsConnected)
            {
                if (!(Component is IComponentSelector))
                    throw new ParsingException(0, ComponentName.Source, $"Component '{ComponentName}' must be a selector.");
            }

            return IsConnected;
        }
    }
}
