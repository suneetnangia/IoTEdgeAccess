namespace Azure.Iot.Edge.Modules.SecureAccess.Device
{
    using Microsoft.Azure.Devices.Client;
    using System.Threading;
    using System.Threading.Tasks;

    internal class DeviceClientWrapper : IDeviceClient
    {
        private const TransportType deviceTransportType = TransportType.Amqp;
        private readonly DeviceClient deviceClient;

        internal DeviceClientWrapper(string connectionString)
        {
            this.deviceClient = DeviceClient.CreateFromConnectionString(connectionString, deviceTransportType);
        }

        public async Task<DeviceStreamRequest> WaitForDeviceStreamRequestAsync(CancellationToken cancellationToken)
        {
            return await this.deviceClient.WaitForDeviceStreamRequestAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task AcceptDeviceStreamRequestAsync(DeviceStreamRequest request, CancellationToken cancellationToken)
        {
            await this.deviceClient.AcceptDeviceStreamRequestAsync(request, cancellationToken).ConfigureAwait(false);
        }

        public async Task RejectDeviceStreamRequestAsync(DeviceStreamRequest request, CancellationToken cancellationToken)
        {
            await this.deviceClient.RejectDeviceStreamRequestAsync(request, cancellationToken).ConfigureAwait(false);
        }

        public async Task SetMethodHandlerAsync(string methodName, MethodCallback methodHandler, object userContext)
        {
            await this.deviceClient.SetMethodHandlerAsync(methodName, methodHandler, userContext).ConfigureAwait(false);
        }

        public void Dispose()
        {
            this.deviceClient.Dispose();
        }
    }
}