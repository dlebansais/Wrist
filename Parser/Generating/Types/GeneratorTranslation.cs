using System.Collections.Generic;

namespace Parser
{
    public class GeneratorTranslation : IGeneratorTranslation
    {
        public GeneratorTranslation(ITranslation translation)
        {
            TranslationTable = translation.TranslationTable;
            LanguageList = translation.LanguageList;
            KeyList = translation.KeyList;
        }

        public IDictionary<string, IDictionary<string, string>> TranslationTable { get; private set; }
        public IList<string> LanguageList { get; private set; }
        public IList<string> KeyList { get; private set; }
    }
}
