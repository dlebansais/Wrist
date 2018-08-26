namespace Parser
{
    public interface IGeneratorComponentCheckBox : IGeneratorComponent, IGeneratorBindableComponent
    {
        IGeneratorResource ContentResource { get; }
        IGeneratorObject ContentObject { get; }
        IGeneratorObjectProperty ContentObjectProperty { get; }
        IDeclarationSource ContentKey { get; }
        IGeneratorObject CheckedObject { get; }
        IGeneratorObjectPropertyBoolean CheckedObjectProperty { get; }
    }
}
