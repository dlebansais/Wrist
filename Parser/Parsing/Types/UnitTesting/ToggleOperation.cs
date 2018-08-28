namespace Parser
{
    public class ToggleOperation : TestingOperation, IToggleOperation
    {
        public ToggleOperation(IDeclarationSource pageName, IDeclarationSource areaName, IDeclarationSource componentName)
            : base(pageName, areaName, componentName)
        {
        }

        public override bool Connect(IDomain domain)
        {
            bool IsConnected = base.Connect(domain);

            if (IsConnected)
            {
                if (!(Component is IComponentPopup))
                    throw new ParsingException(230, ComponentName.Source, $"Component '{ComponentName}' must be a popup.");
            }

            return IsConnected;
        }
    }
}
