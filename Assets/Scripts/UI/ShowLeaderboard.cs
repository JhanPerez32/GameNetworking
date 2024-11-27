using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowLeaderboard : MonoBehaviour
{
    public GameObject leaderboardUI;

    private void Awake()
    {
        leaderboardUI.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            bool isActive = leaderboardUI.activeSelf;
            leaderboardUI.SetActive(!isActive);

            if (CursorManager.Instance != null)
            {
                if (isActive)
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
