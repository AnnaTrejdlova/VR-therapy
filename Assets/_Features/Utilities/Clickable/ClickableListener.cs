using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClickableListener : MonoBehaviour {

    Camera mainCamera;
    IClickable currentHoveredObject;
    public LayerMask notRaycastTargetLayer;


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

        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, ~notRaycastTargetLayer)) {
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
