﻿namespace Azure.Iot.Edge.Modules.SecureAccess.Device
{
    using Microsoft.Azure.Devices.Client;

    using System;
    using System.Net.WebSockets;
    using System.Threading;
    using System.Threading.Tasks;

    public abstract class StreamDevice : IStreamingDevice
    {
        private const int bufferSize = 1024;

        public StreamDevice(IDeviceClient deviceClient, string hostName, int port)
        {
            this.DeviceClient = deviceClient;                       

            this.HostName = hostName;
            this.Port = port;
        }

        /// <summary>
        /// Device client.
        /// </summary>
        public IDeviceClient DeviceClient { get; }        

        /// <summary>
        /// Host name or IP address of the target service e.g. localhost.
        /// </summary>
        public string HostName { get; }

        /// <summary>
        /// Port number of the target service e.g. 22.
        /// </summary>
        public int Port { get; }        

        public async Task OpenConnectionAsync(IClientWebSocket clientWebSocket, ITcpClient tcpClient, CancellationTokenSource cancellationTokenSource)
        {
            DeviceStreamRequest streamRequest = await this.DeviceClient.WaitForDeviceStreamRequestAsync(cancellationTokenSource.Token).ConfigureAwait(false);

            if (streamRequest != null)
            {
                await this.DeviceClient.AcceptDeviceStreamRequestAsync(streamRequest, cancellationTokenSource.Token).ConfigureAwait(false);
                Console.WriteLine($"Device stream accepted from IoT Hub, at {DateTime.UtcNow}");

                clientWebSocket.Options.SetRequestHeader("Authorization", $"Bearer {streamRequest.AuthorizationToken}");

                await clientWebSocket.ConnectAsync(streamRequest.Url, cancellationTokenSource.Token).ConfigureAwait(false);
                Console.WriteLine($"Device stream connected to IoT Hub, at {DateTime.UtcNow}");

                await tcpClient.ConnectAsync(this.HostName, this.Port).ConfigureAwait(false);
                Console.WriteLine($"Device stream connected to local endpoint, at {DateTime.UtcNow}");

                using (var localStream = tcpClient.GetStream())
                {
                    await Task.WhenAny(
                        this.HandleIncomingDataAsync(clientWebSocket, localStream, cancellationTokenSource.Token),
                        this.HandleOutgoingDataAsync(clientWebSocket, localStream, cancellationTokenSource.Token)).ConfigureAwait(false);

                    localStream.Close();
                    Console.WriteLine($"Device stream closed to local endpoint, at {DateTime.UtcNow}");
                }

                await clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, String.Empty, cancellationTokenSource.Token).ConfigureAwait(false);
                Console.WriteLine($"Device stream closed to remote websocket endpoint, at {DateTime.UtcNow}");
            }
            else
            {
                await this.DeviceClient.RejectDeviceStreamRequestAsync(streamRequest, cancellationTokenSource.Token).ConfigureAwait(false);
            }
        }            

        private async Task HandleIncomingDataAsync(IClientWebSocket clientWebSocket, INetworkStream localStream, CancellationToken cancellationToken)
        {
            var buffer = new byte[bufferSize];

            while (clientWebSocket.State == WebSocketState.Open)
            {
                var receiveResult = await clientWebSocket.ReceiveAsync(buffer, cancellationToken).ConfigureAwait(false);

                await localStream.WriteAsync(buffer, 0, receiveResult.Count).ConfigureAwait(false);
            }
        }

        private async Task HandleOutgoingDataAsync(IClientWebSocket clientWebSocket, INetworkStream localStream, CancellationToken cancellationToken)
        {
            var buffer = new byte[bufferSize];

            while (localStream.CanRead)
            {
                var receiveCount = await localStream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);

                await clientWebSocket.SendAsync(new ArraySegment<byte>(buffer, 0, receiveCount), WebSocketMessageType.Binary, true, cancellationToken).ConfigureAwait(false);
            }
        }

        public void Dispose()
        {
            this.DeviceClient.Dispose();
        }
    }
}