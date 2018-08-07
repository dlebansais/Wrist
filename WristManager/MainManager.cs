using Parser;
using System.IO;

namespace WristManager
{
    public class MainManager
    {
        public void Build(string inputFolder, string outputFolder)
        {
            IDomain Domain = ParserDomain.Parse(inputFolder, "home", "default");
            string OutputName = Path.GetFileName(outputFolder);
            IGeneratorDomain Generator = new GeneratorDomain(OutputName, Domain);
            Generator.Generate(outputFolder);
        }
    }
}
