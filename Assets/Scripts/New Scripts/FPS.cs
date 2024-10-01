using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPS : MonoBehaviour
{
    public Transform cameraPosition;
    [SerializeField] float mouseSensitivity;

    private float verticalRot;
    private float horizontalRot;

    private void LateUpdate()
    {
        if (cameraPosition == null) return;

        transform.position = cameraPosition.position;

        var mousInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        horizontalRot += mousInput.x * mouseSensitivity;
        verticalRot += -mousInput.y * mouseSensitivity;

        verticalRot = Mathf.Clamp(verticalRot, -90f, 90f);
        transform. rotation = Quaternion.Euler(verticalRot, horizontalRot, 0);
        //cameraPosition.transform.localRotation = Quaternion.Euler(0, verticalRot, 0);
    }
}
