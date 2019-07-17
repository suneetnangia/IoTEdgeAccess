namespace Azure.Iot.Edge.Modules.SecureAccess.Device
{
    using System;

    // Implement any service specific code here e.g. buffer size, default ports etc.
    public class SecureShell : StreamDevice
    {
        public SecureShell(string hostName, int port = 22)
            : base(hostName, port, "SecureShell")
        {
            Console.WriteLine($"Secure Shell virtual device initiating...");
        }        
    }
}