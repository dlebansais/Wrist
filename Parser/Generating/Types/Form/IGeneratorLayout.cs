using System.Collections.Generic;
using System.IO;

namespace Parser
{
    public interface IGeneratorLayout : IGeneratorForm
    {
        string Name { get; }
        string XamlName { get; }
        IGeneratorPanel Content { get; }
        bool Connect(IGeneratorDomain domain, IReadOnlyCollection<IGeneratorComponent> components);
        void Generate(Dictionary<IGeneratorArea, IGeneratorLayout> areaLayouts, IGeneratorDesign design, int indentation, IGeneratorPage currentPage, IGeneratorObject currentObject, IGeneratorColorScheme colorScheme, StreamWriter xamlWriter);
    }
}
