namespace Azure.Iot.Edge.Modules.SecureAccess.Module
{
    using Azure.Iot.Edge.Modules.SecureAccess.Device;
    using Microsoft.Azure.Devices.Client;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Base class to host virtual device and act as a module of its own in IoT Edge environment.
    /// Features-
    /// 1. Streaming Session TTL 
    /// 2. Device Isolation
    /// </summary>
    internal abstract class DeviceHost : IDeviceHost
    {
        private readonly IStreamingDevice streamingDevice;

        internal TransportType TransportType { get; }

        internal DeviceHost(IStreamingDevice streamingDevice, TransportType transportType)
        {
            this.streamingDevice = streamingDevice;
            this.TransportType = transportType;
        }

        public async Task OpenConnectionAsync(CancellationTokenSource cts)
        {
            ITransportSettings[] settings = { new AmqpTransportSettings(this.TransportType) };

            // Open a connection to the Edge runtime
            using (var ioTHubModuleClient = await ModuleClient.CreateFromEnvironmentAsync(settings).ConfigureAwait(false))
            {
                await ioTHubModuleClient.OpenAsync().ConfigureAwait(false);

                // Register callback to be called when a message is received by the module
                await ioTHubModuleClient.SetInputMessageHandlerAsync("input1", this.PipeMessage, ioTHubModuleClient, cts.Token).ConfigureAwait(false);
            }

            // Run a virtual device.
            await this.RunDevice(cts).ConfigureAwait(false);
        }

        internal abstract Task<MessageResponse> PipeMessage(Message message, object userContext);

        internal async Task RunDevice(CancellationTokenSource cts)
        {
            await this.streamingDevice.OpenConnectionAsync(cts).ConfigureAwait(false);
        }
    }
}