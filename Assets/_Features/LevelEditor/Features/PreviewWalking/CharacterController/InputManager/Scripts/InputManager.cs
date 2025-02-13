using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Text;
using UnityEditor;
using System.IO;
using System.Collections;

public class InputManager : Singleton<InputManager> {

    public InputActionAsset inputActionAsset;
    private InputActionMap playerActionMap;
    private InputActionMap uiActionMap;

    protected override void Awake() {
        base.Awake();
        playerActionMap = inputActionAsset.FindActionMap("Player");
        uiActionMap = inputActionAsset.FindActionMap("UI");
        playerActionMap.Enable();
    }

    public void SwitchToPlayerControls() {
        uiActionMap.Disable();
        playerActionMap.Enable();

     //   Cursor.visible = false;
     //   Cursor.lockState = CursorLockMode.Locked;
    }

    public void SwitchToUIControls() {
        playerActionMap.Disable();
        uiActionMap.Enable();

     //   Cursor.visible = true;
     //   Cursor.lockState = CursorLockMode.Confined;
    }

    public void SubscribeToAction(string actionName, Action<InputAction.CallbackContext> callback, bool continuous = false) {
        InputAction[] actions = new InputAction[]
        {
            playerActionMap.FindAction(actionName),
            uiActionMap.FindAction(actionName)
        };

        foreach (var action in actions) {
            if (action != null) {
                action.performed += callback;
                if (continuous) action.canceled += callback;
            }
        }

    }

    public void UnsubscribeFromAction(string actionName, Action<InputAction.CallbackContext> callback, bool continuous = false) {
        InputAction[] actions = new InputAction[]
        {
            playerActionMap.FindAction(actionName),
            uiActionMap.FindAction(actionName)
        };

        foreach (var action in actions) {
            if (action != null) {
                action.performed -= callback;
                if (continuous) action.canceled -= callback;
            }
        }
    }
}

/*
 * Subscribing to this input system from external scripts
 * 
 * 
 
    void OnEnable()
    {
        InputManager.Instance.SubscribeToAction("Move", OnMove);
        InputManager.Instance.SubscribeToAction("Jump", OnJump);
    }

    private void OnDisable()
    {
        InputManager.Instance.UnsubscribeFromAction("Move", OnMove);
        InputManager.Instance.UnsubscribeFromAction("Jump", OnJump);
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        Vector2 moveInput = context.ReadValue<Vector2>();
        // Handle move input
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        // Handle jump input
    }
*/

/*
 * 
 *  Switching UI / Player externally 
 * 
    InputManager.Instance.SwitchToUIControls();
    InputManager.Instance.SwitchToPlayerControls();
*/