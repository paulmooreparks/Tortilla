using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

/* One thing that you'll notice upon reading the code, especially if you have any familiarity with C# and 
with .NET in general, is that I don't use a lot of properties, but rather fields. I did this for speed and 
simplicity. Inside a CPU, and in assembly code in general, there's not a lot of information hiding, though 
there is encapsulation to some degree. My primary concern in this code is not to write enterprise-grade 
interfaces, but rather to get raw speed wherever I can. I'm sacrificing some sacred OO principles in pursuit 
of performance, and inside the ugly innards of the CPU implementation, it's probably worth it. */

namespace Tortilla {
    public enum ClockState {
        Dynamic =                   0b_0000_0000_0000,
        TickExecute =               0b_0000_0000_0001,
        TickEnableToAddressBus =    0b_0000_0000_0010,
        TickEnableToDataBus =       0b_0000_0000_0100,
        TickEnableToIOBus =         0b_0000_0000_1000,
        TickSetFromAddressBus =     0b_0000_0001_0000,
        TickSetFromDataBus =        0b_0000_0010_0000,
        TickSetFromIOBus =          0b_0000_0100_1000,
        TickStore =                 0b_0000_1000_0000,
        TickLoad =                  0b_0001_0000_0000,
    }

    public enum Register {
        A = 0x00,
        B = 0x01,
        C = 0x02,
        D = 0x03,
        E = 0x04,
        G = 0x05,
        H = 0x06,
        J = 0x07,
        K = 0x08,
        L = 0x09,
        M = 0x0A,
        Z = 0x0B,
        F = 0x0C,
        I = 0x0D,
        P = 0x0E,
        S = 0x0F
    }

    public enum SubRegister {
        B0 = 0x00,
        B1 = 0x01,
        B2 = 0x02,
        B3 = 0x03,
        B4 = 0x04,
        B5 = 0x05,
        B6 = 0x06,
        B7 = 0x07,
        Q0 = 0x08,
        Q1 = 0x09,
        Q2 = 0x0A,
        Q3 = 0x0B,
        H0 = 0x0C,
        H1 = 0x0D,
        W0 = 0x0E
    }

    public interface IClock {
        bool IsRunning { get; }
        void Start();
        void Stop();
        void ConnectComponent(IBusComponent component);
        void Tick(IBusComponent flags);
        void Initialize();
    }

    public interface IDataBus<DataType> {
        DataType Value { get; set; }
    }

    public enum BusTypes {
        AddressBus,
        DataBus,
        IOBus
    }

    public interface IBusComponent {
        void EnableToAddressBus(SubRegister subReg);
        void EnableToDataBus(SubRegister subReg);
        void EnableToIOBus(SubRegister subReg);

        void SetFromAddressBus(SubRegister subReg);
        void SetFromDataBus(SubRegister subReg);
        void SetFromIOBus(SubRegister subReg);

        event Action<IBusComponent> RequestTickExecute;
        event Action<IBusComponent> RequestTickUpdate;
        event Action<IBusComponent> RequestTickEnableToAddressBus;
        event Action<IBusComponent> RequestTickEnableToDataBus;
        event Action<IBusComponent> RequestTickEnableToIOBus;
        event Action<IBusComponent> RequestTickSetFromAddressBus;
        event Action<IBusComponent> RequestTickSetFromDataBus;
        event Action<IBusComponent> RequestTickSetFromIOBus;
        event Action<IBusComponent> OnRegisterTickStore;
        event Action<IBusComponent> OnRegisterTickLoad;

        void OnTickUpdate(IBusComponent cpuFlags);

        void OnTickEnableToAddressBus(IBusComponent cpuFlags);
        void OnTickEnableToDataBus(IBusComponent cpuFlags);
        void OnTickEnableToIOBus(IBusComponent cpuFlags);

        void OnTickSetFromAddressBus(IBusComponent cpuFlags);
        void OnTickSetFromDataBus(IBusComponent cpuFlags);
        void OnTickSetFromIOBus(IBusComponent cpuFlags);

        void OnTickLoad(IBusComponent cpuFlags);
        void OnTickStore(IBusComponent cpuFlags);
        void OnTickExecute(IBusComponent cpuFlags);
    }

    public interface IMotherboard<DataType> {
        void OnDebug();
        void OnPowerOff();
        void OnRaiseException(byte id);
        void RaiseInterrupt(DataType id);

        event EventHandler<string> Debug;
        event EventHandler PoweredOff;
        event EventHandler<byte> RaiseException;

        IDataBus<DataType> DataBus { get; }
        IDataBus<DataType> AddressBus { get; }
        IDataBus<DataType> IOBus { get; }
        ICpu<DataType> Cpu { get; }
        UInt32 MemorySize { get; }
        void Reset();
        void PowerOn();
        void PowerOff();
        byte ReadByte(DataType address);
        void WriteByte(DataType address, byte value);
        void EnablePortIO(DataType address);
        void SetPortIO(DataType address);
        void SetPortAddress(DataType address);

        void ConnectComponent(IBusComponent component);
        void ConnectDevice(IBusComponent component, DataType address);
        DataType ConnectInterrupt(IBusComponent component, DataType address);
        void EnableDebug(bool @checked);
    }

    public interface ICpu<DataType> : IBusComponent {
        void PowerOn();
        void PowerOff();
        void Reset();
        void RaiseInterrupt(DataType id);
        void Break();
        bool SingleStep { get; set; }
        void Run();
        string RegisterDump { get; }
        bool IsPowerOn { get; }

        event EventHandler<Tuple<UInt64, UInt64>> DecodeInstruction;
    }

    public interface IDissasember<DataType, AddressType> {
        int Decode(DataType value, AddressType address, out string text);
    }

    public interface IConsole {
        void Connect(IMotherboard<UInt64> _motherboard);
        void Show();
        void Close();
        void Clear();
    }
}
