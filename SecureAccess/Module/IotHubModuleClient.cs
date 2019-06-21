namespace Azure.Iot.Edge.Modules.SecureAccess.Module
{
    using Microsoft.Azure.Devices.Client;
    using System.Net.Security;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading;
    using System.Threading.Tasks;

    public class IotHubModuleClient : IModuleClient
    {
        private readonly ModuleClient moduleClient;

        public IotHubModuleClient(string connectionString)
        {
            var amqpsettings = new AmqpTransportSettings(TransportType.Amqp_Tcp_Only);
            amqpsettings.RemoteCertificateValidationCallback = new RemoteCertificateValidationCallback(ValidateServerCertificate);

            this.moduleClient = ModuleClient.CreateFromConnectionString(connectionString, new ITransportSettings[] { amqpsettings });
            // this.moduleClient = ModuleClient.CreateFromEnvironmentAsync().GetAwaiter().GetResult();
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

        // The following method is invoked by the RemoteCertificateValidationDelegate.
        public static bool ValidateServerCertificate(
              object sender,
              X509Certificate certificate,
              X509Chain chain,
              SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            // TODO: This needs to be fixed.
            return true;
        }

        public void Dispose()
        {
            this.moduleClient.Dispose();
        }
    }
}