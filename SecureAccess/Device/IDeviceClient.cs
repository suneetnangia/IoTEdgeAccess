namespace Azure.Iot.Edge.Modules.SecureAccess.Device
{
    using Microsoft.Azure.Devices.Client;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IDeviceClient : IDisposable
    {
        Task SetMethodHandlerAsync(string methodName, MethodCallback methodHandler, object userContext);
        Task<DeviceStreamRequest> WaitForDeviceStreamRequestAsync(CancellationToken cancellationToken);
        Task AcceptDeviceStreamRequestAsync(DeviceStreamRequest request, CancellationToken cancellationToken);
        Task RejectDeviceStreamRequestAsync(DeviceStreamRequest request, CancellationToken cancellationToken);
    }
}