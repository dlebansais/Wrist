namespace Parser
{
    public abstract class ObjectPropertyIndex : ObjectProperty, IObjectPropertyIndex
    {
        public ObjectPropertyIndex(IDeclarationSource nameSource, string cSharpName)
            : base(nameSource, cSharpName)
        {
        }
    }
}
