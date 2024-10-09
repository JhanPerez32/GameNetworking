using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    public GameObject PlayerPrefab;
    //public GameObject SelectSpawnPointUI;
    public GameObject StatusBar;
    public Slider UIHealthBar;
    public Slider UIGunEnergyBar;

    public SpawnManager spawnManager;
    public int selectedSpawnIndex = 0;

    void Awake()
    {
        //SelectSpawnPointUI.SetActive(false);
        StatusBar.SetActive(false);
        Cursor.visible = true;
    }

    public void PlayerJoined(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {
            //SelectSpawnPointUI.SetActive(true);

            SpawnPlayer();
            /*The Player Spawns when being spawned by Player Joined, however this line of code
             * var spawnedPlayer = Runner.Spawn(PlayerPrefab, spawnPosition, Quaternion.identity, Runner.LocalPlayer);
             * is showing error that it is Null
             */
        }
    }

    //Accessed by the UI Button PlayerCanvas -> Selection
    public void SetSpawnLocation(int index)
    {
        selectedSpawnIndex = index;

        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        if (selectedSpawnIndex >= 0 && selectedSpawnIndex < spawnManager.spawnPoints.Length)
        {
            Vector3 spawnPosition = spawnManager.spawnPoints[selectedSpawnIndex].position;
            Debug.LogWarning("Spawning player at: " + spawnPosition);

            var spawnedPlayer = Runner.Spawn(PlayerPrefab, spawnPosition, Quaternion.identity, Runner.LocalPlayer);

            StatusBar.SetActive(true);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            var healthBarController = spawnedPlayer.GetComponent<HealthBarController>();
            if (healthBarController)
            {
                healthBarController.UIHealthBar = UIHealthBar;
            }

            var energyGunBar = spawnedPlayer.GetComponent<RaycastAttack>();
            if (energyGunBar)
            {
                energyGunBar.UIGunEnergyBar = UIGunEnergyBar;
            }
        }
        else
        {
            Debug.LogError("Selected spawn index is out of bounds. Index: " + selectedSpawnIndex);
        }
    }
}
