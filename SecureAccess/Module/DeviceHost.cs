namespace Azure.Iot.Edge.Modules.SecureAccess.Module
{
    using Azure.Iot.Edge.Modules.SecureAccess.Device;
    using Microsoft.Azure.Devices.Client;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Base class to host a virtual device and act as a module of its own in IoT Edge environment.    
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

            // Register callback to be called when a message is received by the module
            await this.IotHubModuleClient.SetInputMessageHandlerAsync("input1", this.PipeMessage, this.IotHubModuleClient, cts.Token).ConfigureAwait(false);


            // Run a virtual device.
            await this.StreamingDevice.OpenConnectionAsync(clientWebSocket, tcpClient, cts).ConfigureAwait(false);
        }

        internal abstract Task<MessageResponse> PipeMessage(Message message, object userContext);

        public void Dispose()
        {
            this.StreamingDevice.Dispose();
        }
    }
}