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

        public void AddMessToChat(string message)
        {
            ChatMessages.Add(message);
        }

        public void AddServerMessage(string message)
        {
            ServerMessages.Add(message);
        }

        public void ChangeServerStatus(string status)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Sever connection: ").Append(status);
            ServerStatus = sb.ToString();

        }

    }
}
