using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace FinancialTracker.StateMachines {
    public partial class SyncServer : IDisposable {
        public const int Port = 8080;

        private readonly IConfiguration config;

        TcpListener? tcpListener;
        TcpClient? tcpClient;

        CancellationTokenSource acceptCts = new();

        public string? ClientIp => tcpClient?.Client?.RemoteEndPoint?.ToString();

        public SyncServer(IConfiguration config) {
            this.config = config;
        }

        public void StartServer() {
            tcpListener = new TcpListener(IPAddress.Any, Port);
            tcpListener.Start(1);
        }

        public void Dispose() {
            tcpListener?.Dispose();
            tcpClient?.Dispose();

            GC.SuppressFinalize(this);
        }

        private async Task SendDatabase() {
            if (tcpClient is null) {
                throw new Exception("Client isn't connected!");
            }

            using var stream = tcpClient.GetStream();
            using var writer = new BinaryWriter(stream);

            string dbPath = config.GetDatabasePath();
            long fileSize = new FileInfo(dbPath).Length;
            writer.Write(fileSize);
            writer.Flush();

            try {
                CancellationTokenSource cts = new(10000);

                using var fileStream = File.OpenRead(dbPath);
                await fileStream.CopyToAsync(stream, cts.Token);
            } catch {
                DispatchEvent(EventId.DISCONNECT);

                return;
            }

            DispatchEvent(EventId.FINISHEDSENDING);
        }

        private async Task TryConnectAsync() {
            if (tcpListener is null) return;

            try {
                tcpClient = await tcpListener.AcceptTcpClientAsync(acceptCts.Token);
                DispatchEvent(EventId.CONNECTED);
            } catch {
                DispatchEvent(EventId.CONNECTIONFAILED);
            }
        }

        void CancelTryConnect() {
            acceptCts.Cancel();
            acceptCts = new CancellationTokenSource();
        }

        private void OnConnectedExit() {
            tcpClient?.Dispose();
        }

        private void OnConnectingExit() {
            CancelTryConnect();
        }
    }
}
