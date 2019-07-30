namespace Azure.Iot.Edge.Modules.SecureAccess.Device
{
    using System;
    using System.Net.WebSockets;
    using System.Threading;
    using System.Threading.Tasks;

    public class ClientWebSocketWrapper : IClientWebSocket
    {
        private bool disposed = false;
        private readonly ClientWebSocket clientWebSocket;

        public ClientWebSocketWrapper()
        {
            this.clientWebSocket = new ClientWebSocket();
        }

        public WebSocketState State => this.clientWebSocket.State;

        public ClientWebSocketOptions Options { get { return this.clientWebSocket.Options; } }

        public Task CloseAsync(WebSocketCloseStatus closeStatus, string statusDescription, CancellationToken cancellationToken)
        {
            return this.clientWebSocket.CloseAsync(closeStatus, statusDescription, cancellationToken);
        }

        public Task ConnectAsync(Uri uri, CancellationToken cancellationToken)
        {
            return this.clientWebSocket.ConnectAsync(uri, cancellationToken);
        }

        public Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken)
        {
            return this.clientWebSocket.ReceiveAsync(buffer, cancellationToken);
        }

        public Task SendAsync(ArraySegment<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationToken)
        {
            return this.clientWebSocket.SendAsync(buffer, messageType, endOfMessage, cancellationToken);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
                return;

            if (disposing)
                this.clientWebSocket.Dispose();

            this.disposed = true;
        }

        ~ClientWebSocketWrapper()
        {
            this.Dispose(false);
        }
    }
}
