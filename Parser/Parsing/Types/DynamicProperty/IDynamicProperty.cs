namespace Parser
{
    public interface IDynamicProperty
    {
        IDeclarationSource Source { get; }
        string CSharpName { get; }
        DynamicOperationResults Result { get; }
        IDynamicOperation RootOperation { get; }
        bool Connect(IDomain domain, IDynamic currentDynamic, IObject currentObject);
        bool IsUsed { get; }
        void SetIsUsed();
    }
}
