namespace Parser
{
    public interface IGrid : IPanel
    {
        int ColumnCount { get; set; }
        int RowCount { get; set; }
        string ColumnWidths { get; set; }
        string RowHeights { get; set; }
        double[] ColumnWidthArray { get; }
        double[] RowHeightArray { get; }
    }
}
