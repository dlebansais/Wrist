namespace Parser
{
    public class ObjectPropertyStringDictionary : ObjectProperty, IObjectPropertyStringDictionary
    {
        public static ObjectPropertyStringDictionary StringsProperty = new ObjectPropertyStringDictionary("strings");

        private ObjectPropertyStringDictionary(string name)
            : base(new DeclarationSource(name, null), null, false)
        {
        }

        public ObjectPropertyStringDictionary(IDeclarationSource nameSource, string cSharpName)
            : base(nameSource, cSharpName, false)
        {
        }

        public override bool Connect(IDomain domain)
        {
            return false;
        }
    }
}
