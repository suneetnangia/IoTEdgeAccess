namespace Azure.Iot.Edge.Modules.SecureAccess.Device
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    public interface ITcpClient : IDisposable
    {
        Task ConnectAsync(string host, int port);

        Stream GetStream();
    }
}