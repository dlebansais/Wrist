using System.Collections.Generic;
using System.IO;

namespace Parser
{
    public class GeneratorObjectEvent : IGeneratorObjectEvent
    {
        public static Dictionary<IObjectEvent, IGeneratorObjectEvent> GeneratorObjectEventMap { get; } = new Dictionary<IObjectEvent, IGeneratorObjectEvent>();

        public static IGeneratorObjectEvent Convert(IObjectEvent eventObject)
        {
            IGeneratorObjectEvent Result;

            Result = new GeneratorObjectEvent(eventObject);

            GeneratorObjectEventMap.Add(eventObject, Result);
            return Result;
        }

        public GeneratorObjectEvent(IObjectEvent eventObject)
        {
            NameSource = eventObject.NameSource;
            CSharpName = eventObject.CSharpName;
            IsProvidingCustomPageName = eventObject.IsProvidingCustomPageName.HasValue && eventObject.IsProvidingCustomPageName.Value;
        }

        public IDeclarationSource NameSource { get; private set; }
        public string CSharpName { get; private set; }
        public bool IsProvidingCustomPageName { get; set; }

        public override string ToString()
        {
            return $"{NameSource.Name} event";
        }

        public void Generate(IGeneratorDomain domain, StreamWriter cSharpWriter)
        {
            if (IsProvidingCustomPageName)
                cSharpWriter.WriteLine($"        void On_{CSharpName}(PageNames pageName, string sourceName, string sourceContent, out PageNames destinationPageName);");
            else
                cSharpWriter.WriteLine($"        void On_{CSharpName}(PageNames pageName, string sourceName, string sourceContent);");
        }
    }
}
