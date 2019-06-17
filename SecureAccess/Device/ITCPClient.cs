namespace Azure.Iot.Edge.Modules.SecureAccess.Device
{
    using System.Threading.Tasks;

    public interface ITCPClient
    {
        Task ConnectAsync(string host, int port);

        INetworkStream GetStream();
    }
}