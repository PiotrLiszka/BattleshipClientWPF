using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using WebSocketClient_WPF.ViewModel;

namespace WebSocketClient_WPF.Websocket
{
    internal class SeverConnection
    {

        private readonly ClientWebSocket _client;
        private readonly MainWindowViewModel _vm;

        public ClientWebSocket Client => _client;

        public SeverConnection(MainWindowViewModel vm)
        {
            _client = new ClientWebSocket();
            _client.Options.KeepAliveInterval = TimeSpan.FromSeconds(30);

            _vm = vm;
        }

        internal async Task Connect()
        {
            await _client.ConnectAsync(new Uri("ws://localhost:5000"), CancellationToken.None);
            _ = Task.Run(CheckConnectionLoop);
            Console.WriteLine("Connected to server.");
        }

        private async Task CheckConnectionLoop()
        {
            while (true)
            {
                await Task.Delay(1000);
                _vm.ChangeServerStatus(_client.State.ToString());
            }
        }

    }
}
