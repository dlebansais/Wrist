using System.Collections.Generic;
using System.IO;

namespace Parser
{
    public interface IGeneratorLayoutElement
    {
        string Style { get; }
        string Width { get; }
        string Height { get; }
        string Margin { get; }
        string HorizontalAlignment { get; }
        string VerticalAlignment { get; }
        IGeneratorDynamicProperty DynamicController { get; }
        bool Connect(IGeneratorDomain domain, IReadOnlyCollection<IGeneratorComponent> components);
        void Generate(Dictionary<IGeneratorArea, IGeneratorLayout> areaLayouts, IGeneratorDesign design, int indentation, IGeneratorPage currentPage, IGeneratorObject currentObject, IGeneratorColorTheme colorTheme, StreamWriter xamlWriter, string visibilityBinding);
    }
}
