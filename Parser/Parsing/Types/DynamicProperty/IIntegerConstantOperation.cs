namespace Parser
{
    public interface IIntegerConstantOperation : IDynamicOperation
    {
        int Value { get; }
    }
}
