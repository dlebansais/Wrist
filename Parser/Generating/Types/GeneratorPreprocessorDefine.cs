using System.Collections.Generic;

namespace Parser
{
    public class GeneratorPreprocessorDefine : IGeneratorPreprocessorDefine
    {
        public GeneratorPreprocessorDefine(IPreprocessorDefine translation)
        {
            PreprocessorDefineTable = translation.PreprocessorDefineTable;
        }

        public IDictionary<string, bool> PreprocessorDefineTable { get; private set; }

        public bool Connect(IGeneratorDomain domain)
        {
            return false;
        }
    }
}
