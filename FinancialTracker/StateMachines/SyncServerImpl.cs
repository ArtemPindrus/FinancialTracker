using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace FinancialTracker.StateMachines {
    public partial class SyncServer : IDisposable {
        public const int Port = 8080;

        TcpListener? tcpListener;
        TcpClient? tcpClient;

        CancellationTokenSource acceptCts = new();

        public string? ClientIp => tcpClient?.Client?.RemoteEndPoint?.ToString();

        public void Init() {
            tcpListener = new TcpListener(IPAddress.Any, Port);
            tcpListener.Start(1);
        }

        public void Dispose() {
            tcpListener?.Dispose();
            tcpClient?.Dispose();

            GC.SuppressFinalize(this);
        }

        private async Task TryConnectAsync() {
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

        private void OnConnectingExit() {
            CancelTryConnect();
        }
    }
}
