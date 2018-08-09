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
        IFormCollection<IColorScheme> ColorSchemes { get; }
        ITranslation Translation { get; }
        IPage HomePage { get; }
        IColorScheme SelectedColorScheme { get; }
    }
}
