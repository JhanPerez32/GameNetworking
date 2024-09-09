using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using GNW2.Input;
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


        private void Awake()
        {
            _cc = GetComponent<NetworkCharacterController>();

            if (playerCamera == null)
            {
                playerCamera = GetComponentInChildren<Camera>();
            }

            if (playerCamera == null)
            {
                Debug.LogError("Player Camera not found in child objects!");
            }
        }

        public override void FixedUpdateNetwork()
        {
            if (GetInput(out NetworkInputData data))
            {
                data.Direction.Normalize();
                _cc.Move(speed *data.Direction * Runner.DeltaTime);

                if (data.Jump && _cc.Grounded && Time.time >= lastJumpTime + jumpCooldown)
                {
                    _cc.Jump(overrideImpulse: jumpForce);
                    lastJumpTime = Time.time;
                }
            }
        }

        public override void Spawned()
        {
            Debug.Log($"Input Authority: {Object.HasInputAuthority}");

            if (Object.HasInputAuthority)
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

            /*if (Object.HasInputAuthority)
                {
                    playerCamera.enabled = true;
                }
                else
                {
                    playerCamera.enabled = false;
                }*/
        }
    }
}
