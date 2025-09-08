using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WebSocketClient_WPF.ViewModel;

namespace WebSocketClient_WPF.Websocket
{

    internal class MessageHandler
    {
        private readonly ClientWebSocket _client;
        private readonly MainWindowViewModel _vm;
        public enum MessageType
        {
            Chat,
            Shot,
            ServerInfo
        }

        public MessageHandler(ClientWebSocket client, MainWindowViewModel vm)
        {
            _client = client;
            _vm = vm;
        }

        private async Task<string?> ReceiveMessageAsync()
        {
            var sb = new StringBuilder();
            var buffer = new ArraySegment<byte>(new byte[1024]);

            while (true)
            {
                WebSocketReceiveResult result = await _client.ReceiveAsync(buffer, CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await _client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Connection close by server.", CancellationToken.None);
                    return null;
                }

                if (buffer.Array == null)
                    return null;

                sb.Append(Encoding.UTF8.GetString(buffer.Array, buffer.Offset, result.Count));

                if (result.EndOfMessage)
                {
                    return sb.ToString();
                }
            }
        }

        internal async Task StartMessageListener()
        {
            try
            {
                while (_client.State == WebSocketState.Open)
                {
                    string? message = await ReceiveMessageAsync();
                    if (string.IsNullOrEmpty(message))
                        break;

                    //_vm.AddMessToChat($"Enemy: {message.Trim()}");

                    //// tutaj odpalic funkcje ktora sprawdza jaka wiadomosc przyszla i gdzie ja wyswietlic
                    _vm.ParseMessageToUI(message.Trim());
                }
            }
            catch (WebSocketException ex)
            {
                Console.WriteLine($"Websocket exception: {ex.Message}");
            }

            //// tutaj skończone bo mam pierdolca
        }

        internal async Task SendMessageAsync(UIElement messageOrigin, MessageType messageType)
        {
            // target message string for now - "messageType|messageContents"
            StringBuilder sb = new StringBuilder(messageType.ToString()).Append('|');

            switch (messageType)
            {
                case MessageType.Chat:
                    var messageOriginBox = (TextBox)messageOrigin;
                    sb.Append(messageOriginBox.Text);
                    break;

                default:
                    break;
            }

            if (_client.State == WebSocketState.Open)
            {
                var data = new ArraySegment<byte>(Encoding.UTF8.GetBytes(sb.ToString()));
                await _client.SendAsync(data, WebSocketMessageType.Text, true, CancellationToken.None);
            }

        }

    }
}
