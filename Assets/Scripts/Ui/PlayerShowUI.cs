using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShowUI : MonoBehaviour
{
    void Update()
    {
        if (CursorManager.Instance.IsCursorLocked())
        {
            //Show UI's
        }
        else
        {
            //Hide UI's
        }
    }
}
