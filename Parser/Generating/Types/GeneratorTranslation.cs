﻿using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Parser
{
    public class GeneratorTranslation : IGeneratorTranslation
    {
        public GeneratorTranslation(ITranslation translation)
        {
            TranslationTable = translation.TranslationTable;
        }

        public IDictionary<string, IDictionary<string, string>> TranslationTable { get; private set; }

        public void Generate(string outputFolderName, string appNamespace)
        {
            string TranslationFileName = Path.Combine(outputFolderName, "Translation.cs");

            using (FileStream TranslationFile = new FileStream(TranslationFileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (StreamWriter CSharpWriter = new StreamWriter(TranslationFile, Encoding.UTF8))
                {
                    Generate(appNamespace, CSharpWriter);
                }
            }
        }

        public bool Connect(IGeneratorDomain domain)
        {
            return false;
        }

        private void Generate(string appNamespace, StreamWriter cSharpWriter)
        {
            cSharpWriter.WriteLine("using System.Collections.Generic;");
            cSharpWriter.WriteLine("using System.ComponentModel;");
            cSharpWriter.WriteLine("using System.Globalization;");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine($"namespace {appNamespace}");
            cSharpWriter.WriteLine("{");
            cSharpWriter.WriteLine("    public class Translation");
            cSharpWriter.WriteLine("    {");
            cSharpWriter.WriteLine("        static Translation()");
            cSharpWriter.WriteLine("        {");
            cSharpWriter.WriteLine("            string CurrentCultureName = CultureInfo.CurrentCulture.Name;");
            cSharpWriter.WriteLine("            if (CurrentCultureName != null && AllStrings.ContainsKey(CurrentCultureName))");
            cSharpWriter.WriteLine("                Selected = CurrentCultureName;");
            cSharpWriter.WriteLine("            else");
            cSharpWriter.WriteLine("                Selected = \"en-US\";");
            cSharpWriter.WriteLine("        }");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine("        public static void SetSelected(string language)");
            cSharpWriter.WriteLine("        {");
            cSharpWriter.WriteLine("            if (language != Selected && AllStrings.ContainsKey(language))");
            cSharpWriter.WriteLine("                Selected = language;");
            cSharpWriter.WriteLine("        }");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine("        public static string Selected { get; private set; }");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine("        public static IDictionary<string, IDictionary<string, string>> AllStrings { get; } = new Dictionary<string, IDictionary<string, string>>()");
            cSharpWriter.WriteLine("        {");

            string FirstLanguage = null;

            foreach (KeyValuePair<string, IDictionary<string, string>> Entry in TranslationTable)
            {
                string Language = Entry.Key;
                IDictionary<string, string> LanguageTable = Entry.Value;

                if (FirstLanguage != null)
                    cSharpWriter.WriteLine();
                else
                    FirstLanguage = Language;

                cSharpWriter.WriteLine($"            {{ \"{Language}\", new Dictionary<string, string>()");
                cSharpWriter.WriteLine("                {");

                foreach (KeyValuePair<string, string> LanguageEntry in LanguageTable)
                {
                    string Key = LanguageEntry.Key;
                    string TranslatedString = LanguageEntry.Value;
                    TranslatedString = TranslatedString.Replace("\"", "\\\"");

                    cSharpWriter.WriteLine($"                    {{ \"{Key}\", \"{TranslatedString}\" }},");
                }

                cSharpWriter.WriteLine("                }");
                cSharpWriter.WriteLine("            },");
            }

            cSharpWriter.WriteLine("        };");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine("        public Translation()");
            cSharpWriter.WriteLine("        {");
            cSharpWriter.WriteLine("            Strings = AllStrings[Selected];");
            cSharpWriter.WriteLine("        }");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine("        public IDictionary<string, string> Strings { get; private set; }");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine("        public void SetLanguage(string language)");
            cSharpWriter.WriteLine("        {");
            cSharpWriter.WriteLine("            if (language != Selected && AllStrings.ContainsKey(language))");
            cSharpWriter.WriteLine("            {");
            cSharpWriter.WriteLine("                Selected = language;");
            cSharpWriter.WriteLine("                Strings = AllStrings[Selected];");
            cSharpWriter.WriteLine("                NotifyLanguageChanged();");
            cSharpWriter.WriteLine("            }");
            cSharpWriter.WriteLine("        }");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine("        public event PropertyChangedEventHandler PropertyChanged;");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine("        internal void NotifyLanguageChanged()");
            cSharpWriter.WriteLine("        {");
            cSharpWriter.WriteLine($"            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Strings)));");
            cSharpWriter.WriteLine("        }");
            cSharpWriter.WriteLine("    }");
            cSharpWriter.WriteLine("}");
        }
    }
}
