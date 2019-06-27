namespace Azure.Iot.Edge.Modules.SecureAccess.Device
{
    public class SecureShell : StreamDevice
    {
        public SecureShell(IDeviceClient deviceClient, string hostName, int port)
            : base(deviceClient, hostName, port)
        {
        }
    }
}