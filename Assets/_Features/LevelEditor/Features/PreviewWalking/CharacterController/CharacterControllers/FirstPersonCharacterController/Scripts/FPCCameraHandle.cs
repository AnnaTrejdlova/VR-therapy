using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.LowLevel;
using UnityEngine.Windows;

public class FPCCameraHandle : CCAction {

    [Header("Refferences (Must have)")]
    public bool CinemachineControl = false;
    public CinemachineVirtualCamera vCamPOV;
    public CinemachineVirtualCamera vCamClassic;
    public Transform vCamTarget;

    [Header("Base Settings")]
    public float FOV = 70f;

    [Header("Classic Mode Settings")]
    public float sensitivity = 10f;
    public float verticalClamp = 80f;

    Camera mainCamera;
    Vector2 input;
    float xRotation = 0f;

    void Start() {
        mainCamera = Camera.main;
        InitializeCameras();
    }

    void Update() {
        if (!CinemachineControl) {
            CameraMove();
        }
    }

    void CameraMove() {
        float mouseX = input.x * sensitivity * Time.deltaTime;
        float mouseY = input.y * sensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -verticalClamp, verticalClamp);

        vCamTarget.localRotation = Quaternion.Euler(xRotation, 0f, 0f); // Rotate camera up/down
        transform.Rotate(Vector3.up * mouseX); // Rotate player left/right
    }

    void DeactivateVCamPOVControl() {
        vCamPOV.gameObject.SetActive(false);
        vCamClassic.gameObject.SetActive(true);
    }

    void DeactivateClassicControl() {
        vCamPOV.gameObject.SetActive(true);
        vCamClassic.gameObject.SetActive(false);
    }

    void InitializeCameras() {
        // Keep relevant camera active
        StartCoroutine(ActivateCorrectCameraCoroutine()); // prevent blending with the coroutine cuz all else failed
        // Set FOV
        vCamClassic.m_Lens.FieldOfView = FOV;
        vCamPOV.m_Lens.FieldOfView = FOV;
    }

    IEnumerator ActivateCorrectCameraCoroutine() {
        CinemachineBrain brain = FindObjectOfType<CinemachineBrain>();
        CinemachineBlendDefinition.Style initialBlendStyle = brain.m_DefaultBlend.m_Style;
        brain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.Cut;
        if (CinemachineControl) {
            DeactivateClassicControl();
        } else {
            DeactivateVCamPOVControl();
        }
        yield return new WaitForSeconds(brain.m_DefaultBlend.m_Time);
        brain.m_DefaultBlend.m_Style = initialBlendStyle;
    }

    public override void InputHandle(InputAction.CallbackContext context) {
        if (CinemachineControl) return;
        if (context.performed) {
            input = context.ReadValue<Vector2>();
        } else if(context.canceled) {
            input = Vector2.zero;
        }
    }
}

