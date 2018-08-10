using System.Collections.Generic;

namespace Parser
{
    public interface IGeneratorPage : IGeneratorForm
    {
        string Name { get; }
        string FileName { get; }
        string XamlName { get; }
        IGeneratorArea Area { get; }
        double Width { get; }
        double Height { get; }
        bool IsScrollable { get; }
        IGeneratorBackground Background { get; }
        string BackgroundColor { get; }
        Dictionary<IGeneratorArea, IGeneratorLayout> AreaLayouts { get; }
        bool Connect(IGeneratorDomain domain);
        void Generate(IGeneratorDomain domain, string outputFolderName, string appNamespace, IGeneratorColorTheme colorTheme);
    }
}
