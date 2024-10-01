using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerColor : NetworkBehaviour
{
    public MeshRenderer renderer;


    [Networked, OnChangedRender(nameof(OnColorChange))]
    public Color color { get; set; }

    public override void FixedUpdateNetwork()
    {
        if(!HasStateAuthority) return;
        if(!Input.GetKeyDown(KeyCode.E)) return;

        color  = new Color(Random.Range(0f,1f), Random.Range(0f,1f), Random.Range(0f,1f));
    }

    private void OnColorChange()
    {
        if(renderer != null)
        {
            renderer.material.color = color;
        }
    }
}
