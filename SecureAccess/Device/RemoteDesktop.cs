namespace Azure.Iot.Edge.Modules.SecureAccess.Device
{
    using System;

    // Implement any service specific code here e.g. buffer size, default ports etc.
    public class RemoteDesktop : StreamDevice
    {
        public RemoteDesktop(string hostName, int port = 3389)
            : base(hostName, port, "RemoteDesktop")
        {
            Console.WriteLine($"Remote Desktop virtual device initiating...");
        }
    }
}