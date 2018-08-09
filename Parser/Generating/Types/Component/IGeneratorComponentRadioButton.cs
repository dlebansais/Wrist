using System.Collections.Generic;

namespace Parser
{
    public interface IGeneratorComponentRadioButton : IGeneratorComponent
    {
        IGeneratorResource ContentResource { get; }
        IGeneratorObject ContentObject { get; }
        IGeneratorObjectProperty ContentObjectProperty { get; }
        IDeclarationSource ContentKey { get; }
        string GroupName { get; }
        ICollection<IGeneratorComponentRadioButton> Group { get; }
    }
}
