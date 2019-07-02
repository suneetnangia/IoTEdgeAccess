namespace Azure.Iot.Edge.Modules.SecureAccess.Device
{
    using System;
    using System.IO;
    using System.Net.Sockets;
    using System.Threading.Tasks;

    public class TcpClientWrapper : ITcpClient
    {
        private readonly TcpClient tcpClient;

        public TcpClientWrapper()
        {
            this.tcpClient = new TcpClient();
        }

        public async Task ConnectAsync(string host, int port)
        {
            await this.tcpClient.ConnectAsync(host, port).ConfigureAwait(false);
        }

        public Stream GetStream()
        {
            return this.tcpClient.GetStream();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            this.tcpClient.Dispose();
        }
    }
}