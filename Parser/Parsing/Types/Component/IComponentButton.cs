namespace Parser
{
    public interface IComponentButton : IComponent
    {
        IResource ContentResource { get; }
        IObject ContentObject { get; }
        IObjectProperty ContentObjectProperty { get; }
        IDeclarationSource ContentKey { get; }
        IPageNavigation GoTo { get; }
    }
}
