using System;
using System.Collections;
using System.Collections.Generic;
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
                      ITranslation translation, 
                      IPage homePage, 
                      IColorTheme selectedColorTheme, 
                      IUnitTest selectedUnitTest)
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

        public void CheckUnused(Action<string> handler, IDictionary<ConditionalDefine, bool> conditionalDefineTable)
        {
            InitUnusedLists(out IList<IArea> unusedAreaList,
                            out IList<IComponent> unusedComponentList,
                            out IList<IObject> unusedObjectList,
                            out IList<IObjectProperty> unusedObjectPropertyList,
                            out IList<IObjectEvent> unusedObjectEventList,
                            out IList<IPage> unusedPageList,
                            out IList<IResource> unusedResourceList,
                            out IList<IDynamicProperty> unusedDynamicPropertyList,
                            out IList<string> unusedKeyList);

            CheckUnused(unusedAreaList, 
                        unusedComponentList, 
                        unusedObjectList, 
                        unusedObjectPropertyList, 
                        unusedObjectEventList,
                        unusedPageList,
                        unusedResourceList,
                        unusedDynamicPropertyList,
                        unusedKeyList);

            WarnUnused(handler,
                       conditionalDefineTable,
                       unusedAreaList,
                       unusedComponentList,
                       unusedObjectList,
                       unusedObjectPropertyList,
                       unusedObjectEventList,
                       unusedPageList,
                       unusedResourceList,
                       unusedDynamicPropertyList,
                       unusedKeyList);
        }

        private void InitUnusedLists(out IList<IArea> unusedAreaList,
                                     out IList<IComponent> unusedComponentList,
                                     out IList<IObject> unusedObjectList,
                                     out IList<IObjectProperty> unusedObjectPropertyList,
                                     out IList<IObjectEvent> unusedObjectEventList,
                                     out IList<IPage> unusedPageList,
                                     out IList<IResource> unusedResourceList,
                                     out IList<IDynamicProperty> unusedDynamicPropertyList,
                                     out IList<string> unusedKeyList)
        {
            unusedAreaList = new List<IArea>();
            unusedComponentList = new List<IComponent>();
            foreach (IArea Area in Areas)
            {
                unusedAreaList.Add(Area);

                foreach (IComponent Component in Area.Components)
                    unusedComponentList.Add(Component);
            }

            unusedObjectList = new List<IObject>();
            unusedObjectPropertyList = new List<IObjectProperty>();
            unusedObjectEventList = new List<IObjectEvent>();
            foreach (IObject Object in Objects)
            {
                unusedObjectList.Add(Object);

                foreach (IObjectProperty ObjectProperty in Object.Properties)
                    unusedObjectPropertyList.Add(ObjectProperty);

                foreach (IObjectEvent ObjectEvent in Object.Events)
                    unusedObjectEventList.Add(ObjectEvent);
            }

            unusedPageList = new List<IPage>();
            foreach (IPage Page in Pages)
                unusedPageList.Add(Page);

            unusedResourceList = new List<IResource>();
            foreach (IResource Resource in Resources)
                unusedResourceList.Add(Resource);

            unusedDynamicPropertyList = new List<IDynamicProperty>();
            foreach (IDynamic Dynamic in Dynamics)
                foreach (IDynamicProperty Property in Dynamic.Properties)
                    unusedDynamicPropertyList.Add(Property);

            unusedKeyList = new List<string>();
            foreach (string Key in Translation.KeyList)
                unusedKeyList.Add(Key);
        }

        public void CheckUnused(IList<IArea> unusedAreaList, 
                                IList<IComponent> unusedComponentList,
                                IList<IObject> unusedObjectList,
                                IList<IObjectProperty> unusedObjectPropertyList,
                                IList<IObjectEvent> unusedObjectEventList,
                                IList<IPage> unusedPageList,
                                IList<IResource> unusedResourceList,
                                IList<IDynamicProperty> unusedDynamicPropertyList,
                                IList<string> unusedKeyList)
        {
            foreach (IArea Area in Areas)
                if (Area.IsUsed)
                {
                    if (unusedAreaList.Contains(Area))
                        unusedAreaList.Remove(Area);

                    foreach (IComponent Component in Area.Components)
                        if (Component.IsUsed)
                        {
                            if (unusedComponentList.Contains(Component))
                                unusedComponentList.Remove(Component);
                        }
                }

            foreach (IObject Object in Objects)
                if (Object.IsUsed)
                {
                    if (unusedObjectList.Contains(Object))
                        unusedObjectList.Remove(Object);

                    foreach (IObjectProperty Property in Object.Properties)
                        if (Property.IsRead || Property.IsWrite)
                        {
                            if (unusedObjectPropertyList.Contains(Property))
                                unusedObjectPropertyList.Remove(Property);
                        }

                    foreach (IObjectEvent Event in Object.Events)
                        if (Event.IsUsed)
                        {
                            if (unusedObjectEventList.Contains(Event))
                                unusedObjectEventList.Remove(Event);
                        }
                }

            foreach (IPage Page in Pages)
                if (Page.IsReachable)
                {
                    if (unusedPageList.Contains(Page))
                        unusedPageList.Remove(Page);
                }

            foreach (IResource Resource in Resources)
                if (Resource.IsUsed)
                {
                    if (unusedResourceList.Contains(Resource))
                        unusedResourceList.Remove(Resource);
                }

            foreach (IDynamic Dynamic in Dynamics)
                foreach (IDynamicProperty Property in Dynamic.Properties)
                    if (Property.IsUsed)
                    {
                        if (unusedDynamicPropertyList.Contains(Property))
                            unusedDynamicPropertyList.Remove(Property);
                    }

            foreach (string Key in Translation.KeyList)
                if (Translation.UsedKeyList.Contains(Key) || Parser.Translation.IsKeyReserved(Key))
                {
                    if (unusedKeyList.Contains(Key))
                        unusedKeyList.Remove(Key);
                }
        }

        private void WarnUnused(Action<string> handler,
                                IDictionary<ConditionalDefine, bool> conditionalDefineTable,
                                IList<IArea> unusedAreaList,
                                IList<IComponent> unusedComponentList,
                                IList<IObject> unusedObjectList,
                                IList<IObjectProperty> unusedObjectPropertyList,
                                IList<IObjectEvent> unusedObjectEventList,
                                IList<IPage> unusedPageList,
                                IList<IResource> unusedResourceList,
                                IList<IDynamicProperty> unusedDynamicPropertyList,
                                IList<string> unusedKeyList)
        {
            bool IsFirstWarning = true;

            foreach (IArea Area in unusedAreaList)
                Warn(handler, $"unused area: '{Area.Name}'", conditionalDefineTable, ref IsFirstWarning);

            foreach (IArea Area in Areas)
                foreach (IComponent Component in Area.Components)
                    if (unusedComponentList.Contains(Component))
                        Warn(handler, $"unused component: '{Component.Source.Name}' in area '{Area.Name}'", conditionalDefineTable, ref IsFirstWarning);

            foreach (IObject Object in unusedObjectList)
                Warn(handler, $"unused object: '{Object.Name}'", conditionalDefineTable, ref IsFirstWarning);

            foreach (IObject Object in Objects)
            {
                foreach (IObjectProperty Property in Object.Properties)
                    if (unusedObjectPropertyList.Contains(Property))
                        Warn(handler, $"unused property: '{Object.Name}.{Property.NameSource.Name}'", conditionalDefineTable, ref IsFirstWarning);

                foreach (IObjectEvent Event in Object.Events)
                    if (unusedObjectEventList.Contains(Event))
                        Warn(handler, $"unused event: '{Object.Name}.{Event.NameSource.Name}'", conditionalDefineTable, ref IsFirstWarning);
            }

            foreach (IPage Page in unusedPageList)
                Warn(handler, $"Unreachable page: '{Page.Name}'", conditionalDefineTable, ref IsFirstWarning);

            foreach (IResource Resource in unusedResourceList)
                Warn(handler, $"unused resource: '{Resource.Name}'", conditionalDefineTable, ref IsFirstWarning);

            foreach (IDynamic Dynamic in Dynamics)
                foreach (IDynamicProperty Property in Dynamic.Properties)
                    if (unusedDynamicPropertyList.Contains(Property))
                        Warn(handler, $"unused dynamic property: '{Property.Source.Name}' in '{Dynamic.Name}'", conditionalDefineTable, ref IsFirstWarning);

            foreach (string Key in unusedKeyList)
                Warn(handler, $"unused translation key: '{Key}'", conditionalDefineTable, ref IsFirstWarning);
        }

        private void Warn(Action<string> handler, string text, IDictionary<ConditionalDefine, bool> conditionalDefineTable, ref bool isFirstWarning)
        {
            if (isFirstWarning)
            {
                string Defines = "";
                foreach (KeyValuePair<ConditionalDefine, bool> Entry in conditionalDefineTable)
                {
                    if (Defines.Length > 0)
                        Defines += ", ";

                    Defines += $"{Entry.Key.Name}={Entry.Value}";
                }

                if (Defines.Length > 0)
                {
                    Defines = "Defines: " + Defines;
                    handler(Defines);
                    Debug.WriteLine(Defines);
                }

                isFirstWarning = false;
            }

            handler(text);
            Debug.WriteLine(text);
        }

        public void Verify()
        {
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
                        throw new ParsingException(9, Page.AreaSource.Source, $"Layout specified for area '{SpecifiedArea.Name}' but this area isn't used in page '{Page.Name}'.");
                }

                foreach (KeyValuePair<IArea, IDeclarationSource> Entry in SpecifiedAreas)
                {
                    IArea SpecifiedArea = Entry.Key;

                    if (!Page.AreaLayouts.ContainsKey(SpecifiedArea))
                        throw new ParsingException(10, Page.AllAreaLayoutsSource, $"Area '{SpecifiedArea.Name}' has not layout specified.");

                    if (ComponentProperty.AreaWithCurrentPage.ContainsKey(SpecifiedArea))
                    {
                        IDeclarationSource Declaration = ComponentProperty.AreaWithCurrentPage[SpecifiedArea];

                        string PageKey = ParserDomain.ToKeyName($"page {Page.Name}");
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
                            DesignKeyList.Add($"{Page.Design.XamlName}{ParserDomain.StyleTypeConverter(AsTypeKey.Name)}");
                        else
                            throw new ParsingException(240, "", $"Unexpected key in design '{Page.Design.Name}'.");
                    else
                        throw new ParsingException(240, "", $"Unexpected key in design '{Page.Design.Name}'.");

                foreach (string Key in KeyList)
                    if (!DesignKeyList.Contains(Key) && !Key.EndsWith("DesignHtml"))
                        throw new ParsingException(241, "", $"Resource key '{Key}' not found in design '{Page.Design.Name}'.");
            }

            VerifyAttachedProperties();
        }

        public void VerifyAttachedProperties()
        {
            List<IDockPanel> DockPanels = new List<IDockPanel>();
            List<IGrid> Grids = new List<IGrid>();
            foreach (ILayout Layout in Layouts)
                Layout.ReportElementsWithAttachedProperties(DockPanels, Grids);

            CheckIfAttachedPropertyIsValid(DockPanel.DockTargets.Keys, new List<IPanel>(DockPanels), "DockPanel.Dock", DockPanel.ValidateDock);
            CheckIfAttachedPropertyIsValid(Grid.ColumnTargets.Keys, new List<IPanel>(Grids), "Grid.Column", Grid.ValidateColumn);
            CheckIfAttachedPropertyIsValid(Grid.ColumnSpanTargets.Keys, new List<IPanel>(Grids), "Grid.ColumnSpan", Grid.ValidateColumnSpan);
            CheckIfAttachedPropertyIsValid(Grid.RowTargets.Keys, new List<IPanel>(Grids), "Grid.Row", Grid.ValidateRow);
            CheckIfAttachedPropertyIsValid(Grid.RowSpanTargets.Keys, new List<IPanel>(Grids), "Grid.RowSpan", Grid.ValidateRowSpan);
        }

        private void ListAreas(IArea rootArea, IDeclarationSource declaration, IFormCollection<IArea> usedAreas, Dictionary<IArea, IDeclarationSource> specifiedAreas)
        {
            if (usedAreas.Contains(rootArea))
                usedAreas.Remove(rootArea);

            if (rootArea != Area.EmptyArea)
            {
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
        }

        private void CheckIfAttachedPropertyIsValid(ICollection<ILayoutElement> targetItemTable, ICollection<IPanel> PanelList, string propertyName, Action<IPanel, ILayoutElement> handlerValidate)
        {
            string[] Splitted = propertyName.Split('.');
            string PanelName = Splitted.Length > 0 ? Splitted[0] : "<unknown panel>";

            foreach (ILayoutElement TargetItem in targetItemTable)
            {
                IPanel TargetPanel = null;
                foreach (IPanel Item in PanelList)
                    if (Item.Items.Contains(TargetItem))
                    {
                        TargetPanel = Item;
                        break;
                    }

                if (TargetPanel == null)
                    throw new ParsingException(14, TargetItem.Source, $"Property {propertyName} specified for {TargetItem} but not included in a {PanelName}.");

                handlerValidate(TargetPanel, TargetItem);
            }
        }
    }
}
