using System.Collections.Generic;

namespace Parser
{
    public interface IGeneratorDomain
    {
        string AppNamespace { get; }
        string InputFolderName { get; }
        List<IGeneratorArea> Areas { get; }
        List<IGeneratorDesign> Designs { get; }
        List<IGeneratorLayout> Layouts { get; }
        List<IGeneratorObject> Objects { get; }
        List<IGeneratorPage> Pages { get; }
        List<IGeneratorResource> Resources { get; }
        List<IGeneratorBackground> Backgrounds { get; }
        List<IGeneratorColorScheme> ColorSchemes { get; }
        IGeneratorTranslation Translation { get; }
        IGeneratorPage HomePage { get; }
        IGeneratorColorScheme SelectedColorScheme { get; }
        void Generate(string outputFolderName);
    }
}
