namespace Parser
{
    public interface IObjectPropertyString : IObjectProperty
    {
        int MaximumLength { get; }
        ObjectPropertyStringCategory Category { get; }
    }
}
