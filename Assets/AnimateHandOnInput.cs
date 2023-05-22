using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class AnimateHandOnInput : MonoBehaviour
{
    public InputActionProperty pinchAnimationAction;
    public InputActionProperty gripAnimationAction;
    public Animator handAnimator;

    private UnityEngine.XR.InputDevice targetDevice;

    void Start()
    {
        InputDevices.deviceConnected += OnDeviceConnected; // Subscribe to device connected event
        InputDevices.deviceDisconnected += OnDeviceDisconnected; // Subscribe to device disconnected event
    }

    void OnDestroy()
    {
        InputDevices.deviceConnected -= OnDeviceConnected; // Unsubscribe from device connected event
        InputDevices.deviceDisconnected -= OnDeviceDisconnected; // Unsubscribe from device disconnected event
    }

    void Update()
    {
        Debug.Log("Update method called");

        float triggerValue = pinchAnimationAction.action.ReadValue<float>();
        handAnimator.SetFloat("Trigger", triggerValue);

        float gripValue = gripAnimationAction.action.ReadValue<float>();
        handAnimator.SetFloat("Grip", gripValue);


        Debug.Log("Trigger Value: " + triggerValue);
        Debug.Log("Grip Value: " + gripValue);
    }

    void OnDeviceConnected(UnityEngine.XR.InputDevice device)
    {
        if (device.characteristics.HasFlag(InputDeviceCharacteristics.Controller))
        {
            if (device.name.Contains("Vive")) // Check for HTC Vive controller
            {
                targetDevice = device;
                EnableInputActions();
                Debug.Log("HTC Vive Controller Connected");
            }
        }
    }

    void OnDeviceDisconnected(UnityEngine.XR.InputDevice device)
    {
        if (device == targetDevice)
        {
            targetDevice = default(UnityEngine.XR.InputDevice);
            DisableInputActions();
            Debug.Log("Controller Disconnected");
        }
    }

    void EnableInputActions()
    {
        pinchAnimationAction.action.Enable();
        gripAnimationAction.action.Enable();
    }

    void DisableInputActions()
    {
        pinchAnimationAction.action.Disable();
        gripAnimationAction.action.Disable();
    }
}



/* OLD SCRIPT
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimateHandOnInput : MonoBehaviour
{
    public InputActionProperty pinchAnimationAction;
    public InputActionProperty gripAnimationAction;
    public Animator handAnimator;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float triggerValue = pinchAnimationAction.action.ReadValue<float>();
        handAnimator.SetFloat("Trigger", triggerValue);

        float gripValue = gripAnimationAction.action.ReadValue<float>();
        handAnimator.SetFloat("Grip", gripValue);
    }
}*/
