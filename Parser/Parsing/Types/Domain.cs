namespace Parser
{
    public class Domain : IDomain
    {
        public Domain(string inputFolderName, IFormCollection<IArea> areas, IFormCollection<IDesign> designs, IFormCollection<ILayout> layouts, IFormCollection<IObject> objects, IFormCollection<IPage> pages, IFormCollection<IResource> resources, IFormCollection<IBackground> backgrounds, IFormCollection<IColorScheme> colorSchemes, IPage homePage, IColorScheme selectedColorScheme)
        {
            InputFolderName = inputFolderName;
            Areas = areas;
            Designs = designs;
            Layouts = layouts;
            Objects = objects;
            Pages = pages;
            Resources = resources;
            Backgrounds = backgrounds;
            ColorSchemes = colorSchemes;
            HomePage = homePage;
            SelectedColorScheme = selectedColorScheme;
        }

        public string InputFolderName { get; private set; }
        public IFormCollection<IArea> Areas { get; private set; }
        public IFormCollection<IDesign> Designs { get; private set; }
        public IFormCollection<ILayout> Layouts { get; private set; }
        public IFormCollection<IObject> Objects { get; private set; }
        public IFormCollection<IPage> Pages { get; private set; }
        public IFormCollection<IResource> Resources { get; private set; }
        public IFormCollection<IBackground> Backgrounds { get; private set; }
        public IFormCollection<IColorScheme> ColorSchemes { get; private set; }
        public IPage HomePage { get; private set; }
        public IColorScheme SelectedColorScheme { get; private set; }
    }
}
