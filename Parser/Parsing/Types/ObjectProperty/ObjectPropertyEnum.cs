namespace Parser
{
    public class ObjectPropertyEnum : ObjectPropertyIndex, IObjectPropertyEnum
    {
        public ObjectPropertyEnum(IDeclarationSource nameSource, string cSharpName)
            : base(nameSource, cSharpName)
        {
        }

        public override bool Connect(IDomain domain)
        {
            return false;
        }
    }
}
