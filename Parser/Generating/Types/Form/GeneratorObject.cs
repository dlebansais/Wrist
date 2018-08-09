using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Parser
{
    public class GeneratorObject : IGeneratorObject
    {
        public static Dictionary<IObject, IGeneratorObject> GeneratorObjectMap { get; } = new Dictionary<IObject, IGeneratorObject>();
        public static GeneratorObject TranslationObject = new GeneratorObject(Object.TranslationObject.Name);

        private GeneratorObject(string name)
        {
            Name = name;

            List<IGeneratorObjectProperty> LocalProperties = new List<IGeneratorObjectProperty>() { GeneratorObjectPropertyStringDictionary.StringsProperty };
            Properties = LocalProperties.AsReadOnly();

            GeneratorObjectMap.Add(Object.TranslationObject, this);
        }

        public GeneratorObject(IObject obj)
        {
            Name = obj.Name;
            CSharpName = obj.CSharpName;
            States = obj.States;

            IGeneratorObjectPropertyCollection ConvertedProperties = new GeneratorObjectPropertyCollection();
            foreach (IObjectProperty Property in obj.Properties)
                ConvertedProperties.Add(GeneratorObjectProperty.Convert(Property, this));
            Properties = ConvertedProperties.AsReadOnly();

            List<IGeneratorTransition> ConvertedTransitions = new List<IGeneratorTransition>();
            foreach (ITransition Transition in obj.Transitions)
                ConvertedTransitions.Add(new GeneratorTransition(Transition));
            Transitions = ConvertedTransitions;

            List<IGeneratorObjectEvent> ConvertedEvents = new List<IGeneratorObjectEvent>();
            foreach (IObjectEvent Event in obj.Events)
                ConvertedEvents.Add(GeneratorObjectEvent.Convert(Event));
            Events = ConvertedEvents;

            GeneratorObjectMap.Add(obj, this);
        }

        public string Name { get; private set; }
        public string CSharpName { get; private set; }
        public IReadOnlyCollection<string> States { get; private set; }
        public IReadOnlyCollection<IGeneratorObjectProperty> Properties { get; private set; }
        public IReadOnlyCollection<IGeneratorTransition> Transitions { get; private set; }
        public IReadOnlyCollection<IGeneratorObjectEvent> Events { get; private set; }

        public bool Connect(IGeneratorDomain domain)
        {
            bool IsConnected = false;

            foreach (IGeneratorObjectProperty Property in Properties)
                IsConnected |= Property.Connect(domain);

            return IsConnected;
        }

        public void Generate(IGeneratorDomain domain, string outputFolderName, string appNamespace)
        {
            GenerateInterface(domain, outputFolderName, appNamespace);
            GenerateStates(domain, outputFolderName, appNamespace);
            CopyImplementation(domain, outputFolderName, appNamespace);
        }

        public void GenerateInterface(IGeneratorDomain domain, string outputFolderName, string appNamespace)
        {
            string ObjectsFolderName = Path.Combine(outputFolderName, "Objects");

            if (!Directory.Exists(ObjectsFolderName))
                Directory.CreateDirectory(ObjectsFolderName);

            string CSharpFileName = Path.Combine(ObjectsFolderName, $"I{CSharpName}.cs");

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
            cSharpWriter.WriteLine("using System.Collections.Generic;");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine($"namespace {appNamespace}");
            cSharpWriter.WriteLine("{");
            cSharpWriter.WriteLine($"    public interface I{CSharpName}");
            cSharpWriter.WriteLine("    {");

            foreach (IGeneratorObjectProperty Property in Properties)
                Property.Generate(domain, cSharpWriter);

            foreach (IGeneratorObjectEvent Event in Events)
                Event.Generate(domain, cSharpWriter);

            cSharpWriter.WriteLine("    }");
            cSharpWriter.WriteLine("}");
        }

        public void GenerateStates(IGeneratorDomain domain, string outputFolderName, string appNamespace)
        {
            string CSharpFileName = Path.Combine(outputFolderName, $"Objects/{CSharpName}States.cs");

            using (FileStream CSharpFile = new FileStream(CSharpFileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (StreamWriter CSharpWriter = new StreamWriter(CSharpFile, Encoding.UTF8))
                {
                    GenerateStates(domain, appNamespace, CSharpWriter);
                }
            }
        }

        private void GenerateStates(IGeneratorDomain domain, string appNamespace, StreamWriter cSharpWriter)
        {
            cSharpWriter.WriteLine($"namespace {appNamespace}");
            cSharpWriter.WriteLine("{");
            cSharpWriter.WriteLine($"    public enum {CSharpName}States");
            cSharpWriter.WriteLine("    {");

            foreach (string State in States)
                cSharpWriter.WriteLine($"        {State},");

            cSharpWriter.WriteLine("    }");
            cSharpWriter.WriteLine("}");
        }

        public void CopyImplementation(IGeneratorDomain domain, string outputFolderName, string appNamespace)
        {
            string ObjectsInputFolderName = Path.Combine(domain.InputFolderName, "object");
            string ObjectsOutputFolderName = Path.Combine(outputFolderName, "Objects");

            if (!Directory.Exists(ObjectsOutputFolderName))
                Directory.CreateDirectory(ObjectsOutputFolderName);

            string InputCSharpFileName = Path.Combine(ObjectsInputFolderName, Path.Combine(Name, $"{CSharpName}.cs"));
            string OutputCSharpFileName = Path.Combine(ObjectsOutputFolderName, $"{CSharpName}.cs");

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

        public override string ToString()
        {
            return $"{GetType().Name} '{Name}'";
        }
    }
}
