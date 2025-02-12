using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class ClickableListener : MonoBehaviour {

    Camera _mainCamera;
    IClickable _currentHoveredObject;
    [SerializeField] private string _ignoredLayerName = "IgnoreRaycast";
    private int _ignoredLayerMask;


    private void Start() {
        _mainCamera = Camera.main;
        _ignoredLayerMask = ~LayerMask.GetMask(_ignoredLayerName);
    }

    private void Update() {
        bool isOverUI = EventSystem.current.IsPointerOverGameObject();
        if (!isOverUI) {
            HandleHover();
            // Old input system
            if (Input.GetMouseButtonDown(0)) {
                HandleClick();
            }
        } else if (_currentHoveredObject != null) {
            _currentHoveredObject.OnHoverExit();
            _currentHoveredObject = null;
        }
    }

    void HandleHover() {
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, _ignoredLayerMask)) {
            IClickable clickedObject = hitInfo.collider.GetComponent<IClickable>();
            if (clickedObject != _currentHoveredObject) {
                if (_currentHoveredObject != null) {
                    _currentHoveredObject.OnHoverExit();
                }
                _currentHoveredObject = clickedObject;
                if (_currentHoveredObject != null) {
                    _currentHoveredObject.OnHoverEnter();
                }
            }
        } else {
            if (_currentHoveredObject != null) {
                _currentHoveredObject.OnHoverExit();
                _currentHoveredObject = null;
            }
        }
    }

    void HandleClick() {
        _currentHoveredObject?.OnClick();
    }
}
