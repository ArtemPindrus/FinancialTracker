using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace FinancialTracker {
    public partial class SyncServer : ObservableObject, IDisposable {
        public const int Port = 8080;

        private readonly IConfiguration config;

        private CancellationTokenSource? serverCts;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsRunning))]
        private TcpListener? server;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsConnected))]
        [NotifyPropertyChangedFor(nameof(ClientIp))]
        private TcpClient? client;

        public bool IsRunning => Server != null;

        public bool IsConnected => Client != null;

        public string? ClientIp => Client?.Client?.RemoteEndPoint?.ToString();
        
        public SyncServer(IConfiguration config) {
            this.config = config;
        }

        public async Task StartServerAsync() {
            if (IsRunning) throw new Exception("Server is already running.");

            IPEndPoint localEndPoint = new(IPAddress.Any, Port);
            Server = new(localEndPoint);

            Server.Start(1);

            serverCts = new CancellationTokenSource();
        }

        public async Task ConnectClient() {
            if (Server is null || serverCts is null) throw new Exception("Server isn't running.");

            try {
                Client = await Server.AcceptTcpClientAsync(serverCts.Token);
            } catch (OperationCanceledException) {
                return;
            }
        }

        public async Task StopServerAsync() {
            if (serverCts is null) return;

            await serverCts.CancelAsync();
            Server?.Dispose();
            Client?.Dispose();

            Server = null;
            Client = null;
        }

        public async Task SendDatabase() {
            if (Client is null) {
                throw new Exception("Client isn't connected!");
            }

            using var stream = Client.GetStream();
            using var writer = new BinaryWriter(stream);

            string dbPath = config.GetDatabasePath();
            long fileSize = new FileInfo(dbPath).Length;
            writer.Write(fileSize);
            writer.Flush();

            using var fileStream = File.OpenRead(dbPath);
            await fileStream.CopyToAsync(stream);
        }

        public void Dispose() {
            Server?.Dispose();
            Client?.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
