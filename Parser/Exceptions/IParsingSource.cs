using System;
using System.Xaml;

namespace Parser
{
    public interface IParsingSource : IDisposable
    {
        string FileName { get; }
        string Line { get; }
        int LineIndex { get; }
        bool EndOfStream { get; }

        IParsingSource Open();
        IParsingSource OpenXamlFromFile(XamlSchemaContext context);
        IParsingSource OpenXamlFromBytes(byte[] contentBytes, XamlSchemaContext context);
        void Close();
        void ReadLine();
        string ReadToEnd();
        object LoadXaml();
    }
}
