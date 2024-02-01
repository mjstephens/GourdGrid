namespace GalaxyGourd.Grid
{
    public struct DataGridAStarRequest
    {
        public Grid Grid;
        public IGridView View;
        public IGridCellAStarNavigable Source;
        public IGridCellAStarNavigable Destination;
        public string RequestTag;
    }
}