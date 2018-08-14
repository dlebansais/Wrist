namespace Parser
{
    public interface IComponentCheckBox : IComponent
    {
        IComponentProperty ContentProperty { get; }
        IResource ContentResource { get; }
        IObject ContentObject { get; }
        IObjectProperty ContentObjectProperty { get; }
        IDeclarationSource ContentKey { get; }
        IComponentProperty CheckedProperty { get; }
        IObject CheckedObject { get; }
        IObjectPropertyBoolean CheckedObjectProperty { get; }
        bool IsController { get; }
        void SetController();
    }
}
