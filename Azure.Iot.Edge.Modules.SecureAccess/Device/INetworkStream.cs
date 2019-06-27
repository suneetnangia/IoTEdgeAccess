namespace Azure.Iot.Edge.Modules.SecureAccess.Device
{
    using System;
    using System.Threading.Tasks;

    public interface INetworkStream : IDisposable
    {
        Task<int> ReadAsync(byte[] buffer, int offset, int count);
        Task WriteAsync(byte[] buffer, int offset, int count);
        bool CanRead { get; }
        void Close();
    }
}