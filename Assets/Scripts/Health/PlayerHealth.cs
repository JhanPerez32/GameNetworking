using System;
using System.Text;
using Fusion;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class PlayerHealth : NetworkBehaviour
{
    [Networked, OnChangedRender(nameof(HealthChanged))]
    private float NetworkedHealth { get; set; }

    private const float MaxHealth = 200f;
    public event Action<float> OnDamageEvent;

    [Header("Health Regen")]
    [Range(50f, 100f)]
    public float regenRate;
    [Range(1f, 10f)]
    public float regenCooldown;
    private float lastDamageTime;

    public bool isRegenerating;

    private Coroutine regenCoroutine;

    [Header("Visual FX's")]
    public GameObject playerHitFX;
    public GameObject explosionFX;

    private bool isProtected;

    // Kill and Death Stats
    [Networked] private int kills { get; set; }
    [Networked] private int deaths { get; set; }

    // UI references
    public TextMeshProUGUI killText;
    public TextMeshProUGUI deathText;

    public override void Spawned()
    {
        NetworkedHealth = MaxHealth;
        lastDamageTime = Runner.DeltaTime;

        // Start spawn protection
        isProtected = true;
        StartCoroutine(DisableSpawnProtection());
    }

    private System.Collections.IEnumerator DisableSpawnProtection()
    {
        yield return new WaitForSeconds(5f);
        isProtected = false;
    }

    private System.Collections.IEnumerator HealthRegenCoroutine()
    {
        // Wait until the cooldown expires before allowing regeneration
        yield return new WaitForSeconds(regenCooldown);

        while (NetworkedHealth < MaxHealth)
        {
            // Regenerate health
            NetworkedHealth = Mathf.Min(NetworkedHealth + regenRate * Time.deltaTime, MaxHealth);
            OnDamageEvent?.Invoke(NetworkedHealth / MaxHealth);
            yield return null;
        }

        // Stop regenerating when health is maxed out
        regenCoroutine = null;
    }

    void HealthChanged()
    {
        OnDamageEvent?.Invoke(NetworkedHealth/MaxHealth);

        if (NetworkedHealth <= 0)
        {
            HandleDeath();
        }
    }
    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void DealDamageRpc(float damage)
    {
        if (isProtected)
        {
            return; // Ignore damage during spawn protection
        }

        NetworkedHealth -= damage;

        lastDamageTime = Runner.DeltaTime;
        if (regenCoroutine == null)
        {
            regenCoroutine = StartCoroutine(HealthRegenCoroutine());
        }

        RPC_HitFx(transform.position);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_HitFx(Vector3 position)
    {
        if (playerHitFX != null)
        {
            Instantiate(playerHitFX, position, Quaternion.identity);
        }
    }

    private void HandleDeath()
    {
        if (explosionFX != null)
        {
            Instantiate(explosionFX, transform.position, Quaternion.identity);
        }
        deaths++;
        StartCoroutine(RespawnPlayer());
        UpdateDeathUI();
    }

    private IEnumerator RespawnPlayer()
    {
        yield return new WaitForSeconds(2f);
        NetworkedHealth = MaxHealth;

        transform.position = new Vector3(0, 1, 0);
        Debug.Log("Player respawned!");
    }

    public void HandleKill()
    {
        kills++;
        UpdateKillUI();
    }


    private void UpdateKillUI()
    {
        if (killText != null)
        {
            killText.text = $"K: {kills}";
        }
    }

    private void UpdateDeathUI()
    {
        if (deathText != null)
        {
            deathText.text = $"D: {deaths}";
        }
    }

    public void SetUIReferences(TextMeshProUGUI killUI, TextMeshProUGUI deathUI)
    {
        killText = killUI;
        deathText = deathUI;
    }
}