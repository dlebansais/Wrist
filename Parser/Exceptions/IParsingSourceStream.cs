using System;
using System.Xaml;

namespace Parser
{
    public interface IParsingSourceStream : IDisposable
    {
        string FileName { get; }
        string Line { get; }
        int LineIndex { get; }
        bool EndOfStream { get; }

        IParsingSourceStream Open();
        IParsingSourceStream OpenXamlFromFile(XamlSchemaContext context);
        IParsingSourceStream OpenXamlFromBytes(byte[] contentBytes, XamlSchemaContext context);
        void Close();
        void ReadLine();
        string ReadToEnd();
        object LoadXaml();

        IParsingSource FreezedPosition();
    }
}
