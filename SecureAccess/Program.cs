namespace Azure.Iot.Edge.Modules.SecureAccess
{
    using Azure.Iot.Edge.Modules.SecureAccess.Device;
    using Azure.Iot.Edge.Modules.SecureAccess.Module;
    using Microsoft.Extensions.DependencyInjection;

    using System;
    using System.Linq;
    using System.Runtime.Loader;
    using System.Threading;
    using System.Threading.Tasks;

    internal class Program
    {
        private static async Task Main()
        {
            // Wait until the app unloads or is cancelled by external triggers, use it for exceptional scnearios only.
            using (var cts = new CancellationTokenSource())
            {
                AssemblyLoadContext.Default.Unloading += (ctx) => cts.Cancel();
                Console.CancelKeyPress += (sender, cpe) => cts.Cancel();

                // Bootstrap modules and virtual devices.
                var services = new ServiceCollection();

                services.AddSingleton<IStreamingDevice>(isvc =>
                    new SecureShell(Environment.GetEnvironmentVariable("sshTargetHost"), GetPortFromEnvironmentVariable("sshTargetPort")));

                services.AddSingleton<IStreamingDevice>(isvc =>
                    new SecureCopy(Environment.GetEnvironmentVariable("scpTargetHost"), GetPortFromEnvironmentVariable("scpTargetPort")));

                services.AddSingleton<IStreamingDevice>(isvc =>
                    new RemoteDesktop(Environment.GetEnvironmentVariable("rdpTargetHost"), GetPortFromEnvironmentVariable("rdpTargetPort")));

                services.AddSingleton<IDeviceHost, PureDeviceHost>(isvc =>
                                     new PureDeviceHost(new ModuleClientWrapper()));

                // Dispose method of ServiceProvider will dispose all disposable objects constructed by it as well.
                using (var serviceProvider = services.BuildServiceProvider())
                {
                    // Get a new module.
                    using (var module = serviceProvider.GetService<IDeviceHost>())
                    {
                        await module.OpenConnectionAsync().ConfigureAwait(false);

                        // Run all tasks in parallel.
                        Task.WaitAny(
                        RunVirtualDevice(cts, serviceProvider, "SecureShell", Environment.GetEnvironmentVariable("sshDeviceConnectionString"), module),
                        RunVirtualDevice(cts, serviceProvider, "SecureCopy", Environment.GetEnvironmentVariable("scpDeviceConnectionString"), module),
                        RunVirtualDevice(cts, serviceProvider, "RemoteDesktop", Environment.GetEnvironmentVariable("rdpDeviceConnectionString"), module));
                    }
                }

                await WhenCancelled(cts.Token).ConfigureAwait(false);
            }
        }

        private static async Task RunVirtualDevice(CancellationTokenSource cts, ServiceProvider serviceProvider, string deviceName, string deviceConnectionString, IDeviceHost module)
        {
            // Keep on looking for the new streams on IoT Hub when the previous one closes or aborts.                        
            while (!cts.IsCancellationRequested)
            {
                try
                {
                    // Run virtual device
                    var device = serviceProvider.GetServices<IStreamingDevice>()
                        .FirstOrDefault(sd => sd.StreamDeviceName.Equals(deviceName, StringComparison.InvariantCulture));

                    using (var deviceClient = new DeviceClientWrapper(deviceConnectionString))
                    {
                        using (var clientWebSocket = new ClientWebSocketWrapper())
                        {
                            using (var tcpClient = new TcpClientWrapper())
                            {
                                Console.WriteLine($"{deviceName} awaiting connection...");
                                await module.OpenDeviceConnectionAsync(device, deviceClient, clientWebSocket, tcpClient, cts)
                                    .ConfigureAwait(false);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }

        private static int GetPortFromEnvironmentVariable(string key)
        {
            if (!int.TryParse(Environment.GetEnvironmentVariable(key), out var port))
            {
                throw new ArgumentOutOfRangeException(key, "Could not convert port number to integer.");
            }
            return port;
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
