namespace Azure.Iot.Edge.Modules.SecureAccess.Device
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IStreamingDevice : IDisposable
    {
        Task OpenConnectionAsync(IDeviceClient deviceClient, IClientWebSocket webSocket, ITcpClient tcpClient, CancellationTokenSource cancellationTokenSource);

        string StreamDeviceName { get; }
        string HostName { get; }
        int Port { get; }        
    }
}