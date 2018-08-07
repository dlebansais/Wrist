namespace Parser
{
    public class ObjectPropertyState : ObjectPropertyIndex, IObjectPropertyState
    {
        public ObjectPropertyState(IDeclarationSource nameSource, string cSharpName)
            : base(nameSource, cSharpName)
        {
        }

        public override bool Connect(IDomain domain)
        {
            return false;
        }
    }
}
