using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class RaycastAttack : NetworkBehaviour
{
    public float Damage = 20;
    public PlayerMovement PlayerMovement;

    public GameObject tracerPrefab;
    public Transform firePoint;

    public const float fireDelay = .5f;
    public float delayTimeLeft;

    [Header("UI Health bar")]
    public Slider UIGunEnergyBar;
    public float maxGunEnergy = 100f;  // Maximum gun energy
    public float gunEnergyCost = 10f;  // Energy cost per shot
    [HideInInspector] public float currentGunEnergy;

    public bool isRefilling = false;

    public override void Spawned()
    {
        base.Spawned();
        currentGunEnergy = maxGunEnergy;

        if (UIGunEnergyBar != null)
        {
            UIGunEnergyBar.maxValue = maxGunEnergy;
            UIGunEnergyBar.value = currentGunEnergy;
        }
    }

    public void RefillGunEnergy(float amount)
    {
        currentGunEnergy = Mathf.Min(currentGunEnergy + amount, maxGunEnergy);

        if (UIGunEnergyBar != null)
        {
            UIGunEnergyBar.value = currentGunEnergy;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority || isRefilling) return;

        if (CursorManager.Instance.IsCursorLocked())
        {
            var ray = PlayerMovement.mainCamera.ScreenPointToRay(Input.mousePosition);
            ray.origin += PlayerMovement.mainCamera.transform.forward;

            delayTimeLeft -= Runner.DeltaTime;

            if (!Input.GetMouseButton(0)) return;
            if (delayTimeLeft > 0) return;
            if (currentGunEnergy < gunEnergyCost) return;

            delayTimeLeft = fireDelay;
            Debug.Log("Firing Raycast");

            currentGunEnergy -= gunEnergyCost;  // Reduce gun energy
            if (UIGunEnergyBar != null)
            {
                UIGunEnergyBar.value = currentGunEnergy;  // Update UI Gun energy slider
            }

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
        GameObject tracer = Instantiate(tracerPrefab, firePoint.position, Quaternion.identity);
        //NetworkObject tracerNetworkObject = Runner.Spawn(tracerPrefab, firePoint.position, Quaternion.identity);
        //GameObject tracer = tracerNetworkObject.gameObject;

        //tracer.GetComponent<Projectile>()?.Init();

        Vector3 direction = (hitPoint - firePoint.position).normalized;
        float distance = Vector3.Distance(firePoint.position, hitPoint);

        Rigidbody tracerRb = tracer.GetComponent<Rigidbody>();
        tracerRb.velocity = direction * distance * 10;
    }
}