using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WebSocketClient_WPF.ViewModel;
using WebSocketClient_WPF.Websocket;

namespace WebSocketClient_WPF
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SeverConnection _serverConnection;

        private MainWindowViewModel _vm;

        public MainWindow()
        {
            InitializeComponent();
            _vm = new MainWindowViewModel();
            DataContext = _vm;

            _serverConnection = new SeverConnection(_vm);

            CreateBoardGrid(PlayerBoardGrid);
            CreateBoardGrid(EnemyBoardGrid);


            /// TEST MESSAGES
            for (int i = 1; i <= 10; i++)
            {
                _vm.AddMessToChat($"Test message {i}");
            }

            for (int i = 1; i <= 10; i++)
            {
                _vm.AddServerMessage($"Test message {i}");
            }

        }

        private void CreateBoardGrid(Grid grid)
        {
            for (int i = 1; i <= 10; i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.ShowGridLines = true;
                grid.HorizontalAlignment = HorizontalAlignment.Center;
                grid.VerticalAlignment = VerticalAlignment.Center;

                for (int j = 1; j <= 10; j++)
                {
                    TextBlock text = new TextBlock()
                    { Text = $"{j}{i}", HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch, MinHeight = 30, MinWidth = 30, TextAlignment = TextAlignment.Center };

                    text.MouseLeftButtonDown += new MouseButtonEventHandler(BoardMouseLeftButtonDown);
                    Border boardField = new Border()
                    { BorderBrush = Brushes.Black, BorderThickness = new Thickness(1), Background = Brushes.LightBlue, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
                    boardField.Child = text;
                    Grid.SetColumn(boardField, j - 1);
                    Grid.SetRow(boardField, i - 1);
                    grid.Children.Add(boardField);
                }
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //await _serverConnection.Connect();
            //_ = _serverConnection.StartMessageListener();
        }

        private async void Button_Click_SendChatMessage(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(MessageTextBox.Text))
                return;

            await _serverConnection.SendMessageAsync(MessageTextBox, SeverConnection.MessageType.Chat);
            _vm.AddMessToChat($"You: {MessageTextBox.Text}");
            ChatScroll.ScrollToEnd();
            MessageTextBox.Clear();
        }


        private void Button_Click_PlayerShot(object sender, RoutedEventArgs e)
        {
            _vm.SelectedBox = "CLICK";
        }

        

        private void BoardMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender == null)
                return;
            if (sender is TextBlock)
                _vm.SelectedBox = ((TextBlock)sender).Text;
            if (sender is Border)
                _vm.SelectedBox = ((Border)sender).Name;

        }
    }
}