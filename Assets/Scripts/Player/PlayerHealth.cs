using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GNW2.Player
{
    public class PlayerHealth : NetworkBehaviour
    {
        private int maxHealth = 60;

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
        }
    }
}

