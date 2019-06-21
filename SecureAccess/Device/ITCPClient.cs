namespace Azure.Iot.Edge.Modules.SecureAccess.Device
{
    using System;
    using System.Threading.Tasks;

    public interface ITCPClient : IDisposable
    {
        Task ConnectAsync(string host, int port);

        INetworkStream GetStream();
    }
}