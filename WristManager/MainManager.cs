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

            // Verify all combinations of defines, except the selected one.
            if (ConditionalDefineTable.Count >= 64)
                throw new ParsingException(0, inputFolderName, "Too many directives.");

            List<ConditionalDefine> ConditionalDefineList = new List<ConditionalDefine>();
            foreach (KeyValuePair<ConditionalDefine, bool> Entry in ConditionalDefineTable)
                ConditionalDefineList.Add(Entry.Key);

            ulong CombinationMax = 1UL << ConditionalDefineList.Count;
            for (ulong n = 0; n < CombinationMax; n++)
            {
                IDictionary<ConditionalDefine, bool> TestTable = new Dictionary<ConditionalDefine, bool>();
                bool IsSelected = true;
                for (int i = 0; i < ConditionalDefineList.Count; i++)
                {
                    bool IsSet = ((n >> i) & 1) != 0;
                    TestTable.Add(ConditionalDefineList[i], IsSet);
                    if (IsSelected && ConditionalDefineTable[ConditionalDefineList[i]] != IsSet)
                        IsSelected = false;
                }

                if (!IsSelected)
                {
                    domain = ParserDomain.Parse(inputFolderName, homePageName, colorThemeName, unitTestName, TestTable);
                    domain.Verify();
                    domain.CheckUnused((string message) => Console.WriteLine(message), TestTable);
                }
            }

            // Process the selected combination of defines.
            domain = ParserDomain.Parse(inputFolderName, homePageName, colorThemeName, unitTestName, ConditionalDefineTable);
            domain.Verify();
            domain.CheckUnused((string message) => Console.WriteLine(message), ConditionalDefineTable);

            string OutputName = Path.GetFileName(outputFolderName);
            IGeneratorDomain Generator = new GeneratorDomain(OutputName, domain);
            Generator.Generate(outputFolderName, ConditionalDefineTable);
        }
    }
}
