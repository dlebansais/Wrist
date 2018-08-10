using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Parser
{
    public class ParsingException : Exception
    {
        public ParsingException(int code, string directory, string message,
                                [CallerLineNumber] int lineNumber = 0,
                                [CallerFilePath] string filePath = null,
                                [CallerMemberName] string caller = null)
            : base(message)
        {
            Code = code;
            Directory = directory;
            ParsingSource = null;
            LineNumber = lineNumber;
            FilePath = ShortFilePath(filePath);
            Caller = caller;
        }

        public ParsingException(string directory, string message,
                                [CallerLineNumber] int lineNumber = 0,
                                [CallerFilePath] string filePath = null,
                                [CallerMemberName] string caller = null)
            : base(message)
        {
            Code = 0;
            Directory = directory;
            ParsingSource = null;
            LineNumber = lineNumber;
            FilePath = ShortFilePath(filePath);
            Caller = caller;
        }

        public ParsingException(IParsingSourceStream sourceStream, string message,
                                [CallerLineNumber] int lineNumber = 0,
                                [CallerFilePath] string filePath = null,
                                [CallerMemberName] string caller = null)
            : base(message)
        {
            Code = 0;
            Directory = null;
            ParsingSource = sourceStream.FreezedPosition();
            LineNumber = lineNumber;
            FilePath = ShortFilePath(filePath);
            Caller = caller;
        }

        public ParsingException(IParsingSource parsingSource, string message,
                                [CallerLineNumber] int lineNumber = 0,
                                [CallerFilePath] string filePath = null,
                                [CallerMemberName] string caller = null)
            : base(message)
        {
            Code = 0;
            Directory = null;
            ParsingSource = parsingSource;
            LineNumber = lineNumber;
            FilePath = ShortFilePath(filePath);
            Caller = caller;
        }

        public ParsingException(IParsingSourceStream sourceStream, Exception innerException,
                                [CallerLineNumber] int lineNumber = 0,
                                [CallerFilePath] string filePath = null,
                                [CallerMemberName] string caller = null)
            : base(innerException.Message, innerException)
        {
            Code = 0;
            Directory = null;
            ParsingSource = sourceStream.FreezedPosition();
            LineNumber = lineNumber;
            FilePath = ShortFilePath(filePath);
            Caller = caller;
        }

        public ParsingException(IParsingSource parsingSource, Exception innerException,
                                [CallerLineNumber] int lineNumber = 0,
                                [CallerFilePath] string filePath = null,
                                [CallerMemberName] string caller = null)
            : base(innerException.Message, innerException)
        {
            Code = 0;
            Directory = null;
            ParsingSource = parsingSource;
            LineNumber = lineNumber;
            FilePath = ShortFilePath(filePath);
            Caller = caller;
        }

        public int Code { get; private set; }
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
            writer.WriteLine();
            writer.WriteLine($"WTE{Code.ToString("D5")}");
            writer.WriteLine($"Folder: {Directory}");
            writer.WriteLine($"Error:  {Message}");
            writer.WriteLine($"At line {LineNumber} of \"{FilePath}\" in '{Caller}'");
            writer.WriteLine();
        }

        public void WriteSourceDiagnostic(StreamWriter writer)
        {
            writer.WriteLine();
            writer.WriteLine($"WTE{Code.ToString("D5")}");
            writer.WriteLine($"File:  {ParsingSource.FileName} at line {ParsingSource.LineIndex}");
            writer.WriteLine($"Line:  {ParsingSource.Line}");
            writer.WriteLine($"Error: {Message}");
            writer.WriteLine($"At line {LineNumber} of \"{FilePath}\" in '{Caller}'");
            writer.WriteLine();
        }

        private string ShortFilePath(string s)
        {
            int Index = s.IndexOf("Wrist\\");
            if (Index < 0)
                Index = s.IndexOf("Wrist/");

            if (Index > 0)
                return s.Substring(Index + 6);
            else
                return s;
        }
    }
}
