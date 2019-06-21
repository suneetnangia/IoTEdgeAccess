namespace Azure.Iot.Edge.Modules.SecureAccess.Module
{
    using Azure.Iot.Edge.Modules.SecureAccess.Device;
    using Microsoft.Azure.Devices.Client;

    using System;
    using System.Text;
    using System.Threading.Tasks;

    public class PassThroughDeviceHost : DeviceHost
    {
        public PassThroughDeviceHost(IModuleClient moduleClient, IStreamingDevice streamingDevice)
            : base(moduleClient, streamingDevice)
        {            
        }

        /// <summary>
        /// This method pass through the message receieved.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="userContext"></param>
        /// <returns></returns>
        internal override async Task<MessageResponse> PipeMessage(Message message, object userContext)
        {
            var moduleClient = userContext as ModuleClient;
            if (moduleClient == null)
            {
                throw new InvalidOperationException("UserContext doesn't contain expected values.");
            }

            var messageBytes = message.GetBytes();
            var messageString = Encoding.UTF8.GetString(messageBytes);
            Console.WriteLine($"Received message: Body: [{messageString}]");

            if (!string.IsNullOrEmpty(messageString))
            {
                using (var pipeMessage = new Message(messageBytes))
                {
                    foreach (var prop in message.Properties)
                    {
                        pipeMessage.Properties.Add(prop.Key, prop.Value);
                    }

                    await moduleClient.SendEventAsync("output1", pipeMessage).ConfigureAwait(false);
                    Console.WriteLine("Received message sent.");
                }
            }
            return MessageResponse.Completed;
        }
    }
}