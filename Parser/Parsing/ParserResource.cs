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
            IParsingSource Source = ParsingSource.CreateFromFileName(fileName);

            try
            {
                using (Source.Open())
                {
                    return new Resource(Name, ParserDomain.ToXamlName(Source, Name, "Resource"), fileName);
                }
            }
            catch (Exception e)
            {
                throw new ParsingException(Source, e);
            }
        }
    }
}
