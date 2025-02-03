using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class ClickableListener : MonoBehaviour {

    Camera mainCamera;
    IClickable currentHoveredObject;


    private void Start() {
        mainCamera = Camera.main;
    }

    private void Update() {
        bool isOverUI = EventSystem.current.IsPointerOverGameObject();
        if (!isOverUI) {
            HandleHover();
            // Old input system
            if (Input.GetMouseButtonDown(0)) {
                HandleClick();
            }
        } else if (currentHoveredObject != null) {
            currentHoveredObject.OnHoverExit();
            currentHoveredObject = null;
        }
    }

    void HandleHover() {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hitInfo)) {
            IClickable clickedObject = hitInfo.collider.GetComponent<IClickable>();
            if (clickedObject != currentHoveredObject) {
                if (currentHoveredObject != null) {
                    currentHoveredObject.OnHoverExit();
                }
                currentHoveredObject = clickedObject;
                if (currentHoveredObject != null) {
                    currentHoveredObject.OnHoverEnter();
                }
            }
        } else {
            if (currentHoveredObject != null) {
                currentHoveredObject.OnHoverExit();
                currentHoveredObject = null;
            }
        }
    }

    void HandleClick() {
        currentHoveredObject?.OnClick();
    }
}
