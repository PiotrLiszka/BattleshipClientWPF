using System;
using System.Collections.Generic;
using System.Windows.Shapes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSocketClient_WPF.ViewModel
{
    internal class PlayerCell
    {
        private readonly Rectangle _field;
        private byte _placed;

        public PlayerCell(Rectangle field, byte placed)
        {
            _field = field;
            _placed = placed;
        }

        public Rectangle Field
        {
            get => _field; 
        }
        public byte Placed
        {
            get => _placed;
            set => _placed = value;
        }

    }
}
