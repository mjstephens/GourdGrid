using UnityEngine;

namespace GalaxyGourd.Grid
{
    [RequireComponent(typeof(RectTransform))]
    public class UIScalableGridCell : GridCell
    {
        #region VARIABLES

        public RectTransform CellRect { get; private set; }

        #endregion VARIABLES


        #region INITIALIZATION

        public override void SetupCell(int x, int y, int index)
        {
            base.SetupCell(x, y, index);
            
            CellRect = GetComponent<RectTransform>();
        }

        #endregion INITIALIZATION
    }
}