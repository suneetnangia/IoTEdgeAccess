namespace Azure.Iot.Edge.Modules.SecureAccess
{
    using Azure.Iot.Edge.Modules.SecureAccess.Device;
    using Azure.Iot.Edge.Modules.SecureAccess.Module;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Net.WebSockets;
    using System.Runtime.Loader;
    using System.Threading;
    using System.Threading.Tasks;

    internal class Program
    {
        private const string targetPortKey = "targetPort";

        private static async Task Main()
        {
            // Wait until the app unloads or is cancelled by external triggers, use it for exceptional scnearios only.
            using (var cts = new CancellationTokenSource())
            {
                AssemblyLoadContext.Default.Unloading += (ctx) => cts.Cancel();
                Console.CancelKeyPress += (sender, cpe) => cts.Cancel();

                if (!int.TryParse(Environment.GetEnvironmentVariable(targetPortKey), out var targetPort))
                {
                    throw new ArgumentOutOfRangeException(targetPortKey,
                                                          "Could not convert port number to integer.");
                }

                // Bootstrap modules and virtual devices.
                var services = new ServiceCollection();

                // print all env vars
                foreach (DictionaryEntry envvar in Environment.GetEnvironmentVariables())
                {
                    Console.WriteLine($"{envvar.Key}:{envvar.Value}");
                }

                if(Environment.GetEnvironmentVariable("EdgeHubConnectionString") is null)
                {
                    Console.WriteLine("EdgeHubConnectionString is NULL");
                }

                if (Environment.GetEnvironmentVariable("IotHubConnectionString") is null)
                {
                    Console.WriteLine("IotHubConnectionString is NULL");
                }

                var hubConnectionString = Environment.GetEnvironmentVariable("EdgeHubConnectionString") ?? Environment.GetEnvironmentVariable("IotHubConnectionString");

                services.AddTransient<IDeviceHost, PureDeviceHost>(isvc =>
                                    new PureDeviceHost(new ModuleClientWrapper(hubConnectionString),
                                    new SecureShell(new DeviceClientWrapper(Environment.GetEnvironmentVariable("deviceConnectionString")),
                                    Environment.GetEnvironmentVariable("targetHost"), targetPort)));

                // Dispose method of ServiceProvider will dispose all disposable objects constructed by it as well.
                using (var serviceProvider = services.BuildServiceProvider())
                {
                    // Get a new module.
                    using (var module = serviceProvider.GetService<IDeviceHost>())
                    {
                        // Keep on looking for the new streams on IoT Hub when the previous one closes or aborts.                        
                        while (!cts.IsCancellationRequested)
                        {
                            try
                            {
                                using (var webSocket = new ClientWebSocketWrapper())
                                {
                                    using (var tcpClient = new TcpClientWrapper())
                                    {
                                        // Run module
                                        Console.WriteLine("Initiating open connection...");
                                        await module.OpenConnectionAsync(webSocket, tcpClient, cts).ConfigureAwait(false);
                                    }

                                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, String.Empty, cts.Token).ConfigureAwait(false);
                                    Console.WriteLine($"Device stream closed to remote websocket endpoint, at {DateTime.UtcNow}");
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"{ex.Message}");
                            }
                        }
                    }
                }

                await WhenCancelled(cts.Token).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Handles cleanup operations when app is cancelled or unloads
        /// </summary>
        public static Task WhenCancelled(CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();
            cancellationToken.Register(s => ((TaskCompletionSource<bool>)s).SetResult(true), tcs);
            return tcs.Task;
        }
    }
}
