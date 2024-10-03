using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public Transform CameraPosition;

    private void LateUpdate()
    {
        if (CameraPosition == null)
        {
            return;
        }

        transform.position = CameraPosition.position;
    }
}
