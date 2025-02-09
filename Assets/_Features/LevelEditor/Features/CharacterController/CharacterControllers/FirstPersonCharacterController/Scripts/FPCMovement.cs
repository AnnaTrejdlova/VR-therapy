using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FPCMovement : CCAction {

    #region Variables

    [Header("Movement")]
    public float maxBaseMovementSpeed = 5.0f;
    public float gravity = -9.81f;
    public float movementAcceleration = 1f;
    public float movementDeceleration = 1f;
    public float movementSmoothing = 1f;
    public float slopeSlideDownSpeed = 1f;
    public float slopeSlideDownRaycastLength = 3f;
    
    CharacterController controller;
    Vector2 moveInput;
    float currentSpeed = 0f;
    float verticalVelocity = 0f;
    Vector3 lastDirection = Vector3.zero; // For movement decelaration


    #endregion

    void Awake() {
        controller = GetComponent<CharacterController>();
    }

    void Update() {
        HandleMovement(moveInput);
        HandleSliding();
    }

    public override void InputHandle(InputAction.CallbackContext context) {
        if (context.performed) {
            moveInput = context.ReadValue<Vector2>();
        } else if (context.canceled) {
            moveInput = Vector2.zero;
        }
    }

    #region Movement, Gravity, Slope sliding

    // Movement logic
    void HandleMovement(Vector2 moveInput) {
        bool isMoving = moveInput.magnitude > 0;
        float targetSpeed = maxBaseMovementSpeed;

        // Apply camera rotation to movement direction
        Camera mainCamera = Camera.main;
        Vector3 cameraForward = mainCamera.transform.forward;
        Vector3 cameraRight = mainCamera.transform.right;
        cameraForward.y = 0;
        cameraForward.Normalize();
        cameraRight.y = 0;
        cameraRight.Normalize();
        Vector3 desiredDirection = (cameraForward * moveInput.y + cameraRight * moveInput.x).normalized;

        // Smooth movement
        if (isMoving) {
            lastDirection = Vector3.Lerp(lastDirection, desiredDirection, movementSmoothing * Time.deltaTime);
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, movementAcceleration * Time.deltaTime);
        } else {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, movementDeceleration * Time.deltaTime);
        }
        if(currentSpeed <= 0) {
            lastDirection = Vector2.zero;
        }

        // Gravity and slope handling
        if (controller.isGrounded) {
            // Apply downward force on slopes to prevent bouncing
            if (Vector3.Angle(Vector3.up, lastDirection) > controller.slopeLimit) {
                verticalVelocity = -5f; // Apply more gravity force to make it stick
            } else {
                verticalVelocity = -0.5f; // Slight downward force to stay grounded
            }
        } else {
            verticalVelocity += gravity * Time.deltaTime;
        }

        Vector3 movement = lastDirection * currentSpeed + Vector3.up * verticalVelocity;

        controller.Move(movement * Time.deltaTime);
    }

    // Sliding down very steep slopes
    void HandleSliding() {
        if (!controller.isGrounded) return;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, slopeSlideDownRaycastLength)) {
            Vector3 surfaceNormal = hit.normal;
            float slopeAngle = Vector3.Angle(Vector3.up, surfaceNormal);

            // Check if the slope angle is greater than the controller's slope limit
            if (slopeAngle > controller.slopeLimit) {
                Vector3 slideDirection = Vector3.ProjectOnPlane(Vector3.down, surfaceNormal).normalized;
                controller.Move(slideDirection * (slopeSlideDownSpeed * Time.deltaTime));
            }
        }
    }

    #endregion

    #region GetterSetters

    public float GetCurrentSpeed() {
        return currentSpeed;
    }

    #endregion
}