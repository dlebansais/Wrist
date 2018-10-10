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
        List<IGeneratorColorTheme> ColorThemes { get; }
        List<IGeneratorFont> Fonts { get; }
        List<IGeneratorDynamic> Dynamics { get; }
        List<IGeneratorUnitTest> UnitTests { get; }
        IGeneratorTranslation Translation { get; }
        IGeneratorPage HomePage { get; }
        IGeneratorColorTheme SelectedColorTheme { get; }
        void Generate(string outputFolderName, IDictionary<ConditionalDefine, bool> conditionalDefineTable);
    }
}
