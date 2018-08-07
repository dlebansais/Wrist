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
            IParsingSource Source = ParsingSource.CreateFromFileName(fileName);

            List<string> Lines;
            LoadResourceFile(Source, out Lines);

            return new Background(Name, ParserDomain.ToXamlName(Source, Name, "Background"), Lines);
        }

        private void LoadResourceFile(IParsingSource source, out List<string> Lines)
        {
            try
            {
                using (source.Open())
                {
                    Lines = new List<string>();

                    while (!source.EndOfStream)
                    {
                        source.ReadLine();
                        Lines.Add(source.Line);
                    }
                }
            }
            catch (Exception e)
            {
                throw new ParsingException(source, e);
            }
        }
    }
}
