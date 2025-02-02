using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClickableListener : MonoBehaviour {

    Camera mainCamera;
    IClickable currentHoveredObject;


    private void Start() {
        mainCamera = Camera.main;
    }

    private void Update() {
        HandleHover();
        // Old input system
        if (Input.GetMouseButtonDown(0)) {
            HandleClick();
        }
    }

    void HandleHover() {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        bool isOverUI = UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();

        if (!isOverUI && Physics.Raycast(ray, out RaycastHit hitInfo)) {
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
