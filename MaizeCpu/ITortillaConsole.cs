using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Tortilla;

namespace Tortilla {
    public interface ITortillaConsole {
        void Connect(IMotherboard<UInt64> _motherboard);
        void Show();
        void Close();
        void Clear();
    }
}
