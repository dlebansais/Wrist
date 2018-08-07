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

        private IPage Parse(string name, IParsingSource source)
        {
            IDeclarationSource AreaSource = null;
            Dictionary<IDeclarationSource, string> AreaLayoutsPairs = null;
            IDeclarationSource DesignSource = null;
            IDeclarationSource WidthSource = null;
            IDeclarationSource HeightSource = null;
            bool IsScrollable = false;
            IDeclarationSource BackgroundSource = null;
            IDeclarationSource BackgroundColorSource = null;

            while (!source.EndOfStream)
            {
                source.ReadLine();
                string Line = source.Line;
                if (!string.IsNullOrWhiteSpace(Line))
                    ParseComponent(source, ref AreaSource, ref AreaLayoutsPairs, ref DesignSource, ref WidthSource, ref HeightSource, ref IsScrollable, ref BackgroundSource, ref BackgroundColorSource);
            }

            if (AreaSource == null || string.IsNullOrEmpty(AreaSource.Name))
                throw new ParsingException(source, "Missing area name");

            if (AreaLayoutsPairs == null)
                throw new ParsingException(source, "Missing default area layout");

            if (DesignSource == null || string.IsNullOrEmpty(DesignSource.Name))
                throw new ParsingException(source, "Missing design name");

            if (WidthSource == null || string.IsNullOrEmpty(WidthSource.Name))
                throw new ParsingException(source, "Missing width");

            if (HeightSource == null || string.IsNullOrEmpty(HeightSource.Name))
                throw new ParsingException(source, "Missing height");

            if (BackgroundColorSource == null || string.IsNullOrEmpty(BackgroundColorSource.Name))
                throw new ParsingException(source, "Missing background color");

            return new Page(name, ParserDomain.ToCSharpName(source, name + "Page"), ParserDomain.ToXamlName(source, name, "Page"), AreaSource, AreaLayoutsPairs, DesignSource, WidthSource, HeightSource, IsScrollable, BackgroundSource, BackgroundColorSource);
        }

        private void ParseComponent(IParsingSource source, ref IDeclarationSource areaSource, ref Dictionary<IDeclarationSource, string> areaLayoutsPairs, ref IDeclarationSource designSource, ref IDeclarationSource widthSource, ref IDeclarationSource heightSource, ref bool isScrollable, ref IDeclarationSource backgroundSource, ref IDeclarationSource backgroundColorSource)
        {
            string Line = source.Line;
            if (Line.Trim() == "scrollable")
            {
                isScrollable = true;
                return;
            }

            IDeclarationSource ComponentSource;
            string ComponentValue;
            ParserDomain.ParseStringPair(source, ':', out ComponentSource, out ComponentValue);
            //ComponentValue = ComponentValue.ToLower();

            if (ComponentSource.Name == "area")
                areaSource = new DeclarationSource(ComponentValue, source);
            else if (ComponentSource.Name == "default area layout")
                areaLayoutsPairs = ParseAreaLayoutsPairs(source, ComponentValue);
            else if (ComponentSource.Name == "design")
                designSource = new DeclarationSource(ComponentValue, source);
            else if (ComponentSource.Name == "width")
                widthSource = new DeclarationSource(ComponentValue, source);
            else if (ComponentSource.Name == "height")
                heightSource = new DeclarationSource(ComponentValue, source);
            else if (ComponentSource.Name == "background")
                backgroundSource = new DeclarationSource(ComponentValue, source);
            else if (ComponentSource.Name == "background color")
                backgroundColorSource = new DeclarationSource(ComponentValue, source);
            else
                throw new ParsingException(source, $"Unexpected specifier: {ComponentSource.Name}");
        }

        private Dictionary<IDeclarationSource, string> ParseAreaLayoutsPairs(IParsingSource source, string line)
        {
            Dictionary<IDeclarationSource, string> Result = new Dictionary<IDeclarationSource, string>();
            string[] Splitted = line.Split(',');

            foreach (string Split in Splitted)
            {
                IDeclarationSource AreaSource;
                string LayoutName;
                ParserDomain.ParseStringPair(source, Split, '=', out AreaSource, out LayoutName);

                Result.Add(AreaSource, LayoutName);
            }

            return Result;
        }
    }
}
