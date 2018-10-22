namespace Parser
{
    public interface IGeneratorComponentHtml : IGeneratorComponent
    {
        IGeneratorResource HtmlResource { get; }
        IGeneratorObject HtmlObject { get; }
        IGeneratorObjectProperty HtmlObjectProperty { get; }
        IDeclarationSource HtmlKey { get; }
    }
}
