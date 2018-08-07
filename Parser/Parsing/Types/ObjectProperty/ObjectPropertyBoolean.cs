namespace Parser
{
    public class ObjectPropertyBoolean : ObjectPropertyIndex, IObjectPropertyBoolean
    {
        public ObjectPropertyBoolean(IDeclarationSource nameSource, string cSharpName)
            : base(nameSource, cSharpName)
        {
        }

        public override bool Connect(IDomain domain)
        {
            return false;
        }
    }
}
