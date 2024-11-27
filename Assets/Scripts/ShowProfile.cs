using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowProfile : MonoBehaviour
{
    public GameObject profileParent;

    private void Awake()
    {
        profileParent.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            bool isActive = profileParent.activeSelf;
            profileParent.SetActive(!isActive);

            if(CursorManager.Instance != null)
            {
                if(isActive)
                {
                    CursorManager.Instance.ToggleCursorLock();
                }
                else
                {
                    CursorManager.Instance.ToggleCursorLock();
                }
            }
        }
    }


}
