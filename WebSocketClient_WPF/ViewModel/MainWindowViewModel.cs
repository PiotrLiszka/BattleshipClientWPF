using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WebSocketClient_WPF.Websocket;

namespace WebSocketClient_WPF.ViewModel
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        private string _serverStatus = "Server Connection";
        private string _selectedBox = "CLICK";

        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyname = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
        }

        public ObservableCollection<string> ChatMessages { get; } = new ObservableCollection<string>();
        public ObservableCollection <string> ServerMessages { get; } = new ObservableCollection<string> ();
        public string ServerStatus
        {
            get
            {
                return this._serverStatus;
            }
            set
            {
                if (value != this._serverStatus)
                {
                    this._serverStatus = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string SelectedBox
        {
            get
            {
                return this._selectedBox;
            }
            set
            {
                if (value != this._selectedBox)
                {
                    this._selectedBox = value;
                    NotifyPropertyChanged();
                }
            }
        }

        internal void ParseMessageToUI(string message)
        {
            string[] result;
            char messageSeparator = '|';
            result = message.Split(messageSeparator, StringSplitOptions.TrimEntries);

            if (result.Length == 0)
                return;

            if (result[0].Equals(MessageHandler.MessageType.Chat.ToString()))
            {
                ChatMessages.Add($"Enemy: {result[1]}");
                return;
            }
            if (result[0].Equals(MessageHandler.MessageType.ServerInfo.ToString()))
            {
                ServerMessages.Add($"Server Message: {result[1]}");
                return;
            }
        }

        public void AddMessToChat(string message)
        {
            ChatMessages.Add(message);
        }

        public void ChangeServerStatus(string status)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Sever connection: ").Append(status);
            ServerStatus = sb.ToString();

        }

    }
}
