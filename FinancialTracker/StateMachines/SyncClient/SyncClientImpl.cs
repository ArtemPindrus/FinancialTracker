using CommunityToolkit.Mvvm.ComponentModel;
using FinancialTracker.DataAccessLayer.Services;
using FinancialTracker.Services;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace FinancialTracker.StateMachines {
    public partial class SyncClient : BaseStateMachine<SyncClient.EventId>, IDisposable {
        private readonly string databasePath;
        private readonly IErrorNotifier notifier;

        private CancellationTokenSource? connectionCts;
        private CancellationTokenSource? receivingCts;

        private TcpClient? tcpClient;
        private long fileSize = -1;

        [ObservableProperty]
        public partial float ReceivingProgress { get; private set; }

        public IPAddress? RequestedIpAddress { get; private set; }
        public IPAddress? ConnectedIpAddress => (tcpClient?.Client.RemoteEndPoint as IPEndPoint)?.Address;

        public SyncClient(IDatabasePathProvider databasePathProvider, IErrorNotifier notifier) {
            databasePath = databasePathProvider.GetDatabasePath();
            this.notifier = notifier;
        }

        public void Connect(IPAddress ipAddress) {
            RequestedIpAddress = ipAddress;
            DispatchEventNotify(EventId.CONNECTREQUEST);
        }

        protected override void DispatchEventImpl(EventId eventId) => DispatchEvent(eventId);

        async void OnConnectingEnter() {
            if (RequestedIpAddress is null) throw new Exception("No IpAddress was requested.");

            connectionCts = new();
            tcpClient = new();

            try {
                await tcpClient.ConnectAsync(RequestedIpAddress, SyncServer.Port, connectionCts.Token);
            } catch (OperationCanceledException) {
                notifier.Info("Connection cancelled.");

                DispatchEventNotify(EventId.CONNECTIONCANCELED);

                return;
            } catch {
                notifier.Error("Connection failed.");

                DispatchEventNotify(EventId.CONNECTIONFAILED);

                return;
            }

            DispatchEventNotify(EventId.CONNECTIONSUCCESS);
        }

        void OnConnectingExit() {
            connectionCts?.Cancel();
        }

        async void OnConnectedEnter() {
            if (tcpClient is null
                || !tcpClient.Connected) throw new Exception("TcpClient is expected to be connected.");

            var stream = tcpClient.GetStream();
            var reader = new BinaryReader(stream);

            try {
                await Task.Run(() => {
                    fileSize = reader.ReadInt64();
                });
            } catch {
                notifier.Error("Failed to start receiving database. Server might have rejected connection.");

                DispatchEventNotify(EventId.DISCONNECTED);

                return;
            }

            DispatchEventNotify(EventId.RECEIVE);
        }

        void OnConnectedExit() {
            tcpClient?.Close();
        }

        async void OnReceivingEnter() {
            if (tcpClient is null
                || !tcpClient.Connected) throw new Exception("TcpClient is expected to be connected.");

            receivingCts = new();
            var stream = tcpClient.GetStream();

            // TODO: write to a temporary file and then replace the original file to avoid data loss in case of an error
            using var fileStream = File.Create(databasePath);
            byte[] buffer = new byte[81920];
            long remaining = fileSize;

            while (remaining > 0) {
                long sent = fileSize - remaining;
                ReceivingProgress = (float)sent / fileSize;

                try {
                    int read = await stream.ReadAsync(buffer, 0, (int)Math.Min(buffer.Length, remaining), receivingCts.Token);
                    if (read == 0) break;

                    await fileStream.WriteAsync(buffer, 0, read, receivingCts.Token);
                    remaining -= read;
                } catch (OperationCanceledException) {
                    notifier.Info("Receiving canceled.");

                    DispatchEventNotify(EventId.DISCONNECTED);

                    return;
                } catch {
                    notifier.Error("Receiving failed.");

                    DispatchEventNotify(EventId.DISCONNECTED);

                    return;
                }
            }

            notifier.Info("Database sent. Disconnecting.");
            DispatchEventNotify(EventId.DISCONNECTED);
        }

        void OnReceivingExit() {
            receivingCts?.Cancel();
        }

        public void Dispose() {
            tcpClient?.Dispose();
        }
    }
}
