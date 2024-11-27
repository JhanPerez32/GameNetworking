using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public GameObject spawnSelectionUI;
    public PlayerSpawner playerSpawner;

    public void OnSpawnLocationSelected(int spawnIndex)
    {
        //playerSpawner.SetSpawnLocation(spawnIndex);

        //spawnSelectionUI.SetActive(false);

        //playerSpawner.SpawnPlayer();
    }
}
