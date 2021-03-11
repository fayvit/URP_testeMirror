using FayvitMessageAgregator;

namespace FayvitCam
    {
    public struct ControlableReachedMessage : IMessageBase
    {
        public CamFeatures camFeatures;
    }

    public struct RequestShakeCamMessage : IMessageBase
    {
        public ShakeAxis shakeAxis;
        public int numShake;
        public float shakeAngle;
    }
}