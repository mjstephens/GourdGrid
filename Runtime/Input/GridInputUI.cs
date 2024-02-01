using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GalaxyGourd.Grid
{
    public class GridInputUI : GridInput
    {
        #region VARIABLES
        
        public DataInputValuesGrid InputValues => _inputValues;
        public Camera UICamera => _camera;

        private UIScalableGrid _grid;
        private Canvas _overlay;
        private Camera _camera;
        private DataInputValuesGrid _inputValues;
        private bool _active;

        #endregion VARIABLES
        
        
        #region INITIALIZATION

        private void Awake()
        {
            _grid = GetComponent<UIScalableGrid>();
            _overlay = GetComponentInParent<Canvas>();

            GridInputUIEventReceiver receiver = GetComponentInChildren<GridInputUIEventReceiver>(true);
            receiver.Init(this);
            _camera = _overlay.renderMode == RenderMode.ScreenSpaceCamera ? _overlay.worldCamera : null;
        }

        private void OnEnable()
        {
            RegisterComponent(_grid);
            SetGridInputEnabled(true);
        }

        private void OnDisable()
        {
            SetGridInputEnabled(false);
            RemoveComponent(_grid);
        }

        #endregion INITIALIZATION


        #region TICK

        protected override void GridTick(float delta)
        {
            OnGridInput(_inputValues);
            
            _inputValues.DidEnterThisFrame = false;
            _inputValues.DidExitThisFrame = false;
            _inputValues.WasPressedThisFrame = false;
            _inputValues.WasAlternatePressedThisFrame = false;
            _inputValues.WasReleasedThisFrame = false;
            _inputValues.WasAlternateReleasedThisFrame = false;
        }

        #endregion TICK
 

        #region POINTER

        public Tuple<UIScalableGridCell, float> GetCellClosestToPosition(Vector3 targetPosition)
        {
            // We use screen space to measure distance
            Vector3 sourceSS = _camera == null ? targetPosition : _camera.WorldToScreenPoint(targetPosition);

            float closestDist = float.MaxValue;
            UIScalableGridCell closestCell = _grid.CellAtIndex(0);
            foreach (UIScalableGridCell cell in _grid.CellViews)
            {
                Vector3 targetSS = _camera == null ? cell.CellRect.position : _camera.WorldToScreenPoint(cell.CellRect.position);
                float distance = Vector3.Distance(sourceSS, targetSS);
                if (distance < closestDist)
                {
                    closestDist = distance;
                    closestCell = cell;
                }
            }
            
            return new Tuple<UIScalableGridCell, float>(closestCell, closestDist);
        }

        /// <summary>
        /// Returns the raw position of the pointer (including off-grid position)
        /// </summary>
        protected virtual Vector3 GetPointerPositionRaw()
        {
            return _camera == null ? Input.mousePosition : _camera.WorldToScreenPoint(Input.mousePosition);
        }

        public virtual Vector3 GetPointerPositionScreen()
        {
            return Input.mousePosition;
        }

        /// <summary>
        /// Manually refreshes the active pointer position through the input struct
        /// </summary>
        public void RefreshPointerPositionManual()
        {
            ProliferateGridPointerPosition(GetPointerPositionRaw());
        }

        #endregion POINTER


        #region EVENTS

        internal void OnPointerEnter(PointerEventData eventData)
        {
            _inputValues.DidEnterThisFrame = true;
            _inputValues.IsInside = true;
        }

        internal void OnPointerExit(PointerEventData eventData)
        {
            _inputValues.DidExitThisFrame = true;
            _inputValues.IsInside = false;
            _inputValues.ActiveCell = null;
        }

        internal void OnPointerDown(PointerEventData eventData)
        {
            _inputValues.WasPressedThisFrame = eventData.button == PointerEventData.InputButton.Left;
            _inputValues.WasAlternatePressedThisFrame = eventData.button == PointerEventData.InputButton.Right;
            _inputValues.IsPressed = true;
        }

        internal void OnPointerUp(PointerEventData eventData)
        {
            _inputValues.WasReleasedThisFrame = eventData.button == PointerEventData.InputButton.Left;
            _inputValues.WasAlternateReleasedThisFrame = eventData.button == PointerEventData.InputButton.Right;
            _inputValues.IsPressed = false;
        }
        
        public virtual void OnPointerMove(PointerEventData eventData)
        {
            ProliferateGridPointerPosition(_camera != null? GetAdjustedPP(eventData.position) : eventData.position);
        }

        #endregion EVENTS


        #region UTILITY

        protected void ProliferateGridPointerPosition(Vector3 position)
        {
            _inputValues.GridPointerPosition = position;
            Tuple<UIScalableGridCell, float> closestCell = GetCellClosestToPosition(position);
            _inputValues.ActiveCell = closestCell.Item1;
        }

        #endregion UTILITY
        
        
        #region UTILITY

        protected Vector3 GetAdjustedPP(Vector2 position)
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(
                _overlay.GetComponent<RectTransform>(), 
                position, 
                _camera, 
                out Vector3 pp);
            return pp;
        }
        #endregion UTILITY
    }
}