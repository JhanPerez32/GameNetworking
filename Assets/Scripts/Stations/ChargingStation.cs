using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargingStation : MonoBehaviour
{
    public float refillRate = 10f;
    public float detectionRange = 5f;
    public Transform zoneCenter;

    private RaycastAttack playerInRange;

    private void Update()
    {
        // If the Player is in Range or has Already left
        if (playerInRange != null)
        {
            float distance = Vector3.Distance(playerInRange.transform.position, zoneCenter.position);

            // If the player is outside the range, stop refilling and allow firing
            if (distance > detectionRange)
            {
                playerInRange.isRefilling = false;
                playerInRange = null;
            }
            else
            {
                // When still in Range, prevent firing and refill gun energy
                playerInRange.isRefilling = true;
                playerInRange.RefillGunEnergy(refillRate * Time.deltaTime);
            }
        }
        else
        {
            Collider[] playersInRange = Physics.OverlapSphere(zoneCenter.position, detectionRange);
            foreach (var player in playersInRange)
            {
                RaycastAttack raycastAttack = player.GetComponent<RaycastAttack>();
                if (raycastAttack != null)
                {
                    // Player is in Zone start refilling
                    playerInRange = raycastAttack;
                    raycastAttack.isRefilling = true;
                    break;
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (zoneCenter != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(zoneCenter.position, detectionRange);
        }
    }
}
