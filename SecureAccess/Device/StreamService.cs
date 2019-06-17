namespace Azure.Iot.Edge.Modules.SecureAccess.Device
{
    using Microsoft.Azure.Devices.Client;

    using System;
    using System.Net.WebSockets;
    using System.Threading;
    using System.Threading.Tasks;

    public abstract class StreamDevice : IStreamingDevice
    {
        private const int bufferSize = 10240;

        internal StreamDevice(IDeviceClient deviceClient, IClientWebSocket clientWebSocket, ITCPClient tcpClient, string hostName, int port)
        {
            this.DeviceClient = deviceClient;
            this.ClientWebSocket = clientWebSocket;
            this.TCPClient = tcpClient;

            // Switch on/off the device stream.
            this.IsTurnedOn = true;

            this.HostName = hostName;
            this.Port = port;
        }

        /// <summary>
        /// Device client.
        /// </summary>
        public IDeviceClient DeviceClient { get; }

        /// <summary>
        /// WebSocket client.
        /// </summary>
        public IClientWebSocket ClientWebSocket { get; }

        /// <summary>
        /// TCP client.
        /// </summary>
        public ITCPClient TCPClient { get; }

        /// <summary>
        /// Device stream TTL in seconds.
        /// </summary>
        public bool IsTurnedOn { get; set; }

        /// <summary>
        /// Host name or IP address of the target service e.g. localhost.
        /// </summary>
        public string HostName { get; }

        /// <summary>
        /// Port number of the target service e.g. 22.
        /// </summary>
        public int Port { get; }

        public async Task ConfigureDirectMethods()
        {
            // Wire direct methods from device in IoT Hub.
            await this.DeviceClient.SetMethodHandlerAsync(nameof(this.TurnOn), this.TurnOn, null).ConfigureAwait(false);
            await this.DeviceClient.SetMethodHandlerAsync(nameof(this.TurnOff), this.TurnOff, null).ConfigureAwait(false);
        }

        public async Task OpenConnectionAsync(CancellationTokenSource cancellationTokenSource)
        {
            await this.OpenConnection(cancellationTokenSource).ConfigureAwait(false);
        }

        /// <summary>
        /// Virtual because it allows websockets to other than IoT Hub stream endpoint if needed by a service.
        /// </summary>        
        /// <param name="cancellationTokenSource">Use this to cancel in-progress activity.</param>
        /// <returns></returns>
        internal virtual async Task OpenConnection(CancellationTokenSource cancellationTokenSource)
        {
            DeviceStreamRequest streamRequest = await this.DeviceClient.WaitForDeviceStreamRequestAsync(cancellationTokenSource.Token).ConfigureAwait(false);

            if (streamRequest != null && this.IsTurnedOn)
            {
                await this.DeviceClient.AcceptDeviceStreamRequestAsync(streamRequest, cancellationTokenSource.Token).ConfigureAwait(false);
                Console.WriteLine($"Device stream accepted from IoT Hub, at {DateTime.UtcNow}");

                this.ClientWebSocket.Options.SetRequestHeader("Authorization", $"Bearer {streamRequest.AuthorizationToken}");

                await this.ClientWebSocket.ConnectAsync(streamRequest.Url, cancellationTokenSource.Token).ConfigureAwait(false);
                Console.WriteLine($"Device stream connected to IoT Hub, at {DateTime.UtcNow}");

                await this.TCPClient.ConnectAsync(this.HostName, this.Port).ConfigureAwait(false);
                Console.WriteLine($"Device stream connected to local endpoint, at {DateTime.UtcNow}");

                using (var localStream = this.TCPClient.GetStream())
                {
                    await Task.WhenAny(
                        this.HandleIncomingDataAsync(localStream, cancellationTokenSource.Token),
                        this.HandleOutgoingDataAsync(localStream, cancellationTokenSource.Token)).ConfigureAwait(false);

                    localStream.Close();
                }

                await this.ClientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, String.Empty, cancellationTokenSource.Token).ConfigureAwait(false);
                Console.WriteLine($"Device stream closed to local endpoint, at {DateTime.UtcNow}");
            }
            else
            {
                await this.DeviceClient.RejectDeviceStreamRequestAsync(streamRequest, cancellationTokenSource.Token).ConfigureAwait(false);
            }
        }

        private async Task<MethodResponse> TurnOn(MethodRequest methodRequest, object userContext)
        {
            // Switch on the device stream for new connections.
            this.IsTurnedOn = true;
            return await Task.FromResult(new MethodResponse(Array.Empty<byte>(), 200)).ConfigureAwait(false);
        }

        private Task<MethodResponse> TurnOff(MethodRequest methodRequest, object userContext)
        {
            // Switch off the device stream for new connections, existing connections will persist.
            this.IsTurnedOn = false;
            return Task.FromResult(new MethodResponse(Array.Empty<byte>(), 200));
        }

        private async Task HandleIncomingDataAsync(INetworkStream localStream, CancellationToken cancellationToken)
        {
            var buffer = new byte[bufferSize];

            while (this.ClientWebSocket.State == WebSocketState.Open)
            {
                var receiveResult = await this.ClientWebSocket.ReceiveAsync(buffer, cancellationToken).ConfigureAwait(false);

                await localStream.WriteAsync(buffer, 0, receiveResult.Count).ConfigureAwait(false);
            }
        }

        private async Task HandleOutgoingDataAsync(INetworkStream localStream, CancellationToken cancellationToken)
        {
            var buffer = new byte[bufferSize];

            while (localStream.CanRead)
            {
                var receiveCount = await localStream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);

                await this.ClientWebSocket.SendAsync(new ArraySegment<byte>(buffer, 0, receiveCount), WebSocketMessageType.Binary, true, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}