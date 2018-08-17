namespace Parser
{
    public interface IDynamicProperty
    {
        string PropertyName { get; }
        IDynamicOperation RootOperation { get; }
    }
}
