namespace Azure.Iot.Edge.Modules.SecureAccess.Device
{
    using System;
    using System.IO;
    using System.Net.Sockets;
    using System.Threading.Tasks;

    public class TcpClientWrapper : ITcpClient
    {
        private bool disposed = false;
        private readonly TcpClient tcpClient;

        public TcpClientWrapper()
        {
            this.tcpClient = new TcpClient();
        }

        public Task ConnectAsync(string host, int port)
        {
            return this.tcpClient.ConnectAsync(host, port);
        }

        public Stream GetStream()
        {
            return this.tcpClient.GetStream();
        }


        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
                return;

            if (disposing)
                this.tcpClient.Dispose();

            this.disposed = true;
        }

        ~TcpClientWrapper()
        {
            this.Dispose(false);
        }
    }
}