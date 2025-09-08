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
using static System.Net.Mime.MediaTypeNames;

namespace WebSocketClient_WPF
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SeverConnection _serverConnection;
        private MessageHandler _messageHandler;

        private MainWindowViewModel _vm;

        public MainWindow()
        {
            InitializeComponent();
            _vm = new MainWindowViewModel();
            DataContext = _vm;

            _serverConnection = new SeverConnection(_vm);
            _messageHandler = new MessageHandler(_serverConnection.Client, _vm);


            CreateBoardGrid(PlayerBoardGrid);
            CreateBoardGrid(EnemyBoardGrid);

        }

        private void CreateBoardGrid(Grid playableAreaGrid)
        {
            // creating 11x11 grid
            for (int i = 0; i <= 10; i++)
            {
                playableAreaGrid.ColumnDefinitions.Add(new ColumnDefinition());
                playableAreaGrid.RowDefinitions.Add(new RowDefinition());
                playableAreaGrid.ShowGridLines = false;
                playableAreaGrid.HorizontalAlignment = HorizontalAlignment.Center;
                playableAreaGrid.VerticalAlignment = VerticalAlignment.Center;
            }

            char boardNaming; 

            // populating grid with UI objects
            for (int i = 1; i <= 10; i++)
            {
                boardNaming = '@';  // @ is one char "before" A
                for (int j = 1; j <= 10; j++)
                {
                    boardNaming++;

                    // textblocks with board naming (A,B,C ...)
                    TextBlock boardNamingTop = new TextBlock()
                    {
                        Text = $"{boardNaming}",
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Stretch,
                        MinHeight = 20,
                        TextAlignment = TextAlignment.Center
                    };
                    Grid.SetRow(boardNamingTop, 0);
                    Grid.SetColumn(boardNamingTop, j);
                    playableAreaGrid.Children.Add(boardNamingTop);

                    // textblock with board naming (1,2,3 ...)
                    TextBlock boardNamingLeft = new TextBlock()
                    {
                        Text = $"{j}",
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Stretch,
                        MinWidth = 20,
                        TextAlignment = TextAlignment.Center
                    };

                    Grid.SetRow(boardNamingLeft, j);
                    Grid.SetColumn(boardNamingLeft, 0);
                    playableAreaGrid.Children.Add(boardNamingLeft);

                    // TODO: change textblock to some other clickable object
                    TextBlock text = new TextBlock()
                    { Text = $"{boardNaming}{i}", HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch, MinHeight = 30, MinWidth = 30, TextAlignment = TextAlignment.Center };

                    text.MouseLeftButtonDown += new MouseButtonEventHandler(BoardMouseLeftButtonDown);
                    Border boardField = new Border()
                    { BorderBrush = Brushes.Black, BorderThickness = new Thickness(1), Background = Brushes.LightBlue, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
                    boardField.Child = text;
                    Grid.SetColumn(boardField, j );
                    Grid.SetRow(boardField, i );
                    playableAreaGrid.Children.Add(boardField);
                }
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //await _serverConnection.Connect();
            //_ = _messageHandler.StartMessageListener();
        }

        private async void Button_Click_SendChatMessage(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(MessageTextBox.Text))
                return;

            await _messageHandler.SendMessageAsync(messageOrigin: MessageTextBox, messageType: MessageHandler.MessageType.Chat);
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

        }
    }
}