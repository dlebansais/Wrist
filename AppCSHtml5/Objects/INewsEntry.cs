using System.Collections.Generic;

namespace AppCSHtml5
{
    public interface INewsEntry
    {
        NewsEntryStates State { get; set; }
        string Title { get; }
        string Text { get; }
    }
}
