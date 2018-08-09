namespace Parser
{
    public abstract class Component : IComponent
    {
        public Component(IDeclarationSource source, string xamlName)
        {
            Source = source;
            XamlName = xamlName;
        }

        public IDeclarationSource Source { get; private set; }
        public string XamlName { get; private set; }

        public abstract bool Connect(IDomain domain, IArea rootArea, IObject currentObject);

        public virtual bool IsReferencing(IArea other)
        {
            return false;
        }

        public override string ToString()
        {
            return $"{GetType().Name} '{Source.Name}'";
        }
    }
}
