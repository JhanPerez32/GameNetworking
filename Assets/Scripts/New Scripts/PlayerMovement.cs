using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovement : NetworkBehaviour
{
    private CharacterController characterController;
    [SerializeField] float speed = 5f;

    private Camera cam;

    public override void Spawned()
    {
        if (!HasStateAuthority) return;

        cam = Camera.main;
        if(cam != null && cam.TryGetComponent<FPS>(out var FPScam))
        {
            FPScam.cameraPosition = this.transform;
        }
    }

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority) return;
        var move = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")) * Runner.DeltaTime * speed;

        if(move != Vector3.zero)
        {
            characterController.Move(move);
        }
    }
}
