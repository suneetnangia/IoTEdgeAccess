namespace Azure.Iot.Edge.Modules.SecureAccess.Device
{
    using System.Net.Sockets;
    using System.Threading.Tasks;

    public class LocalNetworkStream : INetworkStream
    {
        private readonly NetworkStream networkStream;

        public LocalNetworkStream(NetworkStream networkStream)
        {
            this.networkStream = networkStream;
        }

        public bool CanRead { get { return this.networkStream.CanRead; } }

        public Task<int> ReadAsync(byte[] buffer, int offset, int count)
        {
            return this.networkStream.ReadAsync(buffer, offset, count);
        }

        public Task WriteAsync(byte[] buffer, int offset, int count)
        {
            return this.networkStream.WriteAsync(buffer, offset, count);
        }

        public void Close()
        {
            this.networkStream.Close();
        }

        public void Dispose()
        {
            this.Dispose();
        }
    }
}
