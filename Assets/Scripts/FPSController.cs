using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]

public class FPSController : MonoBehaviour
{
    public float walkingSpeed = 7.5f;
    public float runningSpeed = 11.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public Camera playerCamera;

    public GameObject LeftHand;
    public GameObject RightHand;

    public float lookSpeed = 2.0f;
    public float lookXLimit = 90.0f;

    public float reachDistance = 5f;

    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;

    [HideInInspector]
    public bool canMove = true;

    float lastSpaceDown = 0;
    float lastSpaceUp = 0;

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (ApplicationModel.isVR)
        {
            GetComponent<FPSController>().enabled = false;
        }
        else
        {
            LeftHand.SetActive(false);
            RightHand.SetActive(false);
        }
    }

    void FixedUpdate()
    {
    }

    void Update()
    {
        if (!ApplicationModel.isPaused)
        {
            HandleMovement();
        }
    }

    void HandleMovement()
    {
        // We are grounded, so recalculate move direction based on axes
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        // Press Left Shift to run
        bool isRunning = Input.GetKey(KeyCode.LeftControl);
        float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetButtonDown("Jump"))
        {
            if ((Time.time - lastSpaceUp) <= 0.200f)
            {
                if (gravity == 0)
                {
                    gravity = 20f;
                }
                else
                {
                    gravity = 0;
                }
            }
            lastSpaceDown = Time.time;
        }
        if ((Time.time - lastSpaceDown) <= 0.200f && Input.GetButtonUp("Jump"))
        {
            lastSpaceUp = Time.time;
        }
        if (Input.GetButtonDown("Jump"))
        {

        }

        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpSpeed;
        }
        else if (Input.GetButton("Jump") && gravity == 0 && canMove)
        {
            moveDirection.y = walkingSpeed * Input.GetAxis("Jump");
        }
        else if (gravity == 0 && canMove)
        {
            moveDirection.y = 0;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);

        // Player and Camera rotation
        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }
}