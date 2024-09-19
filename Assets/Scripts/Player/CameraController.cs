using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : NetworkBehaviour
{
    [SerializeField] private float mouseSensitivity = 100f;

    // Rotation limits
    [SerializeField] private float minXRotation = -90f;
    [SerializeField] private float maxXRotation = 90f;
    [SerializeField] private float minYRotation = -60f;
    [SerializeField] private float maxYRotation = 60f;

    private float xRotation = 0f;
    private float yRotation = 0f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        //if (!Object.HasStateAuthority) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, minXRotation, maxXRotation);

        yRotation += mouseX;
        yRotation = Mathf.Clamp(yRotation, minYRotation, maxYRotation);

        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }
}
