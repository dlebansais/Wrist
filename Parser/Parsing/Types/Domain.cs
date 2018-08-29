using System;
using System.Diagnostics;

namespace Parser
{
    public class Domain : IDomain
    {
        public Domain(string inputFolderName, 
                      IFormCollection<IArea> areas, 
                      IFormCollection<IDesign> designs, 
                      IFormCollection<ILayout> layouts, 
                      IFormCollection<IObject> objects, 
                      IFormCollection<IPage> pages, 
                      IFormCollection<IResource> resources, 
                      IFormCollection<IBackground> backgrounds, 
                      IFormCollection<IColorTheme> colorThemes,
                      IFormCollection<IFont> fonts,
                      IFormCollection<IDynamic> dynamics,
                      IFormCollection<IUnitTest> unitTests,
                      ITranslation translation, IPage homePage, IColorTheme selectedColorTheme, IUnitTest selectedUnitTest)
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
            Dynamics = dynamics;
            UnitTests = unitTests;
            Translation = translation;
            HomePage = homePage;
            SelectedColorTheme = selectedColorTheme;
            SelectedUnitTest = selectedUnitTest;

            HomePage.SetIsReachable();
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
        public IFormCollection<IDynamic> Dynamics { get; private set; }
        public IFormCollection<IUnitTest> UnitTests { get; private set; }
        public ITranslation Translation { get; private set; }
        public IPage HomePage { get; private set; }
        public IColorTheme SelectedColorTheme { get; private set; }
        public IUnitTest SelectedUnitTest { get; private set; }

        public void CheckUnused(Action<string> handler)
        {
            foreach (IArea Area in Areas)
            {
                if (!Area.IsUsed)
                {
                    string Text = $"Unused area: '{Area.Name}'";
                    handler(Text);
                    Debug.WriteLine(Text);
                }

                foreach (IComponent Component in Area.Components)
                    if (!Component.IsUsed)
                    {
                        string Text = $"Unused component: '{Component.Source.Name}' in area '{Area.Name}'";
                        handler(Text);
                        Debug.WriteLine(Text);
                    }
            }

            foreach (IObject Object in Objects)
            {
                if (!Object.IsUsed)
                {
                    string Text = $"Unused object: '{Object.Name}'";
                    handler(Text);
                    Debug.WriteLine(Text);
                }

                foreach (IObjectProperty Property in Object.Properties)
                    if (!Property.IsRead && !Property.IsWrite)
                    {
                        string Text = $"Unused property: '{Object.Name}.{Property.NameSource.Name}'";
                        handler(Text);
                        Debug.WriteLine(Text);
                    }

                foreach (IObjectEvent Event in Object.Events)
                    if (!Event.IsUsed)
                    {
                        string Text = $"Unused event: '{Object.Name}.{Event.NameSource.Name}'";
                        handler(Text);
                        Debug.WriteLine(Text);
                    }
            }

            foreach (IPage Page in Pages)
            {
                if (!Page.IsReachable)
                {
                    string Text = $"Unreachable page: '{Page.Name}'";
                    handler(Text);
                    Debug.WriteLine(Text);
                }
            }

            foreach (IDynamic Dynamic in Dynamics)
            {
                foreach (IDynamicProperty Property in Dynamic.Properties)
                    if (!Property.IsUsed)
                    {
                        string Text = $"Unused dynamic property: '{Property.Source.Name}' in '{Dynamic.Name}'";
                        handler(Text);
                        Debug.WriteLine(Text);
                    }
            }

            foreach (string Key in Translation.KeyList)
                if (!Translation.UsedKeyList.Contains(Key) && !Parser.Translation.IsKeyReserved(Key))
                {
                    string Text = $"Unused translation key: '{Key}'";
                    handler(Text);
                    Debug.WriteLine(Text);
                }
        }
    }
}
