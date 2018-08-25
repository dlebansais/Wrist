using System.Collections.Generic;

namespace Parser
{
    public interface IArea : IForm
    {
        string Name { get; }
        string XamlName { get; }
        IObject CurrentObject { get; }
        IReadOnlyCollection<IComponent> Components { get; }
        void SetCurrentObject(IDeclarationSource componentSource, IObject currentObject);
        bool IsReferencedBy(IArea other);
        void FindOtherRadioButtons(string groupName, ICollection<IComponentRadioButton> group);
        bool IsUsed { get; }
        void SetIsUsed();
    }
}
