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
            FileName = dynamic.FileName;
            XamlPageName = dynamic.XamlPageName;
            BaseDynamic = dynamic;

            GeneratorDynamicPropertyCollection DynamicPropertyList = new GeneratorDynamicPropertyCollection();
            foreach (IDynamicProperty DynamicProperty in dynamic.Properties)
                DynamicPropertyList.Add(new GeneratorDynamicProperty(DynamicProperty));

            Properties = DynamicPropertyList.AsReadOnly();

            GeneratorDynamicMap.Add(dynamic, this);
        }

        private IDynamic BaseDynamic;

        public string Name { get; private set; }
        public string FileName { get; private set; }
        public string XamlPageName { get; private set; }
        public IReadOnlyCollection<IGeneratorDynamicProperty> Properties { get; private set; }

        public bool HasProperties
        {
            get { return Properties.Count > 0; }
        }

        public bool Connect(IGeneratorDomain domain)
        {
            bool IsConnected = false;

            foreach (IGeneratorDynamicProperty DynamicProperty in Properties)
                IsConnected |= DynamicProperty.Connect(domain);

            return IsConnected;
        }

        public void Generate(IGeneratorDomain domain, string outputFolderName, string appNamespace)
        {
            if (!HasProperties)
                return;

            string ObjectsFolderName = Path.Combine(outputFolderName, "Dynamics");

            if (!Directory.Exists(ObjectsFolderName))
                Directory.CreateDirectory(ObjectsFolderName);

            string CSharpFileName = Path.Combine(ObjectsFolderName, $"{FileName}.cs");

            using (FileStream CSharpFile = new FileStream(CSharpFileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (StreamWriter CSharpWriter = new StreamWriter(CSharpFile, Encoding.UTF8))
                {
                    Generate(domain, appNamespace, CSharpWriter);
                }
            }
        }

        private void Generate(IGeneratorDomain domain, string appNamespace, StreamWriter cSharpWriter)
        {
            Dictionary<IGeneratorObject, List<IGeneratorObjectProperty>> UsedObjectTable = new Dictionary<IGeneratorObject, List<IGeneratorObjectProperty>>();
            foreach (IGeneratorDynamicProperty DynamicProperty in Properties)
                DynamicProperty.GetUsedObjects(UsedObjectTable);

            cSharpWriter.WriteLine("using System.ComponentModel;");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine($"namespace {appNamespace}");
            cSharpWriter.WriteLine("{");
            cSharpWriter.WriteLine($"    public class {XamlPageName}Dynamic : INotifyPropertyChanged");
            cSharpWriter.WriteLine("    {");
            cSharpWriter.WriteLine($"        public {XamlPageName}Dynamic({XamlPageName} page)");
            cSharpWriter.WriteLine("        {");
            cSharpWriter.WriteLine("            Page = page;");

            foreach (KeyValuePair<IGeneratorObject, List<IGeneratorObjectProperty>> Entry in UsedObjectTable)
                cSharpWriter.WriteLine($"            Page.{Entry.Key.CSharpName}.PropertyChanged += On{Entry.Key.CSharpName}PropertyChanged;");

            cSharpWriter.WriteLine("        }");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine($"        public {XamlPageName} Page {{ get; private set; }}");

            foreach (KeyValuePair<IGeneratorObject, List<IGeneratorObjectProperty>> Entry in UsedObjectTable)
            {
                string ObjectName = Entry.Key.CSharpName;

                cSharpWriter.WriteLine();
                cSharpWriter.WriteLine($"        public void On{ObjectName }PropertyChanged(object sender, PropertyChangedEventArgs e)");
                cSharpWriter.WriteLine("        {");
                cSharpWriter.WriteLine($"            OnPropertyChanged($\"{{nameof({ObjectName })}}.{{e.PropertyName}}\");");
                cSharpWriter.WriteLine("        }");
            }

            cSharpWriter.WriteLine();

            foreach (IGeneratorDynamicProperty DynamicProperty in Properties)
                DynamicProperty.Generate(cSharpWriter);

            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine($"        public void OnPropertyChanged(string propertyName)");
            cSharpWriter.WriteLine("        {");

            foreach (KeyValuePair<IGeneratorObject, List<IGeneratorObjectProperty>> Entry in UsedObjectTable)
            {
                string ObjectName = Entry.Key.CSharpName;

                foreach (IGeneratorObjectProperty ObjectProperty in Entry.Value)
                {
                    string ObjectPropertyName = ObjectProperty.CSharpName;

                    cSharpWriter.WriteLine($"            if (propertyName == $\"{{nameof({ObjectName})}}.{{nameof({ObjectName}.{ObjectPropertyName})}}\")");
                    cSharpWriter.WriteLine("            {");

                    foreach (IGeneratorDynamicProperty DynamicProperty in Properties)
                        DynamicProperty.GenerateNotification(Entry.Key, ObjectProperty, XamlPageName, cSharpWriter);

                    cSharpWriter.WriteLine("            }");
                }
            }

            cSharpWriter.WriteLine("        }");
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
