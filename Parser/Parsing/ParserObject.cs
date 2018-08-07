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

        private IObject Parse(string name, IParsingSource source)
        {
            List<string> StateList;
            IObjectPropertyCollection ObjectPropertyList;
            List<ITransition> TransitionList;
            List<IObjectEvent> ObjectEventList;
            string Line = null;

            try
            {
                StateList = ParseStates(source, ref Line);
                ObjectPropertyList = ParseObjectProperties(source, ref Line);
                TransitionList = ParseTransitions(source, ObjectPropertyList, ref Line);
                ObjectEventList = ParseEvents(source, ref Line);
            }
            catch (ParsingException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new ParsingException(source, e);
            }

            return new Object(name, ParserDomain.ToCSharpName(source, name), StateList, ObjectPropertyList, TransitionList, ObjectEventList);
        }

        private List<string> ParseStates(IParsingSource source, ref string line)
        {
            List<string> StateList = new List<string>();

            source.ReadLine();
            string HeaderLine = source.Line;
            if (HeaderLine != "states")
                throw new ParsingException(source, "Expected: states");

            while (!source.EndOfStream)
            {
                source.ReadLine();
                if (string.IsNullOrEmpty(source.Line))
                    break;

                string State = ParseState(source);
                StateList.Add(ParserDomain.ToCSharpName(source, State));
            }

            return StateList;
        }

        private string ParseState(IParsingSource source)
        {
            return source.Line.Trim();
        }

        private IObjectPropertyCollection ParseObjectProperties(IParsingSource source, ref string line)
        {
            IObjectPropertyCollection ObjectPropertyList = new ObjectPropertyCollection();
            ObjectPropertyList.Add(new ObjectPropertyState(new DeclarationSource("state", source), "State"));

            source.ReadLine();
            string HeaderLine = source.Line;
            if (HeaderLine != "properties")
                throw new ParsingException(source, "Expected: properties");

            while (!source.EndOfStream)
            {
                source.ReadLine();
                if (string.IsNullOrEmpty(source.Line))
                    break;

                IObjectProperty NewProperty = ParseProperty(source);

                foreach (IObjectProperty ObjectProperty in ObjectPropertyList)
                    if (ObjectProperty.NameSource.Name == NewProperty.NameSource.Name)
                        throw new ParsingException(source, $"Object already contains a property called {NewProperty.NameSource.Name}");

                ObjectPropertyList.Add(NewProperty);
            }

            return ObjectPropertyList;
        }

        private IObjectProperty ParseProperty(IParsingSource source)
        {
            IDeclarationSource NameSource;
            string Details;
            ParserDomain.ParseStringPair(source, ':', out NameSource, out Details);

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
                        throw new ParsingException(source, "Maximum length specified more than once");

                else if (Detail == "email address")
                    if (Category == ObjectPropertyStringCategory.Normal)
                        Category = ObjectPropertyStringCategory.EmailAddress;
                    else
                        throw new ParsingException(source, "Email address or password specified more than once");

                else if (Detail == "password")
                    if (Category == ObjectPropertyStringCategory.Normal)
                        Category = ObjectPropertyStringCategory.Password;
                    else
                        throw new ParsingException(source, "Email address or password specified more than once");

                else if (Detail == "encrypted")
                    if (!IsEncrypted)
                        IsEncrypted = true;
                    else
                        throw new ParsingException(source, "Encrypted specified more than once");

                else if (SplittedDetail.Length == 2 && SplittedDetail[0].Trim() == "object")
                    if (ObjectSource == null)
                    {
                        ObjectSource = new DeclarationSource(SplittedDetail[1].Trim(), source);
                        if (string.IsNullOrEmpty(ObjectSource.Name))
                            throw new ParsingException(source, "Invalid empty object name");
                    }
                    else
                        throw new ParsingException(source, "Object name specified more than once");

                else
                    throw new ParsingException(source, $"Unknown specifier {Detail}");
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
                throw new ParsingException(source, "Specifiers not valid for this property type");
            else if (PropertyTypeName == "integer")
                return new ObjectPropertyInteger(NameSource, CSharpName);
            else if (PropertyTypeName == "boolean")
                return new ObjectPropertyBoolean(NameSource, CSharpName);
            else if (PropertyTypeName == "item")
                if (ObjectSource != null)
                    return new ObjectPropertyItem(NameSource, CSharpName, ObjectSource);
                else
                    throw new ParsingException(source, "Object name not specified for item");
            else if (PropertyTypeName == "item list")
                if (ObjectSource != null)
                    return new ObjectPropertyItemList(NameSource, CSharpName, ObjectSource);
                else
                    throw new ParsingException(source, "Object name not specified for item list");
            else
                throw new ParsingException(source, $"Unknown property type {PropertyTypeName}");
        }

        private List<ITransition> ParseTransitions(IParsingSource source, IObjectPropertyCollection ObjectPropertyList, ref string line)
        {
            List<ITransition> TransitionList = new List<ITransition>();

            source.ReadLine();
            string HeaderLine = source.Line;
            if (HeaderLine != "transitions")
                throw new ParsingException(source, "Transitions expected");

            while (!source.EndOfStream)
            {
                ITransition Transition = ParseTransition(source, ObjectPropertyList, ref line);
                if (Transition == null)
                    break;

                TransitionList.Add(Transition);
            }

            return TransitionList;
        }

        private ITransition ParseTransition(IParsingSource source, IObjectPropertyCollection ObjectPropertyList, ref string line)
        {
            if (source.EndOfStream)
                throw new ParsingException(source, "Unexpected end of file");

            source.ReadLine();
            line = source.Line;
            if (string.IsNullOrEmpty(line) || line == "events")
                return null;

            string FromState = ParseTransitionState(source, "from", line);

            source.ReadLine();
            line = source.Line;
            if (string.IsNullOrEmpty(line))
                throw new ParsingException(source, "Unexpected empty line");

            string ToState = ParseTransitionState(source, "to", line);

            IObjectPropertyCollection PropertyListProvide = null;
            IObjectPropertyCollection PropertyListUnassign = null;

            while (!source.EndOfStream)
            {
                source.ReadLine();
                line = source.Line;
                if (string.IsNullOrEmpty(line))
                    break;

                string Detail = line.Trim();
                if (Detail == "provide")
                    if (PropertyListProvide == null)
                        PropertyListProvide = ParseTransitionPropertyList(source, ObjectPropertyList);
                    else
                        throw new ParsingException(source, "Provided property list specified more than once");

                else if (Detail == "unassign")
                    if (PropertyListUnassign == null)
                        PropertyListUnassign = ParseTransitionPropertyList(source, ObjectPropertyList);
                    else
                        throw new ParsingException(source, "Unassigned property list specified more than once");
            }

            if (PropertyListProvide == null)
                PropertyListProvide = new ObjectPropertyCollection();

            if (PropertyListUnassign == null)
                PropertyListUnassign = new ObjectPropertyCollection();

            ITransition NewTransition = new Transition(FromState, ToState, PropertyListProvide, PropertyListUnassign);
            return NewTransition;
        }

        private string ParseTransitionState(IParsingSource source, string prolog, string line)
        {
            line = line.Trim();
            if (!line.StartsWith(prolog))
                throw new ParsingException(source, $"Expected: {prolog}");

            line = line.Substring(prolog.Length);
            if (line.Length <= 2 || line[0] != ' ')
                throw new ParsingException(source, $"Expected: property name");

            string StateName = line.Trim();
            return StateName;
        }

        private IObjectPropertyCollection ParseTransitionPropertyList(IParsingSource source, IObjectPropertyCollection ObjectPropertyList)
        {
            source.ReadLine();
            if (string.IsNullOrEmpty(source.Line))
                throw new ParsingException(source, "At least one property name is expected");

            IObjectPropertyCollection PropertyList = new ObjectPropertyCollection();

            string Line = source.Line.Trim();
            string[] Splitted = Line.Split(',');

            foreach (string Detail in Splitted)
            {
                string PropertyName = Detail.Trim();

                if (PropertyName.Length == 0)
                    throw new ParsingException(source, "Property name cannot be empty");
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
                        throw new ParsingException(source, $"Property name {PropertyName} not found");
                }
            }

            if (PropertyList.Count == 0)
                throw new ParsingException(source, "At least one property name is expected");

            return PropertyList;
        }

        private List<IObjectEvent> ParseEvents(IParsingSource source, ref string line)
        {
            List<IObjectEvent> ObjectEventList = new List<IObjectEvent>();

            if (line != "events")
            {
                source.ReadLine();
                string HeaderLine = source.Line;
                if (HeaderLine != "events")
                    throw new ParsingException(source, "Expected: events");
            }

            while (!source.EndOfStream)
            {
                source.ReadLine();
                line = source.Line.Trim();
                if (string.IsNullOrEmpty(line))
                    break;

                foreach (IObjectEvent ObjectEvent in ObjectEventList)
                    if (ObjectEvent.Name == line)
                        throw new ParsingException(source, $"Event name {line} specified more than once");

                IObjectEvent NewEvent = ParseEvent(source, line);
                ObjectEventList.Add(NewEvent);
            }

            return ObjectEventList;
        }

        private IObjectEvent ParseEvent(IParsingSource source, string line)
        {
            string Name = line.Trim();
            if (Name.Length <= 0)
                throw new ParsingException(source, "Event name cannot be empty");

            return new ObjectEvent(Name, ParserDomain.ToCSharpName(source, Name));
        }
    }
}
