using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Parser
{
    public class Translation : ITranslation
    {
        public Translation(string translationFile, char separator)
        {
            TranslationFile = translationFile;
            Separator = separator;
        }

        public string TranslationFile { get; private set; }
        public char Separator { get; private set; }
        public IDictionary<string, IDictionary<string, string>> TranslationTable { get; private set; }
        public IList<string> LanguageList { get; private set; }
        public IList<string> KeyList { get; private set; }
        public IList<string> UsedKeyList { get; private set; }

        public void Process()
        {
            using (FileStream fs = new FileStream(TranslationFile, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
                {
                    Process(sr);
                }
            }
        }

        public void Process(StreamReader sr)
        {
            TranslationTable = new Dictionary<string, IDictionary<string, string>>();
            LanguageList = new List<string>();
            KeyList = new List<string>();
            UsedKeyList = new List<string>();

            IParsingSourceStream SourceStream = ParsingSourceStream.CreateFromFileName(TranslationFile);
            //DeclarationSource FileSource = new DeclarationSource("Translation", SourceStream);
            int LineNumber = 0;

            while (!sr.EndOfStream)
            {
                string Line = sr.ReadLine();
                string[] Splitted = Line.Split(Separator);

                if (TranslationTable.Count == 0)
                {
                    if (Splitted.Length < 2)
                        throw new ParsingException(178, SourceStream, "The translation file is expected to have a header.");

                    string KeyHeader = Splitted[0].Trim();
                    if (KeyHeader != "Key")
                        throw new ParsingException(179, SourceStream, "The translation file is expected to have a header starting with 'Key' for the first column.");

                    for (int i = 1; i < Splitted.Length; i++)
                    {
                        string Language = Splitted[i].Trim();
                        if (Language.Length == 0 || Language.Length >= 100)
                            throw new ParsingException(180, SourceStream, $"The translation file is expected to have the name of a language at the header of column #{i + 1}.");

                        if (TranslationTable.ContainsKey(Language))
                            throw new ParsingException(181, SourceStream, $"Language '{Language}' found more than once in the header.");

                        LanguageList.Add(Language);
                        TranslationTable.Add(Language, new Dictionary<string, string>());
                    }
                }

                else if (Splitted.Length != LanguageList.Count + 1)
                    throw new ParsingException(182, SourceStream, $"Inconsistent format at line {LineNumber + 1}.");

                else
                {
                    string Key = Splitted[0];
                    if (!IsKeyReserved(Key) && !IsKeyValid(Key))
                        throw new ParsingException(183, SourceStream, $"Invalid key '{Key}' at line {LineNumber + 1}.");

                    for (int i = 1; i < Splitted.Length; i++)
                    {
                        string Language = LanguageList[i - 1];
                        IDictionary<string, string> LanguageTable = TranslationTable[Language];

                        if (i == 1 && LanguageTable.ContainsKey(Key))
                            throw new ParsingException(184, SourceStream, $"Translation for key '{Key}' found at line {LineNumber + 1} but this key already has an entry.");

                        if (!IsKeyReserved(Key))
                            LanguageTable.Add(Key, Splitted[i].Trim());
                    }

                    if (!IsKeyReserved(Key))
                        KeyList.Add(Key);
                }

                LineNumber++;
            }
        }

        public static bool IsKeyReserved(string key)
        {
            return !string.IsNullOrEmpty(key) && key[0] == '*';
        }

        public static bool IsKeyValid(string key)
        {
            foreach (char c in key)
                if (!((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9') || c == '-' || c == '_'))
                    return false;

            return true;
        }
    }
}
