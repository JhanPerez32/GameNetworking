using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    public GameObject PlayerPrefab;
    public GameObject StatusBar;
    public Slider UIHealthBar;
    public Slider UIGunEnergyBar;
    public GameObject PlayerProfileUIPrefab;
    public Http http;

    [SerializeField] private TextMeshProUGUI killTextBox;
    [SerializeField] private TextMeshProUGUI deathTextBox;

    void Awake()
    {
        StatusBar.SetActive(false);
        Cursor.visible = true;
    }

    public void PlayerJoined(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {
            SpawnPlayer();
        }
    }

    private void SpawnPlayer()
    {
        var spawnedPlayer = Runner.Spawn(PlayerPrefab, new Vector3(0, 1, 0), Quaternion.identity, Runner.LocalPlayer);

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

        var playerProfileUI = Instantiate(PlayerProfileUIPrefab);

        var killTextTransform = playerProfileUI.transform.Find("Profile Section/Stats/Kill");
        var deathTextTransform = playerProfileUI.transform.Find("Profile Section/Stats/Death");

        if (killTextTransform != null && deathTextTransform != null)
        {
            var killText = killTextTransform.GetComponent<TextMeshProUGUI>();
            var deathText = deathTextTransform.GetComponent<TextMeshProUGUI>();

            // Assign the references to PlayerHealth
            var playerHealth = spawnedPlayer.GetComponent<PlayerHealth>();
            if (playerHealth)
            {
                killTextBox = killText;
                deathTextBox = deathText;
                playerHealth.SetUIReferences(killTextBox, deathTextBox);
            }
        }
        else
        {
            Debug.LogError("Kill or Death text not found in PlayerProfileUI prefab.");
        }

        if (http != null)
        {
            http.LoggedIn();
        }
    }

    public void UpdateKillDeathText(int kills, int deaths)
    {
        if (killTextBox != null && deathTextBox != null)
        {
            killTextBox.text = $"K: {kills}";
            deathTextBox.text = $"D: {deaths}";
        }
    }
}
