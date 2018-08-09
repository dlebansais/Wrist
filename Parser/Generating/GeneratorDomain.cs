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

            ColorSchemes = new List<IGeneratorColorScheme>();
            foreach (IColorScheme ColorScheme in domain.ColorSchemes)
                ColorSchemes.Add(new GeneratorColorScheme(ColorScheme));

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
            }
            while (IsConnected);

            HomePage = GeneratorPage.GeneratorPageMap[domain.HomePage];
            SelectedColorScheme = GeneratorColorScheme.GeneratorColorSchemeMap[domain.SelectedColorScheme];
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
        public List<IGeneratorColorScheme> ColorSchemes { get; private set; }
        public IGeneratorPage HomePage { get; private set; }
        public IGeneratorColorScheme SelectedColorScheme { get; private set; }

        public void Generate(string outputFolderName)
        {
            if (!Directory.Exists(outputFolderName))
                Directory.CreateDirectory(outputFolderName);

            string AppNamespace = Path.GetFileName(outputFolderName);

            foreach (IGeneratorPage Page in Pages)
                Page.Generate(this, outputFolderName, AppNamespace, SelectedColorScheme);

            foreach (IGeneratorObject Object in Objects)
                Object.Generate(this, outputFolderName, AppNamespace);

            foreach (IGeneratorResource Resource in Resources)
                Resource.Generate(this, outputFolderName);

            GenerateAppXaml(outputFolderName, AppNamespace, SelectedColorScheme);
            GenerateAppCSharp(outputFolderName, AppNamespace);
            GenerateAppProject(outputFolderName, AppNamespace);
        }

        private void GenerateAppXaml(string outputFolderName, string appNamespace, IGeneratorColorScheme colorScheme)
        {
            string XamlFileName = Path.Combine(outputFolderName, "App.xaml");

            using (FileStream XamlFile = new FileStream(XamlFileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (StreamWriter XamlWriter = new StreamWriter(XamlFile, Encoding.UTF8))
                {
                    GenerateAppXaml(outputFolderName, appNamespace, XamlWriter, colorScheme);
                }
            }
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

        private void GenerateAppXaml(string outputFolderName, string appNamespace, StreamWriter xamlWriter, IGeneratorColorScheme colorScheme)
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

            if (LineList.Count > 0)
                LineList[LineList.Count - 1] = LineList[LineList.Count - 1] + ">";

            foreach (string Line in LineList)
                xamlWriter.WriteLine(Line);
            xamlWriter.WriteLine("    <Application.Resources>");

            foreach (XmlnsContentPair Resource in ResourceList)
                colorScheme.WriteXamlLine(xamlWriter, Resource.Content);

            xamlWriter.WriteLine("    </Application.Resources>");
            xamlWriter.WriteLine("</Application>");
        }

        private void GenerateAppCSharp(string outputFolderName, string appNamespace, StreamWriter cSharpWriter)
        {
            cSharpWriter.WriteLine("using Presentation;");
            cSharpWriter.WriteLine("using Windows.UI.Xaml;");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine($"namespace {appNamespace}");
            cSharpWriter.WriteLine("{");
            cSharpWriter.WriteLine($"    public sealed partial class App : Application");
            cSharpWriter.WriteLine("    {");
            cSharpWriter.WriteLine($"        public App()");
            cSharpWriter.WriteLine("        {");
            cSharpWriter.WriteLine("            InitializeComponent();");
            cSharpWriter.WriteLine($"            GoTo(Persistent.GetValue(\"page\", \"{HomePage.XamlName}\"));");
            cSharpWriter.WriteLine("        }");
            cSharpWriter.WriteLine();

            foreach (IGeneratorObject Object in Objects)
                cSharpWriter.WriteLine($"        public static {Object.CSharpName} {Object.CSharpName} {{ get; }} = new {Object.CSharpName}();");

            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine("        public void GoTo(string pageName)");
            cSharpWriter.WriteLine("        {");
            cSharpWriter.WriteLine($"            if (pageName == null)");
            cSharpWriter.WriteLine($"                return;");

            for (int i = 0; i < Pages.Count; i++)
            {
                IGeneratorPage Page = Pages[i];

                if (Page != HomePage)
                {
                    cSharpWriter.WriteLine($"            else if (pageName == \"{Page.Name}\")");
                    cSharpWriter.WriteLine($"                Window.Current.Content = new {Page.XamlName}();");
                }
            }

            cSharpWriter.WriteLine($"            else");
            cSharpWriter.WriteLine("            {");
            cSharpWriter.WriteLine($"                pageName = \"{HomePage.Name}\";");
            cSharpWriter.WriteLine($"                Window.Current.Content = new {HomePage.XamlName}();");
            cSharpWriter.WriteLine("            }");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine($"            Persistent.SetValue(\"page\", pageName);");
            cSharpWriter.WriteLine("        }");
            cSharpWriter.WriteLine("    }");
            cSharpWriter.WriteLine("}");
        }

        private void GenerateAppProject(string outputFolderName, string appNamespace)
        {
            string ProjectFileName = Path.Combine(outputFolderName, $"{appNamespace}.csproj");

            using (FileStream ProjectFile = new FileStream(ProjectFileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (StreamWriter ProjectWriter = new StreamWriter(ProjectFile, Encoding.UTF8))
                {
                    GenerateAppProject(outputFolderName, appNamespace, ProjectWriter);
                }
            }
        }

        private void GenerateAppProject(string outputFolderName, string appNamespace, StreamWriter projectWriter)
        {
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
            projectWriter.WriteLine($"    <AssemblyName>{appNamespace}</AssemblyName>");
            projectWriter.WriteLine("    <TargetFrameworkVersion>v4.5</TargetFrameworkVersion>");
            projectWriter.WriteLine("    <FileAlignment>512</FileAlignment>");
            projectWriter.WriteLine("    <IsCSharpXamlForHtml5>true</IsCSharpXamlForHtml5>");
            projectWriter.WriteLine("    <CSharpXamlForHtml5OutputType>Application</CSharpXamlForHtml5OutputType>");
            projectWriter.WriteLine("    <StartAction>Program</StartAction>");
            projectWriter.WriteLine("    <StartProgram>$(MSBuildProgramFiles32)\\MSBuild\\CSharpXamlForHtml5\\InternalStuff\\Simulator\\CSharpXamlForHtml5.Simulator.exe</StartProgram>");
            projectWriter.WriteLine($"    <StartArguments>\"{appNamespace}.dll\"</StartArguments>");
            projectWriter.WriteLine("  </PropertyGroup>");
            projectWriter.WriteLine("  <PropertyGroup Condition=\" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' \">");
            projectWriter.WriteLine("    <DebugSymbols>true</DebugSymbols>");
            projectWriter.WriteLine("    <DebugType>full</DebugType>");
            projectWriter.WriteLine("    <Optimize>false</Optimize>");
            projectWriter.WriteLine("    <OutputPath>bin\\Debug\\</OutputPath>");
            projectWriter.WriteLine("    <DefineConstants>DEBUG;TRACE;CSHARP_XAML_FOR_HTML5;CSHTML5</DefineConstants>");
            projectWriter.WriteLine("    <ErrorReport>prompt</ErrorReport>");
            projectWriter.WriteLine("    <WarningLevel>4</WarningLevel>");
            projectWriter.WriteLine("  </PropertyGroup>");
            projectWriter.WriteLine("  <PropertyGroup Condition=\" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' \">");
            projectWriter.WriteLine("    <DebugType>pdbonly</DebugType>");
            projectWriter.WriteLine("    <Optimize>true</Optimize>");
            projectWriter.WriteLine("    <OutputPath>bin\\Release\\</OutputPath>");
            projectWriter.WriteLine("    <DefineConstants>TRACE;CSHARP_XAML_FOR_HTML5;CSHTML5</DefineConstants>");
            projectWriter.WriteLine("    <ErrorReport>prompt</ErrorReport>");
            projectWriter.WriteLine("    <WarningLevel>4</WarningLevel>");
            projectWriter.WriteLine("  </PropertyGroup>");
            projectWriter.WriteLine("  <ItemGroup>");
            projectWriter.WriteLine("    <Reference Include=\"CSharpXamlForHtml5\">");
            projectWriter.WriteLine("      <HintPath>$(MSBuildProgramFiles32)\\MSBuild\\CSharpXamlForHtml5\\AssembliesToReference\\CSharpXamlForHtml5.dll</HintPath>");
            projectWriter.WriteLine("    </Reference>");
            projectWriter.WriteLine("    <Reference Include=\"CSharpXamlForHtml5.System.dll\">");
            projectWriter.WriteLine("      <HintPath>$(MSBuildProgramFiles32)\\MSBuild\\CSharpXamlForHtml5\\AssembliesToReference\\CSharpXamlForHtml5.System.dll.dll</HintPath>");
            projectWriter.WriteLine("    </Reference>");
            projectWriter.WriteLine("    <Reference Include=\"CSharpXamlForHtml5.System.Runtime.Serialization.dll\">");
            projectWriter.WriteLine("      <HintPath>$(MSBuildProgramFiles32)\\MSBuild\\CSharpXamlForHtml5\\AssembliesToReference\\CSharpXamlForHtml5.System.Runtime.Serialization.dll.dll</HintPath>");
            projectWriter.WriteLine("    </Reference>");
            projectWriter.WriteLine("    <Reference Include=\"CSharpXamlForHtml5.System.ServiceModel.dll\">");
            projectWriter.WriteLine("      <HintPath>$(MSBuildProgramFiles32)\\MSBuild\\CSharpXamlForHtml5\\AssembliesToReference\\CSharpXamlForHtml5.System.ServiceModel.dll.dll</HintPath>");
            projectWriter.WriteLine("    </Reference>");
            projectWriter.WriteLine("    <Reference Include=\"CSharpXamlForHtml5.System.Xaml.dll\">");
            projectWriter.WriteLine("      <HintPath>$(MSBuildProgramFiles32)\\MSBuild\\CSharpXamlForHtml5\\AssembliesToReference\\CSharpXamlForHtml5.System.Xaml.dll.dll</HintPath>");
            projectWriter.WriteLine("    </Reference>");
            projectWriter.WriteLine("    <Reference Include=\"CSharpXamlForHtml5.System.Xml.dll\">");
            projectWriter.WriteLine("      <HintPath>$(MSBuildProgramFiles32)\\MSBuild\\CSharpXamlForHtml5\\AssembliesToReference\\CSharpXamlForHtml5.System.Xml.dll.dll</HintPath>");
            projectWriter.WriteLine("    </Reference>");
            projectWriter.WriteLine("    <Reference Include=\"JSIL.Meta\">");
            projectWriter.WriteLine("      <HintPath>$(MSBuildProgramFiles32)\\MSBuild\\CSharpXamlForHtml5\\AssembliesToReference\\JSIL.Meta.dll</HintPath>");
            projectWriter.WriteLine("    </Reference>");
            projectWriter.WriteLine("    <Reference Include=\"Microsoft.CSharp\"/>");
            projectWriter.WriteLine("  </ItemGroup>");
            projectWriter.WriteLine("  <ItemGroup>");
            projectWriter.WriteLine("    <Compile Include=\"App.xaml.cs\">");
            projectWriter.WriteLine("      <DependentUpon>App.xaml</DependentUpon>");
            projectWriter.WriteLine("    </Compile>");
            projectWriter.WriteLine("    <Compile Include=\"Properties\\AssemblyInfo.cs\"/>");
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
            }

            projectWriter.WriteLine("  </ItemGroup>");
            projectWriter.WriteLine("  <ItemGroup>");

            foreach (IGeneratorObject Object in Objects)
            {
                projectWriter.WriteLine($"    <Compile Include=\"Objects\\I{Object.CSharpName}.cs\"/>");
                projectWriter.WriteLine($"    <Compile Include=\"Objects\\{Object.CSharpName}.cs\"/>");
                projectWriter.WriteLine($"    <Compile Include=\"Objects\\{Object.CSharpName}States.cs\"/>");
            }

            projectWriter.WriteLine("  </ItemGroup>");
            projectWriter.WriteLine("  <ItemGroup>");

            foreach (IGeneratorResource Resource in Resources)
                projectWriter.WriteLine($"    <EmbeddedResource Include=\"Resources\\{Resource.Name}.png\"/>");

            projectWriter.WriteLine("  </ItemGroup>");
            projectWriter.WriteLine("  <ItemGroup>");
            projectWriter.WriteLine("    <ProjectReference Include=\"..\\Presentation\\Presentation.csproj\">");
            projectWriter.WriteLine("      <Project>{c067b9b7-a2de-45f9-af05-beb0a5e70bef}</Project>");
            projectWriter.WriteLine("      <Name>Presentation</Name>");
            projectWriter.WriteLine("    </ProjectReference>");
            projectWriter.WriteLine("    <ProjectReference Include=\"..\\Database\\Database.csproj\">");
            projectWriter.WriteLine("      <Project>{5A9484FA-3316-4E96-9EAB-95AC997B40DA}</Project>");
            projectWriter.WriteLine("      <Name>Database</Name>");
            projectWriter.WriteLine("    </ProjectReference>");
            projectWriter.WriteLine("  </ItemGroup>");
            projectWriter.WriteLine("  <Import Project=\"$(MSBuildProgramFiles32)\\MSBuild\\CSharpXamlForHtml5\\InternalStuff\\Targets\\CSharpXamlForHtml5.Build.targets\"/>");
            projectWriter.WriteLine("</Project>");
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
                    {
                        if (TypeName == "TextBlock")
                            TypeName = "Text";
                        else if (TypeName == "TextBox")
                            TypeName = "Edit";
                        else if (TypeName == "ListBox")
                            TypeName = "Selector";
                    }

                    line = line.Substring(0, TypeIndex) + $"{prefix}{replace}" + TypeName + line.Substring(SeparatorIndex + 1);
                }
                else
                    break;
            }

            return line;
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
