using System;
using Tortilla;

/* One thing that you'll notice upon reading the code, especially if you have any familiarity with C# and 
with .NET in general, is that I don't use a lot of properties, but rather fields. I did this for speed and 
simplicity. Inside a CPU, and in assembly code in general, there's not a lot of information hiding, though 
there is encapsulation to some degree. My primary concern in this code is not to write enterprise-grade 
interfaces, but rather to get raw speed wherever I can. I'm sacrificing some sacred OO principles in pursuit 
of performance, and inside the ugly innards of the CPU implementation, it's probably worth it. */

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
