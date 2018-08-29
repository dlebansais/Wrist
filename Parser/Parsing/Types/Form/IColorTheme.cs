using System.Collections.Generic;

namespace Parser
{
    public interface IColorTheme : IForm, IConnectable
    {
        string Name { get; }
        Dictionary<IDeclarationSource, string> Colors { get; }
    }
}
