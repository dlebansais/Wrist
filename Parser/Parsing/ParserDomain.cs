using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows.Controls;

namespace Parser
{
    public static class ParserDomain
    {
        public static IDomain Parse(string inputFolderName, string homePageName, string colorThemeName, string unitTestName)
        {
            if (inputFolderName == null)
                throw new ArgumentNullException(nameof(inputFolderName));
            if (homePageName == null)
                throw new ArgumentNullException(nameof(homePageName));

            if (!Directory.Exists(inputFolderName))
                throw new ParsingException(1, inputFolderName, "Input folder not found.");

            if (homePageName.Length == 0)
                throw new ParsingException(2, inputFolderName, "Empty home page name.");

            IFormParser FormParserAreas = new ParserArea("area", "txt");
            IFormParser FormParserDesigns = new ParserDesign("design", "xaml");
            IFormParser FormParserLayouts = new ParserLayout("layout", "xaml");
            IFormParser FormParserObjects = new ParserObject("object", "txt");
            IFormParser FormParserPages = new ParserPage("page", "txt");
            IFormParser FormParserResources = new ParserResource("resource", "png");
            IFormParser FormParserBackgrounds = new ParserBackground("background", "xaml");
            IFormParser FormParserColorThemes = new ParserColorTheme("color", "txt");
            IFormParser FormParserFonts = new ParserFont("font", "ttf");
            IFormParser FormParserDynamics = new ParserDynamic("dynamic", "txt");
            IFormParser FormParserUnitTest = new ParserUnitTest("unit_test", "txt");

            IFormParserCollection FormParsers = new FormParserCollection()
            {
                FormParserAreas,
                FormParserDesigns,
                FormParserLayouts,
                FormParserObjects,
                FormParserPages,
                FormParserResources,
                FormParserBackgrounds,
                FormParserColorThemes,
                FormParserFonts,
                FormParserDynamics,
                FormParserUnitTest,
            };

            string[] FolderNames;
            try
            {
                FolderNames = Directory.GetDirectories(inputFolderName, "*.*");
            }
            catch (Exception e)
            {
                throw new ParsingException(3, inputFolderName, e.Message);
            }

            foreach (string FullFolderName in FolderNames)
            {
                bool Parsed = false;
                string FolderName = Path.GetFileName(FullFolderName);

                foreach (IFormParser FormParser in FormParsers)
                    if (FolderName == FormParser.FolderName)
                    {
                        ParseForm(FormParser, Path.Combine(inputFolderName, FolderName));
                        Parsed = true;
                        break;
                    }

                if (!Parsed)
                    throw new ParsingException(4, FullFolderName, $"Unexpected folder '{FolderName}'.");
            }

            foreach (IFormParser FormParser in FormParsers)
                if (FormParser.ParsedResult == null)
                    throw new ParsingException(5, inputFolderName, $"Missing folder '{FormParser.FolderName}'.");

            IFormCollection<IArea> Areas = (IFormCollection<IArea>)FormParserAreas.ParsedResult;
            IFormCollection<IDesign> Designs = (IFormCollection<IDesign>)FormParserDesigns.ParsedResult;
            IFormCollection<ILayout> Layouts = (IFormCollection<ILayout>)FormParserLayouts.ParsedResult;
            IFormCollection<IObject> Objects = (IFormCollection<IObject>)FormParserObjects.ParsedResult;
            IFormCollection<IPage> Pages = (IFormCollection<IPage>)FormParserPages.ParsedResult;
            IFormCollection<IResource> Resources = (IFormCollection<IResource>)FormParserResources.ParsedResult;
            IFormCollection<IBackground> Backgrounds = (IFormCollection<IBackground>)FormParserBackgrounds.ParsedResult;
            IFormCollection<IColorTheme> ColorThemes = (IFormCollection<IColorTheme>)FormParserColorThemes.ParsedResult;
            IFormCollection<IFont> Fonts = (IFormCollection<IFont>)FormParserFonts.ParsedResult;
            IFormCollection<IDynamic> Dynamics = (IFormCollection<IDynamic>)FormParserDynamics.ParsedResult;
            IFormCollection<IUnitTest> UnitTests = (IFormCollection<IUnitTest>)FormParserUnitTest.ParsedResult;

            string TranslationFile = Path.Combine(inputFolderName, "translations.cvs");
            ITranslation Translation;
            if (File.Exists(TranslationFile))
            {
                Translation = new Translation(TranslationFile, '\t');
                Translation.Process();
            }
            else
                Translation = null;

            IPage HomePage = null;
            foreach (IPage Page in Pages)
                if (Page.Name == homePageName)
                {
                    HomePage = Page;
                    break;
                }
            if (HomePage == null)
                throw new ParsingException(6, inputFolderName, $"Home page '{homePageName}' not found.");

            IColorTheme SelectedColorTheme = null;
            foreach (IColorTheme ColorTheme in ColorThemes)
                if (ColorTheme.Name == colorThemeName)
                {
                    SelectedColorTheme = ColorTheme;
                    break;
                }
            if (SelectedColorTheme == null)
                throw new ParsingException(7, inputFolderName, $"Color theme '{colorThemeName}' not found.");

            IUnitTest SelectedUnitTest = null;
            if (unitTestName != null)
            {
                foreach (IUnitTest UnitTest in UnitTests)
                    if (UnitTest.Name == unitTestName)
                    {
                        SelectedUnitTest = UnitTest;
                        break;
                    }
                if (SelectedUnitTest == null)
                    throw new ParsingException(0, inputFolderName, $"Unit test '{unitTestName}' not found.");
            }

            IDomain NewDomain = new Domain(inputFolderName, 
                                           Areas, 
                                           Designs, 
                                           Layouts, 
                                           Objects, 
                                           Pages, 
                                           Resources, 
                                           Backgrounds, 
                                           ColorThemes, 
                                           Fonts, 
                                           Dynamics,
                                           UnitTests,
                                           Translation, HomePage, SelectedColorTheme, SelectedUnitTest);

            bool IsConnected = true;
            for (int i = 0; i < 100 && IsConnected; i++)
            {
                IsConnected = false;

                foreach (IFormParser FormParser in FormParsers)
                    foreach (IConnectable Connectable in FormParser.ParsedResult)
                        IsConnected |= Connectable.Connect(NewDomain);

                if (Translation != null)
                    IsConnected |= Translation.Connect(NewDomain);
            }
            if (IsConnected)
                throw new ParsingException(8, inputFolderName, $"Unexpected error during processing of the input folder.");

            // Make sure areas used by other areas are first
            BubbleSort(Areas);

            // Remove all layouts and use their clone.
            //((List<ILayout>)Layouts).Clear();
            foreach (IPage Page in Pages)
                foreach (KeyValuePair<IArea, ILayout> Entry in Page.AreaLayouts)
                    Layouts.Add(Entry.Value);

            foreach (IPage Page in Pages)
            {
                IFormCollection<IArea> UsedAreas = new FormCollection<IArea>();
                Dictionary<IArea, IDeclarationSource> SpecifiedAreas = new Dictionary<IArea, IDeclarationSource>();
                foreach (KeyValuePair<IArea, ILayout> Entry in Page.AreaLayouts)
                    UsedAreas.Add(Entry.Key);

                ListAreas(Page.Area, Page.AreaSource, UsedAreas, SpecifiedAreas);

                if (UsedAreas.Count > 0)
                {
                    IArea SpecifiedArea = UsedAreas[0];
                    if (Page.AreaLayoutBacktracks.ContainsKey(SpecifiedArea))
                        throw new ParsingException(9, Page.AreaLayoutBacktracks[SpecifiedArea].Source, $"Layout specified for area '{SpecifiedArea.Name}' but this area isn't used in page '{Page.Name}'.");
                    else
                        throw new ParsingException(9, inputFolderName, $"Layout specified for area '{SpecifiedArea.Name}' but this area isn't used in page '{Page.Name}'.");
                }

                foreach (KeyValuePair<IArea, IDeclarationSource> Entry in SpecifiedAreas)
                {
                    IArea SpecifiedArea = Entry.Key;

                    if (!Page.AreaLayouts.ContainsKey(SpecifiedArea))
                        throw new ParsingException(10, Page.AllAreaLayoutsSource, $"Area '{SpecifiedArea.Name}' has not layout specified.");

                    if (ComponentProperty.AreaWithCurrentPage.ContainsKey(SpecifiedArea))
                    {
                        IDeclarationSource Declaration = ComponentProperty.AreaWithCurrentPage[SpecifiedArea];

                        string PageKey = ToKeyName($"page {Page.Name}");
                        if (Translation == null)
                            throw new ParsingException(11, Declaration.Source, $"Translation key used in area '{SpecifiedArea.Name}' but no translation file provided.");
                        if (!Translation.KeyList.Contains(PageKey))
                            throw new ParsingException(12, Declaration.Source, $"Translation key for page '{Page.Name}' used in area '{SpecifiedArea.Name}' not found.");

                        if (!Translation.UsedKeyList.Contains(PageKey))
                            Translation.UsedKeyList.Add(PageKey);
                    }

                    ILayout SpecifiedLayout = Page.AreaLayouts[SpecifiedArea];
                    foreach (IComponent Component in SpecifiedArea.Components)
                        if (Component is IComponentWithEvent AsComponentWithEvent)
                        {
                            List<IControl> ControlList = new List<IControl>();
                            SpecifiedLayout.Content.ReportControlsUsingComponent(ControlList, AsComponentWithEvent);
                            if (ControlList.Count > 1)
                                throw new ParsingException(220, Component.Source.Source, $"Component '{Component.Source.Name}' is used more than once in page '{Page.Name}'.");
                        }
                }

                List<string> KeyList = new List<string>();
                foreach (KeyValuePair<IArea, ILayout> Entry in Page.AreaLayouts)
                    Entry.Value.ReportResourceKeys(Page.Design, KeyList);

                List<string> DesignKeyList = new List<string>();
                foreach (object Key in Page.Design.Root)
                    if (Key is DictionaryEntry AsEntry)
                        if (AsEntry.Key is string AsStringKey)
                            DesignKeyList.Add(AsStringKey);
                        else if (AsEntry.Key is Type AsTypeKey)
                            DesignKeyList.Add($"{Page.Design.XamlName}{StyleTypeConverter(AsTypeKey.Name)}");
                        else
                            throw new ParsingException(0, "", $"Unexpected key in design '{Page.Design.Name}'.");
                    else
                        throw new ParsingException(0, "", $"Unexpected key in design '{Page.Design.Name}'.");

                foreach (string Key in KeyList)
                    if (!DesignKeyList.Contains(Key))
                        throw new ParsingException(0, "", $"Resource key '{Key}' not found in design '{Page.Design.Name}'.");
            }

            List<IDockPanel> DockPanels = new List<IDockPanel>();
            List<IGrid> Grids = new List<IGrid>();
            foreach (ILayout Layout in Layouts)
                Layout.ReportElementsWithAttachedProperties(DockPanels, Grids);

            Dictionary<ILayoutElement, Dock> DockPanelDockTargets = DockPanel.DockTargets;
            Dictionary<ILayoutElement, int> GridColumnTargets = Grid.ColumnTargets;
            Dictionary<ILayoutElement, int> GridRowTargets = Grid.RowTargets;

            foreach (KeyValuePair<ILayoutElement, Dock> Entry in DockPanelDockTargets)
            {
                bool Found = false;
                foreach (IDockPanel Item in DockPanels)
                    if (Item.Items.Contains(Entry.Key))
                    {
                        Found = true;
                        break;
                    }

                if (!Found)
                    throw new ParsingException(13, Entry.Key.Source, $"DockPanel.Dock specified for {Entry.Key} not included in a DockPanel.");
            }

            foreach (KeyValuePair<ILayoutElement, int> Entry in GridColumnTargets)
            {
                bool Found = false;
                foreach (IGrid Item in Grids)
                    if (Item.Items.Contains(Entry.Key))
                    {
                        Found = true;
                        break;
                    }

                if (!Found)
                    throw new ParsingException(14, Entry.Key.Source, $"Grid.Column specified for {Entry.Key} not included in a Grid.");
            }

            foreach (KeyValuePair<ILayoutElement, int> Entry in GridRowTargets)
            {
                bool Found = false;
                foreach (IGrid Item in Grids)
                    if (Item.Items.Contains(Entry.Key))
                    {
                        Found = true;
                        break;
                    }

                if (!Found)
                    throw new ParsingException(15, Entry.Key.Source, $"Grid.Row specified for {Entry.Key} not included in a Grid.");
            }

            return NewDomain;
        }

        private static void BubbleSort(IFormCollection<IArea> Areas)
        {
            // This bubble sort is used instead of a faster algorithm because we want the result to be fully ordered.
            bool IsSorted;

            do
            {
                IsSorted = false;

                for (int i = 0; i < Areas.Count; i++)
                    for (int j = i + 1; j < Areas.Count; j++)
                        if (Areas[j].IsReferencedBy(Areas[i]))
                        {
                            IArea Temp = Areas[i];
                            Areas[i] = Areas[j];
                            Areas[j] = Temp;
                            IsSorted = true;
                        }
            }
            while (IsSorted);
        }

        private static void ParseForm(IFormParser parser, string formFolderName)
        {
            string[] FolderNames;
            try
            {
                FolderNames = Directory.GetDirectories(formFolderName, "*.*");
            }
            catch (Exception e)
            {
                throw new ParsingException(16, formFolderName, e.Message);
            }

            parser.InitResult();

            foreach (string FullFolderName in FolderNames)
            {
                string FolderName = Path.GetFileName(FullFolderName);

                IForm NewForm = parser.Parse(Path.Combine(formFolderName, FolderName, FolderName + "." + parser.Extension));
                parser.ParsedResult.Add(NewForm);
            }
        }

        private static void ListAreas(IArea rootArea, IDeclarationSource declaration, IFormCollection<IArea> usedAreas, Dictionary<IArea, IDeclarationSource> specifiedAreas)
        {
            if (usedAreas.Contains(rootArea))
                usedAreas.Remove(rootArea);

            if (!specifiedAreas.ContainsKey(rootArea))
                specifiedAreas.Add(rootArea, declaration);

            foreach (IComponent Component in rootArea.Components)
            {
                if (Component is IComponentArea AsComponentArea)
                    ListAreas(AsComponentArea.Area, AsComponentArea.AreaSource, usedAreas, specifiedAreas);
                else if (Component is IComponentPopup AsComponentPopup)
                    ListAreas(AsComponentPopup.Area, AsComponentPopup.AreaSource, usedAreas, specifiedAreas);
                else if (Component is IComponentContainer AsComponentContainer)
                    ListAreas(AsComponentContainer.ItemNestedArea, AsComponentContainer.AreaSource, usedAreas, specifiedAreas);
                else if (Component is IComponentContainerList AsComponentContainerList)
                    ListAreas(AsComponentContainerList.ItemNestedArea, AsComponentContainerList.AreaSource, usedAreas, specifiedAreas);
            }
        }

        // Strings with only _ are not valid
        public static bool IsIdentifierValid(string xamlName)
        {
            for (int i = 0; i < xamlName.Length; i++)
                if (xamlName[i] != '_')
                    return true;

            return false;
        }

        public static bool IsValidIdentifierSymbol(char c, int position)
        {
            if (c == '_' || (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'))
                return true;

            if (position > 0 && (c >= '0' && c <= '9'))
                return true;

            return false;
        }

        public static string ToKeyName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return "unkown";

            string Result = "";

            for (int i = 0; i < name.Length; i++)
            {
                char c = name[i];
                if (char.IsWhiteSpace(name[i]))
                    Result += '-';
                else
                {
                    if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9') || c == '-' || c == '_')
                        Result += c;
                    else
                        Result += '_';
                }
            }

            return Result;
        }

        public static string ToXamlName(IParsingSourceStream sourceStream, string name, string suffix)
        {
            return ToXamlName(sourceStream.FreezedPosition(), name, suffix);
        }

        public static string ToXamlName(IParsingSource source, string name, string suffix)
        {
            if (string.IsNullOrEmpty(name))
                throw new ParsingException(17, source, "Empty name not valid.");

            string Result = "";

            for (int i = 0; i < name.Length; i++)
                if (IsValidIdentifierSymbol(name[i], i))
                    Result += name[i];
                else
                    Result += '_';

            if (!IsIdentifierValid(Result))
                throw new ParsingException(18, source, $"'{name}' only contains invalid characters.");

            return Result + suffix;
        }

        public static string ToCSharpName(IParsingSourceStream sourceStream, string name)
        {
            return ToCSharpName(sourceStream.FreezedPosition(), name);
        }

        public static string ToCSharpName(IParsingSource source, string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ParsingException(17, source, "Empty name not valid.");

            string Result = "";
            bool SetUpper = true;

            for (int i = 0; i < name.Length; i++)
                if (IsValidIdentifierSymbol(name[i], i))
                    if (SetUpper && name[i] != '_')
                    {
                        SetUpper = false;
                        Result += name[i].ToString().ToUpper();
                    }
                    else
                        Result += name[i];
                else if (name[i] == ' ')
                    SetUpper = true;
                else
                    Result += '_';

            if (!IsIdentifierValid(Result))
                throw new ParsingException(18, source, $"'{name}' only contains invalid characters.");

            return Result;
        }

        public static string StyleTypeConverter(string typeName)
        {
            if (typeName == "TextBlock")
                return "Text";
            else if (typeName == "TextBox")
                return "Edit";
            else if (typeName == "PasswordBox")
                return "PasswordEdit";
            else if (typeName == "ListBox")
                return "Selector";
            else
                return typeName;
        }

        public static void ParseStringPair(IParsingSourceStream sourceStream, char separator, out IDeclarationSource nameSource, out string value)
        {
            ParseStringPair(sourceStream, sourceStream.Line, separator, out nameSource, out value);
        }

        public static void ParseStringPair(IParsingSourceStream sourceStream, string line, char separator, out IDeclarationSource nameSource, out string value)
        {
            if (string.IsNullOrEmpty(line))
                throw new ParsingException(19, sourceStream, "Unexpected empty line.");

            string[] Splitted = line.Split(separator);
            if (Splitted.Length < 2)
                throw new ParsingException(20, sourceStream, $"<key>{separator}<value> expected.");

            string Name = Splitted[0].Trim();
            if (string.IsNullOrEmpty(Name))
                throw new ParsingException(21, sourceStream, $"<key>{separator}<value> expected, found empty key.");

            string Value = Splitted[1];
            for (int i = 2; i < Splitted.Length; i++)
                Value = $"{Value}{separator}{Splitted[i]}";

            value = Value.Trim();
            if (string.IsNullOrEmpty(value))
                throw new ParsingException(22, sourceStream, $"<key>{separator}<value> expected, found empty value.");

            nameSource = new DeclarationSource(Name, sourceStream);
        }

        public static bool TryParseObjectProperty(IParsingSourceStream sourceStream, string text, out IDeclarationSource objectSource, out IDeclarationSource memberSource, out IDeclarationSource keySource)
        {
            if (!text.Contains("."))
            {
                objectSource = null;
                memberSource = null;
                keySource = null;
                return false;
            }

            else
            {
                string MemberName;
                ParseStringPair(sourceStream, text, '.', out objectSource, out MemberName);

                string Key;
                int StartIndex = MemberName.IndexOf("[");
                int EndIndex = MemberName.IndexOf("]");
                if (StartIndex > 0 && EndIndex > StartIndex)
                {
                    Key = MemberName.Substring(StartIndex + 1, EndIndex - StartIndex - 1);
                    MemberName = MemberName.Substring(0, StartIndex);
                    keySource = new DeclarationSource(Key, sourceStream);
                }
                else
                    keySource = null;

                memberSource = new DeclarationSource(MemberName, sourceStream);
                return true;
            }
        }

        public static bool TryParseDouble(string s, out double d)
        {
            return double.TryParse(s, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out d);
        }
    }
}
