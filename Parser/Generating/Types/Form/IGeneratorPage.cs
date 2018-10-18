using System.Collections.Generic;

namespace Parser
{
    public interface IGeneratorPage : IGeneratorForm
    {
        string Name { get; }
        string FileName { get; }
        string XamlName { get; }
        IGeneratorObject QueryObject { get; }
        IGeneratorObjectEvent QueryObjectEvent { get; }
        IGeneratorArea Area { get; }
        double Width { get; }
        double Height { get; }
        bool IsScrollable { get; }
        IGeneratorBackground Background { get; }
        string BackgroundColor { get; }
        string Tag { get; }
        Dictionary<IGeneratorArea, IGeneratorLayout> AreaLayouts { get; }
        IGeneratorDynamic Dynamic { get; }
        bool Connect(IGeneratorDomain domain);
        void Generate(IGeneratorDomain domain, string outputFolderName, string appNamespace, IGeneratorColorTheme colorTheme);
    }
}
