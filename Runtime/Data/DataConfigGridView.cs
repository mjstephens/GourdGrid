using System;
using UnityEngine;

namespace GalaxyGourd.Grid
{
    /// <summary>
    /// Holds data that can be used to influence construction of grid maps
    /// </summary>
    [CreateAssetMenu(
        fileName = "DAT_GridView", 
        menuName = "GG/UI/Grid/Standard View")]
    public class DataConfigGridView : ScriptableObject
    {
        #region PROPERTIES

        [Header("Grid")]
        [SerializeField] public int GridWidth;
        [SerializeField] public int GridHeight;
        
        [Header("View")]
        [SerializeField] public GameObject PrefabCellView;
        
        public int GetFlattenedIndexForCoords(int x, int y) => x + (GridWidth * y);

        #endregion PROPERTIES
        
        
        #region VALIDATION

        internal Action OnValidated;
        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                OnValidated?.Invoke();
            }
        }

        #endregion VALIDATION


        #region UTILITY

        public bool CoordsAreWithinGrid(Vector2Int coords)
        {
            if (coords.x < 0 || coords.y < 0)
                return false;

            if (coords.x >= GridWidth || coords.y >= GridHeight)
                return false;

            return true;
        }

        #endregion UTILITY
    }
}