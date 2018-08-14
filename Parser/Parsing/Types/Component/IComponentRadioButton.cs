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
        IComponentProperty IndexProperty { get; }
        IObject IndexObject { get;  }
        IObjectPropertyIndex IndexObjectProperty { get; }
        string GroupName { get; }
        int GroupIndex { get; }
        ICollection<IComponentRadioButton> Group { get; }
        bool IsController { get; }
        void SetController();
    }
}
