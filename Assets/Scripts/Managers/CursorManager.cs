using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance { get; private set; }
    private bool isCursorLocked = true;

    public KeyCode cursorLockKey = KeyCode.Alpha1;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        Cursor.visible = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(cursorLockKey))
        {
            ToggleCursorLock();
        }
    }

    public void ToggleCursorLock()
    {
        isCursorLocked = !isCursorLocked;

        if (isCursorLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public bool IsCursorLocked()
    {
        return isCursorLocked;
    }
}
