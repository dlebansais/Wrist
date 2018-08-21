namespace Parser
{
    public class ObjectPropertyBoolean : ObjectPropertyIndex, IObjectPropertyBoolean
    {
        public ObjectPropertyBoolean(IDeclarationSource nameSource, string cSharpName)
            : base(nameSource, cSharpName)
        {
        }

        public bool IsClosingPopup { get; private set; }

        public void SetIsClosingPopup()
        {
            IsClosingPopup = true;
        }

        public override bool Connect(IDomain domain)
        {
            return false;
        }
    }
}
