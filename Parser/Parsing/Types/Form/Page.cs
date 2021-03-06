﻿using System.Collections.Generic;

namespace Parser
{
    public class Page : IPage
    {
        public static Page CurrentPage = new Page("<current page>");
        public static Page PreviousPage = new Page("<previous page>");
        public static Page AnyPage = new Page("<custom page>");

        private Page(string name)
        {
            Name = name;
        }

        public Page(string name, string fileName, string xamlName, IComponentEvent queryEvent, IDeclarationSource areaSource, IParsingSource allAreaLayoutsSource, Dictionary<IDeclarationSource, string> areaLayoutPairs, IDeclarationSource designSource, IDeclarationSource widthSource, IDeclarationSource heightSource, bool isScrollable, IDeclarationSource backgroundSource, IDeclarationSource backgroundColorSource, string tag)
        {
            Name = name;
            FileName = fileName;
            XamlName = xamlName;
            QueryEvent = queryEvent;
            AreaSource = areaSource;
            AllAreaLayoutsSource = allAreaLayoutsSource;
            AreaLayoutPairs = areaLayoutPairs;
            DesignSource = designSource;
            WidthSource = widthSource;
            HeightSource = heightSource;
            IsScrollable = isScrollable;
            BackgroundSource = backgroundSource;
            BackgroundColorSource = backgroundColorSource;
            Tag = tag;
        }

        public string Name { get; private set; }
        public string FileName { get; private set; }
        public string XamlName { get; private set; }
        public IComponentEvent QueryEvent { get; private set; }
        public IObject QueryObject { get; private set; }
        public IObjectEvent QueryObjectEvent { get; private set; }
        public IDeclarationSource AreaSource { get; private set; }
        public IArea Area { get; private set; }
        public IParsingSource AllAreaLayoutsSource { get; private set; }
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
        public string Tag { get; private set; }
        public IDynamic Dynamic { get; private set; }
        public bool IsReachable { get; private set; }

        public bool Connect(IDomain domain)
        {
            bool IsConnected = false;

            ConnectQueryEvent(domain, ref IsConnected);
            ConnectArea(domain, ref IsConnected);
            ConnectDynamic(domain, ref IsConnected);
            ConnectLayout(domain, ref IsConnected);
            ConnectDesign(domain, ref IsConnected);
            ConnectBackground(domain, ref IsConnected);

            return IsConnected;
        }

        private void ConnectQueryEvent(IDomain domain, ref bool IsConnected)
        {
            if (QueryEvent != null && (QueryObject == null || QueryObjectEvent == null))
            {
                IObject Object = null;
                IObjectEvent ObjectEvent = null;
                QueryEvent.Connect(domain, ref Object, ref ObjectEvent);
                QueryObject = Object;
                QueryObjectEvent = ObjectEvent;
                QueryObjectEvent.SetIsProvidingCustomPageName(QueryEvent.EventSource, true);

                IsConnected = true;
                SetIsReachable();
            }
        }

        public void ConnectArea(IDomain domain, ref bool IsConnected)
        {
            if (Area == null)
            {
                if (AreaSource.Name == Parser.Area.EmptyArea.Name)
                    Area = Parser.Area.EmptyArea;
                else
                {
                    foreach (IArea Item in domain.Areas)
                        if (Item.Name == AreaSource.Name)
                        {
                            Area = Item;
                            break;
                        }

                    if (Area == null)
                        throw new ParsingException(118, AreaSource.Source, $"Unknown area '{AreaSource.Name}'.");

                    Area.SetIsUsed();
                    Area.SetCurrentObject(AreaSource, null);
                }

                IsConnected = true;
            }
        }

        public void ConnectDynamic(IDomain domain, ref bool IsConnected)
        {
            if (Dynamic == null)
            {
                foreach (IDynamic Item in domain.Dynamics)
                    if (Item.Name == Name)
                    {
                        Dynamic = Item;
                        break;
                    }

                if (Dynamic == null)
                    throw new ParsingException(217, FileName, "Dynamic file not found.");

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

                    if (AreaSource.Name == Parser.Area.EmptyArea.Name)
                        EntryArea = Parser.Area.EmptyArea;
                    else
                    {
                        foreach (IArea Item in domain.Areas)
                            if (Item.Name == AreaSource.Name)
                            {
                                EntryArea = Item;
                                break;
                            }
                    }

                    if (LayoutName == Parser.Layout.EmptyLayout.Name)
                        EntryLayout = Parser.Layout.EmptyLayout;
                    else
                    {
                        foreach (ILayout Item in domain.Layouts)
                            if (Item.Name == LayoutName)
                            {
                                EntryLayout = Item;
                                break;
                            }
                    }

                    if (EntryArea == null)
                        throw new ParsingException(119, AreaSource.Source, $"Unknown area '{AreaSource.Name}'.");
                    else if (EntryLayout == null)
                        throw new ParsingException(120, AreaSource.Source, $"Unknown layout '{LayoutName}'.");
                    else if ((EntryArea == Parser.Area.EmptyArea && EntryLayout != Parser.Layout.EmptyLayout) || (EntryArea != Parser.Area.EmptyArea && EntryLayout == Parser.Layout.EmptyLayout))
                        if (EntryArea == Parser.Area.EmptyArea)
                            throw new ParsingException(0, AreaSource.Source, $"The empty area can only be associated to the empty layout.");
                        else
                            throw new ParsingException(0, AreaSource.Source, $"The empty layout can only be associated to the empty area.");

                    if (EntryArea != Parser.Area.EmptyArea && EntryLayout != Parser.Layout.EmptyLayout)
                    {
                        AreaLayouts.Add(EntryArea, EntryLayout.GetClone());
                        AreaLayoutBacktracks.Add(EntryArea, AreaSource);
                    }
                }

                foreach (KeyValuePair<IArea, ILayout> Entry in AreaLayouts)
                    Entry.Value.ConnectComponents(domain, Dynamic, Entry.Key.Components);

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
                    throw new ParsingException(121, DesignSource.Source, $"Unknown design '{DesignSource.Name}'.");

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
                    throw new ParsingException(122, BackgroundSource.Source, $"Unknown background '{BackgroundSource.Name}'.");

                double WidthValue;
                if (!ParserDomain.TryParseDouble(WidthSource.Name, out WidthValue))
                    throw new ParsingException(123, WidthSource.Source, $"'{WidthSource.Name}' not parsed as a width.");

                Width = WidthValue;

                IsConnected = true;
            }
            else if (Width == 0 && BackgroundSource == null)
            {
                if (WidthSource.Name != "stretch")
                    throw new ParsingException(124, WidthSource.Source, "Only valid width when no background is 'stretch'.");

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
                    if (!ParserDomain.TryParseDouble(HeightSource.Name, out HeightValue))
                        throw new ParsingException(126, HeightSource.Source, $"'{HeightSource.Name}' not parsed as a height (only valid values are integer constants or 'infinite').");

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
                        throw new ParsingException(127, BackgroundColorSource.Source, $"Background color '{BackgroundColorSource.Name}' not found in color theme '{Item.Name}'.");
                }

                IsConnected = true;
            }
        }

        public void SetIsReachable()
        {
            IsReachable = true;
        }

        public override string ToString()
        {
            return $"{GetType().Name} '{Name}'";
        }
    }
}
