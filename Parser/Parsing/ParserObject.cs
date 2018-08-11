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
            List<ITransition> TransitionList;
            List<IObjectEvent> ObjectEventList;
            string Line = null;

            try
            {
                StateList = ParseStates(sourceStream, ref Line);
                ObjectPropertyList = ParseObjectProperties(sourceStream, ref Line);
                TransitionList = ParseTransitions(sourceStream, ObjectPropertyList, ref Line);
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
            return new Object(name, CSharpname, StateList, ObjectPropertyList, TransitionList, ObjectEventList);
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
            ObjectPropertyList.Add(new ObjectPropertyState(new DeclarationSource("state", sourceStream), "State"));

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

            bool IsEncrypted = false;
            int MaximumLength = int.MaxValue;
            ObjectPropertyStringCategory Category = ObjectPropertyStringCategory.Normal;
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

                else if (Detail == "email address")
                    if (Category == ObjectPropertyStringCategory.Normal)
                        Category = ObjectPropertyStringCategory.EmailAddress;
                    else
                        throw new ParsingException(sourceStream, "Email address or password specified more than once");

                else if (Detail == "password")
                    if (Category == ObjectPropertyStringCategory.Normal)
                        Category = ObjectPropertyStringCategory.Password;
                    else
                        throw new ParsingException(sourceStream, "Email address or password specified more than once");

                else if (Detail == "encrypted")
                    if (!IsEncrypted)
                        IsEncrypted = true;
                    else
                        throw new ParsingException(sourceStream, "Encrypted specified more than once");

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
                return new ObjectPropertyString(NameSource, CSharpName, IsEncrypted, MaximumLength, Category);
            else if (PropertyTypeName == "readonly string")
                return new ObjectPropertyReadonlyString(NameSource, CSharpName, IsEncrypted);
            else if (PropertyTypeName == "string dictionary")
                return new ObjectPropertyStringDictionary(NameSource, CSharpName);
            else if (PropertyTypeName == "string list")
                return new ObjectPropertyStringList(NameSource, CSharpName);
            else if (MaximumLength != int.MaxValue || Category != ObjectPropertyStringCategory.Normal)
                throw new ParsingException(sourceStream, "Specifiers not valid for this property type");
            else if (PropertyTypeName == "integer")
                return new ObjectPropertyInteger(NameSource, CSharpName);
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

        private List<ITransition> ParseTransitions(IParsingSourceStream sourceStream, IObjectPropertyCollection ObjectPropertyList, ref string line)
        {
            List<ITransition> TransitionList = new List<ITransition>();

            sourceStream.ReadLine();
            string HeaderLine = sourceStream.Line;
            if (HeaderLine != "transitions")
                throw new ParsingException(sourceStream, "Transitions expected");

            while (!sourceStream.EndOfStream)
            {
                ITransition Transition = ParseTransition(sourceStream, ObjectPropertyList, ref line);
                if (Transition == null)
                    break;

                TransitionList.Add(Transition);
            }

            return TransitionList;
        }

        private ITransition ParseTransition(IParsingSourceStream sourceStream, IObjectPropertyCollection ObjectPropertyList, ref string line)
        {
            if (sourceStream.EndOfStream)
                throw new ParsingException(sourceStream, "Unexpected end of file");

            sourceStream.ReadLine();
            line = sourceStream.Line;
            if (string.IsNullOrEmpty(line) || line == "events")
                return null;

            string FromState = ParseTransitionState(sourceStream, "from", line);

            sourceStream.ReadLine();
            line = sourceStream.Line;
            if (string.IsNullOrEmpty(line))
                throw new ParsingException(sourceStream, "Unexpected empty line");

            string ToState = ParseTransitionState(sourceStream, "to", line);

            IObjectPropertyCollection PropertyListProvide = null;
            IObjectPropertyCollection PropertyListUnassign = null;

            while (!sourceStream.EndOfStream)
            {
                sourceStream.ReadLine();
                line = sourceStream.Line;
                if (string.IsNullOrEmpty(line))
                    break;

                string Detail = line.Trim();
                if (Detail == "provide")
                    if (PropertyListProvide == null)
                        PropertyListProvide = ParseTransitionPropertyList(sourceStream, ObjectPropertyList);
                    else
                        throw new ParsingException(sourceStream, "Provided property list specified more than once");

                else if (Detail == "unassign")
                    if (PropertyListUnassign == null)
                        PropertyListUnassign = ParseTransitionPropertyList(sourceStream, ObjectPropertyList);
                    else
                        throw new ParsingException(sourceStream, "Unassigned property list specified more than once");
            }

            if (PropertyListProvide == null)
                PropertyListProvide = new ObjectPropertyCollection();

            if (PropertyListUnassign == null)
                PropertyListUnassign = new ObjectPropertyCollection();

            ITransition NewTransition = new Transition(FromState, ToState, PropertyListProvide, PropertyListUnassign);
            return NewTransition;
        }

        private string ParseTransitionState(IParsingSourceStream sourceStream, string prolog, string line)
        {
            line = line.Trim();
            if (!line.StartsWith(prolog))
                throw new ParsingException(sourceStream, $"Expected: {prolog}");

            line = line.Substring(prolog.Length);
            if (line.Length <= 2 || line[0] != ' ')
                throw new ParsingException(sourceStream, $"Expected: property name");

            string StateName = line.Trim();
            return StateName;
        }

        private IObjectPropertyCollection ParseTransitionPropertyList(IParsingSourceStream sourceStream, IObjectPropertyCollection ObjectPropertyList)
        {
            sourceStream.ReadLine();
            if (string.IsNullOrEmpty(sourceStream.Line))
                throw new ParsingException(sourceStream, "At least one property name is expected");

            IObjectPropertyCollection PropertyList = new ObjectPropertyCollection();

            string Line = sourceStream.Line.Trim();
            string[] Splitted = Line.Split(',');

            foreach (string Detail in Splitted)
            {
                string PropertyName = Detail.Trim();

                if (PropertyName.Length == 0)
                    throw new ParsingException(sourceStream, "Property name cannot be empty");
                else
                {
                    IObjectProperty MatchingObjectProperty = null;
                    foreach (IObjectProperty ObjectProperty in ObjectPropertyList)
                        if (PropertyName == ObjectProperty.NameSource.Name)
                        {
                            MatchingObjectProperty = ObjectProperty;
                            break;
                        }

                    if (MatchingObjectProperty != null)
                        PropertyList.Add(MatchingObjectProperty);
                    else
                        throw new ParsingException(sourceStream, $"Property name {PropertyName} not found");
                }
            }

            if (PropertyList.Count == 0)
                throw new ParsingException(sourceStream, "At least one property name is expected");

            return PropertyList;
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
