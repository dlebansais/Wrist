using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Parser
{
    public class GeneratorPage : IGeneratorPage
    {
        public static GeneratorPage CurrentPage = new GeneratorPage(Page.CurrentPage.Name);
        public static GeneratorPage AnyPage = new GeneratorPage(Page.AnyPage.Name);

        private GeneratorPage(string name)
        {
            Name = name;
        }

        public static Dictionary<IPage, IGeneratorPage> GeneratorPageMap { get; } = new Dictionary<IPage, IGeneratorPage>();

        public GeneratorPage(IPage page)
        {
            Name = page.Name;
            FileName = page.FileName;
            XamlName = page.XamlName;
            Width = page.Width;
            Height = page.Height;
            IsScrollable = page.IsScrollable;
            BackgroundColor = page.BackgroundColor;
            BasePage = page;

            GeneratorPageMap.Add(page, this);
        }

        protected IPage BasePage;

        public string Name { get; private set; }
        public string FileName { get; private set; }
        public string XamlName { get; private set; }
        public IGeneratorArea Area { get; private set; }
        public Dictionary<IGeneratorArea, IGeneratorLayout> AreaLayouts { get; } = new Dictionary<IGeneratorArea, IGeneratorLayout>();
        public IGeneratorDesign Design { get; private set; }
        public double Width { get; private set; }
        public double Height { get; private set; }
        public bool IsScrollable { get; private set; }
        public IGeneratorBackground Background { get; private set; }
        public string BackgroundColor { get; private set; }
        public IGeneratorDynamic Dynamic { get; private set; }

        public bool Connect(IGeneratorDomain domain)
        {
            bool IsConnected = false;

            if (Area == null)
            {
                IsConnected = true;
                if (GeneratorArea.GeneratorAreaMap.ContainsKey(BasePage.Area))
                    Area = GeneratorArea.GeneratorAreaMap[BasePage.Area];
            }

            if (AreaLayouts.Count < BasePage.AreaLayouts.Count)
            {
                IsConnected = true;

                foreach (KeyValuePair<IArea, ILayout> Entry in BasePage.AreaLayouts)
                    if (GeneratorArea.GeneratorAreaMap.ContainsKey(Entry.Key) && GeneratorLayout.GeneratorLayoutMap.ContainsKey(Entry.Value))
                        if (!AreaLayouts.ContainsKey(GeneratorArea.GeneratorAreaMap[Entry.Key]))
                            AreaLayouts.Add(GeneratorArea.GeneratorAreaMap[Entry.Key], GeneratorLayout.GeneratorLayoutMap[Entry.Value]);

                foreach (KeyValuePair<IGeneratorArea, IGeneratorLayout> Entry in AreaLayouts)
                {
                    IGeneratorLayout Layout = Entry.Value;
                    IsConnected |= Layout.Connect(domain, Entry.Key);
                }
            }

            if (Design == null)
            {
                IsConnected = true;
                if (GeneratorDesign.GeneratorDesignMap.ContainsKey(BasePage.Design))
                    Design = GeneratorDesign.GeneratorDesignMap[BasePage.Design];
            }

            if (Background == null && BasePage.Background != null)
            {
                IsConnected = true;
                if (GeneratorBackground.GeneratorBackgroundMap.ContainsKey(BasePage.Background))
                    Background = GeneratorBackground.GeneratorBackgroundMap[BasePage.Background];
            }

            if (Dynamic == null)
            {
                IsConnected = true;
                if (GeneratorDynamic.GeneratorDynamicMap.ContainsKey(BasePage.Dynamic))
                    Dynamic = GeneratorDynamic.GeneratorDynamicMap[BasePage.Dynamic];
            }

            return IsConnected;
        }

        public void Generate(IGeneratorDomain domain, string outputFolderName, string appNamespace, IGeneratorColorTheme colorTheme)
        {
            string PagesFolderName = Path.Combine(outputFolderName, "Pages");

            if (!Directory.Exists(PagesFolderName))
                Directory.CreateDirectory(PagesFolderName);

            string XamlFileName = Path.Combine(PagesFolderName, $"{FileName}.xaml");
            string CSharpFileName = Path.Combine(PagesFolderName, $"{FileName}.xaml.cs");

            using (FileStream XamlFile = new FileStream(XamlFileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (StreamWriter XamlWriter = new StreamWriter(XamlFile, Encoding.UTF8))
                {
                    GenerateXaml(domain, appNamespace, colorTheme, XamlWriter);
                }
            }

            using (FileStream CSharpFile = new FileStream(CSharpFileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (StreamWriter CSharpWriter = new StreamWriter(CSharpFile, Encoding.UTF8))
                {
                    GenerateCSharp(domain, appNamespace, CSharpWriter);
                }
            }
        }

        private void GenerateXaml(IGeneratorDomain domain, string appNamespace, IGeneratorColorTheme colorTheme, StreamWriter xamlWriter)
        {
            xamlWriter.WriteLine($"<Page x:Class=\"{appNamespace}.{XamlName}\"");
            xamlWriter.WriteLine("      xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"");
            xamlWriter.WriteLine("      xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"");
            xamlWriter.WriteLine("      xmlns:p=\"clr-namespace:Presentation\"");
            xamlWriter.WriteLine("      xmlns:conv=\"clr-namespace:Converters\"");
            xamlWriter.WriteLine($"      xmlns:local=\"using:{appNamespace}\">");
            xamlWriter.WriteLine("    <Page.Resources>");
            xamlWriter.WriteLine("        <conv:KeyToValueConverter x:Key=\"convKeyToValue\"/>");
            xamlWriter.WriteLine("        <conv:IndexToVisibilityConverter x:Key=\"convIndexToVisibility\"/>");
            xamlWriter.WriteLine("        <conv:IndexToCheckedConverter x:Key=\"convIndexToChecked\"/>");

            foreach (IGeneratorResource Resource in domain.Resources)
                Resource.GenerateResourceLine(xamlWriter);

            int Indentation = 2;
            string s = GeneratorLayout.IndentationString(Indentation);

            // Make sure areas used by other areas are first
            List<IGeneratorArea> Areas = new List<IGeneratorArea>();
            foreach (IGeneratorArea Area in domain.Areas)
                if (Area.IsReferencedBy(this.Area))
                    Areas.Add(Area);

            BubbleSort(Areas);

            foreach (IGeneratorArea Area in Areas)
            {
                if (Area.CurrentObject == null)
                    xamlWriter.WriteLine($"{s}<ControlTemplate x:Key=\"{Area.XamlName}\">");
                else
                    xamlWriter.WriteLine($"{s}<DataTemplate x:Key=\"{Area.XamlName}\">");

                IGeneratorLayout Layout = AreaLayouts[Area];
                Area.Generate(Layout, AreaLayouts, domain.Pages, Design, Indentation + 1, this, colorTheme, xamlWriter);

                if (Area.CurrentObject == null)
                    xamlWriter.WriteLine($"{s}</ControlTemplate>");
                else
                    xamlWriter.WriteLine($"{s}</DataTemplate>");
            }

            if (Background != null)
                Background.GenerateResource(xamlWriter, colorTheme);

            xamlWriter.WriteLine("    </Page.Resources>");

            if (IsScrollable)
            {
                string BackgroundColorProperty = $" Color=\"{BackgroundColor}\"";
                colorTheme.WriteXamlLine(xamlWriter, $"    <ScrollViewer>");
                colorTheme.WriteXamlLine(xamlWriter, "        <ScrollViewer.Background>");
                colorTheme.WriteXamlLine(xamlWriter, $"            <SolidColorBrush{BackgroundColorProperty}/>");
                colorTheme.WriteXamlLine(xamlWriter, "        </ScrollViewer.Background>");
                Indentation++;
            }

            s = GeneratorLayout.IndentationString(Indentation - 1);

            xamlWriter.WriteLine($"{s}<Grid PointerPressed=\"OnPointerPressed\">");

            if (Background != null)
                Background.Generate(xamlWriter, Indentation, colorTheme);
            GeneratorComponentArea.Generate(Area, "", "", Indentation, colorTheme, xamlWriter, "", Width);

            xamlWriter.WriteLine($"{s}</Grid>");

            if (IsScrollable)
            {
                Indentation--;
                xamlWriter.WriteLine("    </ScrollViewer>");
            }

            xamlWriter.WriteLine("</Page>");
        }

        private void GenerateCSharp(IGeneratorDomain domain, string appNamespace, StreamWriter cSharpWriter)
        {
            cSharpWriter.WriteLine("using System.Collections.Generic;");
            cSharpWriter.WriteLine("using Windows.UI.Xaml;");
            cSharpWriter.WriteLine("using Windows.UI.Xaml.Controls;");
            cSharpWriter.WriteLine("using Windows.UI.Xaml.Controls.Primitives;");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine($"namespace {appNamespace}");
            cSharpWriter.WriteLine("{");
            cSharpWriter.WriteLine($"    public partial class {XamlName} : Page, IObjectBase");
            cSharpWriter.WriteLine("    {");
            cSharpWriter.WriteLine($"        public {XamlName}()");
            cSharpWriter.WriteLine("        {");
            cSharpWriter.WriteLine("            InitializeComponent();");
            cSharpWriter.WriteLine("            DataContext = this;");

            if (Dynamic.HasProperties)
            {
                cSharpWriter.WriteLine();
                cSharpWriter.WriteLine($"            Dynamic = new {XamlName}Dynamic(this);");
            }

            cSharpWriter.WriteLine("        }");

            string ObjectLine = null;
            foreach (IGeneratorObject Object in domain.Objects)
                if (Object.IsGlobal)
                {
                    if (ObjectLine == null)
                        cSharpWriter.WriteLine();

                    ObjectLine = $"        public I{Object.CSharpName} Get{Object.CSharpName} {{ get {{ return App.Get{Object.CSharpName}; }} }}";
                    cSharpWriter.WriteLine(ObjectLine);
                }

            if (domain.Translation != null)
                cSharpWriter.WriteLine("        public Translation GetTranslation { get { return App.GetTranslation; } }");

            if (Dynamic.HasProperties)
                cSharpWriter.WriteLine($"        public {XamlName}Dynamic Dynamic {{ get; private set; }}");

            List<Tuple<IGeneratorPageNavigation, IGeneratorObject, IGeneratorObjectPropertyBoolean>> GoToList = new List<Tuple<IGeneratorPageNavigation, IGeneratorObject, IGeneratorObjectPropertyBoolean>>();
            Area.CollectGoTo(GoToList, this);

            foreach (Tuple<IGeneratorPageNavigation, IGeneratorObject, IGeneratorObjectPropertyBoolean> Item in GoToList)
            {
                IGeneratorPageNavigation GoTo = Item.Item1;
                IGeneratorObject ClosePopupObject = Item.Item2;
                IGeneratorObjectPropertyBoolean ClosePopupObjectProperty = Item.Item3;

                cSharpWriter.WriteLine();
                cSharpWriter.WriteLine($"        public void {GoTo.EventName}(object sender, RoutedEventArgs e)");
                cSharpWriter.WriteLine("        {");

                if (GoTo.BeforeObject != null && GoTo.BeforeObjectEvent != null)
                {
                    cSharpWriter.WriteLine("            string Content = (sender as Button).Content as string;");

                    if (GoTo.GoToPage == GeneratorPage.AnyPage)
                    {
                        cSharpWriter.WriteLine($"            PageNames DestinationPageName;");
                        if (GoTo.BeforeObject.IsGlobal)
                            cSharpWriter.WriteLine($"            ((IObjectBase)(sender as Button).DataContext).Get{GoTo.BeforeObject.CSharpName}.On_{GoTo.BeforeObjectEvent.CSharpName}(PageNames.{XamlName}, \"{GoTo.Source.Source.Name}\", Content, out DestinationPageName);");
                        else
                            cSharpWriter.WriteLine($"            (({GoTo.BeforeObject.CSharpName})(sender as Button).DataContext).On_{GoTo.BeforeObjectEvent.CSharpName}(PageNames.{XamlName}, \"{GoTo.Source.Source.Name}\", Content, out DestinationPageName);");
                        cSharpWriter.WriteLine($"            (App.Current as App).GoTo(DestinationPageName);");
                    }
                    else
                    {
                        if (GoTo.BeforeObject.IsGlobal)
                            cSharpWriter.WriteLine($"            ((IObjectBase)(sender as Button).DataContext).Get{GoTo.BeforeObject.CSharpName}.On_{GoTo.BeforeObjectEvent.CSharpName}(PageNames.{XamlName}, \"{GoTo.Source.Source.Name}\", Content);");
                        else
                            cSharpWriter.WriteLine($"            (({GoTo.BeforeObject.CSharpName})(sender as Button).DataContext).On_{GoTo.BeforeObjectEvent.CSharpName}(PageNames.{XamlName}, \"{GoTo.Source.Source.Name}\", Content);");
                        cSharpWriter.WriteLine($"            (App.Current as App).GoTo(PageNames.{GoTo.GoToPage.XamlName});");
                    }
                }
                else
                    cSharpWriter.WriteLine($"            (App.Current as App).GoTo(PageNames.{GoTo.GoToPage.XamlName});");

                if (ClosePopupObject != null && ClosePopupObjectProperty != null)
                {
                    if (ClosePopupObject.IsGlobal)
                        cSharpWriter.WriteLine($"            if (((IObjectBase)(sender as Button).DataContext).Get{ClosePopupObject.CSharpName}.{ClosePopupObjectProperty.CSharpName})");
                    else
                        cSharpWriter.WriteLine($"            if ((({ClosePopupObject.CSharpName})(sender as Button).DataContext).{ClosePopupObjectProperty.CSharpName})");
                    cSharpWriter.WriteLine("            {");
                    cSharpWriter.WriteLine($"                ClosePopups();");
                    if (ClosePopupObject.IsGlobal)
                        cSharpWriter.WriteLine($"                ((IObjectBase)(sender as Button).DataContext).Get{ClosePopupObject.CSharpName}.OnPopupClosed_{ClosePopupObjectProperty.CSharpName}();");
                    else
                        cSharpWriter.WriteLine($"                (({ClosePopupObject.CSharpName})(sender as Button).DataContext).OnPopupClosed_{ClosePopupObjectProperty.CSharpName}();");
                    cSharpWriter.WriteLine("            }");
                }

                cSharpWriter.WriteLine("        }");
            }

            if (Dynamic.HasProperties)
            {
                List<IGeneratorBindableComponent> BoundComponentList = new List<IGeneratorBindableComponent>();
                Area.CollectBoundComponents(BoundComponentList, this);

                foreach (IGeneratorBindableComponent Component in BoundComponentList)
                {
                    string ObjectName = Component.BoundObject.CSharpName;
                    string ObjectPropertyName = Component.BoundObjectProperty.CSharpName;
                    string HandlerName = GeneratorComponent.GetLoadedHandlerName(Component.BoundObject, Component.BoundObjectProperty);
                    string HandlerArgumentTypeName = Component.HandlerArgumentTypeName;

                    if (Component is IGeneratorComponentSelector AsSelector)
                    {
                        cSharpWriter.WriteLine();
                        cSharpWriter.WriteLine($"        public void {HandlerName}(object sender, RoutedEventArgs e)");
                        cSharpWriter.WriteLine("        {");

                        cSharpWriter.WriteLine("            ListBox Ctrl = (ListBox)sender;");
                        cSharpWriter.WriteLine($"            {ObjectName} Item = ({ObjectName})Ctrl.DataContext;");
                        cSharpWriter.WriteLine($"            Item.NotifyPropertyChanged(nameof({ObjectName}.{ObjectPropertyName}));");
                        cSharpWriter.WriteLine("        }");
                    }
                }

                foreach (IGeneratorBindableComponent Component in BoundComponentList)
                {
                    string ObjectName = Component.BoundObject.CSharpName;
                    string ObjectPropertyName = Component.BoundObjectProperty.CSharpName;
                    string HandlerName = GeneratorComponent.GetChangedHandlerName(Component.BoundObject, Component.BoundObjectProperty);
                    string HandlerArgumentTypeName = Component.HandlerArgumentTypeName;

                    cSharpWriter.WriteLine();
                    cSharpWriter.WriteLine($"        public void {HandlerName}(object sender, {Component.HandlerArgumentTypeName} e)");
                    cSharpWriter.WriteLine("        {");

                    if (Component is IGeneratorComponentSelector AsSelector)
                    {
                        cSharpWriter.WriteLine("            ListBox Ctrl = (ListBox)sender;");
                        cSharpWriter.WriteLine($"            {ObjectName} Item = ({ObjectName})Ctrl.DataContext;");
                        cSharpWriter.WriteLine($"            if (Ctrl.SelectedIndex >= 0 && Item.{ObjectPropertyName} != Ctrl.SelectedIndex)");
                        cSharpWriter.WriteLine("            {");
                        cSharpWriter.WriteLine($"                Item.{ObjectPropertyName} = Ctrl.SelectedIndex;");
                        cSharpWriter.WriteLine($"                Item.NotifyPropertyChanged(nameof({ObjectName}.{ObjectPropertyName}));");
                        cSharpWriter.WriteLine("            }");
                        cSharpWriter.WriteLine();
                    }

                    string Notification = $"Dynamic.OnPropertyChanged($\"{{nameof({ObjectName})}}.{{nameof({ObjectName}.{ObjectPropertyName})}}\")";
                    if (Component.PostponeChangedNotification)
                        cSharpWriter.WriteLine($"            Dispatcher.BeginInvoke( () => {Notification} );");
                    else
                        cSharpWriter.WriteLine($"            {Notification};");

                    cSharpWriter.WriteLine("        }");
                }
            }

            Dictionary<IGeneratorPage, string> LinkedPageTable = new Dictionary<IGeneratorPage, string>();
            foreach (KeyValuePair<ILayoutElement, IGeneratorLayoutElement> Entry in GeneratorLayoutElement.GeneratorLayoutElementMap)
                if (Entry.Value is IGeneratorTextDecoration AsTextDecoration)
                    foreach (object LinkEntry in AsTextDecoration.LinkedPageList)
                    {
                        IGeneratorPage LinkedPage = (IGeneratorPage)LinkEntry;
                        if (!LinkedPageTable.ContainsKey(LinkedPage))
                            LinkedPageTable.Add(LinkedPage, GeneratorTextDecoration.ToEventHandlerName(LinkedPage));
                    }

            foreach (KeyValuePair<IGeneratorPage, string> LinkEntry in LinkedPageTable)
            {
                cSharpWriter.WriteLine();
                cSharpWriter.WriteLine($"        private void {LinkEntry.Value}(object sender, RoutedEventArgs e)");
                cSharpWriter.WriteLine("        {");
                cSharpWriter.WriteLine($"            (App.Current as App).GoTo(PageNames.{LinkEntry.Key.XamlName});");
                cSharpWriter.WriteLine("        }");
            }

            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine("        private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)");
            cSharpWriter.WriteLine("        {");
            cSharpWriter.WriteLine("            (App.Current as App).OnIsEnabledChanged(sender, e);");
            cSharpWriter.WriteLine("        }");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine("        private List<ToggleButton> ToggleList = new List<ToggleButton>();");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine("        private void OnToggleLoaded(object sender, RoutedEventArgs e)");
            cSharpWriter.WriteLine("        {");
            cSharpWriter.WriteLine("            if ((sender is ToggleButton AsToggle) && !ToggleList.Contains(AsToggle))");
            cSharpWriter.WriteLine("                ToggleList.Add(AsToggle);");
            cSharpWriter.WriteLine("        }");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine("        private void OnPointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)");
            cSharpWriter.WriteLine("        {");
            cSharpWriter.WriteLine("            ClosePopups();");
            cSharpWriter.WriteLine("        }");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine("        private void ClosePopups()");
            cSharpWriter.WriteLine("        {");
            cSharpWriter.WriteLine("            foreach (ToggleButton Toggle in ToggleList)");
            cSharpWriter.WriteLine("                Toggle.IsChecked = false;");
            cSharpWriter.WriteLine("        }");
            cSharpWriter.WriteLine("    }");
            cSharpWriter.WriteLine("}");
        }

        private static void BubbleSort(List<IGeneratorArea> Areas)
        {
            bool IsSorted;

            do
            {
                IsSorted = false;

                for (int i = 0; i < Areas.Count; i++)
                    for (int j = i + 1; j < Areas.Count; j++)
                        if (Areas[j].IsReferencedBy(Areas[i]))
                        {
                            IGeneratorArea Temp = Areas[i];
                            Areas[i] = Areas[j];
                            Areas[j] = Temp;
                            IsSorted = true;
                        }
            }
            while (IsSorted);
        }

        public override string ToString()
        {
            return $"{GetType().Name} '{Name}'";
        }
    }
}
