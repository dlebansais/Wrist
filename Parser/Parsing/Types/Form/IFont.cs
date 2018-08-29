namespace Parser
{
    public interface IFont : IForm, IConnectable
    {
        string Name { get; }
        string XamlName { get; }
        string FilePath { get; }
    }
}
