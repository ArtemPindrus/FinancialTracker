using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace FinancialTracker.StateMachines {
    public partial class SyncServer : ObservableObject, IDisposable {
        public const int Port = 8080;

        private readonly IConfiguration config;

        TcpListener? tcpListener;
        TcpClient? tcpClient;

        CancellationTokenSource acceptCts = new();

        public string? ClientIp => tcpClient?.Client?.RemoteEndPoint?.ToString();

        public SyncServer(IConfiguration config) {
            this.config = config;
        }

        public void DispatchEventNotify(EventId eventId) {
            DispatchEvent(eventId);
            OnPropertyChanged(nameof(stateId));
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

        public void TryConnecting() {
            DispatchEventNotify(EventId.CONNECTREQUEST);
        }

        public void CancelConnection() {
            DispatchEventNotify(EventId.CONNECTIONCANCELED);
        }

        public void Disconnect() {
            DispatchEventNotify(EventId.DISCONNECTED);
        }

        public void Send() {
            DispatchEventNotify(EventId.SENDREQUEST);
        }

        void CancelTryConnect() {
            acceptCts.Cancel();
            acceptCts = new CancellationTokenSource();
        }

        private void OnConnectedExit() {
            tcpClient?.Close();
        }

        private async Task OnConnectingEnter() {
            if (tcpListener is null) {
                DispatchEventNotify(EventId.CONNECTIONFAILED);
                return;
            }

            try {
                tcpClient = await tcpListener.AcceptTcpClientAsync(acceptCts.Token);
                DispatchEventNotify(EventId.CONNECTIONSUCCEEDED);
            } catch {
                DispatchEventNotify(EventId.CONNECTIONFAILED);
            }
        }

        private void OnConnectingExit() {
            CancelTryConnect();
        }

        private async Task OnSendingEnter() {
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
                DispatchEventNotify(EventId.DISCONNECTED);

                return;
            }

            DispatchEventNotify(EventId.SENDINGCOMPLETED);
        }
    }
}
