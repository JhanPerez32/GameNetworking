using Fusion;
using UnityEngine;
using UnityEngine.UI;

namespace GNW2.Player
{
    public class PlayerHealth : NetworkBehaviour
    {
        private int maxHealth = 5;

        [Networked] private int currentHealth {  get; set; }
        [SerializeField] ParticleSystem playerHitFX;
        [SerializeField] ParticleSystem deathFX;

        [SerializeField] private Slider healthBar;

        private Player currentPlayer;
        private bool isDead = false;

        private void Start()
        {
            currentHealth = maxHealth;
            currentPlayer = GetComponent<Player>();
            currentPlayer.OnTakeDamage += HealthDamage;

            UpdateHealthBar();
        }

        public override void FixedUpdateNetwork()
        {
            Debug.Log($"Player: {Runner.LocalPlayer.PlayerId} Health: {currentHealth}");
        }

        private void HealthDamage(int damage)
        {
            if (isDead) return;

            currentHealth -= damage;
            Debug.Log($"Current Health: {currentHealth}");
            UpdateHealthBar();
            RPC_HitFx(transform.position);

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        private void UpdateHealthBar()
        {
            if (healthBar != null)
            {
                if (HasStateAuthority)
                {
                    healthBar.value = currentHealth;
                }
            }
        }

        private void Die()
        {
            isDead = true;
            Debug.Log("Player has died.");
            RPC_DeathFx(transform.position);

            currentPlayer.enabled = false;
            //Runner.Despawn(Object);
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_HitFx(Vector3 position)
        {
            if(playerHitFX != null)
            {
                Instantiate(playerHitFX, position, Quaternion.identity);
            }
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_DeathFx(Vector3 position)
        {
            if (deathFX != null)
            {
                Instantiate(deathFX, position, Quaternion.identity);
            }
        }
    }
}

