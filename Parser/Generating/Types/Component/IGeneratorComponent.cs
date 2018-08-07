using System.IO;

namespace Parser
{
    public interface IGeneratorComponent
    {
        IDeclarationSource Source { get; }
        string XamlName { get; }
        bool IsReferencing(IGeneratorArea other);
        bool Connect(IGeneratorDomain domain);
        void Generate(IGeneratorDesign design, string style, string attachedProperties, string elementProperties, int indentation, IGeneratorPage currentPage, IGeneratorObject currentObject, IGeneratorColorScheme colorScheme, StreamWriter xamlWriter, string visibilityBinding);
    }
}
