using System.Collections.Generic;

namespace AppCSHtml5
{
    public interface ILanguage
    {
        LanguageStates State { get; set; }
        Dictionary<string, string> Strings { get; }
        Dictionary<string, string> PageStrings { get; }
        INewsEntry LastNews { get; }
        INewsEntry ArchiveNews { get; }
        IList<INewsEntry> AllNews { get; }
        void On_Switch(string pageName, string sourceName, string sourceContent);
    }
}
