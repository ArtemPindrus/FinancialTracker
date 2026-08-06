using CommunityToolkit.Mvvm.ComponentModel;
using FinancialTracker.DataAccessLayer.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace FinancialTracker.StateMachines {
    public partial class SyncServer : BaseStateMachine<SyncServer.EventId>, IDisposable {
        public const int Port = 8080;
        private const int SendingTimeout = 10000;
        private readonly string databasePath;

        TcpListener? tcpListener;
        TcpClient? tcpClient;

        CancellationTokenSource acceptCts = new();

        public string? ClientIp => tcpClient?.Client?.RemoteEndPoint?.ToString();

        public SyncServer(IDatabasePathProvider databasePathProvider) {
            databasePath = databasePathProvider.GetDatabasePath();
        }

        protected override void DispatchEventImpl(EventId eventId) => DispatchEvent(eventId);

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

            long fileSize = new FileInfo(databasePath).Length;
            writer.Write(fileSize);
            writer.Flush();

            try {
                CancellationTokenSource cts = new(SendingTimeout);

                using var fileStream = File.OpenRead(databasePath);
                await fileStream.CopyToAsync(stream, cts.Token);
            } catch {
                DispatchEventNotify(EventId.DISCONNECTED);

                return;
            }

            DispatchEventNotify(EventId.SENDINGCOMPLETED);
        }
    }
}
