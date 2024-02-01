using UnityEngine;

namespace GalaxyGourd.Grid
{
    public interface IGridCellOccupant : IGridInputReceiver
    {
        #region PROPERTIES

        GridCell Cell { get; set; }
        Transform Transform { get; }

        #endregion PROPERTIES
    }
}