using System.Collections.Generic;

namespace Presentation
{
    public static class HtmlString
    {
        private static Dictionary<string, string> EncodingTable;
        private static Dictionary<string, string> DecodingTable;

        static HtmlString()
        {
            DecodingTable = new Dictionary<string, string>()
            {
                { "%20", " " },
                { "%21", "!" },
                { "%23", "#" },
                { "%24", "$" },
                { "%26", "&" },
                { "%27", "\'" },
                { "%28", "(" },
                { "%29", ")" },
                { "%2A", "*" },
                { "%2B", "+" },
                { "%2C", "," },
                { "%2F", "/" },
                { "%3A", ":" },
                { "%3B", ";" },
                { "%3D", "=" },
                { "%3F", "?" },
                { "%40", "@" },
                { "%5B", "[" },
                { "%5D", "]" },
            };

            EncodingTable = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> Entry in DecodingTable)
                EncodingTable.Add(Entry.Value, Entry.Key);
        }

        public static string PercentDecode(string s)
        {
            string Result = s;

            foreach (KeyValuePair<string, string> Entry in DecodingTable)
                Result = Result.Replace(Entry.Key, Entry.Value);

            return Result;
        }

        public static string PercentEncoded(string s)
        {
            string Result = "";

            foreach (char c in s)
            {
                string AsString = c.ToString();
                if (EncodingTable.ContainsKey(AsString))
                    Result += EncodingTable[AsString];
                else
                    Result += AsString;
            }

            return Result;
        }
    }
}
