namespace Parser
{
    public class ObjectPropertyStringList : ObjectProperty, IObjectPropertyStringList
    {
        public ObjectPropertyStringList(IDeclarationSource nameSource, string cSharpName)
            : base(nameSource, cSharpName, false)
        {
        }

        public override bool Connect(IDomain domain)
        {
            return false;
        }
    }
}
