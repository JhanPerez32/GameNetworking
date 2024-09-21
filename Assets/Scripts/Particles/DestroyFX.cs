using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GNW2.Particle
{
    public class DestroyFX : MonoBehaviour
    {
        public void Update()
        {
            Destroy(gameObject, 2f);
        }

    }

}

