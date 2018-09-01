using System.IO;
using Windows.UI.Xaml;

namespace Parser
{
    public interface IGeneratorComponent
    {
        IDeclarationSource Source { get; }
        string XamlName { get; }
        string ControlName { get; }
        bool IsReferencing(IGeneratorArea other);
        bool Connect(IGeneratorDomain domain);
        void Generate(IGeneratorDesign design, string styleName, string attachedProperties, string elementProperties, TextWrapping? textWrapping, bool isHorizontalAlignmentStretch, int indentation, IGeneratorPage currentPage, IGeneratorObject currentObject, IGeneratorColorTheme colorTheme, StreamWriter xamlWriter, string visibilityBinding);
        string GetComponentValue(IGeneratorPage currentPage, IGeneratorObject currentObject, IGeneratorResource resourceValue, IGeneratorObject objectValue, IGeneratorObjectProperty objectPropertyValue, IDeclarationSource key, bool isTwoWays);
        string GetObjectBinding(IGeneratorObject currentObject, IGeneratorObject objectValue, IGeneratorObjectProperty objectPropertyValue);
    }
}
