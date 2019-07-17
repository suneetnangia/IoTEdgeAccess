namespace Azure.Iot.Edge.Modules.SecureAccess.Module
{
    /// <summary>
    /// This device host doesnt do any module level message routing, it's purely a host for virtual devices e.g. SSH.
    /// </summary>
    public class PureDeviceHost : DeviceHost
    {
        public PureDeviceHost(IModuleClient moduleClient)
            : base(moduleClient)
        {
        }
    }
}