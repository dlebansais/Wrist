using System.Windows.Controls;

namespace Parser
{
    public interface IStackPanel : IPanel
    {
        Orientation Orientation { get; set; }
    }
}
