namespace Parser
{
    public interface IGeneratorComponentButton : IGeneratorComponent
    {
        IGeneratorResource ContentResource { get; }
        IGeneratorObject ContentObject { get; }
        IGeneratorObjectProperty ContentObjectProperty { get; }
        IDeclarationSource ContentKey { get; }
        IGeneratorPageNavigation GoTo { get; }
        IGeneratorObject ClosePopupObject { get; }
        IGeneratorObjectPropertyBoolean ClosePopupObjectProperty { get; }
    }
}
