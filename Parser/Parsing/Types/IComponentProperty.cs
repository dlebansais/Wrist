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
        bool ConnectToResource(IDomain domain, ref IResource resource);
        bool ConnectToStringList(IDomain domain, IArea currentArea, IObject currentObject, ref IResource resource, ref IObject obj, ref IObjectPropertyStringList objectProperty);
        bool ConnectToObjectIntegerOnly(IDomain domain, IArea currentArea, IObject currentObject, ref IObject obj, ref IObjectPropertyInteger objectProperty);
        bool ConnectToObjectBooleanOnly(IDomain domain, IArea currentArea, IObject currentObject, ref IObject obj, ref IObjectPropertyBoolean objectProperty);
        bool ConnectToObjectStringOnly(IDomain domain, IArea currentArea, IObject currentObject, ref IObject obj, ref IObjectPropertyString objectProperty);
        bool ConnectToObjectStringListOnly(IDomain domain, IArea currentArea, IObject currentObject, ref IObject obj, ref IObjectPropertyStringList objectProperty);
        bool ConnectToObjectItemOnly(IDomain domain, IArea currentArea, IObject currentObject, ref IObject obj, ref IObjectPropertyItem objectProperty, ref IObject nestedObject);
        bool ConnectToObjectItemListOnly(IDomain domain, IArea currentArea, IObject currentObject, ref IObject obj, ref IObjectPropertyItemList objectProperty, ref IObject nestedObject);
        bool ConnectToObjectIndexOnly(IDomain domain, IArea currentArea, IObject currentObject, ref IObject obj, ref IObjectPropertyIndex objectProperty);
    }
}
