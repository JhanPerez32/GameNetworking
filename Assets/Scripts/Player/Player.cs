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
        private NetworkCharacterController _cc;

        [SerializeField] private float speed = 5f;
        [SerializeField] private float jumpForce = 5f;
        [SerializeField] private float jumpCooldown = 1f;
        private float lastJumpTime;

        [SerializeField] private float lookSensitivity = 2f;
        [SerializeField] private float maxLookX = 60f;
        [SerializeField] private float minLookX = -60f;

        [SerializeField] BulletProjectile bulletPrefab;
        [SerializeField] float fireRate = 0.1f;
        [Networked] private TickTimer fireDelayTime { get; set; }

        private Vector3 _bulletSpawnLocation = Vector3.forward * 2;

        private Camera playerCamera;
        private float rotationX;

        private void Awake()
        {
            _cc = GetComponent<NetworkCharacterController>();
            playerCamera = GetComponentInChildren<Camera>();
        }

        public override void Spawned()
        {
            if (Object.HasStateAuthority)
            {
                Debug.LogWarning("Camera");

                playerCamera.enabled = true;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
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

                // Handle mouse look
                MouseLook(data);

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

        private void MouseLook(NetworkInputData data)
        {
            if (!Object.HasStateAuthority) return;

            // Rotate the camera around the X-axis (look up and down)
            rotationX -= data.MouseY * lookSensitivity;
            rotationX = Mathf.Clamp(rotationX, minLookX, maxLookX);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);

            // Rotate the player around the Y-axis (turn left and right)
            transform.Rotate(Vector3.up * data.MouseX * lookSensitivity);
        }


        private void OnBulletSpawned(NetworkRunner runner, NetworkObject bullet)
        {
            bullet.GetComponent<BulletProjectile>()?.Init(Object);
        }
    }
}
