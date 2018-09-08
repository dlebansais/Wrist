namespace Parser
{
    public class ObjectPropertyStringDictionary : ObjectProperty, IObjectPropertyStringDictionary
    {
        public static ObjectPropertyStringDictionary StringsProperty = new ObjectPropertyStringDictionary("strings", "Strings");

        private ObjectPropertyStringDictionary(string name, string cSharpName)
            : base(new DeclarationSource(name, null), cSharpName, true, false)
        {
        }

        public ObjectPropertyStringDictionary(IDeclarationSource nameSource, string cSharpName)
            : base(nameSource, cSharpName, false, false)
        {
        }

        public override bool Connect(IDomain domain)
        {
            return false;
        }
    }
}
