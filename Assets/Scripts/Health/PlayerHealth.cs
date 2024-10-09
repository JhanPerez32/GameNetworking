using System;
using Fusion;
using UnityEngine;

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

        Debug.LogWarning("Regenerating");

        while (NetworkedHealth < MaxHealth)
        {
            // Regenerate health
            NetworkedHealth = Mathf.Min(NetworkedHealth + regenRate * Time.deltaTime, MaxHealth);
            OnDamageEvent?.Invoke(NetworkedHealth / MaxHealth);
            yield return null; // Wait for the next frame
        }

        // Stop regenerating when health is maxed out
        regenCoroutine = null; // Reset coroutine reference
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

        Runner.Despawn(Object);
    }
}