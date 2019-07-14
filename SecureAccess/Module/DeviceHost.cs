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
        private IStreamingDevice StreamingDevice { get; }

        private IModuleClient IotHubModuleClient { get; }

        public DeviceHost(IModuleClient moduleClient, IStreamingDevice streamingDevice)
        {
            this.IotHubModuleClient = moduleClient;
            this.StreamingDevice = streamingDevice;
        }

        public async Task OpenConnectionAsync(IClientWebSocket clientWebSocket, ITcpClient tcpClient, CancellationTokenSource cts)
        {
            // Open a connection to the Edge runtime                        
            await this.IotHubModuleClient.OpenAsync().ConfigureAwait(false);

            // Run a virtual device.
            await this.StreamingDevice.OpenConnectionAsync(clientWebSocket, tcpClient, cts).ConfigureAwait(false);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            this.StreamingDevice.Dispose();
        }
    }
}