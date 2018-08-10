using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Parser
{
    public class ParserBackground : FormParser<IBackground>
    {
        public ParserBackground(string folderName, string extension)
            : base(folderName, extension)
        {
        }

        public override IBackground Parse(string fileName)
        {
            string Name = Path.GetFileNameWithoutExtension(fileName);
            IParsingSourceStream SourceStream = ParsingSourceStream.CreateFromFileName(fileName);

            List<string> Lines;
            LoadResourceFile(SourceStream, out Lines);

            return new Background(Name, ParserDomain.ToXamlName(SourceStream, Name, "Background"), Lines);
        }

        private void LoadResourceFile(IParsingSourceStream sourceStream, out List<string> Lines)
        {
            try
            {
                using (sourceStream.Open())
                {
                    Lines = new List<string>();

                    while (!sourceStream.EndOfStream)
                    {
                        sourceStream.ReadLine();
                        Lines.Add(sourceStream.Line);
                    }
                }
            }
            catch (Exception e)
            {
                throw new ParsingException(sourceStream, e);
            }
        }
    }
}
