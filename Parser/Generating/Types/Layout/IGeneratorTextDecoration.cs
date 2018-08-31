using System.Collections.Generic;

namespace Parser
{
    public interface IGeneratorTextDecoration : IGeneratorLayoutElement
    {
        string Text { get; }
        List<object> LinkedPageList { get; }
    }
}
