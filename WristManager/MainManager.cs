using Parser;
using System.IO;

namespace WristManager
{
    public class MainManager
    {
        public void Build(string inputFolder, string outputFolder, string homePageName, string colorThemeName, string unitTestName, out IDomain domain)
        {
            domain = ParserDomain.Parse(inputFolder, homePageName, colorThemeName, unitTestName);
            string OutputName = Path.GetFileName(outputFolder);
            IGeneratorDomain Generator = new GeneratorDomain(OutputName, domain);
            Generator.Generate(outputFolder);
        }
    }
}
