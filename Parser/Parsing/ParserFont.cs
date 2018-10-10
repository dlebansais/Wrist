using System;
using System.Collections.Generic;
using System.IO;

namespace Parser
{
    public class ParserFont : FormParser<IFont>
    {
        public ParserFont(string folderName, string extension)
            : base(folderName, extension)
        {
        }

        public override IFont Parse(string fileName, IDictionary<ConditionalDefine, bool> conditionalDefineTable)
        {
            string Name = Path.GetFileNameWithoutExtension(fileName);
            IParsingSourceStream SourceStream = ParsingSourceStream.CreateFromFileName(fileName, conditionalDefineTable);

            try
            {
                using (SourceStream.Open())
                {
                    return new Font(Name, ParserDomain.ToXamlName(SourceStream, Name, "Font"), fileName);
                }
            }
            catch (Exception e)
            {
                throw new ParsingException(116, SourceStream, e);
            }
        }
    }
}
