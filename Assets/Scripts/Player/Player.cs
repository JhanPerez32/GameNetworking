using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using GNW2.Input;
using GNW2.Projectile;
using UnityEngine;
using UnityEngine.Windows;

namespace GNW2.Player
{
    public class Player : NetworkBehaviour
    {
        private NetworkCharacterController _cc;

        public Camera playerCamera;

        [SerializeField] private float speed = 5f;
        [SerializeField] private float jumpForce = 5f;
        [SerializeField] private float jumpCooldown = 1f;
        private float lastJumpTime;

        [SerializeField] BulletProjectile bulletPrefab;
        [SerializeField] float fireRate = 0.1f;
        [Networked] private TickTimer fireDelayTime { get; set; }


        private Vector3 _bulletSpawnLocation = Vector3.forward * 2;


        private void Awake()
        {
            _cc = GetComponent<NetworkCharacterController>();
        }

        public override void FixedUpdateNetwork()
        {
            if (GetInput(out NetworkInputData data))
            {
                //Jumping
                data.Direction.Normalize();
                _cc.Move(speed *data.Direction * Runner.DeltaTime);

                if (data.Jump && _cc.Grounded && Time.time >= lastJumpTime + jumpCooldown)
                {
                    _cc.Jump(overrideImpulse: jumpForce);
                    lastJumpTime = Time.time;
                }


                //Bullet Firing
                if (!HasInputAuthority || !fireDelayTime.ExpiredOrNotRunning(Runner)) return;

                if (data.Direction.sqrMagnitude > 0)
                {
                    _bulletSpawnLocation = data.Direction * 2;
                }

                if (data.buttons.IsSet(NetworkInputData.MOUSEBUTTON0))
                {
                    fireDelayTime = TickTimer.CreateFromSeconds(Runner, fireRate);
                    Runner.Spawn(bulletPrefab, transform.position + _bulletSpawnLocation, 
                        Quaternion.LookRotation(_bulletSpawnLocation), Object.InputAuthority, OnBulletSpawned);
                    
                }
            }
        }

        private void OnBulletSpawned(NetworkRunner runner, NetworkObject bullet)
        {
            bullet.GetComponent<BulletProjectile>()?.Init();
        }





        public override void Spawned()
        {
            Debug.Log($"Input Authority: {Object.HasInputAuthority}");

            if (Object.HasStateAuthority)
            {
                if (playerCamera != null)
                {
                    playerCamera.enabled = true;

                    Debug.Log("Player camera enabled for local player.");

                    Camera defaultMainCamera = Camera.main;
                    if (defaultMainCamera != null && defaultMainCamera != playerCamera)
                    {
                        defaultMainCamera.gameObject.SetActive(false);
                        Debug.Log("Default Main Camera disabled.");
                    }
                }
                else
                {
                    Debug.LogError("Player camera is not assigned in the Inspector.");
                }
            }
            else
            {
                if (playerCamera != null)
                {
                    playerCamera.enabled = false;
                    Debug.Log("Player camera disabled for remote player.");
                }
            }
        }
    }
}
