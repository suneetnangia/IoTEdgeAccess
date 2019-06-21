namespace Azure.Iot.Edge.Modules.SecureAccess.Device
{
    using Microsoft.Azure.Devices.Client;
    using System.Threading;
    using System.Threading.Tasks;

    internal class IoTHubDeviceClient : IDeviceClient
    {
        private const TransportType deviceTransportType = TransportType.Amqp;
        private readonly DeviceClient deviceClient;

        internal IoTHubDeviceClient(string connectionString)
        {
            this.deviceClient = DeviceClient.CreateFromConnectionString(connectionString, deviceTransportType);
        }

        public Task<DeviceStreamRequest> WaitForDeviceStreamRequestAsync(CancellationToken cancellationToken)
        {
            return this.deviceClient.WaitForDeviceStreamRequestAsync(cancellationToken);
        }

        public Task AcceptDeviceStreamRequestAsync(DeviceStreamRequest request, CancellationToken cancellationToken)
        {
            return this.deviceClient.AcceptDeviceStreamRequestAsync(request, cancellationToken);
        }

        public Task RejectDeviceStreamRequestAsync(DeviceStreamRequest request, CancellationToken cancellationToken)
        {
            return this.deviceClient.RejectDeviceStreamRequestAsync(request, cancellationToken);
        }

        public Task SetMethodHandlerAsync(string methodName, MethodCallback methodHandler, object userContext)
        {
            return this.deviceClient.SetMethodHandlerAsync(methodName, methodHandler, userContext);
        }

        public void Dispose()
        {
            this.deviceClient.Dispose();
        }
    }
}