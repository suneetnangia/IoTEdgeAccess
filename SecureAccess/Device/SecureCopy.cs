using System;

namespace Azure.Iot.Edge.Modules.SecureAccess.Device
{
    // Implement any service specific code here e.g. buffer size, default ports etc.
    public class SecureCopy : StreamDevice
    {
        public SecureCopy(string hostName, int port = 22)
            : base(hostName, port, "SecureCopy")
        {            
            Console.WriteLine($"Secure Copy virtual device initiating...");
        }        
    }
}