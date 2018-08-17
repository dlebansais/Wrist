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
                throw new ParsingException(0, SourceStream, e);
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

            return new Dynamic(name, ParserDomain.ToXamlName(sourceStream, name, "Page"), Properties);
        }

        private IDynamicProperty Parse(IParsingSourceStream sourceStream, ref string line, ref int indentation, ref bool useTab)
        {
            if (!line.EndsWith(":"))
                throw new ParsingException(0, sourceStream, "");

            string PropertyName = line.Substring(0, line.Length - 1).Trim();
            if (string.IsNullOrEmpty(PropertyName))
                throw new ParsingException(0, sourceStream, "");

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
                    OperationStack.Push(new DynamicOperation(DynamicOperationTypes.NOT));
                else if (Text == "OR")
                    OperationStack.Push(new DynamicOperation(DynamicOperationTypes.OR));
                else if (Text == "AND")
                    OperationStack.Push(new DynamicOperation(DynamicOperationTypes.AND));
                else if (Text == "IS EMPTY")
                    OperationStack.Push(new DynamicOperation(DynamicOperationTypes.IS_EMPTY));
                else
                {
                    IDynamicOperation Operand = new PropertyValueOperation(Text);

                    while (OperationStack.Count > 0 && (OperationStack.Peek() is DynamicOperation AsOperation))
                    {
                        if (AsOperation.Operand1 == null)
                            AsOperation.Operand1 = Operand;
                        else if (AsOperation.Operand2 == null)
                            AsOperation.Operand2 = Operand;
                        else
                            throw new ParsingException(0, sourceStream, "");

                        if (IsOperationUnary(AsOperation.Type))
                        {
                            RootOperation = OperationStack.Pop();
                            Operand = RootOperation;
                        }
                        else if (IsOperationBinary(AsOperation.Type))
                        {
                            if (AsOperation.Operand2 != null)
                            {
                                RootOperation = OperationStack.Pop();
                                Operand = RootOperation;
                            }
                            else
                                break;
                        }
                        else
                            throw new ParsingException(0, sourceStream, "");
                    }

                    if (OperationStack.Count == 0)
                        RootOperation = Operand;
                }
            }

            if (RootOperation == null)
                throw new ParsingException(0, sourceStream, "");

            if (OperationStack.Count > 0)
                throw new ParsingException(0, sourceStream, "");

            return new DynamicProperty(PropertyName, RootOperation);
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
                throw new ParsingException(0, sourceStream, "");
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
                                throw new ParsingException(0, sourceStream, "");
                            break;
                        }
                    if (j < indentation)
                        break;

                    i++;
                }
            }

            if (i == 0)
                throw new ParsingException(0, sourceStream, "");

            return i;
        }

        private static bool IsOperationUnary(DynamicOperationTypes type)
        {
            return type == DynamicOperationTypes.NOT || type == DynamicOperationTypes.IS_EMPTY;
        }

        private static bool IsOperationBinary(DynamicOperationTypes type)
        {
            return type == DynamicOperationTypes.OR || type == DynamicOperationTypes.AND;
        }
    }
}
