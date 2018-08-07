using System.Collections.Generic;

namespace Parser
{
    public interface IGeneratorDomain
    {
        List<IGeneratorArea> Areas { get; }
        List<IGeneratorDesign> Designs { get; }
        List<IGeneratorLayout> Layouts { get; }
        List<IGeneratorObject> Objects { get; }
        List<IGeneratorPage> Pages { get; }
        List<IGeneratorResource> Resources { get; }
        List<IGeneratorBackground> Backgrounds { get; }
        List<IGeneratorColorScheme> ColorSchemes { get; }
        IGeneratorPage HomePage { get; }
        IGeneratorColorScheme SelectedColorScheme { get; }
        void Generate(string rootFolderName);
    }
}
