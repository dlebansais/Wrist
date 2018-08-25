namespace Parser
{
    public class DynamicProperty : IDynamicProperty
    {
        public DynamicProperty(IDeclarationSource source, string cSharpName, DynamicOperationResults result, IDynamicOperation rootOperation)
        {
            Source = source;
            CSharpName = cSharpName;
            Result = result;
            RootOperation = rootOperation;
        }

        public IDeclarationSource Source { get; private set; }
        public string CSharpName { get; private set; }
        public DynamicOperationResults Result { get; private set; }
        public IDynamicOperation RootOperation { get; private set; }
        public bool IsUsed { get; private set; }

        public bool Connect(IDomain domain, IDynamic currentDynamic, IObject currentObject)
        {
            return RootOperation.Connect(domain, currentDynamic, currentObject);
        }

        public void SetIsUsed()
        {
            IsUsed = true;
        }
    }
}
