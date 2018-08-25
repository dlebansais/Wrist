using System;

namespace Parser
{
    public interface IDomain
    {
        string InputFolderName { get; }
        IFormCollection<IArea> Areas { get; }
        IFormCollection<IDesign> Designs { get; }
        IFormCollection<ILayout> Layouts { get; }
        IFormCollection<IObject> Objects { get; }
        IFormCollection<IPage> Pages { get; }
        IFormCollection<IResource> Resources { get; }
        IFormCollection<IBackground> Backgrounds { get; }
        IFormCollection<IColorTheme> ColorThemes { get; }
        IFormCollection<IFont> Fonts { get; }
        IFormCollection<IDynamic> Dynamics { get; }
        ITranslation Translation { get; }
        IPage HomePage { get; }
        IColorTheme SelectedColorTheme { get; }
        void CheckUnused(Action<string> handler);
    }
}
