namespace Parser
{
    public interface IDynamicOperation
    {
        bool Connect(IDomain domain, IDynamic currentDynamic, IObject currentObject);
    }
}
