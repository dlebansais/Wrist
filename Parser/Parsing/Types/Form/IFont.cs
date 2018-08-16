namespace Parser
{
    public interface IFont : IForm
    {
        string Name { get; }
        string XamlName { get; }
        string FilePath { get; }
    }
}
