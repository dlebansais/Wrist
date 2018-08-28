namespace Parser
{
    public class FillOperation : TestingOperation, IFillOperation
    {
        public FillOperation(IDeclarationSource pageName, IDeclarationSource areaName, IDeclarationSource componentName, string content)
            : base(pageName, areaName, componentName)
        {
            Content = content;
        }

        public string Content { get; private set; }

        public override bool Connect(IDomain domain)
        {
            bool IsConnected = base.Connect(domain);

            if (IsConnected)
            {
                if (!(Component is IComponentEdit) && !(Component is IComponentPasswordEdit))
                    throw new ParsingException(225, ComponentName.Source, $"Component '{ComponentName}' must be an edit or password edit.");
            }

            return IsConnected;
        }
    }
}
