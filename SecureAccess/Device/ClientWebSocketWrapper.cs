namespace Azure.Iot.Edge.Modules.SecureAccess.Device
{
    using System;
    using System.Net.WebSockets;
    using System.Threading;
    using System.Threading.Tasks;

    public class ClientWebSocketWrapper : IClientWebSocket
    {
        private readonly ClientWebSocket clientWebSocket;

        public ClientWebSocketWrapper()
        {
            this.clientWebSocket = new ClientWebSocket();
        }

        public WebSocketState State { get { return this.clientWebSocket.State; } }

        public ClientWebSocketOptions Options { get { return this.clientWebSocket.Options; } }

        public async Task CloseAsync(WebSocketCloseStatus closeStatus, string statusDescription, CancellationToken cancellationToken)
        {
            await this.clientWebSocket.CloseAsync(closeStatus, statusDescription, cancellationToken).ConfigureAwait(false);
        }

        public async Task ConnectAsync(Uri uri, CancellationToken cancellationToken)
        {
            await this.clientWebSocket.ConnectAsync(uri, cancellationToken).ConfigureAwait(false);
        }

        public async Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken)
        {
            return await this.clientWebSocket.ReceiveAsync(buffer, cancellationToken).ConfigureAwait(false);
        }

        public async Task SendAsync(ArraySegment<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationToken)
        {
            await this.clientWebSocket.SendAsync(buffer, messageType, endOfMessage, cancellationToken).ConfigureAwait(false);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            this.clientWebSocket.Dispose();
        }
    }
}
