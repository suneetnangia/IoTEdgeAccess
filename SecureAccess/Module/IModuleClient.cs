namespace Azure.Iot.Edge.Modules.SecureAccess.Module
{
    using Microsoft.Azure.Devices.Client;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IModuleClient : IDisposable
    {
        Task OpenAsync();
        Task SendEventAsync(string outputName, Message message);
        Task SetInputMessageHandlerAsync(string inputName, MessageHandler messageHandler, object userContext, CancellationToken cancellationToken);
    }
}