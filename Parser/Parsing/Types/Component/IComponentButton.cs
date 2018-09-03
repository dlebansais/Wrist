namespace Parser
{
    public interface IComponentButton : IComponent, IComponentWithEvent
    {
        IComponentProperty ContentProperty { get; }
        IResource ContentResource { get; }
        IObject ContentObject { get; }
        IObjectProperty ContentObjectProperty { get; }
        IDeclarationSource ContentKey { get; }
        IComponentEvent BeforeEvent { get; }
        string GoToPageName { get; }
        IPageNavigation GoTo { get; }
        bool IsExternal { get; }
        IComponentEvent AfterEvent { get; }
        IComponentProperty ClosePopupProperty { get; }
        IObject ClosePopupObject { get; }
        IObjectPropertyBoolean ClosePopupObjectProperty { get; }
    }
}
