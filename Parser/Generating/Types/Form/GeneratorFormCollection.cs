using System.Collections.Generic;

namespace Parser
{
    public class GeneratorFormCollection<T> : List<T>, IGeneratorFormCollection<T>
        where T : class, IGeneratorForm
    {
    }
}
