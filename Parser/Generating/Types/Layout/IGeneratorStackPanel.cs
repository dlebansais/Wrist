using System.Windows.Controls;

namespace Parser
{
    public interface IGeneratorStackPanel : IGeneratorPanel
    {
        Orientation Orientation { get; }
    }
}
