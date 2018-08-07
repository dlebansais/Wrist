namespace Parser
{
    public interface IResource : IForm
    {
        string Name { get; }
        string XamlName { get; }
        string FilePath { get; }
    }
}
