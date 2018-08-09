using System.Collections.Generic;

namespace Parser
{
    public interface ITranslation
    {
        string TranslationFile { get; }
        char Separator { get; }
        IDictionary<string, IDictionary<string, string>> TranslationTable { get; }
        IList<string> LanguageList { get; }
        IList<string> KeyList { get; }
        void Process();
    }
}
