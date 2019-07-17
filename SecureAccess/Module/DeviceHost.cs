namespace Azure.Iot.Edge.Modules.SecureAccess.Module
{
    using Azure.Iot.Edge.Modules.SecureAccess.Device;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    
    /// <summary>
    /// Base class to host a virtual device and act as a module of its own in IoT Edge environment.
    /// This can be built further to have host level features e.g. multiple devices, inter module messaging.
    /// </summary>
    public abstract class DeviceHost : IDeviceHost
    {
        private IModuleClient IotHubModuleClient { get; }

        public DeviceHost(IModuleClient moduleClient)
        {
            this.IotHubModuleClient = moduleClient;
        }

        public async Task OpenConnectionAsync(IStreamingDevice streamingDevice, IDeviceClient deviceClient, IClientWebSocket webSocket, ITcpClient tcpClient, CancellationTokenSource cts)
        {
            // Open a connection to the Edge runtime                        
            await this.IotHubModuleClient.OpenAsync().ConfigureAwait(false);

            // Run a virtual device
            await streamingDevice.OpenConnectionAsync(deviceClient, webSocket, tcpClient, cts).ConfigureAwait(false);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}