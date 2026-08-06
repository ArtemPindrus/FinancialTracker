using FinancialTracker.DataAccessLayer.Services;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace FinancialTracker.StateMachines {
    public partial class SyncServer : BaseStateMachine<SyncServer.EventId>, IDisposable {
        public const int Port = 8080;
        private readonly string databasePath;

        TcpListener tcpListener;
        TcpClient? tcpClient;

        CancellationTokenSource? sendCts;

        public string? ClientIp => tcpClient?.Client?.RemoteEndPoint?.ToString();

        public SyncServer(IDatabasePathProvider databasePathProvider) {
            databasePath = databasePathProvider.GetDatabasePath();

            tcpListener = new(IPAddress.Any, Port);
        }

        protected override void DispatchEventImpl(EventId eventId) => DispatchEvent(eventId);

        public void Dispose() {
            tcpListener?.Dispose();
            GetRidOfClient();

            GC.SuppressFinalize(this);
        }

        void GetRidOfClient() {
            tcpClient?.Dispose();
            tcpClient = null;
        }

        void OnOpenEnter() {
            tcpListener.Start(1);
        }

        void OnOpenExit() {
            tcpListener.Stop();

            GetRidOfClient();
        }

        async void OnOpenIdleEnter() {
            tcpClient = await tcpListener.AcceptTcpClientAsync();

            DispatchEventNotify(EventId.GOTCONNECTION);
        }

        void OnConnectionRejected() {
            GetRidOfClient();
        }

        void OnConnectedExit() {
            GetRidOfClient();
        }

        async void OnSendingEnter() {
            if (tcpClient is null) {
                throw new Exception("Client isn't connected!");
            }

            sendCts = new();

            using var stream = tcpClient.GetStream();
            using var writer = new BinaryWriter(stream);

            long fileSize = new FileInfo(databasePath).Length;
            writer.Write(fileSize);
            writer.Flush();

            try {
                using var fileStream = File.OpenRead(databasePath);
                await fileStream.CopyToAsync(stream, sendCts.Token);
            } catch {
                // noop
            } finally {
                DispatchEventNotify(EventId.DISCONNECTED);
            }
        }

        void OnSendingExit() {
            sendCts?.Cancel();
        }
    }
}
