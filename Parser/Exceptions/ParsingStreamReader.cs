using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Parser
{
    public class ParsingStreamReader : StreamReader
    {
        public ParsingStreamReader(Stream stream, IDictionary<ConditionalDefine, bool> conditionalDefineTable)
            : base(new MemoryStream(Encoding.UTF8.GetBytes(Parse(stream, conditionalDefineTable))), Encoding.UTF8)
        {
        }

        public ParsingStreamReader(string content, IDictionary<ConditionalDefine, bool> conditionalDefineTable)
            : base(new MemoryStream(Encoding.UTF8.GetBytes(Parse(content, conditionalDefineTable))), Encoding.UTF8)
        {
        }

        private static string Parse(Stream stream, IDictionary<ConditionalDefine, bool> conditionalDefineTable)
        {
            byte[] ContentBytes = new byte[stream.Length];
            stream.Read(ContentBytes, 0, ContentBytes.Length);

            byte[] Preamble = Encoding.Unicode.GetPreamble();

            int i;
            for (i = 0; i < ContentBytes.Length && i < Preamble.Length; i++)
                if (ContentBytes[i] != Preamble[i])
                    break;

            bool IsUnicode;
            if (i >= Preamble.Length)
                IsUnicode = true;
            else
                IsUnicode = false;

            string Content = IsUnicode ? Encoding.Unicode.GetString(ContentBytes) : Encoding.UTF8.GetString(ContentBytes);

            return Parse(Content, conditionalDefineTable);
        }

        private class Condition
        {
            public Condition(string name, bool isActive)
            {
                Name = name;
                IsActive = isActive;
            }

            public void Toggle()
            {
                IsActive = !IsActive;
            }

            public string Name;
            public bool IsActive;
        }

        private static string Parse(string content, IDictionary<ConditionalDefine, bool> conditionalDefineTable)
        {
            StringBuilder sb = new StringBuilder();
            string LastLine = null;

            using (StringReader sr = new StringReader(content))
            {
                List<Condition> ConditionList = new List<Condition>();

                for (;;)
                {
                    string Line = sr.ReadLine();
                    if (Line == null)
                        break;

                    if (LastLine != null)
                        sb.AppendLine(LastLine);

                    Line = Line.TrimEnd();

                    if (Line.StartsWith("#if "))
                    {
                        string RawName = Line.Substring(4).TrimStart();
                        string ConditionName = RawName.ToUpper();
                        Condition Found = null;
                        foreach (KeyValuePair<ConditionalDefine, bool> Entry in conditionalDefineTable)
                            if (ConditionName == Entry.Key.Name)
                            {
                                foreach (Condition Item in ConditionList)
                                    if (Item.Name == ConditionName)
                                        throw new ParsingException(0, "", $"Invalid nested directive '{RawName}'.");

                                Found = new Condition(ConditionName, Entry.Value);
                                break;
                            }

                        if (Found == null)
                            throw new ParsingException(0, "", $"Invalid directive '{RawName}'.");

                        ConditionList.Add(Found);
                        LastLine = null;
                    }

                    else if (Line == "#else")
                    {
                        if (ConditionList.Count == 0)
                            throw new ParsingException(0, "", $"Invalid #else directive.");

                        ConditionList[ConditionList.Count - 1].Toggle();
                        LastLine = null;
                    }

                    else if (Line == "#endif")
                    {
                        if (ConditionList.Count == 0)
                            throw new ParsingException(0, "", $"Invalid #endif directive.");

                        ConditionList.RemoveAt(ConditionList.Count - 1);
                        LastLine = null;
                    }

                    else
                    {
                        bool IsComposedActive = true;
                        foreach (Condition Item in ConditionList)
                            if (!Item.IsActive)
                            {
                                IsComposedActive = false;
                                break;
                            }

                        if (IsComposedActive)
                            LastLine = Line;
                        else
                            LastLine = null;
                    }
                }

                if (LastLine != null)
                    sb.Append(LastLine);

                if (ConditionList.Count > 0)
                    throw new ParsingException(0, "", $"Missing #endif.");
            }

            string ParsedContent = sb.ToString();
            return ParsedContent;
        }
    }
}
