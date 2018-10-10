using System;
using System.Collections.Generic;
using System.Xaml;

namespace Parser
{
    public interface IParsingSourceStream : IDisposable
    {
        string FileName { get; }
        IDictionary<ConditionalDefine, bool> ConditionalDefineTable { get; }
        string Line { get; }
        int LineIndex { get; }
        bool EndOfStream { get; }

        IParsingSourceStream Open();
        IParsingSourceStream OpenXamlFromFile(XamlSchemaContext context);
        IParsingSourceStream OpenXamlFromString(string content, XamlSchemaContext context);
        void Close();
        void ReadLine();
        string ReadToEnd();
        object LoadXaml();

        IParsingSource FreezedPosition();
    }
}
