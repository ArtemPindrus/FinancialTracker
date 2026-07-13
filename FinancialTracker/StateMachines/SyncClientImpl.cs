using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace FinancialTracker.StateMachines {
    public partial class SyncClient : ObservableObject {
        private readonly IConfiguration config;

        private TcpClient? tcpClient;

        public IPAddress? RequestedIpAddress { get; private set; }
        public IPAddress? ConnectedIpAddress => (tcpClient?.Client.RemoteEndPoint as IPEndPoint)?.Address;

        public SyncClient(IConfiguration config) {
            this.config = config;
        }

        public void DispatchEventNotify(EventId eventId) {
            DispatchEvent(eventId);
            OnPropertyChanged(nameof(stateId));
        }

        public void Connect(string ipAddress) {
            RequestedIpAddress = IPAddress.Parse(ipAddress);
            DispatchEventNotify(EventId.CONNECTREQUEST);
        }

        async Task OnConnectingEnterAsync() {
            if (RequestedIpAddress is null) {
                DispatchEventNotify(EventId.CONNECTIONFAILED);
                return;
            }

            tcpClient = new();
            await tcpClient.ConnectAsync(RequestedIpAddress, SyncServer.Port);
        }

        async Task OnReceivingEnter() {
            if (tcpClient is null) {
                DispatchEventNotify(EventId.RECEIVINGFAILED);
                return;
            }

            using var stream = tcpClient.GetStream();
            using var reader = new BinaryReader(stream);

            long fileSize = reader.ReadInt64();

            using var fileStream = File.Create(config.GetDatabasePath());
            byte[] buffer = new byte[81920];
            long remaining = fileSize;

            while (remaining > 0) {
                int read = await stream.ReadAsync(buffer, 0, (int)Math.Min(buffer.Length, remaining));
                if (read == 0) break;
                await fileStream.WriteAsync(buffer, 0, read);
                remaining -= read;
            }

            DispatchEventNotify(EventId.RECEIVINGCOMPLETED);
        }

        void OnReceivingExit() {
            tcpClient?.Close();
        }
    }
}
