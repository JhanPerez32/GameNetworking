using Fusion;
using UnityEngine;

namespace GNW2.Input
{
    public struct NetworkInputData : INetworkInput
    {
        public Vector3 Direction;
        public bool Jump;
        public float MouseX;
        public float MouseY;

        public const byte MOUSEBUTTON0 = 1;
        public NetworkButtons buttons;
    }
}
