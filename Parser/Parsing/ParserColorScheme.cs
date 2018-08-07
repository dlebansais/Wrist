using System;
using System.Collections.Generic;
using System.IO;

namespace Parser
{
    public class ParserColorScheme : FormParser<IColorScheme>
    {
        public ParserColorScheme(string folderName, string extension)
            : base(folderName, extension)
        {
        }

        public override IColorScheme Parse(string fileName)
        {
            string Name = Path.GetFileNameWithoutExtension(fileName);
            IParsingSource Source = ParsingSource.CreateFromFileName(fileName);

            try
            {
                using (Source.Open())
                {
                    return Parse(Name, Source);
                }
            }
            catch (ParsingException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new ParsingException(Source, e);
            }
        }

        private IColorScheme Parse(string name, IParsingSource source)
        {
            Dictionary<IDeclarationSource, string> Colors;

            try
            {
                Colors = ParseColors(source);
            }
            catch (ParsingException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new ParsingException(source, e);
            }

            return new ColorScheme(name, Colors);
        }

        private Dictionary<IDeclarationSource, string> ParseColors(IParsingSource source)
        {
            Dictionary<IDeclarationSource, string> Colors = new Dictionary<IDeclarationSource, string>();

            while (!source.EndOfStream)
            {
                source.ReadLine();

                IDeclarationSource ColorSource;
                string ColorValue;
                ParserDomain.ParseStringPair(source, ':', out ColorSource, out ColorValue);

                foreach (KeyValuePair<IDeclarationSource, string> Entry in Colors)
                    if (Entry.Key.Name == ColorSource.Name)
                        throw new ParsingException(source, $"Color defined more than once: {ColorSource}");

                Colors.Add(ColorSource, ColorValue);
            }

            return Colors;
        }
    }
}
