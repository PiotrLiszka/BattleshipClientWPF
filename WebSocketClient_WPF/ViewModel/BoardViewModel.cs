using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WebSocketClient_WPF.ViewModel
{
    partial class MainWindowViewModel : INotifyPropertyChanged
    {
        private ShipOrientations _shipOrientation = ShipOrientations.Horizontal;
        private int _shipLength = 0;
        private int _shipSize4Count = 1;
        private int _shipSize3Count = 2;
        private int _shipSize2Count = 3;
        private int _shipSize1Count = 4;

        internal RadioButton? checkedShipSizeRadioButton;

        internal enum ShipOrientations
        {
            Horizontal,
            Vertical
        }
        public ShipOrientations ShipOrientation
        {
            get
            {
                return this._shipOrientation;
            }
            set
            {
                this._shipOrientation = value;
                NotifyPropertyChanged();
            }
        }

        public int ShipLength
        {
            get
            {
                return this._shipLength;
            }
            set
            {
                this._shipLength = value;
                NotifyPropertyChanged();
            }
        }
        public void ChangeShipOrientation()
        {
            if (ShipOrientation == ShipOrientations.Horizontal)
            {
                ShipOrientation = ShipOrientations.Vertical;
                return;
            }
            ShipOrientation = ShipOrientations.Horizontal;
        }

        public int ShipSize4Count
        {
            get
            {
                return this._shipSize4Count;
            }
            set
            {
                this._shipSize4Count = value;
                NotifyPropertyChanged();
            }
        }
        public int ShipSize3Count
        {
            get
            {
                return this._shipSize3Count;
            }
            set
            {
                this._shipSize3Count = value;
                NotifyPropertyChanged();
            }
        }
        public int ShipSize2Count
        {
            get
            {
                return this._shipSize2Count;
            }
            set
            {
                this._shipSize2Count = value;
                NotifyPropertyChanged();
            }
        }
        public int ShipSize1Count
        {
            get
            {
                return this._shipSize1Count;
            }
            set
            {
                this._shipSize1Count = value;
                NotifyPropertyChanged();
            }
        }
        public void DecreaseShipCount()
        {
            if (checkedShipSizeRadioButton is null)
                return;

            switch (ShipLength)
            {
                case 1:
                    if (--ShipSize1Count == 0)
                        LockRadioButton(checkedShipSizeRadioButton);
                    break;
                case 2:
                    if(--ShipSize2Count == 0)
                        LockRadioButton(checkedShipSizeRadioButton);
                    break;
                case 3:
                    if (--ShipSize3Count == 0)
                        LockRadioButton(checkedShipSizeRadioButton);
                    break;
                case 4:
                    if (--ShipSize4Count == 0)
                        LockRadioButton(checkedShipSizeRadioButton);
                    break;
                default:
                    break;
            }
        }

        private void LockRadioButton(RadioButton button)
        {
            button.IsChecked = false;
            button.IsEnabled = false;
            ShipLength = 0;
            checkedShipSizeRadioButton = null;
        }

    }
}
