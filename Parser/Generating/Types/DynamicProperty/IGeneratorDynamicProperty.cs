namespace Parser
{
    public interface IGeneratorDynamicProperty
    {
        string PropertyName { get; }
        IGeneratorDynamicOperation RootOperation { get; }
    }
}
