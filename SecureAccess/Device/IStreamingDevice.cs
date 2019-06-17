namespace Azure.Iot.Edge.Modules.SecureAccess.Device
{
    using System.Threading;
    using System.Threading.Tasks;

    internal interface IStreamingDevice
    {
        Task OpenConnectionAsync(CancellationTokenSource cancellationTokenSource);
        Task ConfigureDirectMethods();

        string HostName { get; }
        int Port { get; }
    }
}