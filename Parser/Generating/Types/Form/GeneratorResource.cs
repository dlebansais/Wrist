using System.Collections.Generic;
using System.IO;

namespace Parser
{
    public class GeneratorResource : IGeneratorResource
    {
        public static Dictionary<IResource, IGeneratorResource> GeneratorResourceMap { get; } = new Dictionary<IResource, IGeneratorResource>();

        public GeneratorResource(IResource resource)
        {
            Name = resource.Name;
            XamlName = resource.XamlName;
            FilePath = resource.FilePath;
            Width = resource.Width;
            Height = resource.Height;

            GeneratorResourceMap.Add(resource, this);
        }

        public string Name { get; private set; }
        public string XamlName { get; private set; }
        public string FilePath { get; private set; }
        public double Width { get; private set; }
        public double Height { get; private set; }

        public bool Connect(IGeneratorDomain domain)
        {
            return false;
        }

        public void GenerateResourceLine(StreamWriter xamlWriter)
        {
            xamlWriter.WriteLine($"        <BitmapImage x:Key=\"{XamlName}\" UriSource=\"/resources/{Name}.png\"/>");
        }

        public string GetResourceReference()
        {
            return $"{{StaticResource {XamlName}}}";
        }

        public void Generate(IGeneratorDomain domain, string outputFolderName)
        {
            string ResourcesFolderName = Path.Combine(outputFolderName, "Resources");

            if (!Directory.Exists(ResourcesFolderName))
                Directory.CreateDirectory(ResourcesFolderName);

            string DestinationFileName = Path.Combine(ResourcesFolderName, $"{Name}.png");
            File.Copy(FilePath, DestinationFileName, true);
        }

        public override string ToString()
        {
            return $"{GetType().Name} '{Name}'";
        }
    }
}
