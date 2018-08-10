using Parser;
using System.IO;

namespace WristManager
{
    public class MainManager
    {
        public void Build(string inputFolder, string outputFolder, string homePageName, string colorThemeName)
        {
            IDomain Domain = ParserDomain.Parse(inputFolder, homePageName, colorThemeName);
            string OutputName = Path.GetFileName(outputFolder);
            IGeneratorDomain Generator = new GeneratorDomain(OutputName, Domain);
            Generator.Generate(outputFolder);
        }
    }
}
