using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Parser
{
    public class GeneratorDynamic : IGeneratorDynamic
    {
        public static Dictionary<IDynamic, IGeneratorDynamic> GeneratorDynamicMap { get; } = new Dictionary<IDynamic, IGeneratorDynamic>();

        public GeneratorDynamic(IDynamic dynamic)
        {
            Name = dynamic.Name;
            CSharpName = dynamic.CSharpName;
            BaseDynamic = dynamic;

            GeneratorDynamicPropertyCollection DynamicPropertyList = new GeneratorDynamicPropertyCollection();
            foreach (IDynamicProperty DynamicProperty in dynamic.Properties)
                DynamicPropertyList.Add(new GeneratorDynamicProperty(DynamicProperty));

            Properties = DynamicPropertyList.AsReadOnly();

            GeneratorDynamicMap.Add(dynamic, this);
        }

        private IDynamic BaseDynamic;

        public string Name { get; private set; }
        public string CSharpName { get; private set; }
        public IReadOnlyCollection<IGeneratorDynamicProperty> Properties { get; private set; }

        public bool Connect(IGeneratorDomain domain)
        {
            bool IsConnected = false;

            return IsConnected;
        }

        public void Generate(IGeneratorDomain domain, string outputFolderName, string appNamespace)
        {
            string ObjectsFolderName = Path.Combine(outputFolderName, "Dynamics");

            if (!Directory.Exists(ObjectsFolderName))
                Directory.CreateDirectory(ObjectsFolderName);

            string CSharpFileName = Path.Combine(ObjectsFolderName, $"{CSharpName}Dynamic.cs");

            using (FileStream CSharpFile = new FileStream(CSharpFileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (StreamWriter CSharpWriter = new StreamWriter(CSharpFile, Encoding.UTF8))
                {
                    GenerateInterface(domain, appNamespace, CSharpWriter);
                }
            }
        }

        private void GenerateInterface(IGeneratorDomain domain, string appNamespace, StreamWriter cSharpWriter)
        {
            cSharpWriter.WriteLine("using System.ComponentModel;");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine($"namespace {appNamespace}");
            cSharpWriter.WriteLine("{");
            cSharpWriter.WriteLine($"    public class {CSharpName}Dynamic : INotifyPropertyChanged");
            cSharpWriter.WriteLine("    {");
            cSharpWriter.WriteLine($"        public {CSharpName}Dynamic({CSharpName} page)");
            cSharpWriter.WriteLine("        {");
            cSharpWriter.WriteLine("            Page = page;");
            cSharpWriter.WriteLine("            Page.Login.PropertyChanged += OnLoginPropertyChanged;");
            cSharpWriter.WriteLine("        }");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine($"        public {CSharpName} Page {{ get; private set; }}");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine($"        private void OnLoginPropertyChanged(object sender, PropertyChangedEventArgs e)");
            cSharpWriter.WriteLine("        {");
            cSharpWriter.WriteLine("            OnPropertyChanged($\"{nameof(Login)}.{e.PropertyName}\");");
            cSharpWriter.WriteLine("        }");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine($"        private void OnPropertyChanged(string propertyName)");
            cSharpWriter.WriteLine("        {");
            cSharpWriter.WriteLine("            if (propertyName == $\"{nameof(Login)}.{nameof(Login.Name)}\")");
            cSharpWriter.WriteLine("            {");
            cSharpWriter.WriteLine("                IsNameNotEmpty = !(string.IsNullOrEmpty(Page.Login.Name));");
            cSharpWriter.WriteLine("                NotifyPropertyChanged(nameof(IsNameNotEmpty));");
            cSharpWriter.WriteLine("            }");
            cSharpWriter.WriteLine("        }");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine("        public bool IsNameNotEmpty { get; private set; }");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine("        public event PropertyChangedEventHandler PropertyChanged;");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine("        internal void NotifyPropertyChanged(string propertyName)");
            cSharpWriter.WriteLine("        {");
            cSharpWriter.WriteLine("            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));");
            cSharpWriter.WriteLine("        }");
            cSharpWriter.WriteLine("    }");
            cSharpWriter.WriteLine("}");
        }

        public void Generate(IGeneratorDomain domain, string outputFolderName)
        {
        }

        public override string ToString()
        {
            return $"{GetType().Name} '{Name}'";
        }
    }
}
