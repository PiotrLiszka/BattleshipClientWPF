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

        private PlayerCell[,] _playerCells = new PlayerCell[10,10];

        private int _shipsToPlace = 10;

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
                    { BorderBrush = Brushes.Black, BorderThickness = new Thickness(1), Background = Brushes.LightBlue, MinHeight = 30, MinWidth = 30, Tag = "field" };
                    fieldBorder.Child = field;

                    Grid.SetColumn(fieldBorder, j );
                    Grid.SetRow(fieldBorder, i );
                    playableAreaGrid.Children.Add(fieldBorder);

                    if (playableAreaGrid.Name == EnemyAreaGrid.Name)
                        return;

                    _playerCells[j - 1, i - 1] = new PlayerCell(field, 0);
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

            var cells = GetCellsArray(sender);

            if (cells.Length == 0)
                return;

            if (CheckCollision(cells))
                return;

            foreach (PlayerCell cell in cells)
            {
                cell.Field.Fill = Brushes.Black;
                cell.Placed = 2;
            }

            _vm.DecreaseShipCount();
            if (--_shipsToPlace == 0)
                ReadyButton.IsEnabled = true;
        }


        private void MouseEnterField(object sender, MouseEventArgs e)
        {
            MouseHighlightField(sender, true);
        }

        private void MouseLeaveField(object sender, MouseEventArgs e)
        {
            MouseHighlightField(sender, false);
        }

        private void MouseHighlightField(object sender, bool enter)
        {
            var cells = GetCellsArray(sender);

            if (cells.Length == 0) 
                return;

            bool collision = CheckCollision(cells);

            foreach (PlayerCell cell in cells)
            {
                if (cell.Field.Fill == Brushes.Black)
                    continue;

                if(!enter)
                {
                    cell.Field.Fill = Brushes.Transparent;
                    continue;
                }

                if (collision)
                {
                    cell.Field.Fill = Brushes.Red;
                    continue;
                }

                if (enter)
                    cell.Field.Fill = Brushes.Gray;
            }
        }

        private bool CheckCollision(PlayerCell[] playerCells)
        {
            bool collision = false;

            foreach(PlayerCell cell in playerCells)
            {
                if (cell.Placed !=0)
                    collision = true;
            }

            return collision;
        }

        private PlayerCell[] GetCellsArray(object sender)
        {

            if (sender is not Rectangle)
                return [];

            var baseField = (Rectangle)sender;

            string? coords = baseField.Tag.ToString();
            if (string.IsNullOrEmpty(coords)) 
                return [];
            int col = (int)char.GetNumericValue(coords[0]);
            int row = (int)char.GetNumericValue(coords[1]);

            if (_vm.ShipLength == 0)
                return [];

            if (_vm.ShipLength == 1)
                return [_playerCells[col, row]];

            PlayerCell[] cells = new PlayerCell[_vm.ShipLength];

            if (_vm.ShipOrientation == MainWindowViewModel.ShipOrientations.Horizontal)
            {
                // simple out of bounds check
                if (col + _vm.ShipLength > 10)
                    col = 10 - _vm.ShipLength;

                for (int i = 0; i < _vm.ShipLength; i++)
                {
                    cells[i] = _playerCells[col + i, row];
                }
                return cells;
            }

            // simple out of bounds check
            if (row + _vm.ShipLength > 10)
                row = 10 - _vm.ShipLength;

            for (int i = 0; i < _vm.ShipLength; i++)
            {
                cells[i] = _playerCells[col, row + i];
            }
            return cells;
        }

        private void PlayerAreaGrid_KeyDown(object sender, KeyEventArgs e)
        {
            // rotate ship orientation at the ship placing phase
            if (e.Key == Key.R)
            {
                if (_vm.ShipLength == 0 || _vm.ShipLength == 1)
                    return;

                // check if mouse is over gameboards field, if so try to dynamically rotate ship
                var obj = Mouse.DirectlyOver;
                if (obj == null)
                    return;
                if (obj is not Rectangle)
                    return;

                Rectangle field = (Rectangle)obj;
                if (field.Parent is not Border)
                    return;

                Border border = (Border)field.Parent;
                if (border.Tag.ToString() != "field")
                    return;

                MouseHighlightField(field, false);
                _vm.ChangeShipOrientation();
                MouseHighlightField(field, true);

            }
        }

        private void PlayerAreaGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            PlayerAreaGrid.Focus();
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
            _vm.checkedShipSizeRadioButton = radiobutton;
        }


        private void Button_Click_PlayerReady(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_ResetShips(object sender, RoutedEventArgs e)
        {
            Size1Radio.IsEnabled = true;
            Size2Radio.IsEnabled = true;
            Size3Radio.IsEnabled = true;
            Size4Radio.IsEnabled = true;
            ReadyButton.IsEnabled = false;
            _vm.ShipSize1Count = 4;
            _vm.ShipSize2Count = 3;
            _vm.ShipSize3Count = 2;
            _vm.ShipSize4Count = 1;
            _shipsToPlace = 10;

            foreach(PlayerCell cell in _playerCells)
            {
                cell.Field.Fill = Brushes.Transparent;
                cell.Placed = 0;
            }
        }
    }
}