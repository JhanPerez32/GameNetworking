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

    public GameObject playerHitFX;
    public GameObject explosionFX;

    public override void Spawned()
    {
        NetworkedHealth = MaxHealth;
    }


    void HealthChanged()
    {
        Debug.Log($"Health changed to: {NetworkedHealth}");
        OnDamageEvent?.Invoke(NetworkedHealth/MaxHealth);

        // Check if health has reached zero
        if (NetworkedHealth <= 0)
        {
            HandleDeath();
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

}