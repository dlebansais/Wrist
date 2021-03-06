﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Parser
{
    public struct XmlnsContentPair
    {
        public Dictionary<string, string> Xmlns;
        public string Content;
    }

    public class GeneratorDomain : IGeneratorDomain
    {
        public GeneratorDomain(string appNamespace, IDomain domain)
        {
            AppNamespace = appNamespace;
            InputFolderName = domain.InputFolderName;

            Areas = new List<IGeneratorArea>();
            foreach (IArea Area in domain.Areas)
                Areas.Add(new GeneratorArea(Area));

            Designs = new List<IGeneratorDesign>();
            foreach (IDesign Design in domain.Designs)
                Designs.Add(new GeneratorDesign(Design));

            Layouts = new List<IGeneratorLayout>();
            foreach (ILayout Layout in domain.Layouts)
                Layouts.Add(new GeneratorLayout(Layout));

            Objects = new List<IGeneratorObject>();
            foreach (IObject Object in domain.Objects)
                Objects.Add(new GeneratorObject(Object));

            Pages = new List<IGeneratorPage>();
            foreach (IPage Page in domain.Pages)
                Pages.Add(new GeneratorPage(Page));

            Resources = new List<IGeneratorResource>();
            foreach (IResource Resource in domain.Resources)
                Resources.Add(new GeneratorResource(Resource));

            Backgrounds = new List<IGeneratorBackground>();
            foreach (IBackground Background in domain.Backgrounds)
                Backgrounds.Add(new GeneratorBackground(Background));

            ColorThemes = new List<IGeneratorColorTheme>();
            foreach (IColorTheme ColorTheme in domain.ColorThemes)
                ColorThemes.Add(new GeneratorColorTheme(ColorTheme));

            Fonts = new List<IGeneratorFont>();
            foreach (IFont Font in domain.Fonts)
                Fonts.Add(new GeneratorFont(Font));

            Dynamics = new List<IGeneratorDynamic>();
            foreach (IDynamic Dynamic in domain.Dynamics)
                Dynamics.Add(new GeneratorDynamic(Dynamic));

            UnitTests = new List<IGeneratorUnitTest>();
            foreach (IUnitTest UnitTest in domain.UnitTests)
                UnitTests.Add(new GeneratorUnitTest(UnitTest));

            bool IsConnected;
            do
            {
                IsConnected = false;

                foreach (IGeneratorArea Area in Areas)
                    IsConnected |= Area.Connect(this);

                foreach (IGeneratorPage Page in Pages)
                    IsConnected |= Page.Connect(this);

                foreach (IGeneratorObject Item in Objects)
                    IsConnected |= Item.Connect(this);

                foreach (IGeneratorDynamic Item in Dynamics)
                    IsConnected |= Item.Connect(this);

                foreach (IGeneratorUnitTest Item in UnitTests)
                    IsConnected |= Item.Connect(this);
            }
            while (IsConnected);

            if (domain.Translation != null)
                Translation = new GeneratorTranslation(domain.Translation);
            else
                Translation = null;

            if (domain.PreprocessorDefine != null)
                PreprocessorDefine = new GeneratorPreprocessorDefine(domain.PreprocessorDefine);
            else
                PreprocessorDefine = null;

            HomePage = GeneratorPage.GeneratorPageMap[domain.HomePage];
            SelectedColorTheme = GeneratorColorTheme.GeneratorColorThemeMap[domain.SelectedColorTheme];
            SelectedUnitTest = domain.SelectedUnitTest != null ? GeneratorUnitTest.GeneratorUnitTestMap[domain.SelectedUnitTest] : null;
        }

        public string AppNamespace { get; private set; }
        public string InputFolderName { get; private set; }
        public List<IGeneratorArea> Areas { get; private set; }
        public List<IGeneratorDesign> Designs { get; private set; }
        public List<IGeneratorLayout> Layouts { get; private set; }
        public List<IGeneratorObject> Objects { get; private set; }
        public List<IGeneratorPage> Pages { get; private set; }
        public List<IGeneratorResource> Resources { get; private set; }
        public List<IGeneratorBackground> Backgrounds { get; private set; }
        public List<IGeneratorColorTheme> ColorThemes { get; private set; }
        public List<IGeneratorFont> Fonts { get; private set; }
        public List<IGeneratorDynamic> Dynamics { get; private set; }
        public List<IGeneratorUnitTest> UnitTests { get; private set; }
        public IGeneratorTranslation Translation { get; private set; }
        public IGeneratorPreprocessorDefine PreprocessorDefine { get; private set; }
        public IGeneratorPage HomePage { get; private set; }
        public IGeneratorColorTheme SelectedColorTheme { get; private set; }
        public IGeneratorUnitTest SelectedUnitTest { get; private set; }

        public void Generate(string outputFolderName, IDictionary<ConditionalDefine, bool> conditionalDefineTable)
        {
            if (!Directory.Exists(outputFolderName))
                Directory.CreateDirectory(outputFolderName);

            string AppNamespace = Path.GetFileName(outputFolderName);
            string AssemblyName = Path.GetFileName(InputFolderName);
            AssemblyName = AssemblyName.Substring(0, 1).ToUpper() + AssemblyName.Substring(1);

            foreach (IGeneratorPage Page in Pages)
                Page.Generate(this, outputFolderName, AppNamespace, SelectedColorTheme);

            foreach (IGeneratorObject Object in Objects)
                Object.Generate(this, outputFolderName, AppNamespace);

            foreach (IGeneratorResource Resource in Resources)
                Resource.Generate(this, outputFolderName);

            foreach (IGeneratorFont Font in Fonts)
                Font.Generate(this, outputFolderName);

            foreach (IGeneratorDynamic Dynamic in Dynamics)
                Dynamic.Generate(this, outputFolderName, AppNamespace);

            if (Translation != null)
                Translation.Generate(outputFolderName, AppNamespace);

            if (SelectedUnitTest != null)
                SelectedUnitTest.Generate(outputFolderName, AppNamespace);

            GenerateAppXaml(outputFolderName, AppNamespace, SelectedColorTheme);
            GenerateAppCSharp(outputFolderName, AppNamespace);
            GenerateAppProject(outputFolderName, AppNamespace, AssemblyName, conditionalDefineTable);
            CopyAssemblyInfo(outputFolderName);
            CopySideFiles(outputFolderName, "bridge.json");
            CopySideFiles(outputFolderName, "packages.config");
            GenerateObjectBaseInterface(outputFolderName, AppNamespace);
            GenerateObjectBase(outputFolderName, AppNamespace);
            GeneratePageNames(outputFolderName, AppNamespace);
            GeneratePageBaseInterface(outputFolderName, AppNamespace);
        }

        private void GenerateAppXaml(string outputFolderName, string appNamespace, IGeneratorColorTheme colorTheme)
        {
            string XamlFileName = Path.Combine(outputFolderName, "App.xaml");

            using (FileStream XamlFile = new FileStream(XamlFileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (StreamWriter XamlWriter = new StreamWriter(XamlFile, Encoding.UTF8))
                {
                    GenerateAppXaml(outputFolderName, appNamespace, XamlWriter, colorTheme);
                }
            }
        }

        private void GenerateAppXaml(string outputFolderName, string appNamespace, StreamWriter xamlWriter, IGeneratorColorTheme colorTheme)
        {
            List<XmlnsContentPair> ResourceList = new List<XmlnsContentPair>();

            foreach (IGeneratorDesign Design in Designs)
            {
                for (int i = 1; i < Design.FileNames.Count; i++)
                {
                    string DesignResourceFile = Design.FileNames[i];

                    XmlnsContentPair Resource = ExtractResources(Design.XamlName, DesignResourceFile);
                    ResourceList.Add(Resource);
                }
            }

            xamlWriter.WriteLine($"<Application x:Class=\"{AppNamespace}.App\"");
            xamlWriter.WriteLine("             xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"");

            List<string> PrefixList = new List<string>();
            List<string> LineList = new List<string>();
            foreach (XmlnsContentPair Resource in ResourceList)
                foreach (KeyValuePair<string, string> Entry in Resource.Xmlns)
                    if (!PrefixList.Contains(Entry.Key))
                    {
                        PrefixList.Add(Entry.Key);
                        LineList.Add($"             xmlns:{Entry.Key}={Entry.Value}");
                    }

            if (!PrefixList.Contains("x"))
            {
                PrefixList.Add("x");
                LineList.Add($"             xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"");
            }

            LineList[LineList.Count - 1] = LineList[LineList.Count - 1] + ">";

            foreach (string Line in LineList)
                xamlWriter.WriteLine(Line);
            xamlWriter.WriteLine("  <Application.Resources>");

            foreach (XmlnsContentPair Resource in ResourceList)
                colorTheme.WriteXamlLine(xamlWriter, Fonts, Resource.Content);

            xamlWriter.WriteLine("  </Application.Resources>");
            xamlWriter.WriteLine("</Application>");
        }

        private void GenerateAppCSharp(string outputFolderName, string appNamespace)
        {
            string CSharpFileName = Path.Combine(outputFolderName, "App.xaml.cs");

            using (FileStream CSharpFile = new FileStream(CSharpFileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (StreamWriter CSharpWriter = new StreamWriter(CSharpFile, Encoding.UTF8))
                {
                    GenerateAppCSharp(outputFolderName, appNamespace, CSharpWriter);
                }
            }
        }

        private void GenerateAppCSharp(string outputFolderName, string appNamespace, StreamWriter cSharpWriter)
        {
            DateTime Now = DateTime.UtcNow;
            string AppVersion = Now.ToString("yyyyMMddHHmmss");

            cSharpWriter.WriteLine("using Presentation;");
            cSharpWriter.WriteLine("using System.Collections.Generic;");
            cSharpWriter.WriteLine("using System.ComponentModel;");
            cSharpWriter.WriteLine("using System.Windows.Browser;");
            cSharpWriter.WriteLine("using Windows.UI.Xaml;");
            cSharpWriter.WriteLine("using Windows.UI.Xaml.Controls;");
            cSharpWriter.WriteLine("using Windows.UI.Xaml.Media;");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine($"namespace {appNamespace}");
            cSharpWriter.WriteLine("{");
            cSharpWriter.WriteLine("    public sealed partial class App : Application, INotifyPropertyChanged");
            cSharpWriter.WriteLine("    {");
            cSharpWriter.WriteLine($"        public static string AppVersion=\"{AppVersion}\";");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine("        public App()");
            cSharpWriter.WriteLine("        {");
            cSharpWriter.WriteLine("            InitializeComponent();");
            cSharpWriter.WriteLine("            NetTools.DatabaseOperation.VersionParameter = AppVersion;");
            cSharpWriter.WriteLine("            QueryString = NetTools.UrlTools.GetQueryString();");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine($"            PageNames StartPage = int.TryParse(Persistent.GetValue(\"page\", \"\"), out int PageIndex) ? (PageNames)PageIndex : PageNames.{HomePage.XamlName};");

            foreach (IGeneratorPage Page in Pages)
                if (Page.QueryObject != null && Page.QueryObjectEvent != null)
                    cSharpWriter.WriteLine($"            Get{Page.QueryObject.CSharpName}.On_{Page.QueryObjectEvent.CSharpName}(StartPage, null, null, null, out StartPage);");

            cSharpWriter.WriteLine($"            GoToPage(StartPage, false);");

            if (SelectedUnitTest != null)
                cSharpWriter.WriteLine($"            GetUnitTest.Start((Page)Window.Current.Content);");

            cSharpWriter.WriteLine("        }");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine("        public static IDictionary<string, string> QueryString { get; private set; }");

            foreach (IGeneratorObject Object in Objects)
                if (Object.IsGlobal)
                    cSharpWriter.WriteLine($"        public static I{Object.CSharpName} Get{Object.CSharpName} {{ get; }} = new {Object.CSharpName}();");

            if (Translation != null)
                cSharpWriter.WriteLine("        public static Translation GetTranslation { get; } = new Translation();");

            if (SelectedUnitTest != null)
                cSharpWriter.WriteLine("        public static UnitTest GetUnitTest { get; } = new UnitTest();");

            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine("        public void GoTo(PageNames pageName)");
            cSharpWriter.WriteLine("        {");
            cSharpWriter.WriteLine("            if (pageName == PageNames.CurrentPage)");
            cSharpWriter.WriteLine("                return;");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine("            else if (pageName == PageNames.PreviousPage)");
            cSharpWriter.WriteLine("            {");
            cSharpWriter.WriteLine("                if (NavigationIndex > 0)");
            cSharpWriter.WriteLine("                {");
            cSharpWriter.WriteLine("                    NavigationIndex--;");
            cSharpWriter.WriteLine("                    GoToPage(NavigationHistory[NavigationIndex], false);");
            cSharpWriter.WriteLine("                    NotifyPropertyChanged(nameof(NavigationHistory));");
            cSharpWriter.WriteLine("                }");
            cSharpWriter.WriteLine("            }");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine("            else");
            cSharpWriter.WriteLine("            {");
            cSharpWriter.WriteLine("                if (NavigationHistory.Count > NavigationIndex)");
            cSharpWriter.WriteLine("                    NavigationHistory.RemoveRange(NavigationIndex, NavigationHistory.Count - NavigationIndex);");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine("                NavigationHistory.Add((Window.Current.Content as IPageBase).ThisPage);");
            cSharpWriter.WriteLine("                NavigationIndex++;");
            cSharpWriter.WriteLine("                NotifyPropertyChanged(nameof(NavigationHistory));");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine("                GoToPage(pageName, false);");
            cSharpWriter.WriteLine("            }");
            cSharpWriter.WriteLine("        }");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine("        public void GoToExternal(PageNames pageName)");
            cSharpWriter.WriteLine("        {");
            cSharpWriter.WriteLine("            GoToPage(pageName, true);");
            cSharpWriter.WriteLine("        }");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine("        public void GoToPage(PageNames pageName, bool isExternal)");
            cSharpWriter.WriteLine("        {");
            cSharpWriter.WriteLine("            Page DestinationPage;");
            cSharpWriter.WriteLine("            switch (pageName)");
            cSharpWriter.WriteLine("            {");

            for (int i = 0; i < Pages.Count; i++)
            {
                IGeneratorPage Page = Pages[i];
                if (Page == HomePage)
                {
                    cSharpWriter.WriteLine("                default:");
                    cSharpWriter.WriteLine($"                case PageNames.{Page.XamlName}:");
                    cSharpWriter.WriteLine($"                    pageName = PageNames.{Page.XamlName};");
                    cSharpWriter.WriteLine($"                    DestinationPage = new {Page.XamlName}();");
                    cSharpWriter.WriteLine("                    break;");
                }
                else
                {
                    cSharpWriter.WriteLine($"                case PageNames.{Page.XamlName}:");
                    cSharpWriter.WriteLine($"                    DestinationPage = new {Page.XamlName}();");
                    cSharpWriter.WriteLine("                    break;");
                }
            }

            cSharpWriter.WriteLine("            }");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine("            Persistent.SetValue(\"page\", ((int)pageName).ToString());");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine("            if (isExternal)");
            cSharpWriter.WriteLine("                NavigateToExternal();");
            cSharpWriter.WriteLine("            else");
            cSharpWriter.WriteLine("                Window.Current.Content = DestinationPage;");
            cSharpWriter.WriteLine("        }");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine("        public void NavigateToExternal()");
            cSharpWriter.WriteLine("        {");
            cSharpWriter.WriteLine("            string Url = NetTools.UrlTools.GetBaseUrl();");
            cSharpWriter.WriteLine("            if (Url.Length > 0)");
            cSharpWriter.WriteLine("                NavigateToExternal(Url);");
            cSharpWriter.WriteLine("        }");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine("        public void NavigateToExternal(string url)");
            cSharpWriter.WriteLine("        {");
            cSharpWriter.WriteLine("            HtmlPage.Window.Navigate(new System.Uri(url, System.UriKind.RelativeOrAbsolute), \"_blank\");");
            cSharpWriter.WriteLine("        }");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine("        private Dictionary<Control, Brush> BrushTable = new Dictionary<Control, Brush>();");
            cSharpWriter.WriteLine("        private Dictionary<Control, Style> StyleTable = new Dictionary<Control, Style>();");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine("        public List<PageNames> NavigationHistory { get; } = new List<PageNames>();");
            cSharpWriter.WriteLine("        public int NavigationIndex { get; private set; } = 0;");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine("        public void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)");
            cSharpWriter.WriteLine("        {");
            cSharpWriter.WriteLine("            if ((sender is Control AsControl) && (e.OldValue is bool IsOldEnabled) && (e.NewValue is bool IsNewEnabled) && (IsOldEnabled != IsNewEnabled))");
            cSharpWriter.WriteLine("                ControlTools.ChangeEnabledStyleOrColor(AsControl, IsNewEnabled, BrushTable, StyleTable, Resources);");
            cSharpWriter.WriteLine("        }");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine("        public void AddPasswordControl(string bindingName, PasswordBox control)");
            cSharpWriter.WriteLine("        {");
            cSharpWriter.WriteLine("            if (!ControlTools.IsControlVisible(control))");
            cSharpWriter.WriteLine("                return;");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine("            if (PasswordControlTable.ContainsKey(bindingName))");
            cSharpWriter.WriteLine("                PasswordControlTable[bindingName] = control;");
            cSharpWriter.WriteLine("            else");
            cSharpWriter.WriteLine("                PasswordControlTable.Add(bindingName, control);");
            cSharpWriter.WriteLine("        }");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine("        public bool GetPasswordValue(string bindingName, out string value)");
            cSharpWriter.WriteLine("        {");
            cSharpWriter.WriteLine("            if (PasswordControlTable.ContainsKey(bindingName))");
            cSharpWriter.WriteLine("            {");
            cSharpWriter.WriteLine("                PasswordBox Ctrl = PasswordControlTable[bindingName];");
            cSharpWriter.WriteLine("                value = Ctrl.Password?.Trim();");
            cSharpWriter.WriteLine("                Ctrl.Password = null;");
            cSharpWriter.WriteLine("                if (!string.IsNullOrEmpty(value))");
            cSharpWriter.WriteLine("                    return true;");
            cSharpWriter.WriteLine("                else");
            cSharpWriter.WriteLine("                    value = null;");
            cSharpWriter.WriteLine("            }");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine("            value = null;");
            cSharpWriter.WriteLine("            return false;");
            cSharpWriter.WriteLine("        }");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine("        private Dictionary<string, PasswordBox> PasswordControlTable = new Dictionary<string, PasswordBox>();");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine("        #region Implementation of INotifyPropertyChanged");
            cSharpWriter.WriteLine("        public event PropertyChangedEventHandler PropertyChanged;");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine("        internal void NotifyPropertyChanged(string propertyName)");
            cSharpWriter.WriteLine("        {");
            cSharpWriter.WriteLine("            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));");
            cSharpWriter.WriteLine("        }");
            cSharpWriter.WriteLine("        #endregion");
            cSharpWriter.WriteLine("    }");
            cSharpWriter.WriteLine("}");
        }

        private void GenerateAppProject(string outputFolderName, string appNamespace, string assemblyName, IDictionary<ConditionalDefine, bool> conditionalDefineTable)
        {
            string ProjectFileName = Path.Combine(outputFolderName, $"{appNamespace}.csproj");

            using (FileStream ProjectFile = new FileStream(ProjectFileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (StreamWriter ProjectWriter = new StreamWriter(ProjectFile, Encoding.UTF8))
                {
                    GenerateAppProject(outputFolderName, appNamespace, assemblyName, conditionalDefineTable, ProjectWriter);
                }
            }
        }

        private void GenerateAppProject(string outputFolderName, string appNamespace, string assemblyName, IDictionary<ConditionalDefine, bool> conditionalDefineTable, StreamWriter projectWriter)
        {
            string AdditionalDefines = "";
            foreach (KeyValuePair<ConditionalDefine, bool> Entry in conditionalDefineTable)
                if (Entry.Value)
                    AdditionalDefines += $";{Entry.Key.Name}";

            if (PreprocessorDefine != null)
                foreach (KeyValuePair<string, bool> Entry in PreprocessorDefine.PreprocessorDefineTable)
                    if (Entry.Value)
                        AdditionalDefines += $";{Entry.Key}";

            projectWriter.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            projectWriter.WriteLine("<Project ToolsVersion=\"4.0\" DefaultTargets=\"Build\" xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\">");
            projectWriter.WriteLine("  <Import Project=\"$(MSBuildExtensionsPath)\\$(MSBuildToolsVersion)\\Microsoft.Common.props\" Condition=\"Exists('$(MSBuildExtensionsPath)\\$(MSBuildToolsVersion)\\Microsoft.Common.props')\"/>");
            projectWriter.WriteLine("  <PropertyGroup>");
            projectWriter.WriteLine("    <Configuration Condition=\" '$(Configuration)' == '' \">Debug</Configuration>");
            projectWriter.WriteLine("    <Platform Condition=\" '$(Platform)' == '' \">AnyCPU</Platform>");
            projectWriter.WriteLine("    <ProjectGuid>{10068A84-2B30-4573-B541-27B9A6ECC992}</ProjectGuid>");
            projectWriter.WriteLine("    <OutputType>Library</OutputType>");
            projectWriter.WriteLine("    <AppDesignerFolder>Properties</AppDesignerFolder>");
            projectWriter.WriteLine($"    <RootNamespace>{appNamespace}</RootNamespace>");
            projectWriter.WriteLine($"    <AssemblyName>{assemblyName}</AssemblyName>");
            projectWriter.WriteLine("    <TargetFrameworkVersion>v4.5</TargetFrameworkVersion>");
            projectWriter.WriteLine("    <FileAlignment>512</FileAlignment>");
            projectWriter.WriteLine("    <IsCSharpXamlForHtml5>true</IsCSharpXamlForHtml5>");
            projectWriter.WriteLine("    <CSharpXamlForHtml5OutputType>Application</CSharpXamlForHtml5OutputType>");
            projectWriter.WriteLine("    <IsCshtml5>true</IsCshtml5>");
            projectWriter.WriteLine("    <Cshtml5OutputType>Application</Cshtml5OutputType>");
            projectWriter.WriteLine("    <StartAction>Program</StartAction>");
            projectWriter.WriteLine("    <StartProgram/>");
            projectWriter.WriteLine($"    <StartArguments>\"{appNamespace}.dll\"</StartArguments>");
            projectWriter.WriteLine("  </PropertyGroup>");
            projectWriter.WriteLine("  <PropertyGroup Condition=\" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' \">");
            projectWriter.WriteLine("    <DebugSymbols>true</DebugSymbols>");
            projectWriter.WriteLine("    <DebugType>full</DebugType>");
            projectWriter.WriteLine("    <Optimize>false</Optimize>");
            projectWriter.WriteLine("    <OutputPath>bin\\Debug\\</OutputPath>");
            projectWriter.WriteLine($"    <DefineConstants>DEBUG;TRACE;CSHARP_XAML_FOR_HTML5;BRIDGE;CSHTML5{AdditionalDefines}</DefineConstants>");
            projectWriter.WriteLine("    <ErrorReport>prompt</ErrorReport>");
            projectWriter.WriteLine("    <WarningLevel>4</WarningLevel>");
            projectWriter.WriteLine("    <NoStdLib>true</NoStdLib>");
            projectWriter.WriteLine("  </PropertyGroup>");
            projectWriter.WriteLine("  <PropertyGroup Condition=\" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' \">");
            projectWriter.WriteLine("    <DebugType>pdbonly</DebugType>");
            projectWriter.WriteLine("    <Optimize>true</Optimize>");
            projectWriter.WriteLine("    <OutputPath>bin\\Release\\</OutputPath>");
            projectWriter.WriteLine($"    <DefineConstants>TRACE;CSHARP_XAML_FOR_HTML5;BRIDGE;CSHTML5{AdditionalDefines}</DefineConstants>");
            projectWriter.WriteLine("    <ErrorReport>prompt</ErrorReport>");
            projectWriter.WriteLine("    <WarningLevel>4</WarningLevel>");
            projectWriter.WriteLine("    <NoStdLib>true</NoStdLib>");
            projectWriter.WriteLine("  </PropertyGroup>");
            projectWriter.WriteLine("  <ItemGroup>");
            projectWriter.WriteLine("     <Reference Include=\"Bridge, Version=17.4.0.0, Culture=neutral, processorArchitecture=MSIL\">");
            projectWriter.WriteLine("       <HintPath>..\\packages\\CSHTML5.2.0.0-alpha23-036\\lib\\net40\\Bridge.dll</HintPath>");
            projectWriter.WriteLine("       <Private>True</Private>");
            projectWriter.WriteLine("     </Reference>");
            projectWriter.WriteLine("     <Reference Include=\"CSHTML5, Version=1.0.0.0, Culture=neutral, processorArchitecture=MSIL\">");
            projectWriter.WriteLine("       <HintPath>..\\packages\\CSHTML5.2.0.0-alpha23-036\\lib\\net40\\CSHTML5.dll</HintPath>");
            projectWriter.WriteLine("       <Private>True</Private>");
            projectWriter.WriteLine("     </Reference>");
            projectWriter.WriteLine("     <Reference Include=\"CSHTML5.Stubs, Version=1.0.0.0, Culture=neutral, processorArchitecture=MSIL\">");
            projectWriter.WriteLine("       <HintPath>..\\packages\\CSHTML5.2.0.0-alpha23-036\\lib\\net40\\CSHTML5.Stubs.dll</HintPath>");
            projectWriter.WriteLine("       <Private>True</Private>");
            projectWriter.WriteLine("     </Reference>");
//            projectWriter.WriteLine("    <Reference Include=\"Microsoft.CSharp\"/>");
            projectWriter.WriteLine("  </ItemGroup>");
            projectWriter.WriteLine("  <ItemGroup>");
            projectWriter.WriteLine("    <None Include=\"bridge.json\" />");
            projectWriter.WriteLine("    <None Include=\"packages.config\" />");
            projectWriter.WriteLine("  </ItemGroup>");
            projectWriter.WriteLine("  <ItemGroup>");
            projectWriter.WriteLine("    <Compile Include=\"App.xaml.cs\">");
            projectWriter.WriteLine("      <DependentUpon>App.xaml</DependentUpon>");
            projectWriter.WriteLine("    </Compile>");
            projectWriter.WriteLine("    <Compile Include=\"Properties\\AssemblyInfo.cs\"/>");

            if (Translation != null)
                projectWriter.WriteLine("    <Compile Include=\"Translation.cs\"/>");

            if (SelectedUnitTest != null)
                projectWriter.WriteLine("    <Compile Include=\"UnitTest.cs\"/>");

            projectWriter.WriteLine("  </ItemGroup>");
            projectWriter.WriteLine("  <ItemGroup>");
            projectWriter.WriteLine("    <Content Include=\"App.xaml\">");
            projectWriter.WriteLine("      <SubType>Designer</SubType>");
            projectWriter.WriteLine("      <Generator>MSBuild:Compile</Generator>");
            projectWriter.WriteLine("    </Content>");
            projectWriter.WriteLine("  </ItemGroup>");
            projectWriter.WriteLine("  <ItemGroup>");

            foreach (IGeneratorPage Page in Pages)
            {
                projectWriter.WriteLine($"    <Page Include=\"Pages\\{Page.FileName}.xaml\">");
                projectWriter.WriteLine("      <Generator>MSBuild:Compile</Generator>");
                projectWriter.WriteLine("      <SubType>Designer</SubType>");
                projectWriter.WriteLine("    </Page>");
                projectWriter.WriteLine($"    <Compile Include=\"Pages\\{Page.FileName}.xaml.cs\">");
                projectWriter.WriteLine($"      <DependentUpon>{Page.FileName}.xaml</DependentUpon>");
                projectWriter.WriteLine("    </Compile>");

                if (Page.Dynamic.HasProperties)
                    projectWriter.WriteLine($"    <Compile Include=\"Dynamics\\{Page.Dynamic.FileName}.cs\"/>");
            }

            projectWriter.WriteLine("  </ItemGroup>");
            projectWriter.WriteLine("  <ItemGroup>");

            List<string> EnumTypeNameList = new List<string>();
            foreach (IGeneratorObject Object in Objects)
            {
                projectWriter.WriteLine($"    <Compile Include=\"Objects\\I{Object.CSharpName}.cs\"/>");
                projectWriter.WriteLine($"    <Compile Include=\"Objects\\{Object.CSharpName}.cs\"/>");

                foreach (IGeneratorObjectProperty Property in Object.Properties)
                    if (Property is IGeneratorObjectPropertyEnum AsPropertyEnum)
                        if (!EnumTypeNameList.Contains(AsPropertyEnum.CSharpName))
                            EnumTypeNameList.Add(AsPropertyEnum.CSharpName);
            }

            foreach (string EnumTypeName in EnumTypeNameList)
            {
                projectWriter.WriteLine($"    <Compile Include=\"Objects\\{EnumTypeName}s.cs\"/>");
                CopyEnumFile(outputFolderName, EnumTypeName);
            }

            projectWriter.WriteLine($"    <Compile Include=\"Objects\\IObjectBase.cs\"/>");
            projectWriter.WriteLine($"    <Compile Include=\"Objects\\ObjectBase.cs\"/>");
            projectWriter.WriteLine($"    <Compile Include=\"Pages\\PageNames.cs\"/>");
            projectWriter.WriteLine($"    <Compile Include=\"Pages\\IPageBase.cs\"/>");
            projectWriter.WriteLine("  </ItemGroup>");
            projectWriter.WriteLine("  <ItemGroup>");

            foreach (IGeneratorResource Resource in Resources)
                projectWriter.WriteLine($"    <EmbeddedResource Include=\"Resources\\{Resource.Name}.png\"/>");

            foreach (IGeneratorFont Font in Fonts)
                projectWriter.WriteLine($"    <Content Include=\"Fonts\\{Font.Name}.ttf\"/>");

            projectWriter.WriteLine("  </ItemGroup>");
            projectWriter.WriteLine("  <ItemGroup>");
            projectWriter.WriteLine("    <ProjectReference Include=\"..\\Presentation\\Presentation.csproj\">");
            projectWriter.WriteLine("      <Project>{c067b9b7-a2de-45f9-af05-beb0a5e70bef}</Project>");
            projectWriter.WriteLine("      <Name>Presentation</Name>");
            projectWriter.WriteLine("    </ProjectReference>");
            projectWriter.WriteLine("    <ProjectReference Include=\"..\\NetTools\\NetTools.csproj\">");
            projectWriter.WriteLine("      <Project>{5A9484FA-3316-4E96-9EAB-95AC997B40DA}</Project>");
            projectWriter.WriteLine("      <Name>NetTools</Name>");
            projectWriter.WriteLine("    </ProjectReference>");
            projectWriter.WriteLine("    <ProjectReference Include=\"..\\SmallArgon2d\\SmallArgon2d.csproj\">");
            projectWriter.WriteLine("      <Project>{60faf97d-1086-4753-9fa8-4d736b31d8b2}</Project>");
            projectWriter.WriteLine("      <Name>SmallArgon2d</Name>");
            projectWriter.WriteLine("    </ProjectReference>");
            projectWriter.WriteLine("  </ItemGroup>");
            projectWriter.WriteLine("  <Import Project=\"..\\packages\\CSHTML5.2.0.0-alpha23-036\\build\\CSHTML5.targets\" Condition=\"Exists('..\\packages\\CSHTML5.2.0.0-alpha23-036\\build\\CSHTML5.targets')\" />");
            projectWriter.WriteLine("</Project>");
        }

        private void GenerateObjectBaseInterface(string outputFolderName, string appNamespace)
        {
            string CSharpFileName = Path.Combine(outputFolderName, "Objects/IObjectBase.cs");

            using (FileStream CSharpFile = new FileStream(CSharpFileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (StreamWriter CSharpWriter = new StreamWriter(CSharpFile, Encoding.UTF8))
                {
                    GenerateObjectBaseInterface(outputFolderName, appNamespace, CSharpWriter);
                }
            }
        }

        private void GenerateObjectBaseInterface(string outputFolderName, string appNamespace, StreamWriter cSharpWriter)
        {
            cSharpWriter.WriteLine($"namespace {appNamespace}");
            cSharpWriter.WriteLine("{");
            cSharpWriter.WriteLine("    public interface IObjectBase");
            cSharpWriter.WriteLine("    {");
            cSharpWriter.WriteLine("        App GetApp { get; }");

            foreach (IGeneratorObject Object in Objects)
                if (Object.IsGlobal)
                    cSharpWriter.WriteLine($"        I{Object.CSharpName} Get{Object.CSharpName} {{ get; }}");

            if (Translation != null)
                cSharpWriter.WriteLine("        Translation GetTranslation { get; }");

            cSharpWriter.WriteLine("    }");
            cSharpWriter.WriteLine("}");
        }

        private void GenerateObjectBase(string outputFolderName, string appNamespace)
        {
            string CSharpFileName = Path.Combine(outputFolderName, "Objects/ObjectBase.cs");

            using (FileStream CSharpFile = new FileStream(CSharpFileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (StreamWriter CSharpWriter = new StreamWriter(CSharpFile, Encoding.UTF8))
                {
                    GenerateObjectBase(outputFolderName, appNamespace, CSharpWriter);
                }
            }
        }

        private void GenerateObjectBase(string outputFolderName, string appNamespace, StreamWriter cSharpWriter)
        {
            cSharpWriter.WriteLine($"namespace {appNamespace}");
            cSharpWriter.WriteLine("{");
            cSharpWriter.WriteLine("    public abstract class ObjectBase");
            cSharpWriter.WriteLine("    {");
            cSharpWriter.WriteLine("        public virtual App GetApp { get { return (App)App.Current; } }");

            foreach (IGeneratorObject Object in Objects)
                if (Object.IsGlobal)
                    cSharpWriter.WriteLine($"        public virtual I{Object.CSharpName} Get{Object.CSharpName} {{ get {{ return App.Get{Object.CSharpName}; }} }}");

            if (Translation != null)
                cSharpWriter.WriteLine("        public virtual Translation GetTranslation { get { return App.GetTranslation; } }");

            cSharpWriter.WriteLine("    }");
            cSharpWriter.WriteLine("}");
        }

        private void GeneratePageNames(string outputFolderName, string appNamespace)
        {
            string CSharpFileName = Path.Combine(outputFolderName, "Pages/PageNames.cs");

            using (FileStream CSharpFile = new FileStream(CSharpFileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (StreamWriter CSharpWriter = new StreamWriter(CSharpFile, Encoding.UTF8))
                {
                    GeneratePageNames(outputFolderName, appNamespace, CSharpWriter);
                }
            }
        }

        private void GeneratePageNames(string outputFolderName, string appNamespace, StreamWriter cSharpWriter)
        {
            cSharpWriter.WriteLine($"namespace {appNamespace}");
            cSharpWriter.WriteLine("{");
            cSharpWriter.WriteLine("    public enum PageNames");
            cSharpWriter.WriteLine("    {");
            cSharpWriter.WriteLine("        CurrentPage,");
            cSharpWriter.WriteLine("        PreviousPage,");

            foreach (IGeneratorPage Page in Pages)
                cSharpWriter.WriteLine($"        {Page.XamlName},");

            cSharpWriter.WriteLine("    }");
            cSharpWriter.WriteLine("}");
        }

        private void GeneratePageBaseInterface(string outputFolderName, string appNamespace)
        {
            string CSharpFileName = Path.Combine(outputFolderName, "Pages/IPageBase.cs");

            using (FileStream CSharpFile = new FileStream(CSharpFileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (StreamWriter CSharpWriter = new StreamWriter(CSharpFile, Encoding.UTF8))
                {
                    GeneratePageBaseInterface(outputFolderName, appNamespace, CSharpWriter);
                }
            }
        }

        private void GeneratePageBaseInterface(string outputFolderName, string appNamespace, StreamWriter cSharpWriter)
        {
            cSharpWriter.WriteLine($"namespace {appNamespace}");
            cSharpWriter.WriteLine("{");
            cSharpWriter.WriteLine("    public interface IPageBase : IObjectBase");
            cSharpWriter.WriteLine("    {");
            cSharpWriter.WriteLine("        PageNames ThisPage { get; }");
            cSharpWriter.WriteLine("    }");
            cSharpWriter.WriteLine("}");
        }

        private XmlnsContentPair ExtractResources(string name, string DesignResourceFile)
        {
            using (FileStream fs = new FileStream(DesignResourceFile, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
                {
                    return ExtractResources(name, sr);
                }
            }
        }

        private XmlnsContentPair ExtractResources(string name, StreamReader sr)
        {
            string Content = "";
            Dictionary<string, string> Xmlns = new Dictionary<string, string>();

            string Line;
            string Prolog = "";

            for (;;)
            {
                Line = sr.ReadLine();
                if (Line == null)
                    break;
                if (Line.Trim().EndsWith(">"))
                    break;

                Prolog += Line.Trim() + " ";
            }

            int XmlnsIndex = 0;
            int EndIndex;
            while ((XmlnsIndex = Prolog.IndexOf("xmlns:", XmlnsIndex)) >= 0 && (EndIndex = Prolog.IndexOf("=", XmlnsIndex)) > XmlnsIndex)
            {
                int SeparatorIndex = Prolog.IndexOf(" ", EndIndex);
                if (SeparatorIndex < 0)
                    SeparatorIndex = Prolog.IndexOf(">", EndIndex);

                if (SeparatorIndex > EndIndex + 1)
                {
                    string Prefix = Prolog.Substring(XmlnsIndex + 6, EndIndex - XmlnsIndex - 6);
                    string Namespace = Prolog.Substring(EndIndex + 1, SeparatorIndex - EndIndex - 1);

                    Namespace = Namespace.Replace("XmlnsTest", "CSharpXamlForHtml5");

                    Xmlns.Add(Prefix, Namespace);

                    XmlnsIndex = SeparatorIndex + 1;
                }
                else
                    XmlnsIndex = EndIndex + 1;
            }

            bool IsFirstSeparatingLine = true;
            bool HasSeparatingLine = false;

            for (;;)
            {
                Line = sr.ReadLine();
                if (Line == null)
                    break;
                if (Line.Trim().StartsWith("</ResourceDictionary>"))
                    break;

                if (Line.Trim().Length == 0)
                    HasSeparatingLine = true;
                else
                {
                    if (HasSeparatingLine)
                    {
                        HasSeparatingLine = false;

                        if (IsFirstSeparatingLine)
                            IsFirstSeparatingLine = false;
                        else
                            Content += "\r\n";
                    }

                    Line = ReplaceTypeSyntax(Line, "x:Key=\"", name, true);
                    Line = ReplaceTypeSyntax(Line, "TargetType=\"", "", false);
                    Line = ReplaceTypeSyntax(Line, "BasedOn=\"{StaticResource ", name, true);

                    Content += "    " + Line.TrimEnd() + "\r\n";
                }
            }

            return new XmlnsContentPair() { Xmlns = Xmlns, Content = Content };
        }

        private string ReplaceTypeSyntax(string line, string prefix, string replace, bool renameType)
        {
            int TypeIndex = 0;
            while ((TypeIndex = line.IndexOf($"{prefix}{{x:Type ", TypeIndex)) >= 0)
            {
                int SeparatorIndex = line.IndexOf("}", TypeIndex);
                if (SeparatorIndex > TypeIndex + 8 + prefix.Length)
                {
                    string TypeName = line.Substring(TypeIndex + 8 + prefix.Length, SeparatorIndex - TypeIndex - 8 - prefix.Length);

                    if (renameType)
                        TypeName = ParserDomain.StyleTypeConverter(TypeName);

                    line = line.Substring(0, TypeIndex) + $"{prefix}{replace}" + TypeName + line.Substring(SeparatorIndex + 1);
                }
                else
                    break;
            }

            return line;
        }

        public void CopyAssemblyInfo(string outputFolderName)
        {
            string ObjectsInputFolderName = Path.Combine(InputFolderName, "object");
            string ObjectsOutputFolderName = Path.Combine(outputFolderName, "Properties");

            if (!Directory.Exists(ObjectsOutputFolderName))
                Directory.CreateDirectory(ObjectsOutputFolderName);

            string InputCSharpFileName = Path.Combine(ObjectsInputFolderName, "AssemblyInfo.cs");
            string OutputCSharpFileName = Path.Combine(ObjectsOutputFolderName, "AssemblyInfo.cs");

            DateTime InputWriteTime;
            if (File.Exists(InputCSharpFileName))
                InputWriteTime = File.GetLastWriteTimeUtc(InputCSharpFileName);
            else
                InputWriteTime = DateTime.MinValue;

            DateTime OutputWriteTime;
            if (File.Exists(OutputCSharpFileName))
                OutputWriteTime = File.GetLastWriteTimeUtc(OutputCSharpFileName);
            else
                OutputWriteTime = DateTime.MinValue;

            if (InputWriteTime > OutputWriteTime)
                File.Copy(InputCSharpFileName, OutputCSharpFileName, true);
        }

        public void CopySideFiles(string outputFolderName, string fileName)
        {
            string ObjectsInputFolderName = Path.Combine(InputFolderName, "object");
            string ObjectsOutputFolderName = outputFolderName;

            if (!Directory.Exists(ObjectsOutputFolderName))
                Directory.CreateDirectory(ObjectsOutputFolderName);

            string InputFileName = Path.Combine(ObjectsInputFolderName, fileName);
            string OutputFileName = Path.Combine(ObjectsOutputFolderName, fileName);

            DateTime InputWriteTime;
            if (File.Exists(InputFileName))
                InputWriteTime = File.GetLastWriteTimeUtc(InputFileName);
            else
                InputWriteTime = DateTime.MinValue;

            DateTime OutputWriteTime;
            if (File.Exists(OutputFileName))
                OutputWriteTime = File.GetLastWriteTimeUtc(OutputFileName);
            else
                OutputWriteTime = DateTime.MinValue;

            if (InputWriteTime > OutputWriteTime)
                File.Copy(InputFileName, OutputFileName, true);
        }

        public void CopyEnumFile(string outputFolderName, string EnumTypeName)
        {
            string ObjectsInputFolderName = Path.Combine(InputFolderName, "object");
            string ObjectsOutputFolderName = Path.Combine(outputFolderName, "Objects");

            if (!Directory.Exists(ObjectsOutputFolderName))
                Directory.CreateDirectory(ObjectsOutputFolderName);

            string InputCSharpFileName = Path.Combine(ObjectsInputFolderName, $"{EnumTypeName}s.cs");
            string OutputCSharpFileName = Path.Combine(ObjectsOutputFolderName, $"{EnumTypeName}s.cs");

            DateTime InputWriteTime;
            if (File.Exists(InputCSharpFileName))
                InputWriteTime = File.GetLastWriteTimeUtc(InputCSharpFileName);
            else
                InputWriteTime = DateTime.MinValue;

            DateTime OutputWriteTime;
            if (File.Exists(OutputCSharpFileName))
                OutputWriteTime = File.GetLastWriteTimeUtc(OutputCSharpFileName);
            else
                OutputWriteTime = DateTime.MinValue;

            if (InputWriteTime > OutputWriteTime)
                File.Copy(InputCSharpFileName, OutputCSharpFileName, true);
        }

        public static string GetFilePath(string outputFolderName, string name)
        {
            string XamlFolderName = Path.Combine(outputFolderName, "Design");
            string XamlFileName = Path.Combine(XamlFolderName, name + ".xaml");

            XamlFileName = XamlFileName.Replace("\\", "/");

            return XamlFileName;
        }
    }
}
