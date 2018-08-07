using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace Parser
{
    public class ParsingException : Exception
    {
        public ParsingException(string directory, string message,
                                [CallerLineNumber] int lineNumber = 0,
                                [CallerFilePath] string filePath = null,
                                [CallerMemberName] string caller = null)
            : base(message)
        {
            Directory = directory;
            ParsingSource = null;
            LineNumber = lineNumber;
            FilePath = filePath;
            Caller = caller;
        }

        public ParsingException(IParsingSource parsingSource, string message,
                                [CallerLineNumber] int lineNumber = 0,
                                [CallerFilePath] string filePath = null,
                                [CallerMemberName] string caller = null)
            : base(message)
        {
            Directory = null;
            ParsingSource = parsingSource;
            LineNumber = lineNumber;
            FilePath = filePath;
            Caller = caller;
        }

        public ParsingException(IParsingSource parsingSource, Exception innerException,
                                [CallerLineNumber] int lineNumber = 0,
                                [CallerFilePath] string filePath = null,
                                [CallerMemberName] string caller = null)
            : base(innerException.Message, innerException)
        {
            Directory = null;
            ParsingSource = parsingSource;
            LineNumber = lineNumber;
            FilePath = filePath;
            Caller = caller;
        }

        public string Directory { get; private set; }
        public IParsingSource ParsingSource { get; private set; }
        public int LineNumber { get; private set; }
        public string FilePath { get; private set; }
        public string Caller { get; private set; }

        public void WriteDiagnostic(StreamWriter writer)
        {
            if (Directory != null)
                WriteDirectoryDiagnostic(writer);
            else
                WriteSourceDiagnostic(writer);
        }

        public void WriteDirectoryDiagnostic(StreamWriter writer)
        {
            writer.WriteLine($"Folder: {Directory}");
            writer.WriteLine($"Error:  {Message}");
            writer.WriteLine($"At line {LineNumber} of {Caller}");
        }

        public void WriteSourceDiagnostic(StreamWriter writer)
        {
            writer.WriteLine($"File:  {ParsingSource.FileName} at line {ParsingSource.LineIndex}");
            writer.WriteLine($"Line:  {ParsingSource.Line}");
            writer.WriteLine($"Error: {Message}");
            writer.WriteLine($"In {Caller} at line {LineNumber} of {FilePath}");
        }
    }
}
