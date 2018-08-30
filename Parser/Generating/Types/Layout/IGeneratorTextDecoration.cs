using System.Collections.Generic;

namespace Parser
{
    public interface IGeneratorTextDecoration : IGeneratorLayoutElement
    {
        string Text { get; }
        Dictionary<IGeneratorPage, string> LinkedPageTable { get; }
    }
}
