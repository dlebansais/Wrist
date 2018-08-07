namespace Parser
{
    public interface IComponentArea : IComponent
    {
        IDeclarationSource AreaSource { get; }
        IArea Area { get; }
    }
}
