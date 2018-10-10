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

        public override IPage Parse(string fileName, IDictionary<ConditionalDefine, bool> conditionalDefineTable)
        {
            string Name = Path.GetFileNameWithoutExtension(fileName);
            IParsingSourceStream SourceStream = ParsingSourceStream.CreateFromFileName(fileName, conditionalDefineTable);

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
                throw new ParsingException(108, SourceStream, e);
            }
        }

        private IPage Parse(string name, IParsingSourceStream sourceStream)
        {
            IComponentEvent QueryEvent = null;
            IDeclarationSource AreaSource = null;
            IParsingSource AllAreaLayoutsSource = null;
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
                    ParseComponent(sourceStream, ref QueryEvent, ref AreaSource, ref AllAreaLayoutsSource, ref AreaLayoutsPairs, ref DesignSource, ref WidthSource, ref HeightSource, ref IsScrollable, ref BackgroundSource, ref BackgroundColorSource);
            }

            if (AreaSource == null || string.IsNullOrEmpty(AreaSource.Name))
                throw new ParsingException(109, sourceStream, "Missing area name.");

            if (AreaLayoutsPairs == null)
                throw new ParsingException(110, sourceStream, "Missing default area layout.");

            if (DesignSource == null || string.IsNullOrEmpty(DesignSource.Name))
                throw new ParsingException(111, sourceStream, "Missing design name.");

            if (WidthSource == null || string.IsNullOrEmpty(WidthSource.Name))
                throw new ParsingException(112, sourceStream, "Missing width.");

            if (HeightSource == null || string.IsNullOrEmpty(HeightSource.Name))
                throw new ParsingException(113, sourceStream, "Missing height.");

            if (BackgroundColorSource == null || string.IsNullOrEmpty(BackgroundColorSource.Name))
                throw new ParsingException(114, sourceStream, "Missing background color.");

            return new Page(name, ParserDomain.ToCSharpName(sourceStream, name + "Page"), ParserDomain.ToXamlName(sourceStream, name, "Page"), QueryEvent, AreaSource, AllAreaLayoutsSource, AreaLayoutsPairs, DesignSource, WidthSource, HeightSource, IsScrollable, BackgroundSource, BackgroundColorSource);
        }

        private void ParseComponent(IParsingSourceStream sourceStream, ref IComponentEvent queryEvent, ref IDeclarationSource areaSource, ref IParsingSource allAreaLayoutsSource, ref Dictionary<IDeclarationSource, string> areaLayoutsPairs, ref IDeclarationSource designSource, ref IDeclarationSource widthSource, ref IDeclarationSource heightSource, ref bool isScrollable, ref IDeclarationSource backgroundSource, ref IDeclarationSource backgroundColorSource)
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

            if (ComponentSource.Name == "open on query")
                if (queryEvent == null)
                    queryEvent = ParseQueryEvent(sourceStream, ComponentValue);
                else
                    throw new ParsingException(125, sourceStream, $"Specifier '{ComponentSource.Name}' found more than once.");
            else if (ComponentSource.Name == "area")
                if (areaSource == null)
                    areaSource = new DeclarationSource(ComponentValue, sourceStream);
                else
                    throw new ParsingException(125, sourceStream, $"Specifier '{ComponentSource.Name}' found more than once.");
            else if (ComponentSource.Name == "default area layout")
                if (areaLayoutsPairs == null)
                {
                    allAreaLayoutsSource = sourceStream.FreezedPosition();
                    areaLayoutsPairs = ParseAreaLayoutsPairs(sourceStream, ComponentValue);
                }
                else
                    throw new ParsingException(125, sourceStream, $"Specifier '{ComponentSource.Name}' found more than once.");
            else if (ComponentSource.Name == "design")
                if (designSource == null)
                    designSource = new DeclarationSource(ComponentValue, sourceStream);
                else
                    throw new ParsingException(125, sourceStream, $"Specifier '{ComponentSource.Name}' found more than once.");
            else if (ComponentSource.Name == "width")
                if (widthSource == null)
                    widthSource = new DeclarationSource(ComponentValue, sourceStream);
                else
                    throw new ParsingException(125, sourceStream, $"Specifier '{ComponentSource.Name}' found more than once.");
            else if (ComponentSource.Name == "height")
                if (heightSource == null)
                    heightSource = new DeclarationSource(ComponentValue, sourceStream);
                else
                    throw new ParsingException(125, sourceStream, $"Specifier '{ComponentSource.Name}' found more than once.");
            else if (ComponentSource.Name == "background")
                if (backgroundSource == null)
                    backgroundSource = new DeclarationSource(ComponentValue, sourceStream);
                else
                    throw new ParsingException(125, sourceStream, $"Specifier '{ComponentSource.Name}' found more than once.");
            else if (ComponentSource.Name == "background color")
                if (backgroundColorSource == null)
                    backgroundColorSource = new DeclarationSource(ComponentValue, sourceStream);
                else
                    throw new ParsingException(125, sourceStream, $"Specifier '{ComponentSource.Name}' found more than once.");
            else
                throw new ParsingException(115, sourceStream, $"Specifier '{ComponentSource.Name}' was unexpected.");
        }

        private IComponentEvent ParseQueryEvent(IParsingSourceStream sourceStream, string line)
        {
            ComponentInfo Info = ComponentInfo.Parse(sourceStream, line);
            return new ComponentEvent(Info);
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
