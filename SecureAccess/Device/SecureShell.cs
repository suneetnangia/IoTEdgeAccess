namespace Azure.Iot.Edge.Modules.SecureAccess.Device
{
    // Implement any service specific code here e.g. buffer size, default ports etc.
    public class SecureShell : StreamDevice
    {
        public SecureShell(IDeviceClient deviceClient, string hostName, int port = 22)
            : base(deviceClient, hostName, port)
        {
        }
    }
}