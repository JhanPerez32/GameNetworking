using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

namespace GNW2.Projectile
{

    public class BulletProjectile : NetworkBehaviour 
    {
        [SerializeField] float bulletSpeed = 10f;
        [SerializeField] float lifeTime = 5f;

        [Range (0.1f, 0.5f)]
        [SerializeField] float dyingTime = 0.2f;

        [Networked] private TickTimer life { get; set; }

        [SerializeField] Material normalMaterial;
        [SerializeField] Material dyingMaterial;
        [SerializeField] Renderer bulletRenderer;

        private NetworkObject owner;
        private Transform target;

        private void Start()
        {
            bulletRenderer.material = normalMaterial;
        }

        public void Init(NetworkObject bulletOwner)
        {
            life = TickTimer.CreateFromSeconds(Runner, lifeTime);
            owner = bulletOwner;
            FindNearestTarget();
        }

        public override void FixedUpdateNetwork()
        {
            if (life.Expired(Runner))
            {
                Runner.Despawn(Object);
            }
            else
            {
                transform.position += bulletSpeed * transform.forward * Runner.DeltaTime;

                float remainingLife = life.RemainingTime(Runner).Value;

                if (remainingLife <= lifeTime * dyingTime) //20% or less
                {
                    bulletRenderer.material = dyingMaterial;
                }
                else
                {
                    bulletRenderer.material = normalMaterial;
                }
            }
        }

        private void FindNearestTarget()
        {
            NetworkObject[] networkObjects = FindObjectsOfType<NetworkObject>();
            List<NetworkObject> potentialTargets = new List<NetworkObject>();

            foreach (NetworkObject obj in networkObjects)
            {
                if (obj != owner && obj.HasStateAuthority)
                {
                    potentialTargets.Add(obj);
                }
            }

            float minDistance = Mathf.Infinity;
            foreach (NetworkObject potentialTarget in potentialTargets)
            {
                float distance = Vector3.Distance(transform.position, potentialTarget.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    target = potentialTarget.transform;
                }
            }
        }
    }
}
