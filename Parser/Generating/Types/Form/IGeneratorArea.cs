using System.Collections.Generic;
using System.IO;

namespace Parser
{
    public interface IGeneratorArea : IGeneratorForm
    {
        string Name { get; }
        string XamlName { get; }
        IGeneratorObject CurrentObject { get; }
        IReadOnlyCollection<IGeneratorComponent> Components { get; }
        bool IsReferencedBy(IGeneratorArea other);
        bool Connect(IGeneratorDomain domain);
        void Generate(IGeneratorLayout layout, Dictionary<IGeneratorArea, IGeneratorLayout> areaLayouts, IGeneratorDesign design, int indentation, IGeneratorPage currentPage, IGeneratorColorScheme colorScheme, StreamWriter xamlWriter);
        void CollectGoTo(List<IGeneratorPageNavigation> goToList, IGeneratorPage currentPage);
    }
}
