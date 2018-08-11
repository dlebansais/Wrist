using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Parser
{
    public class ParserObject : FormParser<IObject>
    {
        public ParserObject(string folderName, string extension)
            : base(folderName, extension)
        {
        }

        public override IObject Parse(string fileName)
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
                throw new ParsingException(94, SourceStream, e);
            }
        }

        private IObject Parse(string name, IParsingSourceStream sourceStream)
        {
            List<string> StateList;
            IObjectPropertyCollection ObjectPropertyList;
            List<IObjectEvent> ObjectEventList;
            string Line = null;

            try
            {
                StateList = ParseStates(sourceStream, ref Line);
                ObjectPropertyList = ParseObjectProperties(sourceStream, ref Line);
                ObjectEventList = ParseEvents(sourceStream, ref Line);
            }
            catch (ParsingException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new ParsingException(94, sourceStream, e);
            }

            string CSharpname = ParserDomain.ToCSharpName(sourceStream, name);
            return new Object(name, CSharpname, StateList, ObjectPropertyList, ObjectEventList);
        }

        private List<string> ParseStates(IParsingSourceStream sourceStream, ref string line)
        {
            List<string> StateList = new List<string>();

            sourceStream.ReadLine();
            string HeaderLine = sourceStream.Line;
            if (HeaderLine != "states")
                throw new ParsingException(sourceStream, "Expected: states");

            while (!sourceStream.EndOfStream)
            {
                sourceStream.ReadLine();
                if (string.IsNullOrEmpty(sourceStream.Line))
                    break;

                string State = ParseState(sourceStream);
                StateList.Add(ParserDomain.ToCSharpName(sourceStream, State));
            }

            return StateList;
        }

        private string ParseState(IParsingSourceStream sourceStream)
        {
            return sourceStream.Line.Trim();
        }

        private IObjectPropertyCollection ParseObjectProperties(IParsingSourceStream sourceStream, ref string line)
        {
            IObjectPropertyCollection ObjectPropertyList = new ObjectPropertyCollection();

            sourceStream.ReadLine();
            string HeaderLine = sourceStream.Line;
            if (HeaderLine != "properties")
                throw new ParsingException(sourceStream, "Expected: properties");

            while (!sourceStream.EndOfStream)
            {
                sourceStream.ReadLine();
                if (string.IsNullOrEmpty(sourceStream.Line))
                    break;

                IObjectProperty NewProperty = ParseProperty(sourceStream);

                foreach (IObjectProperty ObjectProperty in ObjectPropertyList)
                    if (ObjectProperty.NameSource.Name == NewProperty.NameSource.Name)
                        throw new ParsingException(sourceStream, $"Object already contains a property called {NewProperty.NameSource.Name}");

                ObjectPropertyList.Add(NewProperty);
            }

            return ObjectPropertyList;
        }

        private IObjectProperty ParseProperty(IParsingSourceStream sourceStream)
        {
            IDeclarationSource NameSource;
            string Details;
            ParserDomain.ParseStringPair(sourceStream, ':', out NameSource, out Details);

            string[] SplittedDetails = Details.Split(',');
            string PropertyTypeName = SplittedDetails[0].Trim();

            int MaximumLength = int.MaxValue;
            IDeclarationSource ObjectSource = null;

            for (int i = 1; i < SplittedDetails.Length; i++)
            {
                string Detail = SplittedDetails[i].Trim();
                string[] SplittedDetail = Detail.Split('=');
                int ParsedLength;

                if (SplittedDetail.Length == 2 && SplittedDetail[0].Trim() == "maximum length" && int.TryParse(SplittedDetail[1].Trim(), out ParsedLength) && ParsedLength >= 0)
                    if (MaximumLength == int.MaxValue)
                        MaximumLength = ParsedLength;
                    else
                        throw new ParsingException(sourceStream, "Maximum length specified more than once");

                else if (SplittedDetail.Length == 2 && SplittedDetail[0].Trim() == "object")
                    if (ObjectSource == null)
                    {
                        ObjectSource = new DeclarationSource(SplittedDetail[1].Trim(), sourceStream);
                        if (string.IsNullOrEmpty(ObjectSource.Name))
                            throw new ParsingException(sourceStream, "Invalid empty object name");
                    }
                    else
                        throw new ParsingException(sourceStream, "Object name specified more than once");

                else
                    throw new ParsingException(sourceStream, $"Unknown specifier {Detail}");
            }

            string CSharpName = ParserDomain.ToCSharpName(NameSource.Source, NameSource.Name);

            if (PropertyTypeName == "string")
                return new ObjectPropertyString(NameSource, CSharpName, MaximumLength);
            else if (PropertyTypeName == "readonly string")
                return new ObjectPropertyReadonlyString(NameSource, CSharpName);
            else if (PropertyTypeName == "string dictionary")
                return new ObjectPropertyStringDictionary(NameSource, CSharpName);
            else if (PropertyTypeName == "string list")
                return new ObjectPropertyStringList(NameSource, CSharpName);
            else if (MaximumLength != int.MaxValue)
                throw new ParsingException(sourceStream, "Specifiers not valid for this property type");
            else if (PropertyTypeName == "integer")
                return new ObjectPropertyInteger(NameSource, CSharpName);
            else if (PropertyTypeName == "enum")
                return new ObjectPropertyEnum(NameSource, CSharpName);
            else if (PropertyTypeName == "boolean")
                return new ObjectPropertyBoolean(NameSource, CSharpName);
            else if (PropertyTypeName == "item")
                if (ObjectSource != null)
                    return new ObjectPropertyItem(NameSource, CSharpName, ObjectSource);
                else
                    throw new ParsingException(sourceStream, "Object name not specified for item");
            else if (PropertyTypeName == "item list")
                if (ObjectSource != null)
                    return new ObjectPropertyItemList(NameSource, CSharpName, ObjectSource);
                else
                    throw new ParsingException(sourceStream, "Object name not specified for item list");
            else
                throw new ParsingException(sourceStream, $"Unknown property type {PropertyTypeName}");
        }

        private List<IObjectEvent> ParseEvents(IParsingSourceStream sourceStream, ref string line)
        {
            List<IObjectEvent> ObjectEventList = new List<IObjectEvent>();

            if (line != "events")
            {
                sourceStream.ReadLine();
                string HeaderLine = sourceStream.Line;
                if (HeaderLine != "events")
                    throw new ParsingException(sourceStream, "Expected: events");
            }

            while (!sourceStream.EndOfStream)
            {
                sourceStream.ReadLine();
                line = sourceStream.Line.Trim();
                if (string.IsNullOrEmpty(line))
                    break;

                foreach (IObjectEvent ObjectEvent in ObjectEventList)
                    if (ObjectEvent.Name == line)
                        throw new ParsingException(sourceStream, $"Event name {line} specified more than once");

                IObjectEvent NewEvent = ParseEvent(sourceStream, line);
                ObjectEventList.Add(NewEvent);
            }

            return ObjectEventList;
        }

        private IObjectEvent ParseEvent(IParsingSourceStream sourceStream, string line)
        {
            string Name = line.Trim();
            if (Name.Length <= 0)
                throw new ParsingException(sourceStream, "Event name cannot be empty");

            return new ObjectEvent(Name, ParserDomain.ToCSharpName(sourceStream, Name));
        }
    }
}
