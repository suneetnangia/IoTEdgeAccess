namespace Azure.Iot.Edge.Modules.SecureAccess.Tests
{
    using Azure.Iot.Edge.Modules.SecureAccess.Device;
    using Microsoft.Azure.Devices.Client;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;
    using System;
    using System.Net.WebSockets;
    using System.Threading;
    using System.Threading.Tasks;

    [TestClass]
    public class StreamDeviceTests
    {
        private static readonly Uri uri = new Uri("ws://dummy.com");
        private static readonly int localPort = 22;
        private static readonly string localhost = "localhost";
        private static readonly int streamReturnValue = 1;
        private static readonly int bufferSize = 1024;

        private Mock<IDeviceClient> deviceClientMock;
        private Mock<IClientWebSocket> clientWebSocket;
        private Mock<ITcpClient> tcpClient;
        private Mock<INetworkStream> networkStream;
        private ClientWebSocket realClientWebSocket;
        private CancellationTokenSource cancellationTokenSource;

        private readonly byte[] buffer = new byte[bufferSize];
        private bool toggleStateWebSocket = false;
        private bool toggleStateNetworkStream = false;
        private DeviceStreamRequest deviceStreamRequest;

        [TestInitialize]
        public void Setup()
        {
            this.deviceClientMock = new Mock<IDeviceClient>();
            this.clientWebSocket = new Mock<IClientWebSocket>();
            this.tcpClient = new Mock<ITcpClient>();
            this.networkStream = new Mock<INetworkStream>();
            this.realClientWebSocket = new ClientWebSocket();
            this.cancellationTokenSource = new CancellationTokenSource();

            this.deviceStreamRequest = new DeviceStreamRequest("001", "teststream01", uri, "dsdfsdrer32");

            this.deviceClientMock.Setup(dc => dc.WaitForDeviceStreamRequestAsync(this.cancellationTokenSource.Token))
                                               .ReturnsAsync(() => { return this.deviceStreamRequest; });

            this.clientWebSocket.Setup(dc => dc.ConnectAsync(uri, this.cancellationTokenSource.Token))
                                        .Returns(Task.FromResult(0));
            this.clientWebSocket.Setup(dc => dc.ReceiveAsync(this.buffer, this.cancellationTokenSource.Token))
                                        .ReturnsAsync(new WebSocketReceiveResult(streamReturnValue, WebSocketMessageType.Binary, true));

            this.clientWebSocket.Setup(dc => dc.Options).Returns(this.realClientWebSocket.Options);
            this.clientWebSocket.Setup(dc => dc.State).Returns(() => { this.toggleStateWebSocket = !this.toggleStateWebSocket; return this.toggleStateWebSocket ? WebSocketState.Open : WebSocketState.Closed; });

            this.tcpClient.Setup(tc => tc.ConnectAsync(localhost, localPort)).Returns(Task.FromResult(0));
            this.tcpClient.Setup(tc => tc.GetStream()).Returns(this.networkStream.Object);

            this.networkStream.Setup(ns => ns.WriteAsync(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>())).Returns(Task.FromResult(0));
            this.networkStream.Setup(ns => ns.CanRead).Returns(() => { this.toggleStateNetworkStream = !this.toggleStateNetworkStream; return this.toggleStateNetworkStream ? true : false; });

            this.networkStream.Setup(ns => ns.ReadAsync(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync((byte[] r, int o, int s) =>
                                        {
                                            r[0] = 1;
                                            return streamReturnValue;
                                        });
        }

        [TestCleanup]
        public void CleanUp()
        {
            this.realClientWebSocket.Dispose();
            this.cancellationTokenSource.Dispose();
        }
              

        [TestMethod]
        public async Task CanAcceptDeviceStreamRequest()
        {
            // Arrange
            using (var secureShellDevice = new SecureShell(this.deviceClientMock.Object, localhost, localPort))
            {
                // Act
                await secureShellDevice.OpenConnectionAsync(this.clientWebSocket.Object, this.tcpClient.Object, this.cancellationTokenSource);
             }

            // Assert
            this.deviceClientMock.Verify(dc => dc.AcceptDeviceStreamRequestAsync(this.deviceStreamRequest, this.cancellationTokenSource.Token), Times.Once);
        }

        [TestMethod]
        public async Task CanOpenWebSocketToIoTHub()
        {
            // Arrange
            using (var secureShellDevice = new SecureShell(this.deviceClientMock.Object, localhost, localPort))
            {
                // Act
                await secureShellDevice.OpenConnectionAsync(this.clientWebSocket.Object, this.tcpClient.Object, this.cancellationTokenSource);
            }

            // Assert
            this.clientWebSocket.Verify(cws => cws.ConnectAsync(uri, this.cancellationTokenSource.Token), Times.Once);
        }

        [TestMethod]
        public async Task CanReadFromWebSocketToLocalStream()
        {
            // Arrange
            using (var secureShellDevice = new SecureShell(this.deviceClientMock.Object, localhost, localPort))
            {
                // Act
                await secureShellDevice.OpenConnectionAsync(this.clientWebSocket.Object, this.tcpClient.Object, this.cancellationTokenSource);
            }

            // Assert
            this.tcpClient.Verify(tc => tc.GetStream(), Times.Once);
            this.clientWebSocket.Verify(cws => cws.ReceiveAsync(this.buffer, this.cancellationTokenSource.Token), Times.Once);
            this.networkStream.Verify(ns => ns.WriteAsync(this.buffer, 0, streamReturnValue), Times.Once);
        }

        [TestMethod]
        public async Task CanReadFromLocalStreamToWebSocket()
        {
            // Arrange
            using (var secureShellDevice = new SecureShell(this.deviceClientMock.Object, localhost, localPort))
            {
                // Act
                await secureShellDevice.OpenConnectionAsync(this.clientWebSocket.Object, this.tcpClient.Object, this.cancellationTokenSource);
            }

            // Assert
            this.tcpClient.Verify(tc => tc.GetStream(), Times.Once);
            this.networkStream.Verify(ns => ns.ReadAsync(It.IsAny<byte[]>(), 0, bufferSize), Times.Once);
            this.clientWebSocket.Verify(cws => cws.SendAsync(It.IsAny<ArraySegment<byte>>(), WebSocketMessageType.Binary, true, this.cancellationTokenSource.Token), Times.Once);
        }
    }
}