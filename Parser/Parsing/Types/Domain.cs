namespace Parser
{
    public class Domain : IDomain
    {
        public Domain(string inputFolderName, IFormCollection<IArea> areas, IFormCollection<IDesign> designs, IFormCollection<ILayout> layouts, IFormCollection<IObject> objects, IFormCollection<IPage> pages, IFormCollection<IResource> resources, IFormCollection<IBackground> backgrounds, IFormCollection<IColorTheme> colorThemes, IFormCollection<IFont> fonts, ITranslation translation, IPage homePage, IColorTheme selectedColorTheme)
        {
            InputFolderName = inputFolderName;
            Areas = areas;
            Designs = designs;
            Layouts = layouts;
            Objects = objects;
            Pages = pages;
            Resources = resources;
            Backgrounds = backgrounds;
            ColorThemes = colorThemes;
            Fonts = fonts;
            Translation = translation;
            HomePage = homePage;
            SelectedColorTheme = selectedColorTheme;
        }

        public string InputFolderName { get; private set; }
        public IFormCollection<IArea> Areas { get; private set; }
        public IFormCollection<IDesign> Designs { get; private set; }
        public IFormCollection<ILayout> Layouts { get; private set; }
        public IFormCollection<IObject> Objects { get; private set; }
        public IFormCollection<IPage> Pages { get; private set; }
        public IFormCollection<IResource> Resources { get; private set; }
        public IFormCollection<IBackground> Backgrounds { get; private set; }
        public IFormCollection<IColorTheme> ColorThemes { get; private set; }
        public IFormCollection<IFont> Fonts { get; private set; }
        public ITranslation Translation { get; private set; }
        public IPage HomePage { get; private set; }
        public IColorTheme SelectedColorTheme { get; private set; }
    }
}
