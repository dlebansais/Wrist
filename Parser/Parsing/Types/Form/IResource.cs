namespace Parser
{
    public interface IResource : IForm, IConnectable
    {
        string Name { get; }
        string XamlName { get; }
        string FilePath { get; }
        double Width { get; }
        double Height { get; }
    }
}
