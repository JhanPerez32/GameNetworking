using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    [SerializeField] float lifeTime;

    [Networked] private TickTimer life { get; set; }

    //Layers that will despawn this GameObject
    [SerializeField] private string[] despawnLayerNames;
    private LayerMask despawnLayers;

    private void Start()
    {
        despawnLayers = LayerMask.GetMask(despawnLayerNames);
        Debug.Log("Despawn layers assigned: " + despawnLayers.value);
    }

    public void Init()
    {
        life = TickTimer.CreateFromSeconds(Runner, lifeTime);
    }

    public override void FixedUpdateNetwork()
    {
        if (life.Expired(Runner))
        {
            Debug.Log("Projectile life expired, despawning.");
            DestroyTracer();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (IsInDespawnLayer(collision.gameObject))
        {
            DestroyTracer();
        }
    }

    private void DestroyTracer()
    {
        if (Object != null)
        {
            Runner.Despawn(Object);
        }
        else
        {
            //TODO: This Section is being called sometimes and that must not happen
            Debug.LogError("Attempted to despawn a null object during DestroyTracer!");
        }
    }

    private bool IsInDespawnLayer(GameObject obj)
    {
        return (despawnLayers.value & (1 << obj.layer)) != 0;
    }

}
