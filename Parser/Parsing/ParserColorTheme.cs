using System;
using System.Collections.Generic;
using System.IO;

namespace Parser
{
    public class ParserColorTheme : FormParser<IColorTheme>
    {
        public ParserColorTheme(string folderName, string extension)
            : base(folderName, extension)
        {
        }

        public override IColorTheme Parse(string fileName)
        {
            string Name = Path.GetFileNameWithoutExtension(fileName);
            IParsingSourceStream SourceStream = ParsingSourceStream.CreateFromFileName(fileName);

            try
            {
                using (SourceStream.Open())
                {
                    return Parse(Name, SourceStream);
                }
            }
            catch (ParsingException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new ParsingException(80, SourceStream, e);
            }
        }

        private IColorTheme Parse(string name, IParsingSourceStream sourceStream)
        {
            Dictionary<IDeclarationSource, string> Colors;

            try
            {
                Colors = ParseColors(sourceStream);
            }
            catch (ParsingException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new ParsingException(80, sourceStream, e);
            }

            return new ColorTheme(name, Colors);
        }

        private Dictionary<IDeclarationSource, string> ParseColors(IParsingSourceStream sourceStream)
        {
            Dictionary<IDeclarationSource, string> Colors = new Dictionary<IDeclarationSource, string>();

            while (!sourceStream.EndOfStream)
            {
                sourceStream.ReadLine();

                IDeclarationSource ColorSource;
                string ColorValue;
                ParserDomain.ParseStringPair(sourceStream, ':', out ColorSource, out ColorValue);

                foreach (KeyValuePair<IDeclarationSource, string> Entry in Colors)
                    if (Entry.Key.Name == ColorSource.Name)
                        throw new ParsingException(81, sourceStream, $"Color '{ColorSource.Name}' defined more than once.");

                Colors.Add(ColorSource, ColorValue);
            }

            return Colors;
        }
    }
}
