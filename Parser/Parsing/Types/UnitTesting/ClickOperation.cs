namespace Parser
{
    public class ClickOperation : TestingOperation, IClickOperation
    {
        public ClickOperation(IDeclarationSource pageName, IDeclarationSource areaName, IDeclarationSource componentName)
            : base(pageName, areaName, componentName)
        {
        }

        public override bool Connect(IDomain domain)
        {
            bool IsConnected = base.Connect(domain);

            if (IsConnected)
            {
                if (!(Component is IComponentButton))
                    throw new ParsingException(0, ComponentName.Source, $"Component '{ComponentName}' must be a button.");
            }

            return IsConnected;
        }
    }
}
