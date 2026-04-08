using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace FinancialTracker.ViewModels {
    public partial class UploadViewModel : ViewModelBase {
        private readonly IConfiguration config;

        private CancellationTokenSource? serverCts;

        public string WifiIpAddress {
            get {
                foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces()) {
                    if (nic.NetworkInterfaceType == NetworkInterfaceType.Wireless80211
                        && nic.OperationalStatus == OperationalStatus.Up
                        && !nic.Description.Contains("Virtual", StringComparison.OrdinalIgnoreCase)) {
                        foreach (UnicastIPAddressInformation ip in nic.GetIPProperties().UnicastAddresses) {
                            if (ip.Address.AddressFamily == AddressFamily.InterNetwork) {
                                return ip.Address.ToString();
                            }
                        }
                    }
                }

                return "Failed to find IP address of Wifi Adapter.";
            }
        }

        public UploadViewModel(IConfiguration config) {
            this.config = config;
        }

        [RelayCommand]
        private async Task StartServerAsync() {
            IPEndPoint localEndPoint = new(IPAddress.Any, 8080);
            TcpListener server = new(localEndPoint);

            server.Start(1);

            serverCts = new CancellationTokenSource();

            var client = await server.AcceptTcpClientAsync(serverCts.Token);

            using var stream = client.GetStream();
            using var writer = new BinaryWriter(stream);

            string dbPath = config.GetDatabasePath();
            long fileSize = new FileInfo(dbPath).Length;
            writer.Write(fileSize);
            writer.Flush();

            using var fileStream = File.OpenRead(dbPath);
            await fileStream.CopyToAsync(stream);
        }
    }
}
