using System.Collections.Generic;

namespace Parser
{
    public interface IGeneratorTranslation
    {
        IDictionary<string, IDictionary<string, string>> TranslationTable { get; }
        IList<string> LanguageList { get; }
        IList<string> KeyList { get; }
    }
}
