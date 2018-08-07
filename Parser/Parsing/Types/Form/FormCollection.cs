using System.Collections.Generic;

namespace Parser
{
    public class FormCollection<T> : List<T>, IFormCollection<T>
        where T : class, IForm
    {
    }
}
