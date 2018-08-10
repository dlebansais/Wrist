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
                    IReadOnlyCollection<IGeneratorComponent> Components = Entry.Key.Components;
                    IGeneratorLayout Layout = Entry.Value;
                    IsConnected |= Layout.Connect(domain, Components);
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
                Area.Generate(Layout, AreaLayouts, Design, Indentation + 1, this, colorTheme, xamlWriter);

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

            if (Background != null)
            {
                xamlWriter.WriteLine($"{s}<Grid>");

                Background.Generate(xamlWriter, Indentation, colorTheme);
                GeneratorComponentArea.Generate(Area, "", "", Indentation, colorTheme, xamlWriter, "", Width);

                xamlWriter.WriteLine($"{s}</Grid>");
            }
            else
                GeneratorComponentArea.Generate(Area, "", "", Indentation, colorTheme, xamlWriter, "", Width);

            if (IsScrollable)
            {
                Indentation--;
                xamlWriter.WriteLine("    </ScrollViewer>");
            }

            xamlWriter.WriteLine("</Page>");
        }

        private void GenerateCSharp(IGeneratorDomain domain, string appNamespace, StreamWriter cSharpWriter)
        {
            cSharpWriter.WriteLine("using Windows.UI.Xaml;");
            cSharpWriter.WriteLine("using Windows.UI.Xaml.Controls;");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine($"namespace {appNamespace}");
            cSharpWriter.WriteLine("{");
            cSharpWriter.WriteLine($"    public partial class {XamlName} : Page");
            cSharpWriter.WriteLine("    {");
            cSharpWriter.WriteLine($"        public {XamlName}()");
            cSharpWriter.WriteLine("        {");
            cSharpWriter.WriteLine("            InitializeComponent();");
            cSharpWriter.WriteLine("            DataContext = this;");
            cSharpWriter.WriteLine("        }");

            string ObjectLine = null;
            foreach (IGeneratorObject Object in domain.Objects)
            {
                if (ObjectLine == null)
                    cSharpWriter.WriteLine();

                ObjectLine = $"        public {Object.CSharpName} {Object.CSharpName} {{ get {{ return App.{Object.CSharpName}; }} }}";
                cSharpWriter.WriteLine(ObjectLine);
            }

            if (domain.Translation != null)
                cSharpWriter.WriteLine("        public Translation Translation { get { return App.Translation; } }");

            List<IGeneratorPageNavigation> GoToList = new List<IGeneratorPageNavigation>();
            Area.CollectGoTo(GoToList, this);

            foreach (IGeneratorPageNavigation GoTo in GoToList)
            {
                cSharpWriter.WriteLine();
                cSharpWriter.WriteLine($"        private void {GoTo.EventName}(object sender, RoutedEventArgs e)");
                cSharpWriter.WriteLine("        {");

                if (GoTo.BeforeObject != null && GoTo.BeforeObjectEvent != null)
                {
                    cSharpWriter.WriteLine($"            string Content = (sender as Button).Content as string;");
                    if (GoTo.GoToPage == GeneratorPage.AnyPage)
                    {
                        cSharpWriter.WriteLine($"            string DestinationPageName;");
                        cSharpWriter.WriteLine($"            {GoTo.BeforeObject.CSharpName}.On_{GoTo.BeforeObjectEvent.CSharpName}(\"{Name}\", \"{GoTo.Source.Source.Name}\", Content, out DestinationPageName);");
                        cSharpWriter.WriteLine($"            (App.Current as App).GoTo(DestinationPageName);");
                    }
                    else
                    {
                        cSharpWriter.WriteLine($"            {GoTo.BeforeObject.CSharpName}.On_{GoTo.BeforeObjectEvent.CSharpName}(\"{Name}\", \"{GoTo.Source.Source.Name}\", Content);");
                        cSharpWriter.WriteLine($"            (App.Current as App).GoTo(\"{GoTo.GoToPage.Name}\");");
                    }
                }
                else
                    cSharpWriter.WriteLine($"            (App.Current as App).GoTo(\"{GoTo.GoToPage.Name}\");");

                cSharpWriter.WriteLine("        }");
            }

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
