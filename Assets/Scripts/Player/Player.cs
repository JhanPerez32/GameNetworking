using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using GNW2.Input;
using GNW2.Projectile;
using TMPro;
using UnityEngine;

namespace GNW2.Player
{
    public class Player : NetworkBehaviour, ICombat
    {
        public static Player Local { get; set; }
        private NetworkCharacterController _cc;

        //Player Camera
        [SerializeField] private float movementSpeed = 5f;
        [SerializeField] private float lookSpeed = 2f;
        [SerializeField] private Camera playerCamera;
        private Vector2 lookRotation;

        //Player Move Mechanics
        [SerializeField] private float jumpForce = 5f;
        [SerializeField] private float jumpCooldown = 1f;
        private float lastJumpTime;

        //Bullet
        [SerializeField] BulletProjectile bulletPrefab;
        [SerializeField] float fireRate = 0.1f;
        [SerializeField] Transform bulletSpawnLocation;
        [Networked] private TickTimer fireDelayTime { get; set; }

        //Player Components
        [SerializeField] GameObject playerUI;
        [SerializeField] Countdown countdown;
        public event Action<int> OnTakeDamage;

        // Cursor control
        private bool isCursorVisible = false;

        void Update()
        {
            if (HasInputAuthority && UnityEngine.Input.GetKeyDown(KeyCode.Alpha1))
            {
                ToggleCursor();
            }

            if (fireDelayTime.ExpiredOrNotRunning(Runner))
            {
                countdown.readyToFire = true;
            }
            else
            {
                countdown.readyToFire = false;
            }
        }

        private void ToggleCursor()
        {
            isCursorVisible = !isCursorVisible;

            if (isCursorVisible)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
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
                playerCamera.enabled = true;
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                Debug.LogWarning("HasInputAuthority is false");
                playerCamera.enabled = false;
                playerUI.gameObject.SetActive(false);
            }
        }

        public override void FixedUpdateNetwork()
        {
            if (GetInput(out NetworkInputData data))
            {
                if (!isCursorVisible)
                {
                    // Movement
                    MovePlayer(data.Direction);

                    // Jumping
                    if (data.Jump && _cc.Grounded && Time.time >= lastJumpTime + jumpCooldown)
                    {
                        _cc.Jump(overrideImpulse: jumpForce);
                        lastJumpTime = Time.time;
                    }

                    // Mouse Look
                    LookAround(data.MouseX, data.MouseY);

                    if (!HasStateAuthority || !fireDelayTime.ExpiredOrNotRunning(Runner)) return;

                    // Jumping
                    if (data.Jump && _cc.Grounded && Time.time >= lastJumpTime + jumpCooldown)
                    {
                        _cc.Jump(overrideImpulse: jumpForce);
                        lastJumpTime = Time.time;
                    }

                    if (data.buttons.IsSet(NetworkInputData.MOUSEBUTTON0))
                    {
                        //Bullet Spawn Location
                        Vector3 spawnPosition = bulletSpawnLocation.position;
                        Vector3 spawnDirection = bulletSpawnLocation.forward;

                        fireDelayTime = TickTimer.CreateFromSeconds(Runner, fireRate);
                        Runner.Spawn(bulletPrefab, spawnPosition, Quaternion.LookRotation(spawnDirection), Object.InputAuthority,
                            (runner, bullet) =>
                            {
                                bullet.GetComponent<BulletProjectile>()?.Init();
                            });
                    }
                }
            }
        }

        private void OnBulletSpawned(NetworkRunner runner, NetworkObject bullet)
        {
            bullet.GetComponent<BulletProjectile>()?.Init();
        }

        private void MovePlayer(Vector3 inputDirection)
        {
            if (HasStateAuthority)
            {
                Vector3 cameraForward = playerCamera.transform.forward;
                Vector3 cameraRight = playerCamera.transform.right;

                cameraForward.y = 0f;
                cameraRight.y = 0f;

                cameraForward.Normalize();
                cameraRight.Normalize();

                Vector3 moveDirection = (cameraForward * inputDirection.z) + (cameraRight * inputDirection.x);

                _cc.Move(moveDirection * movementSpeed * Runner.DeltaTime);
            }
        }

        private void LookAround(float mouseX, float mouseY)
        {
            if (HasStateAuthority)
            {
                lookRotation.x += mouseX * lookSpeed;
                //lookRotation.y += mouseY * lookSpeed;

                lookRotation.y = Mathf.Clamp(lookRotation.y, -90f, 90f);

                transform.localRotation = Quaternion.Euler(0, lookRotation.x, 0);
                playerCamera.transform.localRotation = Quaternion.Euler(/*-lookRotation.y*/ 0, 0, 0);
            }
        }

        public void TakeDamage(int Damage)
        {
            OnTakeDamage?.Invoke(Damage);
        }


    }
}
