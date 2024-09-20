using Fusion;
using UnityEngine;

namespace GNW2.Player
{
    public class PlayerHealth : NetworkBehaviour
    {
        private int maxHealth = 5;

        [Networked] private int currentHealth {  get; set; }
        [SerializeField] ParticleSystem playerHitFX;

        private Player currentPlayer;

        private void Start()
        {
            currentHealth = maxHealth;
            currentPlayer = GetComponent<Player>();
            currentPlayer.OnTakeDamage += HealthDamage;
        }

        public override void FixedUpdateNetwork()
        {
            Debug.Log($"Player: {Runner.LocalPlayer.PlayerId} Health: {currentHealth}");
        }

        private void HealthDamage(int damage)
        {
            currentHealth -= damage;
            Debug.Log($"Current Health: {currentHealth}");
            RPC_HitFx(transform.position);
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_HitFx(Vector3 position)
        {
            if(playerHitFX != null)
            {
                Instantiate(playerHitFX, position, Quaternion.identity);
            }
        }
    }
}

