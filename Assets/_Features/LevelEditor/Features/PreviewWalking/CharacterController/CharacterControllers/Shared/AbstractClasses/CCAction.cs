using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class CCAction : MonoBehaviour {
    /// <summary>
    /// Interface for any action the controlled character can do (move, jump, crouch, ...)
    /// Reacts to CCInput
    /// </summary>
    
    public abstract void InputHandle(InputAction.CallbackContext context);
}
