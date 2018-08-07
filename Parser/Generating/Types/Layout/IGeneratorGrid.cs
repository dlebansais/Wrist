namespace Parser
{
    public interface IGeneratorGrid : IGeneratorPanel
    {
        int ColumnCount { get; }
        int RowCount { get; }
        double[] ColumnWidthArray { get; }
        double[] RowHeightArray { get; }
    }
}
