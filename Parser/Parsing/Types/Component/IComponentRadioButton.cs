using System.Collections.Generic;

namespace Parser
{
    public interface IComponentRadioButton : IComponent
    {
        IComponentProperty ContentProperty { get; }
        IResource ContentResource { get; }
        IObject ContentObject { get; }
        IObjectProperty ContentObjectProperty { get; }
        IDeclarationSource ContentKey { get; }
        string GroupName { get; }
        ICollection<IComponentRadioButton> Group { get; }
    }
}
