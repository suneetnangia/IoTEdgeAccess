namespace Azure.Iot.Edge.Modules.SecureAccess.Device
{
    using System;

    public class SecureShell : StreamDevice
    {
        public SecureShell(IDeviceClient deviceClient, IClientWebSocket clientWebSocket, ITCPClient tcpClient, string hostName, int port)
            : base(deviceClient, clientWebSocket, tcpClient, hostName, port)
        {
        }
    }
}