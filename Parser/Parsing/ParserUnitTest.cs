using System;
using System.Collections.Generic;

namespace Parser
{
    public class ParserUnitTest : FormParser<IUnitTest>
    {
        public ParserUnitTest(string folderName, string extension)
            : base(folderName, extension)
        {
        }

        public override IUnitTest Parse(string fileName, IDictionary<ConditionalDefine, bool> conditionalDefineTable)
        {
            IParsingSourceStream SourceStream = ParsingSourceStream.CreateFromFileName(fileName, conditionalDefineTable);

            try
            {
                using (SourceStream.Open())
                {
                    return Parse(fileName, SourceStream);
                }
            }
            catch (ParsingException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new ParsingException(231, SourceStream, e);
            }
        }

        private IUnitTest Parse(string fileName, IParsingSourceStream SourceStream)
        {
            List<ITestingOperation> Operations = new List<ITestingOperation>();

            while (!SourceStream.EndOfStream)
            {
                SourceStream.ReadLine();
                string Line = SourceStream.Line;
                if (string.IsNullOrEmpty(Line))
                    break;

                string[] Splitted = Line.Split(',');

                if (Splitted.Length < 4)
                    throw new ParsingException(232, SourceStream, "Invalid line in the unit testing file.");

                IDeclarationSource PageName = new DeclarationSource(Splitted[0].Trim(), SourceStream);
                string Operation = Splitted[1].Trim();
                IDeclarationSource AreaName = new DeclarationSource(Splitted[2].Trim(), SourceStream);
                string Parameters = Splitted[3];
                for (int i = 4; i < Splitted.Length; i++)
                    Parameters += "," + Splitted[i];

                ITestingOperation NewOperation;

                if (Operation == "click")
                {
                    IDeclarationSource ComponentName = new DeclarationSource(Parameters.Trim(), SourceStream);
                    NewOperation = new ClickOperation(PageName, AreaName, ComponentName);
                }

                else if (Operation == "toggle")
                {
                    IDeclarationSource ComponentName = new DeclarationSource(Parameters.Trim(), SourceStream);
                    NewOperation = new ToggleOperation(PageName, AreaName, ComponentName);
                }

                else if (Operation == "fill")
                {
                    string[] FillParameters = Parameters.Split('=');
                    if (FillParameters.Length < 2)
                        throw new ParsingException(233, SourceStream, "Invalid line in the unit testing file.");

                    IDeclarationSource ComponentName = new DeclarationSource(FillParameters[0].Trim(), SourceStream);
                    string Content = FillParameters[1].Trim();

                    NewOperation = new FillOperation(PageName, AreaName, ComponentName, Content);
                }

                else if (Operation == "select")
                {
                    string[] FillParameters = Parameters.Split('=');
                    if (FillParameters.Length < 2)
                        throw new ParsingException(234, SourceStream, "Invalid line in the unit testing file.");

                    IDeclarationSource ComponentName = new DeclarationSource(FillParameters[0].Trim(), SourceStream);

                    int Index;
                    if (!int.TryParse(FillParameters[1].Trim(), out Index))
                        throw new ParsingException(235, SourceStream, "Invalid line in the unit testing file.");

                    NewOperation = new SelectOperation(PageName, AreaName, ComponentName, Index);
                }
                else
                    throw new ParsingException(236, SourceStream, $"Unknown unit testing operation '{Operation}'.");

                Operations.Add(NewOperation);
            }

            return new UnitTest(fileName, Operations);
        }
    }
}
