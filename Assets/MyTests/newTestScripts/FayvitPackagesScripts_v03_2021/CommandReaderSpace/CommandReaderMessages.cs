using FayvitMessageAgregator;

namespace FayvitCommandReader
{
    public struct RequestHideControllersMessage : IMessageBase
    {
        public Controlador controlador;
    }

    public struct RequestShowControllsMessage : IMessageBase
    {
        public Controlador controlador;
    }

    public struct ChangeHardwareControllerMessage : IMessageBase
    {
        public Controlador controlador;
    }
}