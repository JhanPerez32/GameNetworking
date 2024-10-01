using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    public GameObject playerPrefab;

    public void PlayerJoined(PlayerRef player)
    {
        if(player == Runner.LocalPlayer)
        {
            if(playerPrefab)
            {
                Runner.Spawn(playerPrefab, new Vector3(0, 1, 0), Quaternion.identity);
            }
            else
            {
                Debug.LogError("No Player Prefab");
            }
        }
    }
}
