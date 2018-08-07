using System.Collections;
using System.Collections.Generic;

namespace Parser
{
    public interface IGeneratorFormCollection : IList
    {
    }

    public interface IGeneratorFormCollection<T> : IList<T>, IGeneratorFormCollection
        where T : IGeneratorForm
    {
        new int Count { get; }
        new T this[int index] { get; set; }
    }
}
