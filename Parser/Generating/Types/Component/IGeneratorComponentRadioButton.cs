using System.Collections.Generic;

namespace Parser
{
    public interface IGeneratorComponentRadioButton : IGeneratorComponent
    {
        IGeneratorResource ContentResource { get; }
        IGeneratorObject ContentObject { get; }
        IGeneratorObjectProperty ContentObjectProperty { get; }
        IDeclarationSource ContentKey { get; }
        IGeneratorObject IndexObject { get; }
        IGeneratorObjectPropertyIndex IndexObjectProperty { get; }
        string GroupName { get; }
        int GroupIndex { get; }
        ICollection<IGeneratorComponentRadioButton> Group { get; }
    }
}
