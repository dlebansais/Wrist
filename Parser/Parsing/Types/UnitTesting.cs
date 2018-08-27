using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Parser
{
    public class UnitTesting : IUnitTesting
    {
        public UnitTesting(string unitTestingFile)
        {
            UnitTestingFile = unitTestingFile;
        }

        public string UnitTestingFile { get; private set; }
        public List<ITestingOperation> Operations { get; private set; }

        public void Process()
        {
            IParsingSourceStream SourceStream = ParsingSourceStream.CreateFromFileName(UnitTestingFile);

            try
            {
                using (SourceStream.Open())
                {
                    Process(SourceStream);
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

        public void Process(IParsingSourceStream SourceStream)
        {
            Operations = new List<ITestingOperation>();

            while (!SourceStream.EndOfStream)
            {
                SourceStream.ReadLine();
                string Line = SourceStream.Line;
                if (string.IsNullOrEmpty(Line))
                    break;

                string[] Splitted = Line.Split(',');

                if (Splitted.Length < 4)
                    throw new ParsingException(0, SourceStream, "Invalid line in the unit testing file.");

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
                        throw new ParsingException(0, SourceStream, "Invalid line in the unit testing file.");

                    IDeclarationSource ComponentName = new DeclarationSource(FillParameters[0].Trim(), SourceStream);
                    string Content = FillParameters[1].Trim();

                    NewOperation = new FillOperation(PageName, AreaName, ComponentName, Content);
                }

                else if (Operation == "select")
                {
                    string[] FillParameters = Parameters.Split('=');
                    if (FillParameters.Length < 2)
                        throw new ParsingException(0, SourceStream, "Invalid line in the unit testing file.");

                    IDeclarationSource ComponentName = new DeclarationSource(FillParameters[0].Trim(), SourceStream);

                    int Index;
                    if (!int.TryParse(FillParameters[1].Trim(), out Index))
                        throw new ParsingException(0, SourceStream, "Invalid line in the unit testing file.");

                    NewOperation = new SelectOperation(PageName, AreaName, ComponentName, Index);
                }
                else
                    throw new ParsingException(0, SourceStream, $"Unknown unit testing operation '{Operation}'.");

                Operations.Add(NewOperation);
            }
        }

        public bool Connect(IDomain domain)
        {
            bool IsConnected = false;

            foreach (TestingOperation Operation in Operations)
                IsConnected |= Operation.Connect(domain);

            return IsConnected;
        }
    }
}
