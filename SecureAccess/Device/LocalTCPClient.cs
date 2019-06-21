namespace Azure.Iot.Edge.Modules.SecureAccess.Device
{
    using System.Net.Sockets;
    using System.Threading.Tasks;

    public class LocalTCPClient : ITcpClient
    {
        private readonly TcpClient tcpClient;

        public LocalTCPClient()
        {
            this.tcpClient = new TcpClient();
        }

        public Task ConnectAsync(string host, int port)
        {
            return this.tcpClient.ConnectAsync(host, port);
        }

        public INetworkStream GetStream()
        {
            return new LocalNetworkStream(this.tcpClient.GetStream());
        }

        public void Dispose()
        {
            this.tcpClient.Dispose();
        }
    }
}