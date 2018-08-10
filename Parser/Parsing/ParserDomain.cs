using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;

namespace Parser
{
    public static class ParserDomain
    {
        public static IDomain Parse(string inputFolderName, string homePageName, string colorSchemeName)
        {
            if (string.IsNullOrEmpty(inputFolderName))
                throw new InvalidDataException("Invalid root folder");

            if (!Directory.Exists(inputFolderName))
                throw new InvalidDataException("Invalid root folder");

            if (string.IsNullOrEmpty(homePageName))
                throw new InvalidDataException("Invalid home page name");

            IFormParser FormParserAreas = new ParserArea("area", "txt");
            IFormParser FormParserDesigns = new ParserDesign("design", "xaml");
            IFormParser FormParserLayouts = new ParserLayout("layout", "xaml");
            IFormParser FormParserObjects = new ParserObject("object", "txt");
            IFormParser FormParserPages = new ParserPage("page", "txt");
            IFormParser FormParserResources = new ParserResource("resource", "png");
            IFormParser FormParserBackgrounds = new ParserBackground("background", "xaml");
            IFormParser FormParserColorSchemes = new ParserColorScheme("color", "txt");

            IFormParserCollection FormParsers = new FormParserCollection()
            {
                FormParserAreas,
                FormParserDesigns,
                FormParserLayouts,
                FormParserObjects,
                FormParserPages,
                FormParserResources,
                FormParserBackgrounds,
                FormParserColorSchemes,
            };

            string[] FolderNames;
            try
            {
                FolderNames = Directory.GetDirectories(inputFolderName, "*.*");
            }
            catch (Exception e)
            {
                throw new ParsingException(inputFolderName, e.Message);
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
                    throw new ParsingException(FullFolderName, $"unexpected folder {FolderName}");
            }

            foreach (IFormParser FormParser in FormParsers)
                if (FormParser.ParsedResult == null)
                    throw new ParsingException(inputFolderName, $"Missing folder '{FormParser.FolderName}'");

            IFormCollection<IArea> Areas = (IFormCollection<IArea>)FormParserAreas.ParsedResult;
            IFormCollection<IDesign> Designs = (IFormCollection<IDesign>)FormParserDesigns.ParsedResult;
            IFormCollection<ILayout> Layouts = (IFormCollection<ILayout>)FormParserLayouts.ParsedResult;
            IFormCollection<IObject> Objects = (IFormCollection<IObject>)FormParserObjects.ParsedResult;
            IFormCollection<IPage> Pages = (IFormCollection<IPage>)FormParserPages.ParsedResult;
            IFormCollection<IResource> Resources = (IFormCollection<IResource>)FormParserResources.ParsedResult;
            IFormCollection<IBackground> Backgrounds = (IFormCollection<IBackground>)FormParserBackgrounds.ParsedResult;
            IFormCollection<IColorScheme> ColorSchemes = (IFormCollection<IColorScheme>)FormParserColorSchemes.ParsedResult;

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
                throw new InvalidDataException($"Home page {homePageName} not found");

            IColorScheme SelectedColorScheme = null;
            foreach (IColorScheme ColorScheme in ColorSchemes)
                if (ColorScheme.Name == colorSchemeName)
                {
                    SelectedColorScheme = ColorScheme;
                    break;
                }
            if (SelectedColorScheme == null)
                throw new InvalidDataException($"Color scheme {colorSchemeName} not found");

            IDomain NewDomain = new Domain(inputFolderName, Areas, Designs, Layouts, Objects, Pages, Resources, Backgrounds, ColorSchemes, Translation, HomePage, SelectedColorScheme);

            bool IsConnected;
            do
            {
                IsConnected = false;

                foreach (IFormParser FormParser in FormParsers)
                    foreach (IConnectable Connectable in FormParser.ParsedResult)
                        IsConnected |= Connectable.Connect(NewDomain);
            }
            while (IsConnected);

            // Make sure areas used by other areas are first
            BubbleSort(Areas);

            foreach (IPage Page in Pages)
            {
                IFormCollection<IArea> UsedAreas = new FormCollection<IArea>();
                IFormCollection<IArea> SpecifiedAreas = new FormCollection<IArea>();
                foreach (KeyValuePair<IArea, ILayout> Entry in Page.AreaLayouts)
                    UsedAreas.Add(Entry.Key);

                ListAreas(Page.Area, UsedAreas, SpecifiedAreas);
                if (UsedAreas.Count > 0)
                    throw new ParsingException(inputFolderName, $"Layout specified for area {UsedAreas[0].Name} but this area isn't used in page {Page.Name}");

                foreach (IArea Area in SpecifiedAreas)
                {
                    if (!Page.AreaLayouts.ContainsKey(Area))
                        throw new ParsingException(inputFolderName, $"Area {Area.Name} has not layout specified");

                    if (ComponentProperty.AreaWithCurrentPage.Contains(Area))
                    {
                        string PageKey = ToKeyName($"page {Page.Name}");
                        if (Translation == null)
                            throw new ParsingException(inputFolderName, $"Translation key used in area '{Area.Name}' but no translation file provided");
                        if (!Translation.KeyList.Contains(PageKey))
                            throw new ParsingException(inputFolderName, $"Translation key for page '{Page.Name}' used in area '{Area.Name}' not found");
                    }
                }
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
                    throw new ParsingException(inputFolderName, $"DockPanel.Dock specified for a control not included in a DockPanel.");
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
                    throw new ParsingException(inputFolderName, $"Grid.Column specified for a control not included in a Grid.");
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
                    throw new ParsingException(inputFolderName, $"Grid.Row specified for a control not included in a Grid.");
            }

            return NewDomain;
        }

        private static void BubbleSort(IFormCollection<IArea> Areas)
        {
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
                throw new ParsingException(formFolderName, e.Message);
            }

            parser.InitResult();
            foreach (string FullFolderName in FolderNames)
            {
                string FolderName = Path.GetFileName(FullFolderName);

                IForm NewForm = parser.Parse(Path.Combine(formFolderName, FolderName, FolderName + "." + parser.Extension));
                parser.ParsedResult.Add(NewForm);
            }
        }

        private static void ListAreas(IArea rootArea, IFormCollection<IArea> usedAreas, IFormCollection<IArea> specifiedAreas)
        {
            if (usedAreas.Contains(rootArea))
                usedAreas.Remove(rootArea);

            if (!specifiedAreas.Contains(rootArea))
                specifiedAreas.Add(rootArea);

            foreach (IComponent Component in rootArea.Components)
            {
                if (Component is IComponentArea AsComponentArea)
                    ListAreas(AsComponentArea.Area, usedAreas, specifiedAreas);
                else if (Component is IComponentPopup AsComponentPopup)
                    ListAreas(AsComponentPopup.Area, usedAreas, specifiedAreas);
                else if (Component is IComponentContainer AsComponentContainer)
                    ListAreas(AsComponentContainer.ItemNestedArea, usedAreas, specifiedAreas);
                else if (Component is IComponentContainerList AsComponentContainerList)
                    ListAreas(AsComponentContainerList.ItemNestedArea, usedAreas, specifiedAreas);
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
                if (char.IsWhiteSpace(name[i]))
                    Result += '-';
                else
                    Result += name[i];

            return Result;
        }

        public static string ToXamlName(IParsingSource source, string name, string suffix)
        {
            if (string.IsNullOrEmpty(name))
                throw new ParsingException(source, "Empty name not valid");

            string Result = "";

            for (int i = 0; i < name.Length; i++)
                if (IsValidIdentifierSymbol(name[i], i))
                    Result += name[i];
                else
                    Result += '_';

            if (!IsIdentifierValid(Result))
                throw new ParsingException(source, "Name only contains invalid characters");

            return Result + suffix;
        }

        public static string ToCSharpName(IParsingSource source, string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ParsingException(source, "Empty name not valid");

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
                throw new ParsingException(source, "Name only contains invalid characters");

            return Result;
        }

        public static void ParseStringPair(IParsingSource source, char separator, out IDeclarationSource nameSource, out string value)
        {
            ParseStringPair(source, source.Line, separator, out nameSource, out value);
        }

        public static void ParseStringPair(IParsingSource source, string line, char separator, out IDeclarationSource nameSource, out string value)
        {
            if (string.IsNullOrEmpty(line))
                throw new ParsingException(source, "Unexpected empty line");

            string[] Splitted = line.Split(separator);
            if (Splitted.Length < 2)
                throw new ParsingException(source, $"<key>{separator}<value> expected");

            string Name = Splitted[0].Trim();
            if (string.IsNullOrEmpty(Name))
                throw new ParsingException(source, $"<key>{separator}<value> expected, found empty key");

            string Value = Splitted[1];
            for (int i = 2; i < Splitted.Length; i++)
                Value = $"{Value}{separator}{Splitted[i]}";

            value = Value.Trim();
            if (string.IsNullOrEmpty(value))
                throw new ParsingException(source, $"<key>{separator}<value> expected, found empty value");

            nameSource = new DeclarationSource(Name, source);
        }
    }
}
