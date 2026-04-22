using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace FinancialTracker.StateMachines; 

public partial class ServerStateMachine {
    private const int Port = 8080;

    private readonly IConfiguration config;
    private readonly TcpListener server;

    public ServerStateMachine(IConfiguration config) {
        this.config = config;

        IPEndPoint localEndPoint = new(IPAddress.Any, Port);
        server = new(localEndPoint);
    }

    private void OnStart() {
        server.Start(1);
    }

    private void OnStop() {
        server.Stop(); 
    }

    private void OnNotConnectedEnter() {

    }
}
