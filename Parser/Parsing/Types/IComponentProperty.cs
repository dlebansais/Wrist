namespace Parser
{
    public interface IComponentProperty
    {
        IDeclarationSource SetterName { get; }
        IDeclarationSource FixedValueSource { get; }
        IDeclarationSource ObjectSource { get; }
        IDeclarationSource ObjectPropertySource { get; }
        IDeclarationSource ObjectPropertyKey { get; }
        bool ConnectToResourceOrObject(IDomain domain, IArea currentArea, IObject currentObject, ref IResource resource, ref IObject obj, ref IObjectProperty objectProperty, ref IDeclarationSource objectPropertyKey);
        bool ConnectToResourceOnly(IDomain domain, ref IResource resource);
        bool ConnectToObjectOnly(IDomain domain, IArea currentArea, IObject currentObject, ref IObject obj, ref IObjectProperty objectProperty, ref IDeclarationSource objectPropertyKey);
        bool ConnectToStringList(IDomain domain, IArea currentArea, IObject currentObject, ref IResource resource, ref IObject obj, ref IObjectPropertyStringList objectProperty);
        bool ConnectToObjectInteger(IDomain domain, IArea currentArea, IObject currentObject, ref IObject obj, ref IObjectPropertyInteger objectProperty);
        bool ConnectToObjectBoolean(IDomain domain, IArea currentArea, IObject currentObject, ref IObject obj, ref IObjectPropertyBoolean objectProperty);
        bool ConnectToObjectString(IDomain domain, IArea currentArea, IObject currentObject, ref IObject obj, ref IObjectPropertyString objectProperty);
        bool ConnectToObjectReadonlyString(IDomain domain, IArea currentArea, IObject currentObject, ref IObject obj, ref IObjectPropertyReadonlyString objectProperty);
        bool ConnectToObjectStringList(IDomain domain, IArea currentArea, IObject currentObject, ref IObject obj, ref IObjectPropertyStringList objectProperty);
        bool ConnectToObjectItem(IDomain domain, IArea currentArea, IObject currentObject, ref IObject obj, ref IObjectPropertyItem objectProperty, ref IObject nestedObject);
        bool ConnectToObjectItemList(IDomain domain, IArea currentArea, IObject currentObject, ref IObject obj, ref IObjectPropertyItemList objectProperty, ref IObject nestedObject);
        bool ConnectToObjectIndex(IDomain domain, IArea currentArea, IObject currentObject, ref IObject obj, ref IObjectPropertyIndex objectProperty);
    }
}
