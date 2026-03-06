using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Assign the CAMERA transform (child) here")]
    [SerializeField] private Transform playerCamera;

    [Header("Look")]
    [SerializeField] private float sensitivity = 0.08f;
    [SerializeField] private float maxPitch = 85f;


    private float pitch;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    private void LateUpdate()
    {
        Look();
    }

    private void Look()
    {
        //use old input system
        float mouseX = Mouse.current.delta.ReadValue().x * sensitivity;
        float mouseY = Mouse.current.delta.ReadValue().y * sensitivity;


        transform.Rotate(Vector3.up * mouseX);


        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -maxPitch, maxPitch);

        playerCamera.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }
}