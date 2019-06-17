namespace Azure.Iot.Edge.Modules.SecureAccess
{
    using Azure.Iot.Edge.Modules.SecureAccess.Device;
    using Azure.Iot.Edge.Modules.SecureAccess.Module;
    using Microsoft.Extensions.DependencyInjection;

    using System;
    using System.Runtime.Loader;
    using System.Threading;
    using System.Threading.Tasks;

    internal class Program
    {
        private static async Task Main()
        {
            // Wait until the app unloads or is cancelled by external triggers, use it for exception scnearios only.
            using (var cts = new CancellationTokenSource())
            {
                AssemblyLoadContext.Default.Unloading += (ctx) => cts.Cancel();
                Console.CancelKeyPress += (sender, cpe) => cts.Cancel();

                // Devices should be injected in module and shutdown and have a sliding window on allocated time basis using interfaces.
                // Module could run those devices in isolation using AssemblyLoadContext for higher security.

                // Bootstrap devices and services.
                string targetPortKey = "targetPort";

                if (!int.TryParse(Environment.GetEnvironmentVariable(targetPortKey), out var targetPort))
                {
                    throw new ArgumentOutOfRangeException(targetPortKey, "Could not convert port number to integer.");
                }

                // Bootstrap services.
                var services = new ServiceCollection();

                services.AddTransient<IDeviceHost, DeviceHost>(isvc => new PassThroughDeviceHost(new SecureShell(
                                                                new IoTHubDeviceClient(Environment.GetEnvironmentVariable("deviceConnectionString")),
                                                                new IoTHubClientWebSocket(),
                                                                new LocalTCPClient(),
                                                                Environment.GetEnvironmentVariable("targetHost"),
                                                                targetPort)));

                // Dispose method of ServiceProvider will dispose all objects constructed by it as well.
                using (var serviceProvider = services.BuildServiceProvider())
                {
                    // Keep on looking for the new streams on IoT Hub when the previous one closes or aborts.
                    // This needs to be pulled up the stack as well.
                    var module = serviceProvider.GetService<IDeviceHost>();

                    while (!cts.Token.IsCancellationRequested)
                    {
                        // Run module
                        await module.OpenConnectionAsync(cts).ConfigureAwait(false);
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