using System;
using System.Collections.Generic;
using System.IO;

namespace Parser
{
    public class ParserResource : FormParser<IResource>
    {
        public ParserResource(string folderName, string extension)
            : base(folderName, extension)
        {
        }

        public override IResource Parse(string fileName, IDictionary<ConditionalDefine, bool> conditionalDefineTable)
        {
            string Name = Path.GetFileNameWithoutExtension(fileName);
            IParsingSourceStream SourceStream = ParsingSourceStream.CreateFromFileName(fileName, conditionalDefineTable);

            try
            {
                using (SourceStream.Open())
                {
                    return new Resource(Name, ParserDomain.ToXamlName(SourceStream, Name, "Resource"), fileName);
                }
            }
            catch (Exception e)
            {
                throw new ParsingException(116, SourceStream, e);
            }
        }
    }
}
