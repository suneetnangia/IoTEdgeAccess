namespace Azure.Iot.Edge.Modules.SecureAccess.Module
{
    using Microsoft.Azure.Devices.Client;
    using System.Threading;
    using System.Threading.Tasks;

    public class ModuleClientWrapper : IModuleClient
    {
        private readonly ModuleClient moduleClient;

        public ModuleClientWrapper(string connectionString)
        {
            // Root Cert available at environment variable ["EdgeModuleCACertificateFile"] 
            // which needs manual adding in dev env if CreateFromEnvironmentAsync is not used.
            var amqpsettings = new AmqpTransportSettings(TransportType.Amqp_Tcp_Only);
            this.moduleClient = ModuleClient.CreateFromConnectionString(connectionString, new ITransportSettings[] { amqpsettings });
        }

        public Task OpenAsync()
        {
            return this.moduleClient.OpenAsync();
        }

        public Task SendEventAsync(string outputName, Message message)
        {
            return this.moduleClient.SendEventAsync(outputName, message);
        }

        public Task SetInputMessageHandlerAsync(string inputName, MessageHandler messageHandler, object userContext, CancellationToken cancellationToken)
        {
            return this.moduleClient.SetInputMessageHandlerAsync(inputName, messageHandler, userContext, cancellationToken);
        }

        public Task SetMethodHandlerAsync(string methodName, MethodCallback methodHandler, object userContext)
        {
            return this.moduleClient.SetMethodHandlerAsync(methodName, methodHandler, userContext);
        }

        public void Dispose()
        {
            this.moduleClient.Dispose();
        }
    }
}