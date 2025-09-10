using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        private Rectangle[,] _playerBoardElements = new Rectangle[10,10];

        public MainWindow()
        {
            _vm = new MainWindowViewModel();
            DataContext = _vm;

            _serverConnection = new SeverConnection(_vm);
            _messageHandler = new MessageHandler(_serverConnection.Client, _vm);

            InitializeComponent();

            CreateBoardGrid(PlayerAreaGrid);
            CreateBoardGrid(EnemyAreaGrid);

            GameGrid.Focus();
        }

        private void CreateBoardGrid(Grid playableAreaGrid)
        {
            // creating 11x11 grid
            for (int i = 0; i <= 10; i++)
            {
                playableAreaGrid.ColumnDefinitions.Add(new ColumnDefinition());
                playableAreaGrid.RowDefinitions.Add(new RowDefinition());
                playableAreaGrid.ShowGridLines = false;
                playableAreaGrid.Margin = new Thickness(20);
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
                        MinWidth = 20,
                        MinHeight = 30,
                        TextAlignment = TextAlignment.Center,
                        Padding = new Thickness(6)
                    };

                    Grid.SetRow(boardNamingLeft, j);
                    Grid.SetColumn(boardNamingLeft, 0);
                    playableAreaGrid.Children.Add(boardNamingLeft);

                    // clickable rectangle element with hover-over events 
                    Rectangle field = new Rectangle()
                    { Fill = Brushes.Transparent, Name = $"F{j -1}{i - 1}", Tag = $"{j-1}{i-1}" };
                    field.MouseLeftButtonDown += new MouseButtonEventHandler(BoardMouseLeftButtonDown);
                    field.MouseEnter += new MouseEventHandler(MouseEnterField);
                    field.MouseLeave += new MouseEventHandler(MouseLeaveField);

                    Border fieldBorder = new Border()
                    { BorderBrush = Brushes.Black, BorderThickness = new Thickness(1), Background = Brushes.LightBlue, MinHeight = 30, MinWidth = 30 };
                    fieldBorder.Child = field;

                    Grid.SetColumn(fieldBorder, j );
                    Grid.SetRow(fieldBorder, i );
                    playableAreaGrid.Children.Add(fieldBorder);

                    if (playableAreaGrid.Name == EnemyAreaGrid.Name)
                        return;
                    _playerBoardElements[j - 1, i - 1] = field;
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

            var fields = GetFieldsArray(sender);

            foreach (Rectangle rectangle in fields)
            {
                rectangle.Fill = Brushes.Black;
            }

            //if (sender is Rectangle)
            //{
            //    var element = (Rectangle)sender;
            //    if (element != null && element.Tag != null)
            //    {
            //        _vm.SelectedBox = element.Tag.ToString();
            //        element.Fill = Brushes.Black;
            //    }
            //}
        }

        private void MouseEnterField(object sender, MouseEventArgs e)
        {
            EnterLeaveFieldColorChange(sender, true);
        }

        private void MouseLeaveField(object sender, MouseEventArgs e)
        {
            EnterLeaveFieldColorChange(sender, false);
        }

        private void EnterLeaveFieldColorChange(object sender, bool enter)
        {
            var fields = GetFieldsArray(sender);

            foreach (Rectangle rectangle in fields)
            {
                if (rectangle.Fill == Brushes.Black)
                    continue;
                if (enter)
                    rectangle.Fill = Brushes.Gray;
                else
                    rectangle.Fill = Brushes.Transparent;
            }

            //if (sender is Rectangle)
            //{
            //    var element = sender as Rectangle;
            //    if (element is null)
            //        return;
            //    if (element.Fill == Brushes.Black)
            //        return;

            //    if (enter)
            //        element.Fill = Brushes.Gray;
            //    else
            //        element.Fill = Brushes.Transparent;
            //}
        }

        private Rectangle[] GetFieldsArray(object sender)
        {
            var baseField = (Rectangle)sender;
            
            string? coords = baseField.Tag.ToString();
            if (string.IsNullOrEmpty(coords)) 
                return [];
            int col = (int)char.GetNumericValue(coords[0]);
            int row = (int)char.GetNumericValue(coords[1]);

            if (_vm.ShipLength == 0)
                return [_playerBoardElements[col, row]];

            if (col + _vm.ShipLength > 10)
                col = 10 - _vm.ShipLength;

            Rectangle[] fields = new Rectangle[_vm.ShipLength];
            for (int i = 0; i < _vm.ShipLength; i++)
            {
                fields[i] = _playerBoardElements[col + i, row];
            }
            return fields;
        }

        private void Grid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.R)
            {
                _vm.ChangeShipOrientation();
            }
        }

        private void GameGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            GameGrid.Focus();
        }

        private void ShipRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var radiobutton = (RadioButton)sender;

            switch (radiobutton.Tag.ToString())
            {
                case "Size1":
                    _vm.ShipLength = 1;
                    break;
                case "Size2":
                    _vm.ShipLength = 2;
                    break;
                case "Size3":
                    _vm.ShipLength = 3;
                    break;
                case "Size4":
                    _vm.ShipLength = 4;
                    break;
                default:
                    break;

            }
        }
    }
}