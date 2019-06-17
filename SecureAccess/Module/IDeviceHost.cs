namespace Azure.Iot.Edge.Modules.SecureAccess.Module
{
    using System.Threading;
    using System.Threading.Tasks;


    internal interface IDeviceHost
    {
        Task OpenConnectionAsync(CancellationTokenSource cts);
    }
}