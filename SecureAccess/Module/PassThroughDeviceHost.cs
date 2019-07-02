namespace Azure.Iot.Edge.Modules.SecureAccess.Module
{
    using Azure.Iot.Edge.Modules.SecureAccess.Device;

    /// <summary>
    /// This device host doesnt do any module level message routing, it's purely a host for virtual device e.g. SSH.
    /// </summary>
    public class PureDeviceHost : DeviceHost
    {
        public PureDeviceHost(IModuleClient moduleClient, IStreamingDevice streamingDevice)
            : base(moduleClient, streamingDevice)
        {
        }
    }
}