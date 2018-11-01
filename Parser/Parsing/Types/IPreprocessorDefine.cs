using System.Collections.Generic;

namespace Parser
{
    public interface IPreprocessorDefine
    {
        string PreprocessorDefineFile { get; }
        IDictionary<string, bool> PreprocessorDefineTable { get; }
        void Process(IDictionary<ConditionalDefine, bool> conditionalDefineTable);
        bool Connect(IDomain domain);
    }
}
