using System.Collections.Generic;
using System.IO;

namespace Parser
{
    public interface IGeneratorLayout : IGeneratorForm
    {
        string Name { get; }
        string XamlName { get; }
        string FileName { get; }
        IGeneratorPanel Content { get; }
        bool Connect(IGeneratorDomain domain, IGeneratorArea area);
        void Generate(IGeneratorArea area, Dictionary<IGeneratorArea, IGeneratorLayout> areaLayouts, IList<IGeneratorPage> pageList, IGeneratorDesign design, int indentation, IGeneratorPage currentPage, IGeneratorObject currentObject, IGeneratorColorTheme colorTheme, StreamWriter xamlWriter);
    }
}
