using System.Collections.Generic;

namespace Parser
{
    public class PreprocessorDefine : IPreprocessorDefine
    {
        public PreprocessorDefine(string translationFile)
        {
            PreprocessorDefineFile = translationFile;
        }

        public string PreprocessorDefineFile { get; private set; }
        public IDictionary<string, bool> PreprocessorDefineTable { get; private set; }

        public void Process(IDictionary<ConditionalDefine, bool> conditionalDefineTable)
        {
            PreprocessorDefineTable = new Dictionary<string, bool>();

            IParsingSourceStream SourceStream = ParsingSourceStream.CreateFromFileName(PreprocessorDefineFile, conditionalDefineTable);
            int LineNumber = 0;

            using (SourceStream.Open())
            {
                while (!SourceStream.EndOfStream)
                {
                    SourceStream.ReadLine();

                    string Line = SourceStream.Line;
                    string[] Splitted = Line.Split('=');

                    if (Splitted.Length != 2)
                        throw new ParsingException(0, SourceStream, $"Inconsistent format at line {LineNumber + 1}.");

                    else
                    {
                        string Define = Splitted[0].Trim();
                        if (!IsDefineValid(Define))
                            throw new ParsingException(0, SourceStream, $"Invalid define '{Define}' at line {LineNumber + 1}.");

                        if (PreprocessorDefineTable.ContainsKey(Define))
                            throw new ParsingException(0, SourceStream, $"'{Define}' at line {LineNumber + 1} already exist.");

                        string Value = Splitted[1].Trim();

                        int ValueAsInt;
                        bool ValueAsBool;
                        if (int.TryParse(Value, out ValueAsInt) && (ValueAsInt == 0 || ValueAsInt == 1))
                            ValueAsBool = (ValueAsInt == 1);
                        else if (bool.TryParse(Value, out ValueAsBool))
                        { }
                        else
                            throw new ParsingException(0, SourceStream, $"Invalid define value '{Value}' at line {LineNumber + 1}.");

                        PreprocessorDefineTable.Add(Define, ValueAsBool);
                    }

                    LineNumber++;
                }
            }
        }

        public bool Connect(IDomain domain)
        {
            return false;
        }

        public static bool IsDefineValid(string define)
        {
            if (define.Length == 0)
                return false;

            if (!(define[0] >= 'A' && define[0] <= 'Z'))
                return false;

            foreach (char c in define)
                if (!((c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9')|| c == '_'))
                    return false;

            return true;
        }
    }
}
