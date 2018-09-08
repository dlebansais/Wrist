namespace Parser
{
    public class ObjectPropertyInteger : ObjectPropertyIndex, IObjectPropertyInteger
    {
        public static ObjectPropertyInteger NavigationIndexProperty = new ObjectPropertyInteger("navigation index", "NavigationIndex");

        private ObjectPropertyInteger(string name, string cSharpName)
            : base(new DeclarationSource(name, null), cSharpName)
        {
        }

        public ObjectPropertyInteger(IDeclarationSource nameSource, string cSharpName)
            : base(nameSource, cSharpName)
        {
        }

        public override bool Connect(IDomain domain)
        {
            return false;
        }
    }
}
