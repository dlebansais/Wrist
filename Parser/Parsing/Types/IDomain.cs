using System;
using System.Collections.Generic;

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
        IFormCollection<IUnitTest> UnitTests { get; }
        ITranslation Translation { get; }
        IPage HomePage { get; }
        IColorTheme SelectedColorTheme { get; }
        IUnitTest SelectedUnitTest { get; }
        void Verify();
        void CheckUnused(Action<string> handler);
    }
}
