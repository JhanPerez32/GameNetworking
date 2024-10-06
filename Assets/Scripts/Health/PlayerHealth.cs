using System;
using Fusion;
using UnityEngine;
using UnityEngine.UIElements;

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

    private Coroutine regenCoroutine;

    [Header("Visual FX's")]
    public GameObject playerHitFX;
    public GameObject explosionFX;

    public override void Spawned()
    {
        NetworkedHealth = MaxHealth;
        lastDamageTime = Runner.DeltaTime;
    }

    void HealthChanged()
    {
        Debug.Log($"Health changed to: {NetworkedHealth}");
        OnDamageEvent?.Invoke(NetworkedHealth/MaxHealth);

        if (NetworkedHealth <= 0)
        {
            HandleDeath();
        }
        else
        {
            if (regenCoroutine != null)
            {
                StopCoroutine(regenCoroutine);
                regenCoroutine = null;
            }
        }
    }
    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void DealDamageRpc(float damage)
    {
        Debug.Log("Received DealDamageRpc on StateAuthority, modifying Networked variable");
        NetworkedHealth -= damage;

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

    private void Update()
    {
        if (Runner.DeltaTime - lastDamageTime >= regenCooldown && regenCoroutine == null)
        {
            regenCoroutine = StartCoroutine(RegenerateHealth());
        }
    }

    private System.Collections.IEnumerator RegenerateHealth()
    {
        while (NetworkedHealth < MaxHealth)
        {
            NetworkedHealth += regenRate * Runner.DeltaTime;
            NetworkedHealth = Mathf.Min(NetworkedHealth, MaxHealth);

            OnDamageEvent?.Invoke(NetworkedHealth / MaxHealth);

            yield return null;
        }

        regenCoroutine = null;
    }

}