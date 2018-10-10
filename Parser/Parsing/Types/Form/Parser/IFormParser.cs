using System.Collections.Generic;

namespace Parser
{
    public interface IFormParser
    {
        string FolderName { get; }
        string Extension { get; }
        IFormCollection ParsedResult { get; }
        void InitResult();
        IForm Parse(string fileName, IDictionary<ConditionalDefine, bool> conditionalDefineTable);
    }
    
    public interface IFormParser<T> : IFormParser
        where T : class, IForm
    {
        new IFormCollection<T> ParsedResult { get; }
        new T Parse(string fileName, IDictionary<ConditionalDefine, bool> conditionalDefineTable);
    }
}
