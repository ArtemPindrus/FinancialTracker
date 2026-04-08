using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HarfBuzzSharp;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace FinancialTracker.ViewModels {
    public partial class DownloadViewModel : ViewModelBase {
        private readonly IConfiguration config;

        [ObservableProperty]
        private string? statusMessage;

        public string? IpAddress { get; set; }

        public DownloadViewModel(IConfiguration config) {
            this.config = config;
        }

        [RelayCommand]
        private async Task DownloadAsync() {
            StatusMessage = "";

            if (!IPAddress.TryParse(IpAddress, out IPAddress? ip)) {
                StatusMessage = "Invalid IP address.";
                return;
            }
            IPEndPoint remoteEndPoint = new(ip, 8080);

            TcpClient client = new();
            await client.ConnectAsync(remoteEndPoint);

            using var stream = client.GetStream();
            using var reader = new BinaryReader(stream);

            long fileSize = reader.ReadInt64();
            Log($"Database size: {fileSize}");

            using var fileStream = File.Create(config.GetDatabasePath());
            byte[] buffer = new byte[81920];
            long remaining = fileSize;

            while (remaining > 0) {
                int read = await stream.ReadAsync(buffer, 0, (int)Math.Min(buffer.Length, remaining));
                if (read == 0) break;
                await fileStream.WriteAsync(buffer, 0, read);
                remaining -= read;
            }
        }

        private void Log(string message) {
            StatusMessage += message + Environment.NewLine;
        }
    }
}
