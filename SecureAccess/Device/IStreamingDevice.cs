namespace Azure.Iot.Edge.Modules.SecureAccess.Device
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IStreamingDevice : IDisposable
    {
        Task OpenConnectionAsync(IClientWebSocket clientWebSocket, ITcpClient tcpClient, CancellationTokenSource cancellationTokenSource);

        string HostName { get; }
        int Port { get; }
    }
}