using System.Collections;
using System.Collections.Generic;

namespace Parser
{
    public interface IFormCollection : IList
    {
    }

    public interface IFormCollection<T> : IList<T>, IFormCollection
        where T : IForm
    {
        new int Count { get; }
        new T this[int index] { get; set; }
    }
}
