using System.Collections.Generic;

namespace Parser
{
    public interface IPage : IForm
    {
        string Name { get; }
        string FileName { get; }
        string XamlName { get; }
        IDeclarationSource AreaSource { get; }
        IArea Area { get; }
        IParsingSource AllAreaLayoutsSource { get; }
        Dictionary<IDeclarationSource, string> AreaLayoutPairs { get; }
        Dictionary<IArea, ILayout> AreaLayouts { get; }
        Dictionary<IArea, IDeclarationSource> AreaLayoutBacktracks { get; }
        IDeclarationSource DesignSource { get; }
        IDesign Design { get; }
        IDeclarationSource WidthSource { get; }
        IDeclarationSource HeightSource { get; }
        double Width { get; }
        double Height { get; }
        bool IsScrollable { get; }
        IDeclarationSource BackgroundSource { get; }
        IBackground Background { get; }
        IDeclarationSource BackgroundColorSource { get; }
        string BackgroundColor { get; }
        IDynamic Dynamic { get; }
    }
}
