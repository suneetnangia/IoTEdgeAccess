namespace Azure.Iot.Edge.Modules.SecureAccess.Device
{
    using Microsoft.Azure.Devices.Client;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    internal class DeviceClientWrapper : IDeviceClient
    {
        private bool disposed = false;
        private const TransportType deviceTransportType = TransportType.Amqp;
        private readonly DeviceClient deviceClient;

        internal DeviceClientWrapper(string connectionString)
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
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
                return;

            if (disposing)
                this.deviceClient.Dispose();

            this.disposed = true;
        }

        ~DeviceClientWrapper()
        {
            this.Dispose(false);
        }
    }
}