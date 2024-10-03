using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    public GameObject PlayerPrefab;

    public void PlayerJoined(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {
            Runner.Spawn(PlayerPrefab, new Vector3(0, 1, 0), Quaternion.identity, player);
        }
    }


    //public SpawnManager spawnManager;
    //private int selectedSpawnIndex = 0;

    /*public void PlayerJoined(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {
            //Debug.LogWarning("Waiting for player to select spawn location...");

            //Vector3 spawnPosition = spawnManager.spawnPoints[selectedSpawnIndex].position;
            Runner.Spawn(PlayerPrefab, new Vector3(0, 1, 0), Quaternion.identity, player);
            //Debug.LogWarning("Player spawned at: " + spawnManager.spawnPoints[selectedSpawnIndex].name);
        }
    }

    public void SetSpawnLocation(int index)
    {
        selectedSpawnIndex = index;
        Debug.LogWarning("Selected spawn index: " + selectedSpawnIndex);
    }

    public void SpawnPlayer()
    {
        if (Runner == null)
        {
            Debug.LogError("Runner is not initialized!");
            return;
        }

        Vector3 spawnPosition = spawnManager.spawnPoints[selectedSpawnIndex].position;
        Runner.Spawn(PlayerPrefab, spawnPosition, Quaternion.identity, Runner.LocalPlayer);
        Debug.LogWarning("Player spawned at: " + spawnManager.spawnPoints[selectedSpawnIndex].name);
    }*/
}
