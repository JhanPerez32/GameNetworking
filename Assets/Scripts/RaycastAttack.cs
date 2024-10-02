using Fusion;
using UnityEngine;

public class RaycastAttack : NetworkBehaviour
{
    public float Damage = 10;

    public PlayerMovement PlayerMovement;

    public const float fireDelay = .2f;
    public float delayTimeLeft;

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority) return;

        var ray = PlayerMovement.mainCamera.ScreenPointToRay(Input.mousePosition);
        ray.origin += PlayerMovement.mainCamera.transform.forward;

        delayTimeLeft -= Runner.DeltaTime;
        if (!Input.GetMouseButton(0)) return;
        if (delayTimeLeft > 0) return;
        delayTimeLeft = fireDelay;
        Debug.DrawRay(ray.origin, ray.direction, Color.red, 1f);
        Debug.Log("Firing");
        
        if (!Physics.Raycast(ray.origin, ray.direction, out var hit)) return;
        Debug.Log($"Firing and hit {hit.collider.gameObject.name}");
        if (!hit.collider.TryGetComponent<PlayerHealth>(out var health)) return;
        Debug.Log("Hit and dealing Damage");
        health.DealDamageRpc(Damage);
    }
}