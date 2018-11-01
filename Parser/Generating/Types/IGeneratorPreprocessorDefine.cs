using System.Collections.Generic;

namespace Parser
{
    public interface IGeneratorPreprocessorDefine
    {
        IDictionary<string, bool> PreprocessorDefineTable { get; }
        bool Connect(IGeneratorDomain domain);
    }
}
