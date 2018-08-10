using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Parser
{
    public class ParserPage : FormParser<IPage>
    {
        public ParserPage(string folderName, string extension)
            : base(folderName, extension)
        {
        }

        public override IPage Parse(string fileName)
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
                throw new ParsingException(SourceStream, e);
            }
        }

        private IPage Parse(string name, IParsingSourceStream sourceStream)
        {
            IDeclarationSource AreaSource = null;
            Dictionary<IDeclarationSource, string> AreaLayoutsPairs = null;
            IDeclarationSource DesignSource = null;
            IDeclarationSource WidthSource = null;
            IDeclarationSource HeightSource = null;
            bool IsScrollable = false;
            IDeclarationSource BackgroundSource = null;
            IDeclarationSource BackgroundColorSource = null;

            while (!sourceStream.EndOfStream)
            {
                sourceStream.ReadLine();
                string Line = sourceStream.Line;
                if (!string.IsNullOrWhiteSpace(Line))
                    ParseComponent(sourceStream, ref AreaSource, ref AreaLayoutsPairs, ref DesignSource, ref WidthSource, ref HeightSource, ref IsScrollable, ref BackgroundSource, ref BackgroundColorSource);
            }

            if (AreaSource == null || string.IsNullOrEmpty(AreaSource.Name))
                throw new ParsingException(sourceStream, "Missing area name");

            if (AreaLayoutsPairs == null)
                throw new ParsingException(sourceStream, "Missing default area layout");

            if (DesignSource == null || string.IsNullOrEmpty(DesignSource.Name))
                throw new ParsingException(sourceStream, "Missing design name");

            if (WidthSource == null || string.IsNullOrEmpty(WidthSource.Name))
                throw new ParsingException(sourceStream, "Missing width");

            if (HeightSource == null || string.IsNullOrEmpty(HeightSource.Name))
                throw new ParsingException(sourceStream, "Missing height");

            if (BackgroundColorSource == null || string.IsNullOrEmpty(BackgroundColorSource.Name))
                throw new ParsingException(sourceStream, "Missing background color");

            return new Page(name, ParserDomain.ToCSharpName(sourceStream, name + "Page"), ParserDomain.ToXamlName(sourceStream, name, "Page"), AreaSource, AreaLayoutsPairs, DesignSource, WidthSource, HeightSource, IsScrollable, BackgroundSource, BackgroundColorSource);
        }

        private void ParseComponent(IParsingSourceStream sourceStream, ref IDeclarationSource areaSource, ref Dictionary<IDeclarationSource, string> areaLayoutsPairs, ref IDeclarationSource designSource, ref IDeclarationSource widthSource, ref IDeclarationSource heightSource, ref bool isScrollable, ref IDeclarationSource backgroundSource, ref IDeclarationSource backgroundColorSource)
        {
            string Line = sourceStream.Line;
            if (Line.Trim() == "scrollable")
            {
                isScrollable = true;
                return;
            }

            IDeclarationSource ComponentSource;
            string ComponentValue;
            ParserDomain.ParseStringPair(sourceStream, ':', out ComponentSource, out ComponentValue);
            //ComponentValue = ComponentValue.ToLower();

            if (ComponentSource.Name == "area")
                areaSource = new DeclarationSource(ComponentValue, sourceStream);
            else if (ComponentSource.Name == "default area layout")
                areaLayoutsPairs = ParseAreaLayoutsPairs(sourceStream, ComponentValue);
            else if (ComponentSource.Name == "design")
                designSource = new DeclarationSource(ComponentValue, sourceStream);
            else if (ComponentSource.Name == "width")
                widthSource = new DeclarationSource(ComponentValue, sourceStream);
            else if (ComponentSource.Name == "height")
                heightSource = new DeclarationSource(ComponentValue, sourceStream);
            else if (ComponentSource.Name == "background")
                backgroundSource = new DeclarationSource(ComponentValue, sourceStream);
            else if (ComponentSource.Name == "background color")
                backgroundColorSource = new DeclarationSource(ComponentValue, sourceStream);
            else
                throw new ParsingException(sourceStream, $"Unexpected specifier: {ComponentSource.Name}");
        }

        private Dictionary<IDeclarationSource, string> ParseAreaLayoutsPairs(IParsingSourceStream sourceStream, string line)
        {
            Dictionary<IDeclarationSource, string> Result = new Dictionary<IDeclarationSource, string>();
            string[] Splitted = line.Split(',');

            foreach (string Split in Splitted)
            {
                IDeclarationSource AreaSource;
                string LayoutName;
                ParserDomain.ParseStringPair(sourceStream, Split, '=', out AreaSource, out LayoutName);

                Result.Add(AreaSource, LayoutName);
            }

            return Result;
        }
    }
}
