using GNW2.Player;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Countdown : MonoBehaviour
{
    public Player player;
    public bool readyToFire;
    public TextMeshProUGUI fireText;

    void Update()
    {
        if (readyToFire)
        {
            fireText.SetText("Ready to fire!");
        }
        else
        {
            fireText.SetText("");
        }
    }
}
