namespace Parser
{
    public interface IComponentHtml : IComponent
    {
        IResource HtmlResource { get; }
        IObject HtmlObject { get; }
        IObjectProperty HtmlObjectProperty { get; }
        IDeclarationSource HtmlKey { get; }
    }
}
