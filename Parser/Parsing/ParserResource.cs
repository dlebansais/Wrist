using System;
using System.IO;
using System.Text;

namespace Parser
{
    public class ParserResource : FormParser<IResource>
    {
        public ParserResource(string folderName, string extension)
            : base(folderName, extension)
        {
        }

        public override IResource Parse(string fileName)
        {
            string Name = Path.GetFileNameWithoutExtension(fileName);
            IParsingSourceStream SourceStream = ParsingSourceStream.CreateFromFileName(fileName);

            try
            {
                using (SourceStream.Open())
                {
                    return new Resource(Name, ParserDomain.ToXamlName(SourceStream, Name, "Resource"), fileName);
                }
            }
            catch (Exception e)
            {
                throw new ParsingException(SourceStream, e);
            }
        }
    }
}
