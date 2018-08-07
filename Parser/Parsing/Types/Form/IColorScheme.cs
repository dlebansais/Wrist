using System.Collections.Generic;

namespace Parser
{
    public interface IColorScheme : IForm
    {
        string Name { get; }
        Dictionary<IDeclarationSource, string> Colors { get; }
    }
}
