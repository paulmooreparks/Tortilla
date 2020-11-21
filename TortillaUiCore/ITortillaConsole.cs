using System;
using System.Collections.Generic;
using System.Text;
using Tortilla;

namespace TortillaUI {
    interface ITortillaConsole {
        void Connect(IMotherboard<UInt64> _motherboard);
        void Show();
        void Close();
        void Clear();
    }
}
