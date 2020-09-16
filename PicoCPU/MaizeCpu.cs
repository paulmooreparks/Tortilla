using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using Tortilla;

namespace Maize {
    using Microcode = List<Action>;

    [StructLayout(LayoutKind.Explicit)]
    public struct RegValue {
        public RegValue(UInt64 init) : this() {
            Value = init;
        }

        public RegValue(UInt32 init) : this() {
            H0 = init;
        }

        public RegValue(UInt16 init) : this() {
            Q0 = init;
        }

        public RegValue(byte init) : this() {
            B0 = init;
        }

        [FieldOffset(0)] public UInt64 Value;
        [FieldOffset(0)] public UInt32 H0;
        [FieldOffset(4)] public UInt32 H1;
        [FieldOffset(0)] public UInt16 Q0;
        [FieldOffset(2)] public UInt16 Q1;
        [FieldOffset(4)] public UInt16 Q2;
        [FieldOffset(6)] public UInt16 Q3;
        [FieldOffset(0)] public byte B0;
        [FieldOffset(1)] public byte B1;
        [FieldOffset(2)] public byte B2;
        [FieldOffset(3)] public byte B3;
        [FieldOffset(4)] public byte B4;
        [FieldOffset(5)] public byte B5;
        [FieldOffset(6)] public byte B6;
        [FieldOffset(7)] public byte B7;

        public byte this[int i] {
            get {
                int shift = i * 8;
                return (byte)(Value >> shift);
            }
            set {
                int shift = i * 8;
                UInt64 newValue = value;
                newValue <<= shift;
                Value |= newValue;
            }
        }

        public UInt64 Base {
            get { return Value >> 8; }
            set { Value |= (value << 8); }
        }

        public int Offset {
            get { return B0; }
            set { B0 = (byte)(value); }
        }

        public static implicit operator RegValue(UInt64 v) => new RegValue(v);
        public static implicit operator RegValue(UInt32 v) => new RegValue(v);
        public static implicit operator RegValue(UInt16 v) => new RegValue(v);
        public static implicit operator RegValue(byte v) => new RegValue(v);

        public override string ToString() {
            return $"0x{Value:X16}";
        }
    }

    public class MaizeRegister : Register<UInt64> {
        protected RegValue RegData;

        public override UInt64 Value {
            get { return RegData.Value; }
            set { RegData.Value = value; }
        }

        public virtual UInt64 W0 {
            get { return Value; }
            set { Value = value; }
        }

        public UInt32 H0 {
            get { return RegData.H0; }
            set { RegData.H0 = value; }
        }

        public UInt32 H1 {
            get { return RegData.H1; }
            set { RegData.H1 = value; }
        }


        public UInt16 Q0 {
            get { return RegData.Q0; }
            set { RegData.Q0 = value; }
        }

        public UInt16 Q1 {
            get { return RegData.Q1; }
            set { RegData.Q1 = value; }
        }

        public UInt16 Q2 {
            get { return RegData.Q2; }
            set { RegData.Q2 = value; }
        }

        public UInt16 Q3 {
            get { return RegData.Q3; }
            set { RegData.Q3 = value; }
        }

        public byte B0 {
            get { return RegData.B0; }
            set { RegData.B0 = value; }
        }

        public byte B1 {
            get { return RegData.B1; }
            set { RegData.B1 = value; }
        }

        public byte B2 {
            get { return RegData.B2; }
            set { RegData.B2 = value; }
        }

        public byte B3 {
            get { return RegData.B3; }
            set { RegData.B3 = value; }
        }

        public byte B4 {
            get { return RegData.B4; }
            set { RegData.B4 = value; }
        }

        public byte B5 {
            get { return RegData.B5; }
            set { RegData.B5 = value; }
        }

        public byte B6 {
            get { return RegData.B6; }
            set { RegData.B6 = value; }
        }

        public byte B7 {
            get { return RegData.B7; }
            set { RegData.B7 = value; }
        }

        public byte this[int i] {
            get {
                return RegData[i];
            }
            set {
                RegData[i] = value;
            }
        }

        public UInt64 Base {
            get { return RegData.Base; }
            set { RegData.Base = value; }
        }

        public int Offset {
            get { return RegData.Offset; }
            set { RegData.Offset = value; }
        }

        public enum SubRegisters : UInt64 {
            W0 = 0b_11111111_11111111_11111111_11111111_11111111_11111111_11111111_11111111,
            H0 = 0b_00000000_00000000_00000000_00000000_11111111_11111111_11111111_11111111,
            H1 = 0b_11111111_11111111_11111111_11111111_00000000_00000000_00000000_00000000,
            Q0 = 0b_00000000_00000000_00000000_00000000_00000000_00000000_11111111_11111111,
            Q1 = 0b_00000000_00000000_00000000_00000000_11111111_11111111_00000000_00000000,
            Q2 = 0b_00000000_00000000_11111111_11111111_00000000_00000000_00000000_00000000,
            Q3 = 0b_11111111_11111111_00000000_00000000_00000000_00000000_00000000_00000000,
            B0 = 0b_00000000_00000000_00000000_00000000_00000000_00000000_00000000_11111111,
            B1 = 0b_00000000_00000000_00000000_00000000_00000000_00000000_11111111_00000000,
            B2 = 0b_00000000_00000000_00000000_00000000_00000000_11111111_00000000_00000000,
            B3 = 0b_00000000_00000000_00000000_00000000_11111111_00000000_00000000_00000000,
            B4 = 0b_00000000_00000000_00000000_11111111_00000000_00000000_00000000_00000000,
            B5 = 0b_00000000_00000000_11111111_00000000_00000000_00000000_00000000_00000000,
            B6 = 0b_00000000_11111111_00000000_00000000_00000000_00000000_00000000_00000000,
            B7 = 0b_11111111_00000000_00000000_00000000_00000000_00000000_00000000_00000000,
        }

        protected Dictionary<SubRegisters, int> OffsetMap { get; set; } = new Dictionary<SubRegisters, int>() { 
            { SubRegisters.B0, 0 },
            { SubRegisters.B1, 8 },
            { SubRegisters.B2, 16 },
            { SubRegisters.B3, 24 },
            { SubRegisters.B4, 32 },
            { SubRegisters.B5, 40 },
            { SubRegisters.B6, 48 },
            { SubRegisters.B7, 56 },
            { SubRegisters.Q0, 0 },
            { SubRegisters.Q1, 16 },
            { SubRegisters.Q2, 32 },
            { SubRegisters.Q3, 48 },
            { SubRegisters.H0, 0 },
            { SubRegisters.H1, 32 },
            { SubRegisters.W0, 0 },
        };

        public SubRegisters SubRegisterMask { get; protected set; } = SubRegisters.W0;

        public override void Enable(BusTypes type) {
            base.Enable(type);
            SubRegisterMask = SubRegisters.W0;
        }

        public override void Set(BusTypes type) {
            base.Set(type);
            SubRegisterMask = SubRegisters.W0;
        }

        public virtual void Enable(BusTypes type, SubRegisters mask) {
            base.Enable(type);
            SubRegisterMask = mask;
        }

        public virtual void Set(BusTypes type, SubRegisters mask) {
            base.Set(type);
            SubRegisterMask = mask;
        }


        public override void OnTick(ClockState state) {
            var mask = (UInt64)SubRegisterMask;
            var offset = OffsetMap[SubRegisterMask];

            switch (state) {
            case ClockState.TickEnable:

                if (DataBusEnabled) {
                    DataBus.Value = (Value & mask) >> offset;
                    DataBusEnabled = false;
                }

                if (AddressBusEnabled) {
                    AddressBus.Value = (Value & mask) >> offset;
                    AddressBusEnabled = false;
                }

                if (IOBusEnabled) {
                    IOBus.Value = (Value & mask) >> offset;
                    IOBusEnabled = false;
                }

                break;

            case ClockState.TickSet:
                var newVal = new UInt64();
                bool set = false;

                if (DataBusSet) {
                    newVal = DataBus.Value & mask;
                    set = true;
                    DataBusSet = false;
                }

                if (AddressBusSet) {
                    newVal = AddressBus.Value & mask;
                    set = true;
                    AddressBusSet = false;
                }

                if (IOBusSet) {
                    newVal = IOBus.Value & mask;
                    set = true;
                    IOBusSet = false;
                }

                if (set) {
                    var notMask = ~mask;
                    var valMask = notMask & Value;
                    Value = valMask | newVal;
                }

                break;
            }
        }
    }

    public class MaizeMotherboard : IMotherboard<UInt64> {
        public MaizeMotherboard() {
            // Reset();
        }

        public void Reset() {
            Clock = new Clock();
            DataBus = new Bus<UInt64>();
            AddressBus = new Bus<UInt64>();
            IOBus = new Bus<UInt64>();
            Cpu = new MaizeCpu(this);
            ConnectComponent(Cpu);

            MemoryModule = new MaizeMemoryModule(this);
        }

        public void PowerOff() {
            Clock.Stop();
            Cpu.PowerOff();
        }

        public void PowerOn() {
            Cpu.PowerOn();
        }


        public IDataBus<UInt64> DataBus { get; set; }
        public IDataBus<UInt64> AddressBus { get; protected set; }
        public IDataBus<UInt64> IOBus { get; set; }

        public IClock Clock { get; set; }

        public ICpu<UInt64> Cpu { get; set; }

        public MaizeMemoryModule MemoryModule { get; set; }

        public UInt32 MemorySize { get { return MemoryModule.MemorySize; } }

        public event EventHandler<string> Debug;
        public event EventHandler PoweredOff;
        public event EventHandler<byte> RaiseException;
        public event EventHandler<byte> RaiseInterrupt;

        public void OnDebug(string disasm) {
            Debug?.Invoke(this, disasm);
        }

        public void OnDebug() {
            Debug?.Invoke(this, null);
        }

        public void OnPowerOff() {
            PoweredOff?.Invoke(this, null);
        }

        public void OnRaiseException(byte id) {
            RaiseException?.Invoke(this, id);
        }

        public void OnRaiseInterrupt(byte id) {
            RaiseInterrupt?.Invoke(this, id);
        }

        public byte ReadByte(UInt64 address) {
            return MemoryModule.ReadByte(address);
        }

        public ushort ReadPort16(ushort address) {
            throw new NotImplementedException();
        }

        public byte ReadPortByte(ushort address) {
            throw new NotImplementedException();
        }

        public void WriteByte(UInt64 address, byte value) {
            MemoryModule.WriteByte(address, value);
        }

        public void WritePort16(ushort address, ushort value) {
            throw new NotImplementedException();
        }

        public void WritePortByte(ushort address, byte value) {
            throw new NotImplementedException();
        }

        protected System.Collections.Generic.Dictionary<byte, IBusComponent> deviceTable =
            new System.Collections.Generic.Dictionary<byte, IBusComponent>();

        public void ConnectComponent(IBusComponent component) {
            Clock.ConnectComponent(component);
        }

        public void ConnectDevice(IBusComponent component, byte address) {
            Clock.ConnectComponent(component);
            deviceTable[address] = component;
        }
    }


    public class MaizeMemoryModule : BusComponent {

        public MaizeMemoryModule(IMotherboard<UInt64> motherboard) {
            AddressRegister.AddressBus = motherboard.AddressBus;
            AddressRegister.DataBus = motherboard.DataBus;
            AddressRegister.IOBus = motherboard.IOBus;
            motherboard.ConnectComponent(AddressRegister);

            DataRegister.AddressBus = motherboard.AddressBus;
            DataRegister.DataBus = motherboard.DataBus;
            DataRegister.IOBus = motherboard.IOBus;
            motherboard.ConnectComponent(DataRegister);

            motherboard.ConnectComponent(this);

            CacheAddress.Value = 0xFFFFFFFF_FFFFFFFF;
        }

        public MaizeRegister AddressRegister { get; set; } = new MaizeRegister();
        public MaizeRegister DataRegister { get; set; } = new MaizeRegister();

        public override void OnTick(ClockState state) {
            switch (state) {
            case ClockState.TickOn:
                if (DataRegister.IsSet) {
                    DataBusEnabled = true;
                }

                if (AddressRegister.IsSet) {
                    DataBusSet = true;
                }

                break;

            case ClockState.TickOff:
                if (DataBusEnabled) {
                    switch (DataRegister.SubRegisterMask) {
                    case MaizeRegister.SubRegisters.B0:
                    case MaizeRegister.SubRegisters.B1:
                    case MaizeRegister.SubRegisters.B2:
                    case MaizeRegister.SubRegisters.B3:
                    case MaizeRegister.SubRegisters.B4:
                    case MaizeRegister.SubRegisters.B5:
                    case MaizeRegister.SubRegisters.B6:
                    case MaizeRegister.SubRegisters.B7:
                        WriteByte(AddressRegister.Value, DataRegister.B0);
                        break;

                    case MaizeRegister.SubRegisters.Q0:
                    case MaizeRegister.SubRegisters.Q1:
                    case MaizeRegister.SubRegisters.Q2:
                    case MaizeRegister.SubRegisters.Q3:
                        WriteQuarterWord(AddressRegister.Value, DataRegister.Q0);
                        break;

                    case MaizeRegister.SubRegisters.H0:
                    case MaizeRegister.SubRegisters.H1:
                        WriteHalfWord(AddressRegister.Value, DataRegister.H0);
                        break;

                    case MaizeRegister.SubRegisters.W0:
                        WriteWord(AddressRegister.Value, DataRegister.Value);
                        break;
                    }

                    DataBusEnabled = false;
                }

                if (DataBusSet) {
                    switch (DataRegister.SubRegisterMask) {
                    case MaizeRegister.SubRegisters.B0:
                    case MaizeRegister.SubRegisters.B1:
                    case MaizeRegister.SubRegisters.B2:
                    case MaizeRegister.SubRegisters.B3:
                    case MaizeRegister.SubRegisters.B4:
                    case MaizeRegister.SubRegisters.B5:
                    case MaizeRegister.SubRegisters.B6:
                    case MaizeRegister.SubRegisters.B7:
                        DataRegister.B0 = ReadByte(AddressRegister.Value);
                        break;

                    case MaizeRegister.SubRegisters.Q0:
                    case MaizeRegister.SubRegisters.Q1:
                    case MaizeRegister.SubRegisters.Q2:
                    case MaizeRegister.SubRegisters.Q3:
                        DataRegister.Q0 = ReadQuarterWord(AddressRegister.Value);
                        break;

                    case MaizeRegister.SubRegisters.H0:
                    case MaizeRegister.SubRegisters.H1:
                        DataRegister.H0 = ReadHalfWord(AddressRegister.Value);
                        break;

                    case MaizeRegister.SubRegisters.W0:
                        DataRegister.Value = ReadWord(AddressRegister.Value);
                        break;
                    }

                    DataBusSet = false;
                }
                break;
            }
        }

        protected byte[] Cache { get; set; }

        protected Dictionary<UInt64, byte[]> MemoryMap { get; set; } = new Dictionary<ulong, byte[]>();

        public UInt32 MemorySize { get { return 0x4000 * 0xFF; } }

        protected RegValue CacheAddress;


        void SetCacheAddress(UInt64 address) {
            RegValue tmp = address;

            if (tmp.Base != CacheAddress.Base) {
                if (!MemoryMap.ContainsKey(tmp.Base)) {
                    MemoryMap[tmp.Base] = new byte[0x100];
                }

                Cache = MemoryMap[tmp.Base];
            }

            CacheAddress.Value = address;
        }

        public UInt64 ReadWord(UInt64 address) {
            RegValue tmp = 0;
            tmp.B0 = ReadByte(address);
            tmp.B1 = ReadByte(++address);
            tmp.B2 = ReadByte(++address);
            tmp.B3 = ReadByte(++address);
            tmp.B4 = ReadByte(++address);
            tmp.B5 = ReadByte(++address);
            tmp.B6 = ReadByte(++address);
            tmp.B7 = ReadByte(++address);
            return tmp.Value;
        }

        public void WriteWord(UInt64 address, UInt64 value) {
            RegValue tmp = value;
            WriteByte(address, tmp.B0);
            WriteByte(++address, tmp.B1);
            WriteByte(++address, tmp.B2);
            WriteByte(++address, tmp.B3);
            WriteByte(++address, tmp.B4);
            WriteByte(++address, tmp.B5);
            WriteByte(++address, tmp.B6);
            WriteByte(++address, tmp.B7);
        }

        public UInt32 ReadHalfWord(UInt64 address) {
            RegValue tmp = 0;
            tmp.B0 = ReadByte(address);
            tmp.B1 = ReadByte(++address);
            tmp.B2 = ReadByte(++address);
            tmp.B3 = ReadByte(++address);
            return tmp.H0;
        }

        public void WriteHalfWord(UInt64 address, UInt32 value) {
            RegValue tmp = value;
            WriteByte(address, tmp.B0);
            WriteByte(++address, tmp.B1);
            WriteByte(++address, tmp.B2);
            WriteByte(++address, tmp.B3);
        }

        public UInt16 ReadQuarterWord(UInt64 address) {
            RegValue tmp = 0;
            tmp.B0 = ReadByte(address);
            tmp.B1 = ReadByte(++address);
            return tmp.Q0;
        }

        public void WriteQuarterWord(UInt64 address, UInt16 value) {
            RegValue tmp = value;
            WriteByte(address, tmp.B0);
            WriteByte(++address, tmp.B1);
        }

        public byte ReadByte(UInt64 address) {
            SetCacheAddress(address);
            return Cache[CacheAddress.Offset];
        }

        public void WriteByte(UInt64 address, byte value) {
            SetCacheAddress(address);
            Cache[CacheAddress.Offset] = value;
        }
    }

    public class MaizeProgramCounter : MaizeRegister {
        public MaizeProgramCounter(IMotherboard<UInt64> motherboard) {
            AddressBus = motherboard.AddressBus;
            DataBus = motherboard.DataBus;
            IOBus = motherboard.IOBus;
            motherboard.ConnectComponent(this);
        }

        protected UInt64 IncrementCount { get; set; }

        public void Increment(int count) {
            IncrementCount = (UInt64)count;
        }

        public override void OnTick(ClockState state) {
            base.OnTick(state);

            switch (state) {
            case ClockState.TickOff:
                Value += IncrementCount;
                IncrementCount = 0;
                break;
            }
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    sealed public class OpCodeAttribute : System.Attribute {
        public byte[] OpCodes { get; set; }

        public OpCodeAttribute(byte opCode) {
            OpCodes = (byte[])Array.CreateInstance(typeof(byte), 1);
            OpCodes[0] = opCode;
        }

        public OpCodeAttribute(params byte[] opCodes) {
            OpCodes = (byte[])Array.CreateInstance(typeof(byte), opCodes.Length);
            Array.Copy(opCodes, OpCodes, opCodes.Length);
        }
    }

    public class MaizeDecoder : MaizeRegister {
        public MaizeDecoder(MaizeMotherboard _motherboard, MaizeCpu _cpu) {
            Motherboard = _motherboard;
            Cpu = _cpu;

            DataBus = Motherboard.DataBus;
            AddressBus = Motherboard.AddressBus;
            IOBus = Motherboard.IOBus;

            OperandRegister1.AddressBus = Motherboard.AddressBus;
            OperandRegister1.DataBus = Motherboard.DataBus;
            OperandRegister1.IOBus = Motherboard.IOBus;
            Motherboard.ConnectComponent(OperandRegister1);

            OperandRegister2.AddressBus = Motherboard.AddressBus;
            OperandRegister2.DataBus = Motherboard.DataBus;
            OperandRegister2.IOBus = Motherboard.IOBus;
            Motherboard.ConnectComponent(OperandRegister2);

            OperandRegister3.AddressBus = Motherboard.AddressBus;
            OperandRegister3.DataBus = Motherboard.DataBus;
            OperandRegister3.IOBus = Motherboard.IOBus;
            Motherboard.ConnectComponent(OperandRegister3);

            OperandRegister4.AddressBus = Motherboard.AddressBus;
            OperandRegister4.DataBus = Motherboard.DataBus;
            OperandRegister4.IOBus = Motherboard.IOBus;
            Motherboard.ConnectComponent(OperandRegister4);

            Motherboard.ConnectComponent(this);

            RegisterMap[OpFlag_RegA] = Cpu.ARegister;
            RegisterMap[OpFlag_RegB] = Cpu.BRegister;
            RegisterMap[OpFlag_RegC] = Cpu.CRegister;
            RegisterMap[OpFlag_RegD] = Cpu.DRegister;
            RegisterMap[OpFlag_RegE] = Cpu.ERegister;
            RegisterMap[OpFlag_RegI] = Cpu.InstructionRegister;
            RegisterMap[OpFlag_RegS] = Cpu.SpRegister;
            RegisterMap[OpFlag_RegF] = Cpu.FRegister;

            BuildMicrocode();
        }

        protected void JumpTo(Microcode microcode) {
            ActiveMicrocode = microcode;
            Step = 0;
            handler = ActiveMicrocode[Step];
            handler();
        }

        protected void End() {
            Cycle = 0;
        }

        protected Dictionary<byte, Microcode> MicrocodeMap;
        protected Microcode ActiveMicrocode;

        public event EventHandler<Tuple<UInt64, UInt64>> InstructionRead;

        public override ulong Value { 
            get => base.Value;
            set {
                base.Value = value;
                InstructionRead?.Invoke(this, Tuple.Create(value, Cpu.ProgramCounter.Value));
            }
        }

        MaizeRegister OperandRegister1 { get; set; } = new MaizeRegister();
        MaizeRegister OperandRegister2 { get; set; } = new MaizeRegister();
        MaizeRegister OperandRegister3 { get; set; } = new MaizeRegister();
        MaizeRegister OperandRegister4 { get; set; } = new MaizeRegister();

        // protected delegate void OpCodeDelegate();

        MaizeMotherboard Motherboard { get; set; }
        MaizeCpu Cpu { get; set; }
        public byte Step { get; protected set; }
        public byte Cycle { get; protected set; }
        Action handler = null;

        protected const byte InsFlag_Imm08b = 0b_10000000;
        protected const byte InsFlag_Imm16b = 0b_10100000;
        protected const byte InsFlag_Imm32b = 0b_11000000;
        protected const byte InsFlag_Imm64b = 0b_11100000;

        protected const byte OpFlag_RegA = 0b_00000000;
        protected const byte OpFlag_RegB = 0b_00000010;
        protected const byte OpFlag_RegC = 0b_00000100;
        protected const byte OpFlag_RegD = 0b_00000110;
        protected const byte OpFlag_RegE = 0b_00001000;
        protected const byte OpFlag_RegI = 0b_00001010;
        protected const byte OpFlag_RegS = 0b_00001100;
        protected const byte OpFlag_RegF = 0b_00001110;

        protected const byte OpFlag_RegW0 =  0b_0001_0000;
        protected const byte OpFlag_RegH0 = 0b_0010_0000;
        protected const byte OpFlag_RegH1 = 0b_0011_0000;
        protected const byte OpFlag_RegQ0 = 0b_0100_0000;
        protected const byte OpFlag_RegQ1 = 0b_0101_0000;
        protected const byte OpFlag_RegQ2 = 0b_0110_0000;
        protected const byte OpFlag_RegQ3 = 0b_0111_0000;
        protected const byte OpFlag_RegB0 = 0b_1000_0000;
        protected const byte OpFlag_RegB1 = 0b_1001_0000;
        protected const byte OpFlag_RegB2 = 0b_1010_0000;
        protected const byte OpFlag_RegB3 = 0b_1011_0000;
        protected const byte OpFlag_RegB4 = 0b_1100_0000;
        protected const byte OpFlag_RegB5 = 0b_1101_0000;
        protected const byte OpFlag_RegB6 = 0b_1110_0000;
        protected const byte OpFlag_RegB7 = 0b_1111_0000;

        protected const byte OpFlag_Imm08b = 0b_0000_0000;
        protected const byte OpFlag_Imm16b = 0b_0000_0010;
        protected const byte OpFlag_Imm32b = 0b_0000_0100;
        protected const byte OpFlag_Imm64b = 0b_0000_0110;

        protected const byte OpFlag_Reserved01 = 0b_0000_1000;
        protected const byte OpFlag_Reserved02 = 0b_0000_1010;
        protected const byte OpFlag_Reserved03 = 0b_0000_1100;
        protected const byte OpFlag_Reserved04 = 0b_0000_1110;

        protected MaizeRegister SrcReg = null;
        protected MaizeRegister DestReg = null;

        protected IDictionary<byte, MaizeRegister> RegisterMap = new System.Collections.Generic.Dictionary<byte, MaizeRegister>();

        protected IDictionary<byte, SubRegisters> SubRegisterMap = new System.Collections.Generic.Dictionary<byte, SubRegisters> {
            { OpFlag_RegB0, SubRegisters.B0 },
            { OpFlag_RegB1, SubRegisters.B1 },
            { OpFlag_RegB2, SubRegisters.B2 },
            { OpFlag_RegB3, SubRegisters.B3 },
            { OpFlag_RegB4, SubRegisters.B4 },
            { OpFlag_RegB5, SubRegisters.B5 },
            { OpFlag_RegB6, SubRegisters.B6 },
            { OpFlag_RegB7, SubRegisters.B7 },
            { OpFlag_RegQ0, SubRegisters.Q0 },
            { OpFlag_RegQ1, SubRegisters.Q1 },
            { OpFlag_RegQ2, SubRegisters.Q2 },
            { OpFlag_RegQ3, SubRegisters.Q3 },
            { OpFlag_RegH0, SubRegisters.H0 },
            { OpFlag_RegH1, SubRegisters.H1 },
            { OpFlag_RegW0, SubRegisters.W0 },
            { OpFlag_Imm08b, SubRegisters.B0 },
            { OpFlag_Imm16b, SubRegisters.Q0 },
            { OpFlag_Imm32b, SubRegisters.H1 },
            { OpFlag_Imm64b, SubRegisters.W0 }
        };

        protected IDictionary<byte, byte> AluOpSizeMap = new Dictionary<byte, byte> {
            { OpFlag_RegB0, MaizeAlu.OpSize_Byte },
            { OpFlag_RegB1, MaizeAlu.OpSize_Byte },
            { OpFlag_RegB2, MaizeAlu.OpSize_Byte },
            { OpFlag_RegB3, MaizeAlu.OpSize_Byte },
            { OpFlag_RegB4, MaizeAlu.OpSize_Byte },
            { OpFlag_RegB5, MaizeAlu.OpSize_Byte },
            { OpFlag_RegB6, MaizeAlu.OpSize_Byte },
            { OpFlag_RegB7, MaizeAlu.OpSize_Byte },
            { OpFlag_RegQ0, MaizeAlu.OpSize_QuarterWord },
            { OpFlag_RegQ1, MaizeAlu.OpSize_QuarterWord },
            { OpFlag_RegQ2, MaizeAlu.OpSize_QuarterWord },
            { OpFlag_RegQ3, MaizeAlu.OpSize_QuarterWord },
            { OpFlag_RegH0, MaizeAlu.OpSize_HalfWord },
            { OpFlag_RegH1, MaizeAlu.OpSize_HalfWord },
            { OpFlag_RegW0, MaizeAlu.OpSize_Word },
            { OpFlag_Imm08b, MaizeAlu.OpSize_Byte },
            { OpFlag_Imm16b, MaizeAlu.OpSize_QuarterWord },
            { OpFlag_Imm32b, MaizeAlu.OpSize_HalfWord },
            { OpFlag_Imm64b, MaizeAlu.OpSize_Word }
        };

        protected Microcode ReadOpcodeAndDecode;
        protected Microcode LD;
        protected Microcode LD_ImmAddr_Reg;
        protected Microcode LD_ImmVal_Reg;
        protected Microcode LD_RegAddr_Reg;
        protected Microcode LD_RegVal_Reg;
        protected Microcode COPY_AB0_Reg;
        protected Microcode COPY_AQ0_Reg;
        protected Microcode COPY_AH0_Reg;
        protected Microcode COPY_AW0_Reg;
        protected Microcode ST;
        protected Microcode ST_Reg_08b;
        protected Microcode ST_Reg_16b;
        protected Microcode ST_Reg_32b;
        protected Microcode ST_Reg_64b;
        protected Microcode ST_AB0_Reg;
        protected Microcode ST_AQ0_Reg;
        protected Microcode ST_AH0_Reg;
        protected Microcode ST_AW0_Reg;
        protected Microcode ADD;
        protected Microcode ADD_ImmAddr_Reg;
        protected Microcode ADD_ImmVal_Reg;
        protected Microcode ADD_RegAddr_Reg;
        protected Microcode ADD_RegVal_Reg;
        protected Microcode ADD_AB0_Reg;
        protected Microcode ADD_AQ0_Reg;
        protected Microcode ADD_AH0_Reg;
        protected Microcode ADD_AW0_Reg;
        protected Microcode HALT;

        public override void OnTick(ClockState state) {
            base.OnTick(state);

            switch (state) {
            case ClockState.TickDecode:
                if (Step >= ActiveMicrocode.Count) {
                    Cycle = 0;
                }

                if (Cycle == 0) {
                    Step = 0;
                    ActiveMicrocode = ReadOpcodeAndDecode;
                }

                handler = ActiveMicrocode[Step];
                handler();
                Motherboard.OnDebug();
                ++Cycle;
                ++Step;

                break;
            }
        }

        protected void BuildMicrocode() {
            ReadOpcodeAndDecode = new Microcode {
                () => {
                    Cpu.ProgramCounter.Enable(BusTypes.AddressBus);
                    Motherboard.MemoryModule.AddressRegister.Set(BusTypes.AddressBus);
                },
                () => {
                    Motherboard.MemoryModule.DataRegister.Enable(BusTypes.DataBus);
                    this.Set(BusTypes.DataBus, SubRegisters.B0);
                    Cpu.ProgramCounter.Increment(1);
                },
                () => {
                    var opcode = this.B0;
                    JumpTo(MicrocodeMap[opcode]);
                },
            };

            // 0x00
            HALT = new Microcode {
                () => {
                    Motherboard.Clock.Stop();
                }
            };

            // 0x01
            LD = new Microcode {
                () => {
                    Cpu.ProgramCounter.Enable(BusTypes.AddressBus);
                    Motherboard.MemoryModule.AddressRegister.Set(BusTypes.AddressBus);
                },
                () => {
                    Motherboard.MemoryModule.DataRegister.Enable(BusTypes.DataBus);
                    OperandRegister1.Set(BusTypes.DataBus, SubRegisters.Q0);
                    this.Set(BusTypes.DataBus, SubRegisters.Q0);
                    Cpu.ProgramCounter.Increment(2);
                },
                () => {
                    bool isAddress = ((OperandRegister1.B0 & 0b_0000_0001) == 0b_0000_0001);

                    if ((OperandRegister1.B0 & 0b_1111_0000) == 0b_0000_0000) {
                        // Source operand is immediate
                        if (isAddress) {
                            JumpTo(LD_ImmAddr_Reg);
                        }
                        else {
                            JumpTo(LD_ImmVal_Reg);
                        }
                    }
                    else {
                        // Source operand is register
                        if (isAddress) {
                            JumpTo(LD_RegAddr_Reg);
                        }
                        else {
                            JumpTo(LD_RegVal_Reg);
                        }
                    }
                },
            };

            LD_ImmAddr_Reg = new Microcode {
                () => {
                    OperandRegister3.Value = 0;
                    Cpu.ProgramCounter.Enable(BusTypes.AddressBus);
                    Motherboard.MemoryModule.AddressRegister.Set(BusTypes.AddressBus);
                },
                () => {
                    Motherboard.MemoryModule.DataRegister.Enable(BusTypes.DataBus);
                    byte subRegMask = (byte)(OperandRegister1.B0 & 0b_0000_1110);
                    OperandRegister3.Set(BusTypes.DataBus, SubRegisterMap[subRegMask]);
                    this.Set(BusTypes.DataBus, SubRegisterMap[subRegMask]);

                    switch (subRegMask) {
                    case OpFlag_Imm08b:
                        Cpu.ProgramCounter.Increment(1);
                        break;

                    case OpFlag_Imm16b:
                        Cpu.ProgramCounter.Increment(2);
                        break;

                    case OpFlag_Imm32b:
                        Cpu.ProgramCounter.Increment(4);
                        break;

                    case OpFlag_Imm64b:
                        Cpu.ProgramCounter.Increment(8);
                        break;
                    }
                },
                () => {
                    OperandRegister3.Enable(BusTypes.AddressBus);
                    Motherboard.MemoryModule.AddressRegister.Set(BusTypes.AddressBus);
                },
                () => {
                    Motherboard.MemoryModule.DataRegister.Enable(BusTypes.DataBus);
                    byte destRegisterFlag = (byte)(OperandRegister1.B1 & 0b_0000_1110);
                    byte destSubRegisterFlag = (byte)(OperandRegister1.B1 & 0b_1111_0000);
                    DestReg = RegisterMap[destRegisterFlag];
                    DestReg.Set(BusTypes.DataBus, SubRegisterMap[destSubRegisterFlag]);
                }
            };

            LD_ImmVal_Reg = new Microcode {
                () => {
                    OperandRegister3.Value = 0;
                    Cpu.ProgramCounter.Enable(BusTypes.AddressBus);
                    Motherboard.MemoryModule.AddressRegister.Set(BusTypes.AddressBus);
                },
                () => {
                    Motherboard.MemoryModule.DataRegister.Enable(BusTypes.DataBus);
                    byte subRegMask = (byte)(OperandRegister1.B0 & 0b_0000_1110);
                    OperandRegister3.Set(BusTypes.DataBus, SubRegisterMap[subRegMask]);
                    this.Set(BusTypes.DataBus, SubRegisterMap[subRegMask]);

                    switch (subRegMask) {
                    case OpFlag_Imm08b:
                        Cpu.ProgramCounter.Increment(1);
                        break;

                    case OpFlag_Imm16b:
                        Cpu.ProgramCounter.Increment(2);
                        break;

                    case OpFlag_Imm32b:
                        Cpu.ProgramCounter.Increment(4);
                        break;

                    case OpFlag_Imm64b:
                        Cpu.ProgramCounter.Increment(8);
                        break;
                    }
                },
                () => {
                    OperandRegister3.Enable(BusTypes.DataBus);
                    byte destRegisterFlag = (byte)(OperandRegister1.B1 & 0b_0000_1110);
                    byte destSubRegisterFlag = (byte)(OperandRegister1.B1 & 0b_1111_0000);
                    DestReg = RegisterMap[destRegisterFlag];
                    DestReg.Set(BusTypes.DataBus, SubRegisterMap[destSubRegisterFlag]);
                }
            };

            LD_RegAddr_Reg = new Microcode {
                () => {
                    byte srcRegisterFlag = (byte)(OperandRegister1.B0 & 0b_0000_1110);
                    byte srcSubRegisterFlag = (byte)(OperandRegister1.B0 & 0b_1111_0000);
                    SrcReg = RegisterMap[srcRegisterFlag];
                    SrcReg.Enable(BusTypes.DataBus, SubRegisterMap[srcSubRegisterFlag]);
                    OperandRegister3.Set(BusTypes.DataBus);
                },
                () => {
                    OperandRegister3.Enable(BusTypes.AddressBus);
                    Motherboard.MemoryModule.AddressRegister.Set(BusTypes.AddressBus);
                },
                () => {
                    Motherboard.MemoryModule.DataRegister.Enable(BusTypes.DataBus);
                    byte destRegisterFlag = (byte)(OperandRegister1.B1 & 0b_0000_1110);
                    byte destSubRegisterFlag = (byte)(OperandRegister1.B1 & 0b_1111_0000);
                    DestReg = RegisterMap[destRegisterFlag];
                    DestReg.Set(BusTypes.DataBus, SubRegisterMap[destSubRegisterFlag]);
                }
            };

            LD_RegVal_Reg = new Microcode {
                () => {
                    byte srcRegisterFlag = (byte)(OperandRegister1.B0 & 0b_0000_1110);
                    byte srcSubRegisterFlag = (byte)(OperandRegister1.B0 & 0b_1111_0000);
                    SrcReg = RegisterMap[srcRegisterFlag];
                    SrcReg.Enable(BusTypes.DataBus, SubRegisterMap[srcSubRegisterFlag]);
                    OperandRegister3.Set(BusTypes.DataBus);
                },
                () => {
                    OperandRegister3.Enable(BusTypes.DataBus);
                    byte destRegisterFlag = (byte)(OperandRegister1.B1 & 0b_0000_1110);
                    byte destSubRegisterFlag = (byte)(OperandRegister1.B1 & 0b_1111_0000);
                    DestReg = RegisterMap[destRegisterFlag];
                    DestReg.Set(BusTypes.DataBus, SubRegisterMap[destSubRegisterFlag]);
                }
            };

            ST = new Microcode {
                () => {
                    OperandRegister3.Value = 0;
                    Cpu.ProgramCounter.Enable(BusTypes.AddressBus);
                    Motherboard.MemoryModule.AddressRegister.Set(BusTypes.AddressBus);
                },
                () => {
                    Motherboard.MemoryModule.DataRegister.Enable(BusTypes.DataBus);
                    OperandRegister1.Set(BusTypes.DataBus, SubRegisters.Q0);
                    this.Set(BusTypes.DataBus, SubRegisters.Q0);
                    Cpu.ProgramCounter.Increment(2);
                },
                () => {
                    byte srcRegisterFlag = (byte)(OperandRegister1.B0 & 0b_0000_1110);
                    byte srcSubRegisterFlag = (byte)(OperandRegister1.B0 & 0b_1111_0000);
                    SrcReg = RegisterMap[srcRegisterFlag];
                    SrcReg.Enable(BusTypes.DataBus, SubRegisterMap[srcSubRegisterFlag]);
                    OperandRegister3.Set(BusTypes.DataBus);
                },
                () => {
                    switch (OperandRegister1.B1 & 0b_0000_1110) {
                    case OpFlag_Imm08b:
                        JumpTo(ST_Reg_08b);
                        break;

                    case OpFlag_Imm16b:
                        JumpTo(ST_Reg_16b);
                        break;

                    case OpFlag_Imm32b:
                        JumpTo(ST_Reg_32b);
                        break;

                    case OpFlag_Imm64b:
                        JumpTo(ST_Reg_64b);
                        break;
                    }
                }
            };

            ST_Reg_08b = new Microcode {
                () => {
                    OperandRegister4.Value = 0;
                    Cpu.ProgramCounter.Enable(BusTypes.AddressBus);
                    Motherboard.MemoryModule.AddressRegister.Set(BusTypes.AddressBus);
                },
                () => {
                    Motherboard.MemoryModule.DataRegister.Enable(BusTypes.DataBus);
                    OperandRegister4.Set(BusTypes.DataBus, SubRegisters.B0);
                    this.Set(BusTypes.DataBus, SubRegisters.B0);
                    Cpu.ProgramCounter.Increment(1);
                },
                () => {
                    OperandRegister3.Enable(BusTypes.DataBus, SubRegisters.B0);
                    OperandRegister4.Enable(BusTypes.AddressBus);
                    Motherboard.MemoryModule.AddressRegister.Set(BusTypes.AddressBus);
                    Motherboard.MemoryModule.DataRegister.Set(BusTypes.DataBus, SubRegisters.B0);
                }
            };

            ST_Reg_16b = new Microcode {
                () => {
                    OperandRegister4.Value = 0;
                    Cpu.ProgramCounter.Enable(BusTypes.AddressBus);
                    Motherboard.MemoryModule.AddressRegister.Set(BusTypes.AddressBus);
                },
                () => {
                    Motherboard.MemoryModule.DataRegister.Enable(BusTypes.DataBus);
                    OperandRegister4.Set(BusTypes.DataBus, SubRegisters.Q0);
                    this.Set(BusTypes.DataBus, SubRegisters.Q0);
                    Cpu.ProgramCounter.Increment(2);
                },
                () => {
                    OperandRegister3.Enable(BusTypes.DataBus, SubRegisters.Q0);
                    OperandRegister4.Enable(BusTypes.AddressBus);
                    Motherboard.MemoryModule.AddressRegister.Set(BusTypes.AddressBus);
                    Motherboard.MemoryModule.DataRegister.Set(BusTypes.DataBus);
                }
            };

            ST_Reg_32b = new Microcode {
                () => {
                    OperandRegister4.Value = 0;
                    Cpu.ProgramCounter.Enable(BusTypes.AddressBus);
                    Motherboard.MemoryModule.AddressRegister.Set(BusTypes.AddressBus);
                },
                () => {
                    Motherboard.MemoryModule.DataRegister.Enable(BusTypes.DataBus);
                    OperandRegister4.Set(BusTypes.DataBus, SubRegisters.H0);
                    this.Set(BusTypes.DataBus, SubRegisters.H0);
                    Cpu.ProgramCounter.Increment(4);
                },
                () => {
                    OperandRegister3.Enable(BusTypes.DataBus, SubRegisters.H0);
                    OperandRegister4.Enable(BusTypes.AddressBus);
                    Motherboard.MemoryModule.AddressRegister.Set(BusTypes.AddressBus);
                    Motherboard.MemoryModule.DataRegister.Set(BusTypes.DataBus, SubRegisters.H0);
                }
            };

            ST_Reg_64b = new Microcode {
                () => {
                    OperandRegister4.Value = 0;
                    Cpu.ProgramCounter.Enable(BusTypes.AddressBus);
                    Motherboard.MemoryModule.AddressRegister.Set(BusTypes.AddressBus);
                },
                () => {
                    Motherboard.MemoryModule.DataRegister.Enable(BusTypes.DataBus);
                    OperandRegister4.Set(BusTypes.DataBus, SubRegisters.W0);
                    this.Set(BusTypes.DataBus, SubRegisters.W0);
                    Cpu.ProgramCounter.Increment(8);
                },
                () => {
                    OperandRegister3.Enable(BusTypes.DataBus, SubRegisters.W0);
                    OperandRegister4.Enable(BusTypes.AddressBus);
                    Motherboard.MemoryModule.AddressRegister.Set(BusTypes.AddressBus);
                    Motherboard.MemoryModule.DataRegister.Set(BusTypes.DataBus, SubRegisters.W0);
                }
            };

            ST_AB0_Reg = new Microcode {
                () => {
                    OperandRegister1.Value = 0;
                    Cpu.ProgramCounter.Enable(BusTypes.AddressBus);
                    Motherboard.MemoryModule.AddressRegister.Set(BusTypes.AddressBus);
                },
                () => {
                    Motherboard.MemoryModule.DataRegister.Enable(BusTypes.DataBus);
                    OperandRegister1.Set(BusTypes.DataBus, SubRegisters.B0);
                    this.Set(BusTypes.DataBus, SubRegisters.B0);
                    Cpu.ProgramCounter.Increment(1);
                },
                () => {
                    OperandRegister1.Enable(BusTypes.AddressBus);
                    Cpu.ARegister.Enable(BusTypes.DataBus, SubRegisters.B0);
                    Motherboard.MemoryModule.AddressRegister.Set(BusTypes.AddressBus);
                    Motherboard.MemoryModule.DataRegister.Set(BusTypes.DataBus);
                }
            };

            ST_AQ0_Reg = new Microcode {
                () => {
                    Cpu.ProgramCounter.Enable(BusTypes.AddressBus);
                    Motherboard.MemoryModule.AddressRegister.Set(BusTypes.AddressBus);
                },
                () => {
                    Motherboard.MemoryModule.DataRegister.Enable(BusTypes.DataBus);
                    OperandRegister1.Set(BusTypes.DataBus, SubRegisters.Q0);
                    this.Set(BusTypes.DataBus, SubRegisters.Q0);
                    Cpu.ProgramCounter.Increment(2);
                },
                () => {
                    // TODO: fault if address mod 2 != 0
                    OperandRegister1.Enable(BusTypes.AddressBus);
                    Cpu.ARegister.Enable(BusTypes.DataBus, SubRegisters.Q0);
                    Motherboard.MemoryModule.AddressRegister.Set(BusTypes.AddressBus);
                    Motherboard.MemoryModule.DataRegister.Set(BusTypes.DataBus);
                }
            };

            ST_AH0_Reg = new Microcode {
                () => {
                    Cpu.ProgramCounter.Enable(BusTypes.AddressBus);
                    Motherboard.MemoryModule.AddressRegister.Set(BusTypes.AddressBus);
                },
                () => {
                    Motherboard.MemoryModule.DataRegister.Enable(BusTypes.DataBus);
                    OperandRegister1.Set(BusTypes.DataBus, SubRegisters.H0);
                    this.Set(BusTypes.DataBus, SubRegisters.H0);
                    Cpu.ProgramCounter.Increment(4);
                },
                () => {
                    // TODO: fault if address mod 4 != 0
                    OperandRegister1.Enable(BusTypes.AddressBus);
                    Cpu.ARegister.Enable(BusTypes.DataBus, SubRegisters.H0);
                    Motherboard.MemoryModule.AddressRegister.Set(BusTypes.AddressBus);
                    Motherboard.MemoryModule.DataRegister.Set(BusTypes.DataBus);
                }
            };

            ST_AW0_Reg = new Microcode {
                () => {
                    OperandRegister1.W0 = 0;
                    Cpu.ProgramCounter.Enable(BusTypes.AddressBus);
                    Motherboard.MemoryModule.AddressRegister.Set(BusTypes.AddressBus);
                },
                () => {
                    Motherboard.MemoryModule.DataRegister.Enable(BusTypes.DataBus);
                    OperandRegister1.Set(BusTypes.DataBus, SubRegisters.W0);
                    this.Set(BusTypes.DataBus, SubRegisters.W0);
                    Cpu.ProgramCounter.Increment(8);
                },
                () => {
                    // TODO: fault if address mod 8 != 0
                    OperandRegister1.Enable(BusTypes.AddressBus);
                    Cpu.ARegister.Enable(BusTypes.DataBus, SubRegisters.W0);
                    Motherboard.MemoryModule.AddressRegister.Set(BusTypes.AddressBus);
                    Motherboard.MemoryModule.DataRegister.Set(BusTypes.DataBus);
                }
            };

            ADD = new Microcode {
                () => {
                    Cpu.ProgramCounter.Enable(BusTypes.AddressBus);
                    Motherboard.MemoryModule.AddressRegister.Set(BusTypes.AddressBus);
                },
                () => {
                    Motherboard.MemoryModule.DataRegister.Enable(BusTypes.DataBus);
                    OperandRegister1.Set(BusTypes.DataBus, SubRegisters.Q0);
                    this.Set(BusTypes.DataBus, SubRegisters.Q0);
                    Cpu.ProgramCounter.Increment(2);
                },
                () => {
                    bool isAddress = ((OperandRegister1.B0 & 0b_0000_0001) == 0b_0000_0001);

                    if ((OperandRegister1.B0 & 0b_1111_0000) == 0b_0000_0000) {
                        // Source operand is immediate
                        if (isAddress) {
                            JumpTo(ADD_ImmAddr_Reg);
                        }
                        else {
                            JumpTo(ADD_ImmVal_Reg);
                        }
                    }
                    else {
                        // Source operand is register
                        if (isAddress) {
                            JumpTo(ADD_RegAddr_Reg);
                        }
                        else {
                            JumpTo(ADD_RegVal_Reg);
                        }
                    }
                }
            };

            ADD_ImmAddr_Reg = new Microcode {
                () => {
                    byte srcRegisterFlag = (byte)(OperandRegister1.B0 & 0b_0000_1110);
                    byte srcSubRegisterFlag = (byte)(OperandRegister1.B0 & 0b_1111_0000);
                    SrcReg = RegisterMap[srcRegisterFlag];
                    SrcReg.Enable(BusTypes.DataBus, SubRegisterMap[srcSubRegisterFlag]);
                    OperandRegister3.Set(BusTypes.DataBus);
                },
                () => {
                    OperandRegister3.Enable(BusTypes.AddressBus);
                    Motherboard.MemoryModule.AddressRegister.Set(BusTypes.AddressBus);
                },
                () => {
                    Motherboard.MemoryModule.DataRegister.Enable(BusTypes.DataBus);
                    byte destRegisterFlag = (byte)(OperandRegister1.B1 & 0b_0000_1110);
                    byte destSubRegisterFlag = (byte)(OperandRegister1.B1 & 0b_1111_0000);
                    DestReg = RegisterMap[destRegisterFlag];
                    DestReg.Set(BusTypes.DataBus, SubRegisterMap[destSubRegisterFlag]);
                }
            };

            ADD_ImmVal_Reg = new Microcode {
                () => {
                    OperandRegister3.Value = 0;
                    Cpu.ProgramCounter.Enable(BusTypes.AddressBus);
                    Motherboard.MemoryModule.AddressRegister.Set(BusTypes.AddressBus);
                },
                () => {
                    Motherboard.MemoryModule.DataRegister.Enable(BusTypes.DataBus);
                    byte subRegMask = (byte)(OperandRegister1.B0 & 0b_0000_1110);
                    Cpu.Alu.SrcReg.Set(BusTypes.DataBus, SubRegisterMap[subRegMask]);
                    this.Set(BusTypes.DataBus, SubRegisterMap[subRegMask]);

                    switch (subRegMask) {
                    case OpFlag_Imm08b:
                        Cpu.ProgramCounter.Increment(1);
                        break;

                    case OpFlag_Imm16b:
                        Cpu.ProgramCounter.Increment(2);
                        break;

                    case OpFlag_Imm32b:
                        Cpu.ProgramCounter.Increment(4);
                        break;

                    case OpFlag_Imm64b:
                        Cpu.ProgramCounter.Increment(8);
                        break;
                    }
                },
                () => {
                    byte destRegisterFlag = (byte)(OperandRegister1.B1 & 0b_0000_1110);
                    byte destSubRegisterFlag = (byte)(OperandRegister1.B1 & 0b_1111_0000);
                    DestReg = RegisterMap[destRegisterFlag];
                    DestReg.Enable(BusTypes.DataBus, SubRegisterMap[destSubRegisterFlag]);
                    Cpu.Alu.DestReg.Set(BusTypes.DataBus);
                    OperandRegister1.B0 = (byte)(MaizeAlu.OpCode_ADD | AluOpSizeMap[destSubRegisterFlag]);
                },
                () => {
                    OperandRegister1.Enable(BusTypes.DataBus);
                    Cpu.Alu.Set(BusTypes.DataBus);
                },
                () => {
                    byte destSubRegisterFlag = (byte)(OperandRegister1.B1 & 0b_1111_0000);
                    Cpu.Alu.DestReg.Enable(BusTypes.DataBus);
                    destSubRegisterFlag = (byte)(OperandRegister1.B1 & 0b_1111_0000);
                    DestReg.Set(BusTypes.DataBus, SubRegisterMap[destSubRegisterFlag]);
                }
            };

            ADD_RegAddr_Reg = new Microcode {
                () => {
                    byte srcRegisterFlag = (byte)(OperandRegister1.B0 & 0b_0000_1110);
                    byte srcSubRegisterFlag = (byte)(OperandRegister1.B0 & 0b_1111_0000);
                    SrcReg = RegisterMap[srcRegisterFlag];
                    SrcReg.Enable(BusTypes.DataBus, SubRegisterMap[srcSubRegisterFlag]);
                    OperandRegister3.Set(BusTypes.DataBus);
                },
                () => {
                    OperandRegister3.Enable(BusTypes.AddressBus);
                    Motherboard.MemoryModule.AddressRegister.Set(BusTypes.AddressBus);
                },
                () => {
                    Motherboard.MemoryModule.DataRegister.Enable(BusTypes.DataBus);
                    byte destRegisterFlag = (byte)(OperandRegister1.B1 & 0b_0000_1110);
                    byte destSubRegisterFlag = (byte)(OperandRegister1.B1 & 0b_1111_0000);
                    DestReg = RegisterMap[destRegisterFlag];
                    DestReg.Set(BusTypes.DataBus, SubRegisterMap[destSubRegisterFlag]);
                }
            };

            ADD_RegVal_Reg = new Microcode {
                () => {
                    byte srcRegisterFlag = (byte)(OperandRegister1.B0 & 0b_0000_1110);
                    byte srcSubRegisterFlag = (byte)(OperandRegister1.B0 & 0b_1111_0000);
                    SrcReg = RegisterMap[srcRegisterFlag];
                    SrcReg.Enable(BusTypes.DataBus, SubRegisterMap[srcSubRegisterFlag]);
                    OperandRegister3.Set(BusTypes.DataBus);
                },
                () => {
                    OperandRegister3.Enable(BusTypes.DataBus);
                    byte destRegisterFlag = (byte)(OperandRegister1.B1 & 0b_0000_1110);
                    byte destSubRegisterFlag = (byte)(OperandRegister1.B1 & 0b_1111_0000);
                    DestReg = RegisterMap[destRegisterFlag];
                    DestReg.Set(BusTypes.DataBus, SubRegisterMap[destSubRegisterFlag]);
                }
            };

            ADD_AB0_Reg = new Microcode {
                () => {
                    OperandRegister1.Value = 0;
                    Cpu.ProgramCounter.Enable(BusTypes.AddressBus);
                    Motherboard.MemoryModule.AddressRegister.Set(BusTypes.AddressBus);
                },
                () => {
                    Motherboard.MemoryModule.DataRegister.Enable(BusTypes.DataBus);
                    Cpu.Alu.SrcReg.Set(BusTypes.DataBus, SubRegisters.B0);
                    this.Set(BusTypes.DataBus, SubRegisters.B0);
                    Cpu.ProgramCounter.Increment(1);
                },
                () => {
                    Cpu.ARegister.Enable(BusTypes.DataBus, SubRegisters.B0);
                    Cpu.Alu.DestReg.Set(BusTypes.DataBus, SubRegisters.B0);
                },
                () => {
                    OperandRegister1.B0 = MaizeAlu.OpCode_ADD | MaizeAlu.OpSize_Byte;
                    OperandRegister1.Enable(BusTypes.DataBus);
                    Cpu.Alu.Set(BusTypes.DataBus);
                },
                () => {
                    Cpu.Alu.DestReg.Enable(BusTypes.DataBus, SubRegisters.B0);
                    Cpu.ARegister.Set(BusTypes.DataBus, SubRegisters.B0);
                }
            };

            ADD_AQ0_Reg = new Microcode {
                () => {
                    OperandRegister1.Value = 0;
                    Cpu.ProgramCounter.Enable(BusTypes.AddressBus);
                    Motherboard.MemoryModule.AddressRegister.Set(BusTypes.AddressBus);
                },
                () => {
                    Motherboard.MemoryModule.DataRegister.Enable(BusTypes.DataBus);
                    Cpu.Alu.SrcReg.Set(BusTypes.DataBus, SubRegisters.Q0);
                    this.Set(BusTypes.DataBus, SubRegisters.Q0);
                    Cpu.ProgramCounter.Increment(2);
                },
                () => {
                    Cpu.ARegister.Enable(BusTypes.DataBus, SubRegisters.Q0);
                    Cpu.Alu.DestReg.Set(BusTypes.DataBus, SubRegisters.Q0);
                },
                () => {
                    OperandRegister1.B0 = MaizeAlu.OpCode_ADD | MaizeAlu.OpSize_QuarterWord;
                    OperandRegister1.Enable(BusTypes.DataBus);
                    Cpu.Alu.Set(BusTypes.DataBus);
                },
                () => {
                    Cpu.Alu.DestReg.Enable(BusTypes.DataBus, SubRegisters.Q0);
                    Cpu.ARegister.Set(BusTypes.DataBus, SubRegisters.Q0);
                }
            };

            ADD_AH0_Reg = new Microcode {
                () => {
                    OperandRegister1.Value = 0;
                    Cpu.ProgramCounter.Enable(BusTypes.AddressBus);
                    Motherboard.MemoryModule.AddressRegister.Set(BusTypes.AddressBus);
                },
                () => {
                    Motherboard.MemoryModule.DataRegister.Enable(BusTypes.DataBus);
                    Cpu.Alu.SrcReg.Set(BusTypes.DataBus, SubRegisters.H0);
                    this.Set(BusTypes.DataBus, SubRegisters.H0);
                    Cpu.ProgramCounter.Increment(4);
                },
                () => {
                    Cpu.ARegister.Enable(BusTypes.DataBus, SubRegisters.H0);
                    Cpu.Alu.DestReg.Set(BusTypes.DataBus, SubRegisters.H0);
                },
                () => {
                    OperandRegister1.B0 = MaizeAlu.OpCode_ADD | MaizeAlu.OpSize_HalfWord;
                    OperandRegister1.Enable(BusTypes.DataBus);
                    Cpu.Alu.Set(BusTypes.DataBus);
                },
                () => {
                    Cpu.Alu.DestReg.Enable(BusTypes.DataBus, SubRegisters.H0);
                    Cpu.ARegister.Set(BusTypes.DataBus, SubRegisters.H0);
                }
            };

            ADD_AW0_Reg = new Microcode {
                () => {
                    OperandRegister1.Value = 0;
                    Cpu.ProgramCounter.Enable(BusTypes.AddressBus);
                    Motherboard.MemoryModule.AddressRegister.Set(BusTypes.AddressBus);
                },
                () => {
                    Motherboard.MemoryModule.DataRegister.Enable(BusTypes.DataBus);
                    Cpu.Alu.SrcReg.Set(BusTypes.DataBus, SubRegisters.W0);
                    this.Set(BusTypes.DataBus, SubRegisters.W0);
                    Cpu.ProgramCounter.Increment(8);
                },
                () => {
                    Cpu.ARegister.Enable(BusTypes.DataBus, SubRegisters.W0);
                    Cpu.Alu.DestReg.Set(BusTypes.DataBus, SubRegisters.W0);
                },
                () => {
                    OperandRegister1.B0 = MaizeAlu.OpCode_ADD | MaizeAlu.OpSize_Word;
                    OperandRegister1.Enable(BusTypes.DataBus);
                    Cpu.Alu.Set(BusTypes.DataBus);
                },
                () => {
                    Cpu.Alu.DestReg.Enable(BusTypes.DataBus, SubRegisters.W0);
                    Cpu.ARegister.Set(BusTypes.DataBus, SubRegisters.W0);
                }
            };

            MicrocodeMap = new Dictionary<byte, Microcode> {
                { 0x00, HALT },
                { 0x01, LD },
                { 0x81, COPY_AB0_Reg },
                { 0xA1, COPY_AQ0_Reg },
                { 0xC1, COPY_AH0_Reg },
                { 0xE1, COPY_AW0_Reg },
                { 0x02, ST },
                { 0x82, ST_AB0_Reg },
                { 0xA2, ST_AQ0_Reg },
                { 0xC2, ST_AH0_Reg },
                { 0xE2, ST_AW0_Reg },
                { 0x03, ADD },
                { 0x83, ADD_AB0_Reg },
                { 0xA3, ADD_AQ0_Reg },
                { 0xC3, ADD_AH0_Reg },
                { 0xE3, ADD_AW0_Reg },
            };

            ActiveMicrocode = ReadOpcodeAndDecode;
        }

        [OpCode(0x00)]
        void HLT() {
            Step = 0;
            Motherboard.Clock.Stop();
        }

        [OpCode(0x80)]
        void INT0() {
            Step = 0;
        }

        [OpCode(0xA0)]
        void INT1() {
            Step = 0;
        }

        [OpCode(0xC0)]
        void INT2() {
            Step = 0;
        }

        [OpCode(0xE0)]
        void INT3() {
            Step = 0;
        }

        [OpCode(0x88)]
        void INT() {
            Step = 0;
        }

        [OpCode(0x99)]
        void NOP() {
            Step = 0;
        }

        [OpCode(0x04)]
        void SUB() {
            NOP();
        }

        [OpCode(0x84)]
        void SUB_Imm() {
            NOP();
        }


        [OpCode(0x0A)]
        void MUL() {
            NOP();
        }

        [OpCode(0x0B)]
        void MUL_Imm() {
            NOP();
        }

        [OpCode(0x0C)]
        void DIV() {
            NOP();
        }

        [OpCode(0x0D)]
        void DIV_Imm() {
            NOP();
        }

        [OpCode(0x0E)]
        void MOD() {
            NOP();
        }

        [OpCode(0x0F)]
        void MOD_Imm() {
            NOP();
        }

        [OpCode(0x10)]
        void INC() {
            NOP();
        }

        [OpCode(0x12)]
        void DEC() {
            NOP();
        }

        [OpCode(0x13)]
        void DEC_Imm() {
            NOP();
        }

        [OpCode(0x14)]
        void AND() {
            NOP();
        }

        [OpCode(0x15)]
        void AND_Imm() {
            NOP();
        }


        [OpCode(0x16)]
        void OR() {
            NOP();
        }

        [OpCode(0x17)]
        void OR_Imm() {
            NOP();
        }

        [OpCode(0x18)]
        void NOR() {
            NOP();
        }

        [OpCode(0x19)]
        void NOR_Imm() {
            NOP();
        }

        [OpCode(0x1A)]
        void NOT() {
            NOP();
        }

        [OpCode(0x1B)]
        void NOT_Imm() {
            NOP();
        }

        [OpCode(0x1C)]
        void NAND() {
            NOP();
        }

        [OpCode(0x1D)]
        void NAND_Imm() {
            NOP();
        }

        [OpCode(0x1E)]
        void XOR() {
            NOP();
        }

        [OpCode(0x1F)]
        void XOR_Imm() {
            NOP();
        }

        [OpCode(0x20)]
        void SHL() {
            NOP();
        }

        [OpCode(0x21)]
        void SHL_Imm() {
            NOP();
        }

        [OpCode(0x22)]
        void SHR() {
            NOP();
        }

        [OpCode(0x23)]
        void SHR_Imm() {
            NOP();
        }

        [OpCode(0x24)]
        void CMP() {
            NOP();
        }

        [OpCode(0x25)]
        void CMP_Imm() {
            NOP();
        }

        [OpCode(0x26)]
        void JMP() {
            NOP();
        }

        [OpCode(0x27)]
        void JMP_Imm() {
            NOP();
        }

        [OpCode(0x28)]
        void JL() {
            NOP();
        }

        [OpCode(0x29)]
        void JL_Imm() {
            NOP();
        }

        [OpCode(0x2A)]
        void JLE() {
            NOP();
        }

        [OpCode(0x2B)]
        void JLE_Imm() {
            NOP();
        }

        [OpCode(0x2C)]
        void JG() {
            NOP();
        }

        [OpCode(0x2D)]
        void JG_Imm() {
            NOP();
        }

        [OpCode(0x2E)]
        void JGE() {
            NOP();
        }

        [OpCode(0x2F)]
        void JGE_Imm() {
            NOP();
        }

        [OpCode(0x30)]
        void JZ() {
            NOP();
        }

        [OpCode(0x31)]
        void JZ_Imm() {
            NOP();
        }

        [OpCode(0x32)]
        void JNZ() {
            NOP();
        }

        [OpCode(0x33)]
        void JNZ_Imm() {
            NOP();
        }

        [OpCode(0x34)]
        void OUT() {
            NOP();
        }

        [OpCode(0x35)]
        void OUT_Imm() {
            NOP();
        }

        [OpCode(0x36)]
        void IN() {
            NOP();
        }

        [OpCode(0x37)]
        void IN_Imm() {
            NOP();
        }

        [OpCode(0x38)]
        void PUSH() {
            NOP();
        }

        [OpCode(0x39)]
        void PUSH_Imm() {
            NOP();
        }

        [OpCode(0x3A)]
        void POP() {
            NOP();
        }

        [OpCode(0x3B)]
        void POP_Imm() {
            NOP();
        }

        [OpCode(0x3C)]
        void LEA() {
            NOP();
        }

        [OpCode(0x3D)]
        void LEA_Imm() {
            NOP();
        }

        [OpCode(0x3E)]
        void CALL() {
            NOP();
        }

        [OpCode(0x3F)]
        void CALL_Imm() {
            NOP();
        }
    }

    public class MaizeCpu : BusComponent, ICpu<UInt64> {

        public MaizeCpu(MaizeMotherboard motherboard) {
            Motherboard = motherboard;

            ARegister.DataBus = Motherboard.DataBus;
            ARegister.AddressBus = motherboard.AddressBus;
            ARegister.IOBus = motherboard.IOBus;
            Motherboard.ConnectComponent(ARegister);

            BRegister.DataBus = Motherboard.DataBus;
            BRegister.AddressBus = motherboard.AddressBus;
            BRegister.IOBus = motherboard.IOBus;
            Motherboard.ConnectComponent(BRegister);

            CRegister.DataBus = Motherboard.DataBus;
            CRegister.AddressBus = motherboard.AddressBus;
            CRegister.IOBus = motherboard.IOBus;
            Motherboard.ConnectComponent(CRegister);

            DRegister.DataBus = Motherboard.DataBus;
            DRegister.AddressBus = motherboard.AddressBus;
            DRegister.IOBus = motherboard.IOBus;
            Motherboard.ConnectComponent(DRegister);

            ERegister.DataBus = Motherboard.DataBus;
            ERegister.AddressBus = motherboard.AddressBus;
            ERegister.IOBus = motherboard.IOBus;
            Motherboard.ConnectComponent(ERegister);

            FRegister.DataBus = Motherboard.DataBus;
            FRegister.AddressBus = motherboard.AddressBus;
            FRegister.IOBus = motherboard.IOBus;
            Motherboard.ConnectComponent(FRegister);

            SpRegister.DataBus = Motherboard.DataBus;
            SpRegister.AddressBus = motherboard.AddressBus;
            SpRegister.IOBus = motherboard.IOBus;
            Motherboard.ConnectComponent(SpRegister);

            ProgramCounter = new MaizeProgramCounter(Motherboard);

            InstructionRegister = new MaizeDecoder(Motherboard, this);
            InstructionRegister.InstructionRead += InstructionRegister_InstructionRead;

            Alu = new MaizeAlu(Motherboard, this);

            Motherboard.ConnectComponent(this);
        }

        private void InstructionRegister_InstructionRead(object sender, Tuple<UInt64, UInt64> e) {
            DecodeInstruction?.Invoke(sender, e);
        }

        MaizeMotherboard Motherboard { get; set; }

        public bool SingleStep { get; set; }

        public string RegisterDump {
            get {
                var regText = $"A={ARegister} B={BRegister} C={CRegister} D={DRegister} E={ERegister} F={FRegister} I={InstructionRegister} P={ProgramCounter} S={SpRegister} Step=0x{InstructionRegister.Step:X8} Cycle=0x{InstructionRegister.Cycle:X8}";
                return regText;
            }
        }

        public override string ToString() {
            return RegisterDump;
        }

        public bool IsPowerOn { get; set; }

        public bool IsHalted { get; set; }

        public MaizeAlu Alu { get; protected set; }

        public MaizeProgramCounter ProgramCounter { get; protected set; }

        public MaizeDecoder InstructionRegister { get; protected set; }

        public MaizeRegister ARegister { get; protected set; } = new MaizeRegister();

        public MaizeRegister BRegister { get; protected set; } = new MaizeRegister();

        public MaizeRegister CRegister { get; protected set; } = new MaizeRegister();

        public MaizeRegister DRegister { get; protected set; } = new MaizeRegister();

        public MaizeRegister ERegister { get; protected set; } = new MaizeRegister();

        public MaizeRegister FRegister { get; protected set; } = new MaizeRegister();

        public MaizeRegister InRegister { get; protected set; } = new MaizeRegister();

        public MaizeRegister SpRegister { get; protected set; } = new MaizeRegister();

        public event EventHandler<Tuple<UInt64, UInt64>> DecodeInstruction;

        public void Break() {
            Motherboard.Clock.Stop();
            SingleStep = true;
        }

        public void Continue() {
            IsPowerOn = true;

            if (SingleStep) {
                Motherboard.Clock.Stop();
                Motherboard.Clock.Tick();
            }
            else {
                Motherboard.Clock.Start();

                while (Motherboard.Clock.IsRunning) {
                    Motherboard.Clock.Tick();
                }
            }
        }

        public void PowerOff() {
            IsPowerOn = false;
        }

        public void PowerOn() {
            IsPowerOn = true;
            Continue();
        }

        public void RaiseInterrupt(byte id) {
            
        }

        public void Reset() {
            ProgramCounter.Value = 0;
            InstructionRegister.Value = 0;
            IsHalted = false;
        }

        public override void OnTick(ClockState state) {
            switch (state) {
            case ClockState.TickOff:
                break;
            }
        }
    }

}
