using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using GNW2.Input;
using GNW2.Projectile;
using TMPro;
using UnityEngine;
using UnityEngine.Windows;

namespace GNW2.Player
{
    public class Player : NetworkBehaviour
    {
        private NetworkCharacterController _cc;

        //public Camera playerCamera;

        [SerializeField] private float speed = 5f;
        [SerializeField] private float jumpForce = 5f;
        [SerializeField] private float jumpCooldown = 1f;
        private float lastJumpTime;

        [SerializeField] BulletProjectile bulletPrefab;
        [SerializeField] float fireRate = 0.1f;
        [Networked] private TickTimer fireDelayTime { get; set; }


        private Vector3 _bulletSpawnLocation = Vector3.forward * 2;

        private TextMeshProUGUI notifUI;
        [SerializeField] private GameObject playerUIPrefab;

        private void Awake()
        {
            _cc = GetComponent<NetworkCharacterController>();

            /*if (Object.HasInputAuthority)
            {
                if (playerUIPrefab == null)
                {
                    Debug.LogError("Player UI Prefab is not assigned in the Inspector.");
                    return;
                }

                GameObject playerUI = Instantiate(playerUIPrefab);
                notifUI = playerUI.GetComponentInChildren<TextMeshProUGUI>();

                if (notifUI == null)
                {
                    Debug.LogError("TextMeshProUGUI component not found in the instantiated prefab.");
                }

            }*/

            GameObject canvas = GameObject.Find("PlayerCanvas");
            if (canvas != null)
            {
                notifUI = canvas.GetComponentInChildren<TextMeshProUGUI>();
            }
            else
            {
                Debug.LogError("PlayerCanvas not found. Make sure it is present in the scene.");
            }
        }

        public override void FixedUpdateNetwork()
        {
            if (Object.HasInputAuthority && notifUI != null)
            {
                if (fireDelayTime.ExpiredOrNotRunning(Runner))
                {
                    notifUI.text = "Ready to Fire!";
                }
                else
                {
                    notifUI.text = "";
                }
            }

            if (GetInput(out NetworkInputData data))
            {   
                data.Direction.Normalize();
                _cc.Move(speed *data.Direction * Runner.DeltaTime);

                //Jumping
                if (data.Jump && _cc.Grounded && Time.time >= lastJumpTime + jumpCooldown)
                {
                    _cc.Jump(overrideImpulse: jumpForce);
                    lastJumpTime = Time.time;
                }


                //Bullet Firing
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
