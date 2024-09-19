using System.Collections;
using System.Collections.Generic;
using Fusion;
using GNW2.Input;
using GNW2.Projectile;
using TMPro;
using UnityEngine;

namespace GNW2.Player
{
    public class Player : NetworkBehaviour
    {
        public static Player Local { get; set; }
        private NetworkCharacterController _cc;

        //Player Move Mechanics
        [SerializeField] private float speed = 5f;
        [SerializeField] private float jumpForce = 5f;
        [SerializeField] private float jumpCooldown = 1f;
        private float lastJumpTime;

        //Bullet
        [SerializeField] BulletProjectile bulletPrefab;
        [SerializeField] float fireRate = 0.1f;
        [Networked] private TickTimer fireDelayTime { get; set; }

        private Vector3 _bulletSpawnLocation = Vector3.forward * 2;

        //Player Components
        //[SerializeField] private Camera playerCamera;
        [SerializeField] GameObject playerUI;
        [SerializeField] Countdown countdown;
        private float rotationX;

        void Update()
        {
            if (fireDelayTime.ExpiredOrNotRunning(Runner))
            {
                countdown.readyToFire = true;
            }
            else
            {
                countdown.readyToFire = false;
            }
        }

        private void Awake()
        {
            _cc = GetComponent<NetworkCharacterController>();
        }

        public override void Spawned()
        {
            Debug.Log($"Runner: {Runner}, HasInputAuthority: {HasInputAuthority}, Owner: {Object.InputAuthority}");

            if (HasInputAuthority)
            {
                Debug.LogWarning("HasInputAuthority is true");
                playerUI.gameObject.SetActive(true);
                Local = this;
                Camera.main.gameObject.SetActive(false);
            }
            else
            {
                Debug.LogWarning("HasInputAuthority is false");
                Camera localCamera = GetComponentInChildren<Camera>();
                localCamera.enabled = false;
                playerUI.gameObject.SetActive(false);
            }
        }

        public override void FixedUpdateNetwork()
        {
            if (GetInput(out NetworkInputData data))
            {
                // Handle movement
                data.Direction.Normalize();
                _cc.Move(speed * data.Direction * Runner.DeltaTime);

                // Jumping
                if (data.Jump && _cc.Grounded && Time.time >= lastJumpTime + jumpCooldown)
                {
                    _cc.Jump(overrideImpulse: jumpForce);
                    lastJumpTime = Time.time;
                }

                // Bullet Firing
                if (!HasStateAuthority || !fireDelayTime.ExpiredOrNotRunning(Runner)) return;

                if (data.Direction.sqrMagnitude > 0)
                {
                    _bulletSpawnLocation = data.Direction * 2f;
                }

                if (!data.buttons.IsSet(NetworkInputData.MOUSEBUTTON0)) return;

                fireDelayTime = TickTimer.CreateFromSeconds(Runner, fireRate);
                Runner.Spawn(bulletPrefab, transform.position + _bulletSpawnLocation,
                    Quaternion.LookRotation(_bulletSpawnLocation), Object.InputAuthority,
                    (runner, bullet) =>
                    {
                        bullet.GetComponent<BulletProjectile>()?.Init(Object);
                    });
            }
        }


        private void OnBulletSpawned(NetworkRunner runner, NetworkObject bullet)
        {
            bullet.GetComponent<BulletProjectile>()?.Init(Object);
        }
    }
}
