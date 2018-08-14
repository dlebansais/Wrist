namespace Parser
{
    public interface IGeneratorComponentCheckBox : IGeneratorComponent
    {
        IGeneratorResource ContentResource { get; }
        IGeneratorObject ContentObject { get; }
        IGeneratorObjectProperty ContentObjectProperty { get; }
        IDeclarationSource ContentKey { get; }
        IGeneratorObject CheckedObject { get; }
        IGeneratorObjectPropertyBoolean CheckedObjectProperty { get; }
        bool IsController { get; }
    }
}
