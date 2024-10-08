using Fusion;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerMovement : NetworkBehaviour
{
    public int PlayerID => Runner.LocalPlayer.PlayerId;


    private Vector3 _velocity;
    private bool _jumpPressed;

    private CharacterController _controller;

    [FormerlySerializedAs("PlayerSpeed")] public float playerSpeed = 2f;

    [FormerlySerializedAs("JumpForce")] public float jumpForce = 5f;
    [FormerlySerializedAs("GravityValue")] public float gravityValue = -9.81f;

    [FormerlySerializedAs("CameraHolder")] public Transform cameraHolder;
    [FormerlySerializedAs("Camera")] public Camera mainCamera;

    [Networked] private Quaternion NetworkedRotation { get; set; }

    public Transform cameraPos;
    public Transform bodyOrientation;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    public override void Spawned()
    {
        if (!HasStateAuthority) return;

        MultiplayerChat.Instance.SetUsername(Runner.LocalPlayer.PlayerId);

        mainCamera = Camera.main;
        if (mainCamera != null)
        {
            FirstPersonCamera fpsCamera = mainCamera.GetComponent<FirstPersonCamera>();
            if (fpsCamera != null)
            {
                fpsCamera.Orientation = bodyOrientation;
            }

            cameraHolder = mainCamera.transform.parent;

            if (cameraHolder != null)
            {
                MoveCamera moveCamera = cameraHolder.GetComponent<MoveCamera>();
                if (moveCamera != null)
                {
                    moveCamera.CameraPosition = cameraPos;
                }
            }
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            _jumpPressed = true;
        }

        if (CursorManager.Instance.IsCursorLocked())
        {
            if (!_controller.enabled)
            {
                _controller.enabled = true;
            }
        }
        else
        {
            _jumpPressed = false;
        }
    }

    public override void FixedUpdateNetwork()
    {
        // FixedUpdateNetwork is only executed on the StateAuthority

        if (!_controller.enabled) return;

        if (_controller.isGrounded)
        {
            _velocity.y = -1f;
        }
        else
        {
            _velocity.y += gravityValue * Runner.DeltaTime;
        }

        if (_jumpPressed && _controller.isGrounded)
        {
            _velocity.y = jumpForce;
        }

        if (CursorManager.Instance.IsCursorLocked())
        {
            //This makes a Strafing movement
            var cameraForward = mainCamera.transform.forward;
            var cameraRight = mainCamera.transform.right;

            cameraForward.y = 0;
            cameraRight.y = 0;

            cameraForward.Normalize();
            cameraRight.Normalize();

            var moveDirection = cameraForward * Input.GetAxis("Vertical") + cameraRight * Input.GetAxis("Horizontal");
            var move = moveDirection * Runner.DeltaTime * playerSpeed;

            _controller.Move(move + _velocity * Runner.DeltaTime);

            //Other Player can also that you are rotating while Idle
            transform.rotation = Quaternion.LookRotation(cameraForward);

            NetworkedRotation = transform.rotation;
        }
        else
        {
            _controller.Move(_velocity * Runner.DeltaTime);
        }

        _jumpPressed = false;
    }

    public override void Render()
    {
        if (!HasStateAuthority)
        {
            transform.rotation = NetworkedRotation;
        }
    }
}