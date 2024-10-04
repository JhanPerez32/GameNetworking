using Fusion;
using UnityEngine;

public class RaycastAttack : NetworkBehaviour
{
    public float Damage = 10;
    public PlayerMovement PlayerMovement;

    public GameObject tracerPrefab;
    public Transform firePoint;

    public const float fireDelay = .5f;
    public float delayTimeLeft;

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority) return;

        if (CursorManager.Instance.IsCursorLocked())
        {
            var ray = PlayerMovement.mainCamera.ScreenPointToRay(Input.mousePosition);
            ray.origin += PlayerMovement.mainCamera.transform.forward;

            delayTimeLeft -= Runner.DeltaTime;
            if (!Input.GetMouseButton(0)) return;
            if (delayTimeLeft > 0) return;

            delayTimeLeft = fireDelay;
            Debug.Log("Firing Raycast");

            if (Physics.Raycast(ray.origin, ray.direction, out var hit))
            {
                Debug.Log($"Firing and hit {hit.collider.gameObject.name}");

                CreateTracerEffect(hit.point);

                if (hit.collider.TryGetComponent<PlayerHealth>(out var health))
                {
                    Debug.Log("Hit and dealing damage");
                    health.DealDamageRpc(Damage);
                }
            }
        }
    }

    private void CreateTracerEffect(Vector3 hitPoint)
    {
        NetworkObject tracerNetworkObject = Runner.Spawn(tracerPrefab, firePoint.position, Quaternion.identity);
        GameObject tracer = tracerNetworkObject.gameObject;

        //tracer.GetComponent<Projectile>()?.Init();

        Vector3 direction = (hitPoint - firePoint.position).normalized;
        float distance = Vector3.Distance(firePoint.position, hitPoint);

        Rigidbody tracerRb = tracer.GetComponent<Rigidbody>();
        tracerRb.velocity = direction * distance * 10;
    }
}