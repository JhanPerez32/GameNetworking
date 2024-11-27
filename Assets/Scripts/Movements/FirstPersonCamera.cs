using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    public Transform Orientation;
    public float MouseSensitivity = 10f;

    float xRotation;
    float yRotation;

    void LateUpdate()
    {
        if (Orientation == null || !CursorManager.Instance.IsCursorLocked())
        {
            return;
        }

        float mouseX = Input.GetAxis("Mouse X") * MouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * MouseSensitivity;

        yRotation += mouseX;
        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        Orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}