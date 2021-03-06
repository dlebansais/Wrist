﻿using System.Collections.Generic;

namespace Parser
{
    public interface IGeneratorTranslation
    {
        IDictionary<string, IDictionary<string, string>> TranslationTable { get; }
        bool Connect(IGeneratorDomain domain);
        void Generate(string outputFolderName, string appNamespace);
    }
}
