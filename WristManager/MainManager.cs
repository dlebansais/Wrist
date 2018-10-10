using Parser;
using System;
using System.Collections.Generic;
using System.IO;

namespace WristManager
{
    public class MainManager
    {
        public void Build(string inputFolderName, string outputFolderName, string homePageName, string colorThemeName, string unitTestName, string conditionalDefines, out IDomain domain)
        {
            IDictionary<ConditionalDefine, bool> ConditionalDefineTable = new Dictionary<ConditionalDefine, bool>();

            if (!string.IsNullOrEmpty(conditionalDefines))
            {
                string[] Directives = conditionalDefines.Split(',');
                foreach (string Directive in Directives)
                {
                    string[] Splitted = Directive.Split('=');
                    if (Splitted.Length != 2)
                        throw new ParsingException(0, inputFolderName, $"Invalid directive '{Directive}'.");

                    string ConditionalName = Splitted[0].Trim();
                    string Applied = Splitted[1].Trim();
                    if (ConditionalName.Length == 0)
                        throw new ParsingException(0, inputFolderName, $"Invalid directive '{Directive}'.");

                    bool AsBool;
                    int AsInt;
                    if (!bool.TryParse(Applied, out AsBool))
                        if (!int.TryParse(Applied, out AsInt) || (AsInt != 0 && AsInt != 1))
                            throw new ParsingException(0, inputFolderName, $"Invalid directive '{Directive}'.");
                        else
                            AsBool = (AsInt != 0);

                    ConditionalDefineTable.Add(new ConditionalDefine(ConditionalName), AsBool);
                }
            }

            if (ConditionalDefineTable.Count >= 64)
                throw new ParsingException(0, inputFolderName, "Too many directives.");

            domain = ParserDomain.Parse(inputFolderName, homePageName, colorThemeName, unitTestName, ConditionalDefineTable);
            domain.Verify();
            domain.CheckUnused((string message) => Console.WriteLine(message));

            string OutputName = Path.GetFileName(outputFolderName);
            IGeneratorDomain Generator = new GeneratorDomain(OutputName, domain);
            Generator.Generate(outputFolderName);
        }
    }
}
