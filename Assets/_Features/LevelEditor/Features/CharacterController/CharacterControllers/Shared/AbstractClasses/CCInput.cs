using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CCInput : MonoBehaviour {
    /// <summary>
    /// This class will be used for any character controllers input control
    /// You can customize what inputAction names will react to what class (class has to inherit from CCAction)
    /// </summary>

    // Instead of dictionary i have to use a class so i can edit it in inspector
    public List<InputActionWithCCAction> actions = new List<InputActionWithCCAction>(); 
    
    void OnEnable() {
        foreach (InputActionWithCCAction pair in actions) {
            InputManager.Instance?.SubscribeToAction(pair.inputActionName, (callbackContext) => { pair.CCActionClass.InputHandle(callbackContext); }, pair.isContinuosAction);
        }
    }

    void OnDisable() {
        foreach (InputActionWithCCAction pair in actions) {
            InputManager.Instance?.UnsubscribeFromAction(pair.inputActionName, (callbackContext) => { pair.CCActionClass.InputHandle(callbackContext); }, pair.isContinuosAction);
        }
    }

}

[Serializable]
public class InputActionWithCCAction {
    public string inputActionName;
    public CCAction CCActionClass;
    // You have to hold the button = true(move, sprint), you only push the button = false(jump, toggled crouch)
    public bool isContinuosAction;
}