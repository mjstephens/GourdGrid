using System.Collections.Generic;
using UnityEngine;

namespace GalaxyGourd.Grid
{
    /// <summary>
    /// Receives and broadcasts input related to a grid instance. Input is received by GridView
    /// </summary>
    public abstract class GridInput : MonoBehaviour
    {
        #region VARIABLES
        
        protected GridCell _pointerCell;
        protected bool _inputEnabled;
        private readonly List<IGridInputReceiver> _components = new();

        #endregion VARIABLES


        #region INITIALIZATION
        
        public void SetGridInputEnabled(bool isEnabled)
        {
            _inputEnabled = isEnabled;
        }

        #endregion INITIALIZATION
        
        
        #region REGISTRATION

        public void RegisterComponent(IGridInputReceiver component)
        {
            _components.Add(component);
        }

        public void RemoveComponent(IGridInputReceiver component)
        {
            _components.Remove(component);
        }

        #endregion REGISTRATION


        #region TICK

        private void Update()
        {
            if (!_inputEnabled)
                return;
            
            GridTick(Time.deltaTime);
        }

        protected abstract void GridTick(float delta);

        #endregion TICK

        
        #region INPUT

        protected void OnGridInput(DataInputValuesGrid inputData)
        {
            // Enter/exit grid
            if (inputData.DidEnterThisFrame)
            {
                foreach (IGridInputReceiver component in _components)
                    component.DoGridPointerEnter();
            }
            else if (inputData.DidExitThisFrame)
            {
                foreach (IGridInputReceiver component in _components)
                    component.DoGridPointerExit();
            }
            
            // Check for click/release
            if (_pointerCell)
            {
                if (inputData.WasPressedThisFrame)
                {
                    foreach (IGridInputReceiver component in _components)
                        component.DoCellPointerSelect(_pointerCell);
                    _pointerCell.SetCellPointerState(GridCellPointerState.Down);
                }
                else if (inputData.WasReleasedThisFrame)
                {
                    foreach (IGridInputReceiver component in _components)
                        component.DoCellPointerSelectRelease(_pointerCell);
                    _pointerCell.SetCellPointerState(GridCellPointerState.Hover);
                }
            }
            else if (inputData.WasReleasedThisFrame)
            {
                foreach (IGridInputReceiver component in _components)
                    component.DoGridPointerReleaseOffGrid();
            }

            // Right-click + release
            if (inputData.WasAlternatePressedThisFrame)
            {
                if (_pointerCell)
                {
                    foreach (IGridInputReceiver component in _components)
                        component.DoCellPointerAlternateSelect(_pointerCell);
                }
                
                foreach (IGridInputReceiver component in _components)
                    component.DoAlternateSelect();
            }
            else if (inputData.WasAlternateReleasedThisFrame && _pointerCell)
            {
                foreach (IGridInputReceiver component in _components)
                    component.DoCellPointerAlternateSelectRelease(_pointerCell);
                _pointerCell.SetCellPointerState(GridCellPointerState.Hover);
            }

            // Find the closest cell to the pointer position using overide logic
            GridCell currentPointer = inputData.ActiveCell;
            if (_pointerCell == currentPointer)
            {
                // Already pointing to this cell
                return;
            }
                
            // Un-select previous cell
            if (_pointerCell)
            {
                _pointerCell.SetCellPointerState(GridCellPointerState.None);
                foreach (IGridInputReceiver component in _components)
                    component.DoCellPointerExit(_pointerCell);
            }

            // Select new cell
            _pointerCell = currentPointer;
            if (_pointerCell)
            {
                _pointerCell.SetCellPointerState(GridCellPointerState.Hover);
                foreach (IGridInputReceiver component in _components)
                    component.DoCellPointerEnter(_pointerCell);
            }
        }
        
        #endregion INPUT
    }
}