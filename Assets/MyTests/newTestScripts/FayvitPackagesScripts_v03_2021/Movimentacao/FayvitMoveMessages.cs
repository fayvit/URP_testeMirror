using FayvitMessageAgregator;
using UnityEngine;

namespace FayvitMove
{
    public struct ChangeMoveSpeedMessage : IMessageBase
    {
        public GameObject gameObject;
        public Vector3 velocity;
        public bool lockTarget;
    }

    public struct AnimateFallMessage : IMessageBase
    {
        public GameObject gameObject;
    }
    
    public struct AnimateStartJumpMessage : IMessageBase
    {
        public GameObject gameObject;
    }

    public struct AnimateDownJumpMessage : IMessageBase
    {
        public GameObject gameObject;
    }
}