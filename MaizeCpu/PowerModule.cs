using System;
using Tortilla;

namespace Maize {
    public class PowerModule : Register {
        public PowerModule(IMotherboard<UInt64> motherboard) {
            MB = motherboard;
            AddressBus = MB.AddressBus;
            IOBus = MB.IOBus;
            MB.ConnectDevice(this, 0x0001);
        }

        IMotherboard<UInt64> MB;

        public override void OnTickSetFromIOBus(IBusComponent cpuFlags) {
            switch (RegData.W0) {
            case 0x0001:
                MB.PowerOff();
                break;
            }
        }
    }

}
