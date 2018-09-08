using System;
using System.Collections.Generic;
using System.IO;

namespace Parser
{
    public class ParserDynamic : FormParser<IDynamic>
    {
        public ParserDynamic(string folderName, string extension)
            : base(folderName, extension)
        {
        }

        public override IDynamic Parse(string fileName)
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
                throw new ParsingException(208, SourceStream, e);
            }
        }

        private IDynamic Parse(string name, IParsingSourceStream sourceStream)
        {
            IDynamicPropertyCollection Properties = new DynamicPropertyCollection();

            sourceStream.ReadLine();
            string Line = sourceStream.Line;
            int Indentation = -1;
            bool UseTab = false;

            while (Line != null)
            {
                IDynamicProperty Property = Parse(sourceStream, ref Line, ref Indentation, ref UseTab);
                Properties.Add(Property);
            }

            string FileName = ParserDomain.ToCSharpName(sourceStream, name + "PageDynamic");
            string XamlPageName = ParserDomain.ToXamlName(sourceStream, name, "Page");
            return new Dynamic(name, FileName, XamlPageName, Properties);
        }

        private IDynamicProperty Parse(IParsingSourceStream sourceStream, ref string line, ref int indentation, ref bool useTab)
        {
            while (string.IsNullOrEmpty(line) && !sourceStream.EndOfStream)
            {
                sourceStream.ReadLine();
                line = sourceStream.Line;
            }

            IDeclarationSource PropertySource;
            string ResultValue;
            ParserDomain.ParseStringPair(sourceStream, ':', out PropertySource, out ResultValue);

            if (string.IsNullOrEmpty(PropertySource.Name))
                throw new ParsingException(209, sourceStream, "Missing dynamic property name.");

            string CSharpName = ParserDomain.ToCSharpName(PropertySource.Source, PropertySource.Name);

            DynamicOperationResults Result;
            if (ResultValue == "boolean")
                Result = DynamicOperationResults.Boolean;
            else
                throw new ParsingException(210, sourceStream, $"Invalid dynamic property type '{ResultValue}'.");

            Stack<IDynamicOperation> OperationStack = new Stack<IDynamicOperation>();
            IDynamicOperation RootOperation = null;

            for (;;)
            {
                sourceStream.ReadLine();
                line = sourceStream.Line;

                if (string.IsNullOrEmpty(line))
                    break;

                if (indentation < 0)
                    MeasureIndentation(sourceStream, ref indentation, ref useTab);

                int Depth = GetIndentation(sourceStream, indentation, useTab);

                string Text = line.Trim();
                if (Text == "NOT")
                    OperationStack.Push(new UnaryOperation(DynamicOperationTypes.NOT));
                else if (Text == "OR")
                    OperationStack.Push(new BinaryOperation(DynamicOperationTypes.OR));
                else if (Text == "AND")
                    OperationStack.Push(new BinaryOperation(DynamicOperationTypes.AND));
                else if (Text == "EQUALS")
                    OperationStack.Push(new BinaryOperation(DynamicOperationTypes.EQUALS));
                else if (Text == "GREATER THAN")
                    OperationStack.Push(new BinaryOperation(DynamicOperationTypes.GREATER_THAN));
                else if (Text == "IS EMPTY")
                    OperationStack.Push(new UnaryOperation(DynamicOperationTypes.IS_EMPTY));
                else
                {
                    IDynamicOperation Operand;

                    int IntegerConstantValue;
                    if (int.TryParse(Text, out IntegerConstantValue))
                        Operand = new IntegerConstantOperation(IntegerConstantValue);
                    else
                    {
                        IDeclarationSource ObjectSource;
                        IDeclarationSource MemberSource;
                        IDeclarationSource KeySource;
                        if (!ParserDomain.TryParseObjectProperty(sourceStream, Text, out ObjectSource, out MemberSource, out KeySource))
                            throw new ParsingException(211, sourceStream, $"Expected operator, integer constant or object property.");

                        ComponentInfo Info = new ComponentInfo();
                        Info.ObjectSource = ObjectSource;
                        Info.MemberSource = MemberSource;
                        Info.KeySource = KeySource;

                        ComponentProperty ValueProperty = new ComponentProperty(Info);

                        Operand = new PropertyValueOperation(ValueProperty);
                    }

                    while (OperationStack.Count > 0)
                    {
                        IDynamicOperation CurrentOperation = OperationStack.Peek();

                        if ((CurrentOperation is IUnaryOperation AsUnary) && (AsUnary.Operand == null))
                        {
                            AsUnary.SetOperand(Operand);

                            RootOperation = OperationStack.Pop();
                            Operand = RootOperation;
                        }

                        else if (CurrentOperation is IBinaryOperation AsBinary)
                        {
                            if (AsBinary.Operand1 == null)
                            {
                                AsBinary.SetOperand1(Operand);
                                break;
                            }
                            else
                            {
                                AsBinary.SetOperand2(Operand);

                                RootOperation = OperationStack.Pop();
                                Operand = RootOperation;
                            }
                        }
                        else
                            throw new ParsingException(212, sourceStream, "Operand not following an operator.");
                    }

                    if (OperationStack.Count == 0)
                        RootOperation = Operand;
                }
            }

            if (RootOperation == null)
                throw new ParsingException(213, sourceStream, $"Dynamic property '{PropertySource.Name}' with no expression.");

            if (OperationStack.Count > 0)
                throw new ParsingException(214, sourceStream, $"Dynamic property '{PropertySource.Name}' not completely specified.");

            return new DynamicProperty(PropertySource, CSharpName, Result, RootOperation);
        }

        private void MeasureIndentation(IParsingSourceStream sourceStream, ref int indentation, ref bool useTab)
        {
            string Line = sourceStream.Line;

            int i = 0;
            while (i < Line.Length && Line[i] == ' ')
                i++;

            if (i == 0 && i < Line.Length && Line[i] == '\t')
            {
                indentation = 1;
                useTab = true;
            }
            else if (i > 0)
            {
                indentation = i;
                useTab = false;
            }
            else
                throw new ParsingException(215, sourceStream, "Indentation expected.");
        }

        private int GetIndentation(IParsingSourceStream sourceStream, int indentation, bool useTab)
        {
            string Line = sourceStream.Line;
            int i = 0;

            if (useTab)
            {
                while (i < Line.Length && Line[i] == '\t')
                    i++;
            }
            else
            {
                while ((i + 1) * indentation <= Line.Length)
                {
                    int j;
                    for (j = 0; j < indentation; j++)
                        if (Line[(i * indentation) + j] != ' ')
                        {
                            if (j != 0)
                                throw new ParsingException(216, sourceStream, "Expression partially indented.");
                            break;
                        }
                    if (j < indentation)
                        break;

                    i++;
                }
            }

            if (i == 0)
                throw new ParsingException(215, sourceStream, "Indentation expected.");

            return i;
        }

        private static bool IsOperationUnary(DynamicOperationTypes type)
        {
            return type == DynamicOperationTypes.NOT || type == DynamicOperationTypes.IS_EMPTY;
        }

        private static bool IsOperationBinary(DynamicOperationTypes type)
        {
            return type == DynamicOperationTypes.OR || type == DynamicOperationTypes.AND || type == DynamicOperationTypes.EQUALS || type == DynamicOperationTypes.GREATER_THAN;
        }
    }
}
