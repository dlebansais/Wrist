using System.Collections.Generic;

namespace Parser
{
    public class Page : IPage, IConnectable
    {
        public static Page CurrentPage = new Page("<current page>");
        public static Page AnyPage = new Page("<custom page>");

        private Page(string name)
        {
            Name = name;
        }

        public Page(string name, string fileName, string xamlName, IDeclarationSource areaSource, Dictionary<IDeclarationSource, string> areaLayoutPairs, IDeclarationSource designSource, IDeclarationSource widthSource, IDeclarationSource heightSource, bool isScrollable, IDeclarationSource backgroundSource, IDeclarationSource backgroundColorSource)
        {
            Name = name;
            FileName = fileName;
            XamlName = xamlName;
            AreaSource = areaSource;
            AreaLayoutPairs = areaLayoutPairs;
            DesignSource = designSource;
            WidthSource = widthSource;
            HeightSource = heightSource;
            IsScrollable = isScrollable;
            BackgroundSource = backgroundSource;
            BackgroundColorSource = backgroundColorSource;
        }

        public string Name { get; private set; }
        public string FileName { get; private set; }
        public string XamlName { get; private set; }
        public IDeclarationSource AreaSource { get; private set; }
        public IArea Area { get; private set; }
        public Dictionary<IDeclarationSource, string> AreaLayoutPairs { get; private set; }
        public Dictionary<IArea, ILayout> AreaLayouts { get; private set; }
        public Dictionary<IArea, IDeclarationSource> AreaLayoutBacktracks { get; } = new Dictionary<IArea, IDeclarationSource>();
        public IDeclarationSource DesignSource { get; private set; }
        public IDesign Design { get; private set; }
        public IDeclarationSource WidthSource { get; private set; }
        public IDeclarationSource HeightSource { get; private set; }
        public double Width { get; private set; }
        public double Height { get; private set; }
        public bool IsScrollable { get; private set; }
        public IDeclarationSource BackgroundSource { get; private set; }
        public IBackground Background { get; private set; }
        public IDeclarationSource BackgroundColorSource { get; private set; }
        public string BackgroundColor { get; private set; }

        public bool Connect(IDomain domain)
        {
            bool IsConnected = false;

            ConnectArea(domain, ref IsConnected);
            ConnectLayout(domain, ref IsConnected);
            ConnectDesign(domain, ref IsConnected);
            ConnectBackground(domain, ref IsConnected);

            return IsConnected;
        }

        public void ConnectArea(IDomain domain, ref bool IsConnected)
        {
            if (Area == null)
            {
                foreach (IArea Item in domain.Areas)
                    if (Item.Name == AreaSource.Name)
                    {
                        Area = Item;
                        break;
                    }

                if (Area == null)
                    throw new ParsingException(AreaSource.Source, $"Unknown area {AreaSource.Name}");

                Area.SetCurrentObject(AreaSource, null);

                IsConnected = true;
            }
        }

        public void ConnectLayout(IDomain domain, ref bool IsConnected)
        {
            if (AreaLayouts == null)
            {
                AreaLayouts = new Dictionary<IArea, ILayout>();

                foreach (KeyValuePair<IDeclarationSource, string> Entry in AreaLayoutPairs)
                {
                    IDeclarationSource AreaSource = Entry.Key;
                    string LayoutName = Entry.Value;
                    IArea EntryArea = null;
                    ILayout EntryLayout = null;

                    foreach (IArea Item in domain.Areas)
                        if (Item.Name == AreaSource.Name)
                        {
                            EntryArea = Item;
                            break;
                        }

                    foreach (ILayout Item in domain.Layouts)
                        if (Item.Name == LayoutName)
                        {
                            EntryLayout = Item;
                            break;
                        }

                    if (EntryArea == null)
                        throw new ParsingException(AreaSource.Source, $"Unknown area {AreaSource.Name}");
                    else if (EntryLayout == null)
                        throw new ParsingException(AreaSource.Source, $"Unknown layout {LayoutName}");

                    AreaLayouts.Add(EntryArea, EntryLayout);
                    AreaLayoutBacktracks.Add(EntryArea, AreaSource);
                }

                foreach (KeyValuePair<IArea, ILayout> Entry in AreaLayouts)
                    Entry.Value.ConnectComponents(domain, Entry.Key.Components);

                IsConnected = true;
            }
        }

        public void ConnectDesign(IDomain domain, ref bool IsConnected)
        {
            if (Design == null)
            {
                foreach (IDesign Item in domain.Designs)
                    if (Item.Name == DesignSource.Name)
                    {
                        Design = Item;
                        break;
                    }

                if (Design == null)
                    throw new ParsingException(DesignSource.Source, $"Unknown design {DesignSource.Name}");

                IsConnected = true;
            }
        }

        public void ConnectBackground(IDomain domain, ref bool IsConnected)
        {
            if (Background == null && BackgroundSource != null)
            {
                foreach (IBackground Item in domain.Backgrounds)
                    if (Item.Name == BackgroundSource.Name)
                    {
                        Background = Item;
                        break;
                    }

                if (Background == null)
                    throw new ParsingException(BackgroundSource.Source, $"Unknown background {BackgroundSource.Name}");

                double WidthValue;
                if (!double.TryParse(WidthSource.Name, out WidthValue))
                    throw new ParsingException(WidthSource.Source, $"{WidthSource.Name} not parsed as a width");

                Width = WidthValue;

                IsConnected = true;
            }
            else if (Width == 0 && BackgroundSource == null)
            {
                if (WidthSource.Name != "stretch")
                    throw new ParsingException(WidthSource.Source, "Only valid width when no background is 'stretch'");

                Width = double.NaN;

                IsConnected = true;
            }

            if (Height == 0)
            {
                if (HeightSource.Name == "infinite")
                    Height = double.NaN;
                else
                {
                    double HeightValue;
                    if (!double.TryParse(HeightSource.Name, out HeightValue))
                        throw new ParsingException(HeightSource.Source, $"{HeightSource.Name} not parsed as a height (only other valid value is 'infinite')");

                    Height = HeightValue;
                }

                IsConnected = true;
            }

            if (BackgroundColor == null)
            {
                foreach (IColorTheme Item in domain.ColorThemes)
                {
                    foreach (KeyValuePair<IDeclarationSource, string> Entry in Item.Colors)
                        if (Entry.Key.Name == BackgroundColorSource.Name)
                        {
                            BackgroundColor = BackgroundColorSource.Name;
                            break;
                        }

                    if (BackgroundColor == null)
                        throw new ParsingException(BackgroundColorSource.Source, $"Background color {BackgroundColorSource.Name} not found in color theme {Item.Name}");
                }

                IsConnected = true;
            }
        }

        public override string ToString()
        {
            return $"{GetType().Name} '{Name}'";
        }
    }
}
