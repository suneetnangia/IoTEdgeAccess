namespace Azure.Iot.Edge.Modules.SecureAccess.Module
{
    using Azure.Iot.Edge.Modules.SecureAccess.Device;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    internal interface IDeviceHost : IDisposable
    {
        Task OpenConnectionAsync(IClientWebSocket clientWebSocket, ITcpClient tcpClient, CancellationTokenSource cts);
    }
}