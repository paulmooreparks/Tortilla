using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
// using System.Runtime.Remoting.Messaging;
using Tortilla;

namespace Maize {
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

        public virtual UInt32 H0 {
            get { return RegData.H0; }
            set { RegData.H0 = value; }
        }

        public virtual UInt32 H1 {
            get { return RegData.H1; }
            set { RegData.H1 = value; }
        }


        public virtual UInt16 Q0 {
            get { return RegData.Q0; }
            set { RegData.Q0 = value; }
        }

        public virtual UInt16 Q1 {
            get { return RegData.Q1; }
            set { RegData.Q1 = value; }
        }

        public virtual UInt16 Q2 {
            get { return RegData.Q2; }
            set { RegData.Q2 = value; }
        }

        public virtual UInt16 Q3 {
            get { return RegData.Q3; }
            set { RegData.Q3 = value; }
        }

        public virtual byte B0 {
            get { return RegData.B0; }
            set { RegData.B0 = value; }
        }

        public virtual byte B1 {
            get { return RegData.B1; }
            set { RegData.B1 = value; }
        }

        public virtual byte B2 {
            get { return RegData.B2; }
            set { RegData.B2 = value; }
        }

        public virtual byte B3 {
            get { return RegData.B3; }
            set { RegData.B3 = value; }
        }

        public virtual byte B4 {
            get { return RegData.B4; }
            set { RegData.B4 = value; }
        }

        public virtual byte B5 {
            get { return RegData.B5; }
            set { RegData.B5 = value; }
        }

        public virtual byte B6 {
            get { return RegData.B6; }
            set { RegData.B6 = value; }
        }

        public virtual byte B7 {
            get { return RegData.B7; }
            set { RegData.B7 = value; }
        }

        public virtual byte this[int i] {
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

        public SubRegisters EnableSubRegisterMask { get; protected set; } = SubRegisters.W0;
        public SubRegisters SetSubRegisterMask { get; protected set; } = SubRegisters.W0;

        public override void Enable(BusTypes type) {
            base.Enable(type);
            EnableSubRegisterMask = SubRegisters.W0;
        }

        public override void Set(BusTypes type) {
            base.Set(type);
            SetSubRegisterMask = SubRegisters.W0;
        }

        public void Enable(BusTypes type, SubRegisters mask) {
            base.Enable(type);
            EnableSubRegisterMask = mask;
        }

        public void Set(BusTypes type, SubRegisters mask) {
            base.Set(type);
            SetSubRegisterMask = mask;
        }

        protected UInt64 IncrementCount { get; set; }
        protected UInt64 DecrementCount { get; set; }

        public void Increment(int count) {
            IncrementCount = (UInt64)count;
        }

        public void Decrement(int count) {
            DecrementCount = (UInt64)count;
        }

        public bool PrivilegeCheck(IBusComponent cpuFlags) {
            var regFlags = cpuFlags as MaizeRegister;
            return (PrivilegeFlags & regFlags.Value) == PrivilegeFlags;
        }

        public override void OnTick(ClockState state, IBusComponent cpuFlags) {
            switch (state) {
            case ClockState.TickOn:
                if (IncrementCount > 0 || DecrementCount > 0) {
                    Value += IncrementCount;
                    IncrementCount = 0;
                    Value -= DecrementCount;
                    DecrementCount = 0;
                }
                break;

            case ClockState.TickEnable:
                var enableMask = (UInt64)EnableSubRegisterMask;
                var enableOffset = OffsetMap[EnableSubRegisterMask];

                if (DataBusEnabled) {
                    DataBus.Value = (Value & enableMask) >> enableOffset;
                    DataBusEnabled = false;
                }
                else if (AddressBusEnabled) {
                    AddressBus.Value = (Value & enableMask) >> enableOffset;
                    AddressBusEnabled = false;
                }
                else if (IOBusEnabled) {
                    IOBus.Value = (Value & enableMask) >> enableOffset;
                    IOBusEnabled = false;
                }

                break;

            case ClockState.TickSet:
                var setMask = (UInt64)SetSubRegisterMask;
                var setOffset = OffsetMap[SetSubRegisterMask];

                if ((PrivilegeMask & setMask) != 0) {
                    if (!PrivilegeCheck(cpuFlags)) {
                        throw new Exception("Privilege exception");
                    }
                }

                if (DataBusSet) {
                    Value = (~setMask & Value) | (DataBus.Value << setOffset) & setMask;
                    DataBusSet = false;
                }
                else if (AddressBusSet) {
                    Value = (~setMask & Value) | (AddressBus.Value << setOffset) & setMask;
                    AddressBusSet = false;
                }
                else if (IOBusSet) {
                    Value = (~setMask & Value) | (IOBus.Value << setOffset) & setMask;
                    IOBusSet = false;
                }

                break;
            }
        }

        public override string ToString() {
            return $"0x{Value:X16}";
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
            Clock?.Stop();
            Cpu?.PowerOff();
        }

        public void PowerOn() {
            Clock.Initialize();
            Cpu.PowerOn();
        }


        public IDataBus<UInt64> DataBus { get; set; }
        public IDataBus<UInt64> AddressBus { get; protected set; }
        public IDataBus<UInt64> IOBus { get; set; }

        public IClock Clock { get; set; }

        public MaizeCpu Cpu { get; set; }

        public MaizeMemoryModule MemoryModule { get; set; }

        public UInt32 MemorySize { get { return MemoryModule.MemorySize; } }

        ICpu<ulong> IMotherboard<ulong>.Cpu => Cpu;

        public event EventHandler<string> Debug;
        public event EventHandler PoweredOff;
        public event EventHandler<byte> RaiseException;

        public void OnDebug() {
            Debug?.Invoke(this, null);
        }

        public void OnPowerOff() {
            PoweredOff?.Invoke(this, null);
        }

        public void OnRaiseException(byte id) {
            RaiseException?.Invoke(this, id);
        }

        public void RaiseInterrupt(int id) {
            Cpu.RaiseInterrupt(id);
        }

        public byte ReadByte(UInt64 address) {
            return MemoryModule.ReadByte(address);
        }

        public void WriteByte(UInt64 address, byte value) {
            MemoryModule.WriteByte(address, value);
        }

        public void EnablePort(byte address) {
            IBusComponent component = deviceTable[address];
            component?.Enable(BusTypes.IOBus);
        }

        public void SetPort(byte address) {
            IBusComponent component = deviceTable[address];
            component?.Set(BusTypes.IOBus);
        }

        protected System.Collections.Generic.Dictionary<int, IBusComponent> deviceTable =
            new System.Collections.Generic.Dictionary<int, IBusComponent>();

        public void ConnectComponent(IBusComponent component) {
            Clock.ConnectComponent(component);
        }

        public void ConnectDevice(IBusComponent component, int address) {
            Clock.ConnectComponent(component);
            deviceTable[address] = component;
        }

        public int ConnectInterrupt(IBusComponent component, int address) {
            return address;
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

        public override void OnTick(ClockState state, IBusComponent cpuFlags) {
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
                    switch (DataRegister.SetSubRegisterMask) {
                    case SubRegisters.B0:
                        WriteByte(AddressRegister.Value, DataRegister.B0);
                        break;

                    case SubRegisters.B1:
                        WriteByte(AddressRegister.Value, DataRegister.B1);
                        break;

                    case SubRegisters.B2:
                        WriteByte(AddressRegister.Value, DataRegister.B2);
                        break;

                    case SubRegisters.B3:
                        WriteByte(AddressRegister.Value, DataRegister.B3);
                        break;

                    case SubRegisters.B4:
                        WriteByte(AddressRegister.Value, DataRegister.B4);
                        break;

                    case SubRegisters.B5:
                        WriteByte(AddressRegister.Value, DataRegister.B5);
                        break;

                    case SubRegisters.B6:
                        WriteByte(AddressRegister.Value, DataRegister.B6);
                        break;

                    case SubRegisters.B7:
                        WriteByte(AddressRegister.Value, DataRegister.B7);
                        break;

                    case SubRegisters.Q0:
                        WriteQuarterWord(AddressRegister.Value, DataRegister.Q0);
                        break;

                    case SubRegisters.Q1:
                        WriteQuarterWord(AddressRegister.Value, DataRegister.Q1);
                        break;

                    case SubRegisters.Q2:
                        WriteQuarterWord(AddressRegister.Value, DataRegister.Q2);
                        break;

                    case SubRegisters.Q3:
                        WriteQuarterWord(AddressRegister.Value, DataRegister.Q3);
                        break;

                    case SubRegisters.H0:
                        WriteHalfWord(AddressRegister.Value, DataRegister.H0);
                        break;

                    case SubRegisters.H1:
                        WriteHalfWord(AddressRegister.Value, DataRegister.H1);
                        break;

                    case SubRegisters.W0:
                        WriteWord(AddressRegister.Value, DataRegister.Value);
                        break;
                    }

                    DataBusEnabled = false;
                }

                if (DataBusSet) {
                    DataRegister.Value = ReadWord(AddressRegister.Value);
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

    public class MappedRegisterH0 : MaizeRegister {
        public MaizeRegister MapReg { get; set; }

        public override byte B0 { get => MapReg.B0; set => MapReg.B0 = value; }
        public override byte B1 { get => MapReg.B1; set => MapReg.B1 = value; }
        public override byte B2 { get => MapReg.B2; set => MapReg.B2 = value; }
        public override byte B3 { get => MapReg.B3; set => MapReg.B3 = value; }
        public override byte B4 { get => MapReg.B0; set => MapReg.B0 = value; }
        public override byte B5 { get => MapReg.B1; set => MapReg.B1 = value; }
        public override byte B6 { get => MapReg.B2; set => MapReg.B2 = value; }
        public override byte B7 { get => MapReg.B3; set => MapReg.B3 = value; }
        public override ushort Q0 { get => MapReg.Q0; set => MapReg.Q0 = value; }
        public override ushort Q1 { get => MapReg.Q1; set => MapReg.Q1 = value; }
        public override ushort Q2 { get => MapReg.Q0; set => MapReg.Q0 = value; }
        public override ushort Q3 { get => MapReg.Q1; set => MapReg.Q1 = value; }
        public override uint H0 { get => MapReg.H0; set => MapReg.H0 = value; }
        public override uint H1 { get => MapReg.H0; set => MapReg.H0 = value; }
        public override ulong W0 { get => Value; set => Value = value; }

        public override ulong Value {
            get => MapReg.H0;
            set => MapReg.H0 = (uint)value & 0b_11111111_11111111_11111111_11111111;
        }
    }


    public class MappedRegisterH1 : MaizeRegister {
        public MaizeRegister MapReg { get; set; }

        public override byte B0 { get => MapReg.B4; set => MapReg.B4 = value; }
        public override byte B1 { get => MapReg.B5; set => MapReg.B5 = value; }
        public override byte B2 { get => MapReg.B6; set => MapReg.B6 = value; }
        public override byte B3 { get => MapReg.B7; set => MapReg.B7 = value; }
        public override byte B4 { get => MapReg.B4; set => MapReg.B4 = value; }
        public override byte B5 { get => MapReg.B5; set => MapReg.B5 = value; }
        public override byte B6 { get => MapReg.B6; set => MapReg.B6 = value; }
        public override byte B7 { get => MapReg.B7; set => MapReg.B7 = value; }
        public override ushort Q0 { get => MapReg.Q2; set => MapReg.Q2 = value; }
        public override ushort Q1 { get => MapReg.Q3; set => MapReg.Q3 = value; }
        public override ushort Q2 { get => MapReg.Q2; set => MapReg.Q2 = value; }
        public override ushort Q3 { get => MapReg.Q3; set => MapReg.Q3 = value; }
        public override uint H0 { get => MapReg.H1; set => MapReg.H1 = value; }
        public override uint H1 { get => MapReg.H1; set => MapReg.H1 = value; }
        public override ulong W0 { get => Value; set => Value = value; }

        public override ulong Value { 
            get => MapReg.H1; 
            set => MapReg.H1 = (uint)value & 0b_11111111_11111111_11111111_11111111; 
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


    public class MaizeInstruction {

        public static MaizeMotherboard MB { get; set; }

        public MaizeInstruction() {
            if (MB == null) {
                throw new Exception("MB static member not initialised");
            }
        }

        public MaizeCpu Cpu => MB.Cpu;
        public MaizeDecoder Decoder => MB.Cpu.Decoder;
        public MaizeRegister OperandRegister1 => Decoder.OperandRegister1;
        public MaizeRegister OperandRegister2 => Decoder.OperandRegister2;
        public MaizeRegister OperandRegister3 => Decoder.OperandRegister3;
        public MaizeRegister OperandRegister4 => Decoder.OperandRegister4;
        public MaizeRegister SrcReg {
            get { return Decoder.SrcReg; }
            set { Decoder.SrcReg = value; }
        }
        public MaizeRegister DestReg {
            get { return Decoder.DestReg; }
            set { Decoder.DestReg = value; }
        }
        public MaizeMemoryModule MemoryModule => MB.MemoryModule;
        public MaizeMemoryModule MM => MB.MemoryModule;
        public MaizeRegister A => MB.Cpu.A;
        public MaizeRegister B => MB.Cpu.B;
        public MaizeRegister C => MB.Cpu.C;
        public MaizeRegister D => MB.Cpu.D;
        public MaizeRegister E => MB.Cpu.E;
        public MaizeRegister G => MB.Cpu.G;
        public MaizeRegister H => MB.Cpu.H;
        public MaizeRegister J => MB.Cpu.J;
        public MaizeRegister K => MB.Cpu.K;
        public MaizeRegister L => MB.Cpu.L;
        public MaizeRegister M => MB.Cpu.M;
        public MaizeRegister Z => MB.Cpu.Z;
        public MaizeRegister F => MB.Cpu.F;
        public MaizeRegister I => MB.Cpu.I;
        public MaizeRegister P => MB.Cpu.P;
        public MaizeRegister S => MB.Cpu.S;

        public MaizeRegister PC => MB.Cpu.PC;
        public MaizeRegister CS => MB.Cpu.CS;
        public MaizeRegister SP => MB.Cpu.SP;
        public MaizeRegister BP => MB.Cpu.BP;
        public MaizeAlu Alu => MB.Cpu.Alu;
        public Tortilla.IClock Clock => MB.Clock;

        protected virtual byte Step { 
            get { return (byte)Decoder.Step; }
            set { Decoder.Step = value; }
        }

        protected virtual byte Cycle => (byte)Decoder.Cycle;
        protected virtual byte Opcode => Decoder.OperandRegister1.B0;
        protected virtual byte Operand1 => Decoder.OperandRegister1.B1;
        protected virtual byte Operand2 => Decoder.OperandRegister1.B2;
        protected virtual byte SrcImmSizeFlag => (byte)(Decoder.OperandRegister1.B1 & OpFlag_ImmSize);
        protected virtual int SrcImmSize => 1 << SrcImmSizeFlag;
        protected virtual byte SrcRegisterFlag => (byte)(Decoder.OperandRegister1.B1 & OpFlag_Reg);
        protected virtual byte SrcSubRegisterFlag => (byte)(Decoder.OperandRegister1.B1 & OpFlag_SubReg);
        protected virtual byte DestImmSizeFlag => (byte)(Decoder.OperandRegister1.B2 & OpFlag_ImmSize);
        protected virtual int DestImmSize => 1 << DestImmSizeFlag;
        protected virtual byte DestRegisterFlag => (byte)(Decoder.OperandRegister1.B2 & OpFlag_Reg);
        protected virtual byte DestSubRegisterFlag => (byte)(Decoder.OperandRegister1.B2 & OpFlag_SubReg);

/*        protected virtual bool IsAddress(byte operand) {
            return (operand & OpFlag_Addr) == OpFlag_Addr;
        }
*/
        protected virtual bool IsSrcImmediate(byte opcode) {
            return (opcode & OpcodeFlag_SrcImm) == OpcodeFlag_SrcImm;
        }

        protected virtual bool IsSrcRegister(byte operand) {
            return !IsSrcImmediate(operand);
        }

        /*
        protected virtual bool IsDestImmediate(byte opcode) {
            return (opcode & OpcodeFlag_DestImm) == OpcodeFlag_DestImm;
        }

        protected virtual bool IsDestRegister(byte operand) {
            return !IsDestImmediate(operand);
        }
        */

        public const byte OpcodeFlag            = 0b_1100_0000;
        public const byte OpcodeFlag_SrcImm     = 0b_0100_0000;
        public const byte OpcodeFlag_SrcAddr    = 0b_1000_0000;

        // public const byte OpFlag_Addr    = 0b_1000_0000;

        public const byte OpFlag_Reg     = 0b_1111_0000;
        public const byte OpFlag_RegA    = 0b_0000_0000;
        public const byte OpFlag_RegB    = 0b_0001_0000;
        public const byte OpFlag_RegC    = 0b_0010_0000;
        public const byte OpFlag_RegD    = 0b_0011_0000;
        public const byte OpFlag_RegE    = 0b_0100_0000;
        public const byte OpFlag_RegG    = 0b_0101_0000;
        public const byte OpFlag_RegH    = 0b_0110_0000;
        public const byte OpFlag_RegJ    = 0b_0111_0000;
        public const byte OpFlag_RegK    = 0b_1000_0000;
        public const byte OpFlag_RegL    = 0b_1001_0000;
        public const byte OpFlag_RegM    = 0b_1010_0000;
        public const byte OpFlag_RegZ    = 0b_1011_0000;
        public const byte OpFlag_RegF    = 0b_1100_0000;
        public const byte OpFlag_RegI    = 0b_1101_0000;
        public const byte OpFlag_RegP    = 0b_1110_0000;
        public const byte OpFlag_RegS    = 0b_1111_0000;

        public const byte OpFlag_RegSP   = 0b_1111_1100; // S.H0 = stack pointer
        public const byte OpFlag_RegBP   = 0b_1111_1101; // S.H1 = base pointer
        public const byte OpFlag_RegPC   = 0b_1110_1100; // P.H0 = program counter
        public const byte OpFlag_RegCS   = 0b_1110_1101; // P.H1 = program segment
        public const byte OpFlag_RegFL   = 0b_1100_1100; // F.H0 = flags

        public const byte OpFlag_SubReg  = 0b_0000_1111;
        public const byte OpFlag_RegB0   = 0b_0000_0000;
        public const byte OpFlag_RegB1   = 0b_0000_0001;
        public const byte OpFlag_RegB2   = 0b_0000_0010;
        public const byte OpFlag_RegB3   = 0b_0000_0011;
        public const byte OpFlag_RegB4   = 0b_0000_0100;
        public const byte OpFlag_RegB5   = 0b_0000_0101;
        public const byte OpFlag_RegB6   = 0b_0000_0110;
        public const byte OpFlag_RegB7   = 0b_0000_0111;
        public const byte OpFlag_RegQ0   = 0b_0000_1000;
        public const byte OpFlag_RegQ1   = 0b_0000_1001;
        public const byte OpFlag_RegQ2   = 0b_0000_1010;
        public const byte OpFlag_RegQ3   = 0b_0000_1011;
        public const byte OpFlag_RegH0   = 0b_0000_1100;
        public const byte OpFlag_RegH1   = 0b_0000_1101;
        public const byte OpFlag_RegW0   = 0b_0000_1110;

        public const byte OpFlag_ImmSize = 0b_0000_0111;
        public const byte OpFlag_Imm08b  = 0b_0000_0000;
        public const byte OpFlag_Imm16b  = 0b_0000_0001;
        public const byte OpFlag_Imm32b  = 0b_0000_0010;
        public const byte OpFlag_Imm64b  = 0b_0000_0011;

        public const byte OpFlag_ImmRes01 = 0b_0100_0000;
        public const byte OpFlag_ImmRes02 = 0b_0101_0000;
        public const byte OpFlag_ImmRes03 = 0b_0110_0000;
        public const byte OpFlag_ImmRes04 = 0b_0111_0000;

        protected static IDictionary<byte, SubRegisters> ImmSizeSubRegMap = new System.Collections.Generic.Dictionary<byte, SubRegisters> {
            { OpFlag_Imm08b, SubRegisters.B0 },
            { OpFlag_Imm16b, SubRegisters.Q0 },
            { OpFlag_Imm32b, SubRegisters.H0 },
            { OpFlag_Imm64b, SubRegisters.W0 }
        };

        protected static IDictionary<byte, SubRegisters> SubRegisterMap = new System.Collections.Generic.Dictionary<byte, SubRegisters> {
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
            { OpFlag_RegW0, SubRegisters.W0 }
        };

        protected static IDictionary<byte, int> SizeMap = new Dictionary<byte, int> {
            { OpFlag_RegB0, 1 },
            { OpFlag_RegB1, 1 },
            { OpFlag_RegB2, 1 },
            { OpFlag_RegB3, 1 },
            { OpFlag_RegB4, 1 },
            { OpFlag_RegB5, 1 },
            { OpFlag_RegB6, 1 },
            { OpFlag_RegB7, 1 },
            { OpFlag_RegQ0, 2 },
            { OpFlag_RegQ1, 2 },
            { OpFlag_RegQ2, 2 },
            { OpFlag_RegQ3, 2 },
            { OpFlag_RegH0, 4 },
            { OpFlag_RegH1, 4 },
            { OpFlag_RegW0, 8 }
        };


        protected static IDictionary<byte, byte> AluOpSizeMap = new Dictionary<byte, byte> {
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
            { OpFlag_RegW0, MaizeAlu.OpSize_Word }
        };

        public Action[] Code { get; set; }

        public virtual void BuildMicrocode() {
            Code = new Action[] { };
        }
    }

    public class InstructionBase<T> : MaizeInstruction where T : MaizeInstruction {
        static InstructionBase() { }

        private static readonly Lazy<T> _instance = new Lazy<T>(() => CreateInstance());

        protected InstructionBase() {
        }

        public static T Instance => _instance.Value;

        public static Action[] GetMicrocode() {
            return Instance.Code;
        }

        public static Action[] Microcode => Instance.Code;

        private static T CreateInstance() {
            T t = Activator.CreateInstance(typeof(T), true) as T;
            t.BuildMicrocode();
            return t;
        }
    }


    public class MaizeCpu : BusComponent, ICpu<UInt64> {
        public const UInt64 Flag_CarryOut =   0b_00000000_00000000_00000000_00000000_00000000_00000000_00000000_00000001;
        public const UInt64 Flag_Negative =   0b_00000000_00000000_00000000_00000000_00000000_00000000_00000000_00000010;
        public const UInt64 Flag_Overflow =   0b_00000000_00000000_00000000_00000000_00000000_00000000_00000000_00000100;
        public const UInt64 Flag_Parity =     0b_00000000_00000000_00000000_00000000_00000000_00000000_00000000_00001000;
        public const UInt64 Flag_Zero =       0b_00000000_00000000_00000000_00000000_00000000_00000000_00000000_00010000;
        public const UInt64 Flag_Sign =       0b_00000000_00000000_00000000_00000000_00000000_00000000_00000000_00100000;
        public const UInt64 Flag_Reserved =   0b_00000000_00000000_00000000_00000000_00000000_00000000_00000000_01000000;
        public const UInt64 Flag_Interrupt =  0b_00000000_00000000_00000000_00000000_00000000_00000000_00000000_10000000;
        public const UInt64 Flag_Privilege =  0b_00000000_00000000_00000000_00000001_00000000_00000000_00000000_00000000;

        public MaizeCpu(MaizeMotherboard motherboard) {
            MB = motherboard;

            A.DataBus = MB.DataBus;
            A.AddressBus = MB.AddressBus;
            A.IOBus = MB.IOBus;
            MB.ConnectComponent(A);

            B.DataBus = MB.DataBus;
            B.AddressBus = MB.AddressBus;
            B.IOBus = MB.IOBus;
            MB.ConnectComponent(B);

            C.DataBus = MB.DataBus;
            C.AddressBus = MB.AddressBus;
            C.IOBus = MB.IOBus;
            MB.ConnectComponent(C);

            D.DataBus = MB.DataBus;
            D.AddressBus = MB.AddressBus;
            D.IOBus = MB.IOBus;
            MB.ConnectComponent(D);

            E.DataBus = MB.DataBus;
            E.AddressBus = MB.AddressBus;
            E.IOBus = MB.IOBus;
            MB.ConnectComponent(E);

            G.DataBus = MB.DataBus;
            G.AddressBus = MB.AddressBus;
            G.IOBus = MB.IOBus;
            MB.ConnectComponent(G);

            H.DataBus = MB.DataBus;
            H.AddressBus = MB.AddressBus;
            H.IOBus = MB.IOBus;
            MB.ConnectComponent(H);

            J.DataBus = MB.DataBus;
            J.AddressBus = MB.AddressBus;
            J.IOBus = MB.IOBus;
            MB.ConnectComponent(J);

            K.DataBus = MB.DataBus;
            K.AddressBus = MB.AddressBus;
            K.IOBus = MB.IOBus;
            MB.ConnectComponent(K);

            L.DataBus = MB.DataBus;
            L.AddressBus = MB.AddressBus;
            L.IOBus = MB.IOBus;
            MB.ConnectComponent(L);

            M.DataBus = MB.DataBus;
            M.AddressBus = MB.AddressBus;
            M.IOBus = MB.IOBus;
            MB.ConnectComponent(M);

            Z.DataBus = MB.DataBus;
            Z.AddressBus = MB.AddressBus;
            Z.IOBus = MB.IOBus;
            MB.ConnectComponent(Z);

            F.PrivilegeFlags = Flag_Privilege;
            F.PrivilegeMask = (UInt64)SubRegisters.H1;
            F.DataBus = MB.DataBus;
            F.AddressBus = MB.AddressBus;
            F.IOBus = MB.IOBus;
            MB.ConnectComponent(F);

            P.PrivilegeFlags = Flag_Privilege;
            P.PrivilegeMask = (UInt64)SubRegisters.H1;
            P.DataBus = MB.DataBus;
            P.AddressBus = MB.AddressBus;
            P.IOBus = MB.IOBus;
            MB.ConnectComponent(P);

            S.DataBus = MB.DataBus;
            S.AddressBus = MB.AddressBus;
            S.IOBus = MB.IOBus;
            MB.ConnectComponent(S);

            // Mapped to P.H0
            PC.MapReg = P;
            PC.DataBus = MB.DataBus;
            PC.AddressBus = MB.AddressBus;
            PC.IOBus = MB.IOBus;
            MB.ConnectComponent(PC);

            // Mapped to P.H1
            CS.MapReg = P;
            CS.DataBus = MB.DataBus;
            CS.AddressBus = MB.AddressBus;
            CS.IOBus = MB.IOBus;
            MB.ConnectComponent(CS);

            // Mapped to S.H0
            SP.MapReg = S;
            SP.DataBus = MB.DataBus;
            SP.AddressBus = MB.AddressBus;
            SP.IOBus = MB.IOBus;
            MB.ConnectComponent(SP);

            // Mapped to S.H1
            BP.MapReg = S;
            BP.DataBus = MB.DataBus;
            BP.AddressBus = MB.AddressBus;
            BP.IOBus = MB.IOBus;
            MB.ConnectComponent(BP);

            Decoder = new MaizeDecoder(MB);
            Decoder.InstructionRead += InstructionRegister_InstructionRead;
            Decoder.RegisterMap[MaizeInstruction.OpFlag_RegA] = A;
            Decoder.RegisterMap[MaizeInstruction.OpFlag_RegB] = B;
            Decoder.RegisterMap[MaizeInstruction.OpFlag_RegC] = C;
            Decoder.RegisterMap[MaizeInstruction.OpFlag_RegD] = D;
            Decoder.RegisterMap[MaizeInstruction.OpFlag_RegE] = E;
            Decoder.RegisterMap[MaizeInstruction.OpFlag_RegG] = G;
            Decoder.RegisterMap[MaizeInstruction.OpFlag_RegH] = H;
            Decoder.RegisterMap[MaizeInstruction.OpFlag_RegJ] = J;
            Decoder.RegisterMap[MaizeInstruction.OpFlag_RegK] = K;
            Decoder.RegisterMap[MaizeInstruction.OpFlag_RegL] = L;
            Decoder.RegisterMap[MaizeInstruction.OpFlag_RegM] = M;
            Decoder.RegisterMap[MaizeInstruction.OpFlag_RegZ] = Z;
            Decoder.RegisterMap[MaizeInstruction.OpFlag_RegF] = F;
            Decoder.RegisterMap[MaizeInstruction.OpFlag_RegI] = Decoder;
            Decoder.RegisterMap[MaizeInstruction.OpFlag_RegP] = P;
            Decoder.RegisterMap[MaizeInstruction.OpFlag_RegS] = S;

            Alu = new MaizeAlu(MB, this);

            MB.ConnectComponent(this);
        }

        public UInt64 Flags {
            get { return F.W0; }
            set { F.W0 = value; }
        }

        /*
        public bool CarryIn {
            get { return (this.B0 & OpCodeCtrl_CarryIn); }
            set { this.B0 = ((this.B0 & ~OpCodeCtrl_CarryIn) | ((value & 0b_0000_0001) << (OpCodeCtrl_CarryIn / 4))); }
        }
        */

        public bool Negative {
            get { return (Flags & Flag_Negative) == Flag_Negative; }
            set { Flags = ((Flags & ~Flag_Negative) | (value ? Flag_Negative : 0)); }
        }

        public bool Overflow {
            get { return (Flags & Flag_Overflow) == Flag_Overflow; }
            set { Flags = ((Flags & ~Flag_Overflow) | (value ? Flag_Overflow : 0)); }
        }

        public bool Zero {
            get { return (Flags & Flag_Zero) == Flag_Zero; }
            set { Flags = ((Flags & ~Flag_Zero) | (value ? Flag_Zero : 0)); }
        }

        public bool Carry {
            get { return (Flags & Flag_CarryOut) == Flag_CarryOut; }
            set { Flags = ((Flags & ~Flag_CarryOut) | (value ? Flag_CarryOut : 0)); }
        }

        public bool Interrupt {
            get { return (Flags & Flag_Interrupt) == Flag_Interrupt; }
            set { Flags = ((Flags & ~Flag_Interrupt) | (value ? Flag_Interrupt : 0)); }
        }

        public bool Privilege {
            get { return (Flags & Flag_Privilege) == Flag_Privilege; }
            set { Flags = ((Flags & ~Flag_Privilege) | (value ? Flag_Privilege : 0)); }
        }

        private void InstructionRegister_InstructionRead(object sender, Tuple<UInt64, UInt64> e) {
            DecodeInstruction?.Invoke(sender, e);
        }

        MaizeMotherboard MB { get; set; }

        public bool SingleStep { get; set; }

        public string RegisterDump {
            get {
                var regText = $"A={A} B={B} C={C} D={D} E={E} G={G} H={H} J={J} K={K} L={L} M={M} Z={Z} F={F} I={Decoder} P={P} S={S} Step=0x{Decoder.Step:X2} Cycle=0x{Decoder.Cycle:X2}";
                return regText;
            }
        }

        public override string ToString() {
            return RegisterDump;
        }

        public bool IsPowerOn { get; set; }

        public bool IsHalted { get; set; }

        public MaizeAlu Alu { get; protected set; }


        public MaizeDecoder Decoder { get; protected set; }

        public MaizeRegister A { get; protected set; } = new MaizeRegister();
        public MaizeRegister B { get; protected set; } = new MaizeRegister();
        public MaizeRegister C { get; protected set; } = new MaizeRegister();
        public MaizeRegister D { get; protected set; } = new MaizeRegister();
        public MaizeRegister E { get; protected set; } = new MaizeRegister();
        public MaizeRegister G { get; protected set; } = new MaizeRegister();
        public MaizeRegister H { get; protected set; } = new MaizeRegister();
        public MaizeRegister J { get; protected set; } = new MaizeRegister();
        public MaizeRegister K { get; protected set; } = new MaizeRegister();
        public MaizeRegister L { get; protected set; } = new MaizeRegister();
        public MaizeRegister M { get; protected set; } = new MaizeRegister();
        public MaizeRegister Z { get; protected set; } = new MaizeRegister();
        public MaizeRegister F { get; protected set; } = new MaizeRegister();
        public MaizeRegister I { get; protected set; } = new MaizeRegister();
        public MaizeRegister P { get; protected set; } = new MaizeRegister();
        public MaizeRegister S { get; protected set; } = new MaizeRegister();
        public MappedRegisterH0 PC { get; protected set; } = new MappedRegisterH0();
        public MappedRegisterH1 CS { get; protected set; } = new MappedRegisterH1();
        public MappedRegisterH0 SP { get; protected set; } = new MappedRegisterH0();
        public MappedRegisterH1 BP { get; protected set; } = new MappedRegisterH1();
     
        public event EventHandler<Tuple<UInt64, UInt64>> DecodeInstruction;

        public void Break() {
            MB.Clock.Stop();
            SingleStep = true;
        }

        public void Continue() {
            IsPowerOn = true;

            if (SingleStep) {
                MB.Clock.Stop();
                MB.Clock.Tick(F);
            }
            else {
                MB.Clock.Start();

                while (MB.Clock.IsRunning) {
                    MB.Clock.Tick(F);
                }
            }
        }

        public void PowerOff() {
            IsPowerOn = false;
        }

        public void PowerOn() {
            IsPowerOn = true;
            Privilege = true;
        }

        public int InterruptID { get; set; }

        public void RaiseInterrupt(int id) {
            // Set interrupt flag
            Interrupt = true;
            InterruptID = id;
            Continue();
        }

        public void Reset() {
            PC.Value = 0;
            Decoder.Value = 0;
            IsHalted = false;
        }

        public override void OnTick(ClockState state, IBusComponent cpuFlags) {
            switch (state) {
            case ClockState.TickOff:
                break;
            }
        }
    }

    public class MaizeDecoder : MaizeRegister {
        public MaizeDecoder(MaizeMotherboard _motherboard) {
            MB = _motherboard;

            DataBus = MB.DataBus;
            AddressBus = MB.AddressBus;
            IOBus = MB.IOBus;

            OperandRegister1.AddressBus = MB.AddressBus;
            OperandRegister1.DataBus = MB.DataBus;
            OperandRegister1.IOBus = MB.IOBus;
            MB.ConnectComponent(OperandRegister1);

            OperandRegister2.AddressBus = MB.AddressBus;
            OperandRegister2.DataBus = MB.DataBus;
            OperandRegister2.IOBus = MB.IOBus;
            MB.ConnectComponent(OperandRegister2);

            OperandRegister3.AddressBus = MB.AddressBus;
            OperandRegister3.DataBus = MB.DataBus;
            OperandRegister3.IOBus = MB.IOBus;
            MB.ConnectComponent(OperandRegister3);

            OperandRegister4.AddressBus = MB.AddressBus;
            OperandRegister4.DataBus = MB.DataBus;
            OperandRegister4.IOBus = MB.IOBus;
            MB.ConnectComponent(OperandRegister4);

            MB.ConnectComponent(this);

            BuildMicrocode();
        }

        public void JumpTo(Action[] microcode) {
            ActiveMicrocode = microcode;
            Step = 0;
            handler = ActiveMicrocode[Step];
            handler();
        }

        protected Dictionary<byte, Action[]> MicrocodeMap;
        protected Action[][] MicrocodeArray;
        protected Action[] ActiveMicrocode;

        public event EventHandler<Tuple<UInt64, UInt64>> InstructionRead;

        public override ulong Value { 
            get => base.Value;
            set {
                base.Value = value;
                /* DEBUG */
                InstructionRead?.Invoke(this, Tuple.Create(value, MB.Cpu.PC.Value - 1));
            }
        }

        public MaizeRegister OperandRegister1 { get; set; } = new MaizeRegister();
        public MaizeRegister OperandRegister2 { get; set; } = new MaizeRegister();
        public MaizeRegister OperandRegister3 { get; set; } = new MaizeRegister();
        public MaizeRegister OperandRegister4 { get; set; } = new MaizeRegister();
        public MaizeRegister SrcReg = null;
        public MaizeRegister DestReg = null;

        MaizeMotherboard MB { get; set; }
        public Tortilla.IClock Clock => MB.Clock;

        public int Step { get; set; }
        public int Cycle { get; protected set; }
        Action handler = null;

        public IDictionary<byte, MaizeRegister> RegisterMap = new System.Collections.Generic.Dictionary<byte, MaizeRegister>();

        public Action[] ReadOpcodeAndDispatch;
        public Action[] ReadImmediate1Byte;
        public Action[] ReadImmediate2Byte;
        public Action[] ReadImmediate4Byte;
        public Action[] ReadImmediate8Byte;

        public override void OnTick(ClockState state, IBusComponent cpuFlags) {
            base.OnTick(state, cpuFlags);

            switch (state) {
            case ClockState.TickDecode:
                if (Step >= ActiveMicrocode.Length) {
                    Cycle = 0;
                }

                if (Cycle == 0) {
                    Step = 0;
                    ActiveMicrocode = ReadOpcodeAndDispatch;

                    if (MB.Cpu.Interrupt) {
                        MB.Cpu.Interrupt = false;

                        if (MB.Cpu.InterruptID != 0) {
                            MB.Cpu.S.Value -= 8;
                            MB.MemoryModule.WriteHalfWord(MB.Cpu.S.H0, MB.Cpu.PC.H0);
                        }

                        // Vector to interrupt handler for MB.Cpu.InterruptID
                        UInt32 intAddress = (UInt32)(MB.Cpu.InterruptID * 4);
                        UInt32 startAddress = MB.MemoryModule.ReadHalfWord(intAddress);
                        MB.Cpu.PC.Value = startAddress;
                    }
                }

                handler = ActiveMicrocode[Step];
                handler();
                /* DEBUG */
                MB.OnDebug();
                ++Cycle;
                ++Step;

                break;
            }
        }

        protected Stack<Action[]> MicrocodeStack = new Stack<Action[]>();
        protected Stack<int> StepStack = new Stack<int>();

        public void LoadImmediate(int size) {
            PushMicrocodeStack();

            switch (size) {
            case 1:
                ActiveMicrocode = ReadImmediate1Byte;
                break;
            case 2:
                ActiveMicrocode = ReadImmediate2Byte;
                break;
            case 4:
                ActiveMicrocode = ReadImmediate4Byte;
                break;
            case 8:
                ActiveMicrocode = ReadImmediate8Byte;
                break;
            }

            handler = ActiveMicrocode[0];
            handler();
        }

        public void ExitInstruction() {
            ActiveMicrocode = new Action[] {
                () => { }
            };

            Step = 0xFF;
        }

        protected void PushMicrocodeStack() {
            MicrocodeStack.Push(ActiveMicrocode);
            StepStack.Push(Step);
            Step = 0;
        }

        protected void PopMicrocodeStack() {
            ActiveMicrocode = MicrocodeStack.Pop();
            Step = StepStack.Pop();
            ++Step;
            handler = ActiveMicrocode[Step];
            handler();
        }

        protected void BuildMicrocode() {
            MaizeInstruction.MB = MB;

            ReadOpcodeAndDispatch = new Action[] {
                () => {
                    MB.Cpu.PC.Enable(BusTypes.AddressBus);
                    MB.MemoryModule.AddressRegister.Set(BusTypes.AddressBus);
                },
                () => {
                    MB.MemoryModule.DataRegister.Enable(BusTypes.DataBus);
                    this.Set(BusTypes.DataBus);
                    this.OperandRegister1.Set(BusTypes.DataBus);
                    MB.Cpu.PC.Increment(1);
                },
                () => {
                    var opcode = this.B0;
                    JumpTo(MicrocodeArray[opcode]);
                },
            };

            ReadImmediate1Byte = new Action[] {
                () => {
                    MB.Cpu.PC.Enable(BusTypes.AddressBus);
                    MB.MemoryModule.AddressRegister.Set(BusTypes.AddressBus);
                },
                () => {
                    MB.Cpu.PC.Increment(1);
                    MB.MemoryModule.DataRegister.Enable(BusTypes.DataBus, SubRegisters.B0);
                    this.Set(BusTypes.DataBus);
                    PopMicrocodeStack();
                }
            };

            ReadImmediate2Byte = new Action[] {
                () => {
                    MB.Cpu.PC.Enable(BusTypes.AddressBus);
                    MB.MemoryModule.AddressRegister.Set(BusTypes.AddressBus);
                },
                () => {
                    MB.Cpu.PC.Increment(2);
                    MB.MemoryModule.DataRegister.Enable(BusTypes.DataBus, SubRegisters.Q0);
                    this.Set(BusTypes.DataBus);
                    PopMicrocodeStack();
                }
            };

            ReadImmediate4Byte = new Action[] {
                () => {
                    MB.Cpu.PC.Enable(BusTypes.AddressBus);
                    MB.MemoryModule.AddressRegister.Set(BusTypes.AddressBus);
                },
                () => {
                    MB.Cpu.PC.Increment(4);
                    MB.MemoryModule.DataRegister.Enable(BusTypes.DataBus, SubRegisters.H0);
                    this.Set(BusTypes.DataBus);
                    PopMicrocodeStack();
                }
            };

            ReadImmediate8Byte = new Action[] {
                () => {
                    MB.Cpu.PC.Enable(BusTypes.AddressBus);
                    MB.MemoryModule.AddressRegister.Set(BusTypes.AddressBus);
                },
                () => {
                    MB.Cpu.PC.Increment(8);
                    MB.MemoryModule.DataRegister.Enable(BusTypes.DataBus, SubRegisters.W0);
                    this.Set(BusTypes.DataBus);
                    PopMicrocodeStack();
                }
            };

            ActiveMicrocode = ReadOpcodeAndDispatch;

            MicrocodeArray = new Action[][] {
                /* 0x00 */ Instructions.HALT.Microcode,
                /* 0x01 */ Instructions.LD_RegVal_Reg.Microcode,
                /* 0x02 */ Instructions.ST.Microcode,
                /* 0x03 */ Instructions.ADD_RegVal_Reg.Microcode,
                /* 0x04 */ Instructions.SUB_RegVal_Reg.Microcode,
                /* 0x05 */ Instructions.MUL_RegVal_Reg.Microcode,
                /* 0x06 */ Instructions.DIV_RegVal_Reg.Microcode,
                /* 0x07 */ Instructions.MOD_RegVal_Reg.Microcode,
                /* 0x08 */ Instructions.AND_RegVal_Reg.Microcode,
                /* 0x09 */ Instructions.OR_RegVal_Reg.Microcode,
                /* 0x0A */ Instructions.NOR_RegVal_Reg.Microcode,
                /* 0x0B */ Instructions.NAND_RegVal_Reg.Microcode,
                /* 0x0C */ Instructions.XOR_RegVal_Reg.Microcode,
                /* 0x0D */ Instructions.SHL_RegVal_Reg.Microcode,
                /* 0x0E */ Instructions.SHR_RegVal_Reg.Microcode,
                /* 0x0F */ Instructions.CMP_RegVal_Reg.Microcode,
                /* 0x10 */ Instructions.TEST_RegVal_Reg.Microcode,
                /* 0x11 */ Exceptions.BadOpcode.Microcode,
                /* 0x12 */ Instructions.INT_RegVal.Microcode,
                /* 0x13 */ Instructions.STIN.Microcode,
                /* 0x14 */ Instructions.OUTR_RegVal_Reg.Microcode,
                /* 0x15 */ Exceptions.BadOpcode.Microcode,
                /* 0x16 */ Instructions.JMP_RegVal.Microcode,
                /* 0x17 */ Instructions.JZ_RegVal.Microcode,
                /* 0x18 */ Instructions.JNZ_RegVal.Microcode,
                /* 0x19 */ Exceptions.BadOpcode.Microcode,
                /* 0x1A */ Exceptions.BadOpcode.Microcode,
                /* 0x1B */ Exceptions.BadOpcode.Microcode,
                /* 0x1C */ Exceptions.BadOpcode.Microcode,
                /* 0x1D */ Instructions.CALL_RegVal.Microcode,
                /* 0x1E */ Instructions.OUT_RegVal_Imm.Microcode,
                /* 0x1F */ Instructions.IN_RegVal_Reg.Microcode,
                /* 0x20 */ Instructions.PUSH_RegVal.Microcode,
                /* 0x21 */ Instructions.PUSH_ImmVal.Microcode,
                /* 0x22 */ Instructions.CLR.Microcode,
                /* 0x23 */ Instructions.INC.Microcode,
                /* 0x24 */ Instructions.DEC.Microcode,
                /* 0x25 */ Exceptions.BadOpcode.Microcode,
                /* 0x26 */ Instructions.POP_RegVal.Microcode,
                /* 0x27 */ Instructions.RET.Microcode,
                /* 0x28 */ Exceptions.BadOpcode.Microcode,
                /* 0x29 */ Exceptions.BadOpcode.Microcode,
                /* 0x2A */ Exceptions.BadOpcode.Microcode,
                /* 0x2B */ Exceptions.BadOpcode.Microcode,
                /* 0x2C */ Exceptions.BadOpcode.Microcode,
                /* 0x2D */ Exceptions.BadOpcode.Microcode,
                /* 0x2E */ Exceptions.BadOpcode.Microcode,
                /* 0x2F */ Exceptions.BadOpcode.Microcode,
                /* 0x30 */ Exceptions.BadOpcode.Microcode,
                /* 0x31 */ Exceptions.BadOpcode.Microcode,
                /* 0x32 */ Exceptions.BadOpcode.Microcode,
                /* 0x33 */ Exceptions.BadOpcode.Microcode,
                /* 0x34 */ Exceptions.BadOpcode.Microcode,
                /* 0x35 */ Exceptions.BadOpcode.Microcode,
                /* 0x36 */ Exceptions.BadOpcode.Microcode,
                /* 0x37 */ Exceptions.BadOpcode.Microcode,
                /* 0x38 */ Exceptions.BadOpcode.Microcode,
                /* 0x39 */ Exceptions.BadOpcode.Microcode,
                /* 0x3A */ Exceptions.BadOpcode.Microcode,
                /* 0x3B */ Exceptions.BadOpcode.Microcode,
                /* 0x3C */ Exceptions.BadOpcode.Microcode,
                /* 0x3D */ Exceptions.BadOpcode.Microcode,
                /* 0x3E */ Exceptions.BadOpcode.Microcode,
                /* 0x3F */ Exceptions.BadOpcode.Microcode,
                /* 0x40 */ Exceptions.BadOpcode.Microcode,
                /* 0x41 */ Instructions.LD_ImmVal_Reg.Microcode,
                /* 0x42 */ Exceptions.BadOpcode.Microcode,
                /* 0x43 */ Instructions.ADD_ImmVal_Reg.Microcode,
                /* 0x44 */ Instructions.SUB_ImmVal_Reg.Microcode,
                /* 0x45 */ Instructions.MUL_ImmVal_Reg.Microcode,
                /* 0x46 */ Instructions.DIV_ImmVal_Reg.Microcode,
                /* 0x47 */ Instructions.MOD_ImmVal_Reg.Microcode,
                /* 0x48 */ Instructions.AND_ImmVal_Reg.Microcode,
                /* 0x49 */ Instructions.OR_ImmVal_Reg.Microcode,
                /* 0x4A */ Instructions.NOR_ImmVal_Reg.Microcode,
                /* 0x4B */ Instructions.NAND_ImmVal_Reg.Microcode,
                /* 0x4C */ Instructions.XOR_ImmVal_Reg.Microcode,
                /* 0x4D */ Instructions.SHL_ImmVal_Reg.Microcode,
                /* 0x4E */ Instructions.SHR_ImmVal_Reg.Microcode,
                /* 0x4F */ Instructions.CMP_ImmVal_Reg.Microcode,
                /* 0x50 */ Instructions.TEST_ImmVal_Reg.Microcode,
                /* 0x51 */ Exceptions.BadOpcode.Microcode,
                /* 0x52 */ Instructions.INT_ImmVal.Microcode,
                /* 0x53 */ Exceptions.BadOpcode.Microcode,
                /* 0x54 */ Instructions.OUTR_ImmVal_Reg.Microcode,
                /* 0x55 */ Exceptions.BadOpcode.Microcode,
                /* 0x56 */ Instructions.JMP_ImmVal.Microcode,
                /* 0x57 */ Instructions.JZ_ImmVal.Microcode,
                /* 0x58 */ Instructions.JNZ_ImmVal.Microcode,
                /* 0x59 */ Exceptions.BadOpcode.Microcode,
                /* 0x5A */ Exceptions.BadOpcode.Microcode,
                /* 0x5B */ Exceptions.BadOpcode.Microcode,
                /* 0x5C */ Exceptions.BadOpcode.Microcode,
                /* 0x5D */ Instructions.CALL_ImmVal.Microcode,
                /* 0x5E */ Exceptions.BadOpcode.Microcode,
                /* 0x5F */ Instructions.IN_ImmVal_Reg.Microcode,
                /* 0x60 */ Exceptions.BadOpcode.Microcode,
                /* 0x61 */ Exceptions.BadOpcode.Microcode,
                /* 0x62 */ Exceptions.BadOpcode.Microcode,
                /* 0x63 */ Exceptions.BadOpcode.Microcode,
                /* 0x64 */ Exceptions.BadOpcode.Microcode,
                /* 0x65 */ Exceptions.BadOpcode.Microcode,
                /* 0x66 */ Exceptions.BadOpcode.Microcode,
                /* 0x67 */ Exceptions.BadOpcode.Microcode,
                /* 0x68 */ Exceptions.BadOpcode.Microcode,
                /* 0x69 */ Exceptions.BadOpcode.Microcode,
                /* 0x6A */ Exceptions.BadOpcode.Microcode,
                /* 0x6B */ Exceptions.BadOpcode.Microcode,
                /* 0x6C */ Exceptions.BadOpcode.Microcode,
                /* 0x6D */ Exceptions.BadOpcode.Microcode,
                /* 0x6E */ Exceptions.BadOpcode.Microcode,
                /* 0x6F */ Exceptions.BadOpcode.Microcode,
                /* 0x70 */ Exceptions.BadOpcode.Microcode,
                /* 0x71 */ Exceptions.BadOpcode.Microcode,
                /* 0x72 */ Exceptions.BadOpcode.Microcode,
                /* 0x73 */ Exceptions.BadOpcode.Microcode,
                /* 0x74 */ Exceptions.BadOpcode.Microcode,
                /* 0x75 */ Exceptions.BadOpcode.Microcode,
                /* 0x76 */ Exceptions.BadOpcode.Microcode,
                /* 0x77 */ Exceptions.BadOpcode.Microcode,
                /* 0x78 */ Exceptions.BadOpcode.Microcode,
                /* 0x79 */ Exceptions.BadOpcode.Microcode,
                /* 0x7A */ Exceptions.BadOpcode.Microcode,
                /* 0x7B */ Exceptions.BadOpcode.Microcode,
                /* 0x7C */ Exceptions.BadOpcode.Microcode,
                /* 0x7D */ Exceptions.BadOpcode.Microcode,
                /* 0x7E */ Exceptions.BadOpcode.Microcode,
                /* 0x7F */ Exceptions.BadOpcode.Microcode,
                /* 0x80 */ Exceptions.BadOpcode.Microcode,
                /* 0x81 */ Instructions.LD_RegAddr_Reg.Microcode,
                /* 0x82 */ Exceptions.BadOpcode.Microcode,
                /* 0x83 */ Instructions.ADD_RegAddr_Reg.Microcode,
                /* 0x84 */ Instructions.SUB_RegAddr_Reg.Microcode,
                /* 0x85 */ Instructions.MUL_RegAddr_Reg.Microcode,
                /* 0x86 */ Instructions.DIV_RegAddr_Reg.Microcode,
                /* 0x87 */ Instructions.MOD_RegAddr_Reg.Microcode,
                /* 0x88 */ Instructions.AND_RegAddr_Reg.Microcode,
                /* 0x89 */ Instructions.OR_RegAddr_Reg.Microcode,
                /* 0x8A */ Instructions.NOR_RegAddr_Reg.Microcode,
                /* 0x8B */ Instructions.NAND_RegAddr_Reg.Microcode,
                /* 0x8C */ Instructions.XOR_RegAddr_Reg.Microcode,
                /* 0x8D */ Instructions.SHL_RegAddr_Reg.Microcode,
                /* 0x8E */ Instructions.SHR_RegAddr_Reg.Microcode,
                /* 0x8F */ Instructions.CMP_RegAddr_Reg.Microcode,
                /* 0x90 */ Instructions.TEST_RegAddr_Reg.Microcode,
                /* 0x91 */ Exceptions.BadOpcode.Microcode,
                /* 0x92 */ Exceptions.BadOpcode.Microcode,
                /* 0x93 */ Exceptions.BadOpcode.Microcode,
                /* 0x94 */ Exceptions.BadOpcode.Microcode,
                /* 0x95 */ Exceptions.BadOpcode.Microcode,
                /* 0x96 */ Instructions.JMP_RegAddr.Microcode,
                /* 0x97 */ Instructions.JZ_RegAddr.Microcode,
                /* 0x98 */ Instructions.JNZ_RegAddr.Microcode,
                /* 0x99 */ Exceptions.BadOpcode.Microcode,
                /* 0x9A */ Exceptions.BadOpcode.Microcode,
                /* 0x9B */ Exceptions.BadOpcode.Microcode,
                /* 0x9C */ Exceptions.BadOpcode.Microcode,
                /* 0x9D */ Instructions.CALL_RegAddr.Microcode,
                /* 0x9E */ Exceptions.BadOpcode.Microcode,
                /* 0x9F */ Exceptions.BadOpcode.Microcode,
                /* 0xA0 */ Exceptions.BadOpcode.Microcode,
                /* 0xA1 */ Exceptions.BadOpcode.Microcode,
                /* 0xA2 */ Exceptions.BadOpcode.Microcode,
                /* 0xA3 */ Exceptions.BadOpcode.Microcode,
                /* 0xA4 */ Exceptions.BadOpcode.Microcode,
                /* 0xA5 */ Exceptions.BadOpcode.Microcode,
                /* 0xA6 */ Exceptions.BadOpcode.Microcode,
                /* 0xA7 */ Exceptions.BadOpcode.Microcode,
                /* 0xA8 */ Exceptions.BadOpcode.Microcode,
                /* 0xA9 */ Exceptions.BadOpcode.Microcode,
                /* 0xAA */ Instructions.NOP.Microcode,
                /* 0xAB */ Exceptions.BadOpcode.Microcode,
                /* 0xAC */ Exceptions.BadOpcode.Microcode,
                /* 0xAD */ Exceptions.BadOpcode.Microcode,
                /* 0xAE */ Exceptions.BadOpcode.Microcode,
                /* 0xAF */ Exceptions.BadOpcode.Microcode,
                /* 0xB0 */ Exceptions.BadOpcode.Microcode,
                /* 0xB1 */ Exceptions.BadOpcode.Microcode,
                /* 0xB2 */ Exceptions.BadOpcode.Microcode,
                /* 0xB3 */ Exceptions.BadOpcode.Microcode,
                /* 0xB4 */ Exceptions.BadOpcode.Microcode,
                /* 0xB5 */ Exceptions.BadOpcode.Microcode,
                /* 0xB6 */ Exceptions.BadOpcode.Microcode,
                /* 0xB7 */ Exceptions.BadOpcode.Microcode,
                /* 0xB8 */ Exceptions.BadOpcode.Microcode,
                /* 0xB9 */ Exceptions.BadOpcode.Microcode,
                /* 0xBA */ Exceptions.BadOpcode.Microcode,
                /* 0xBB */ Exceptions.BadOpcode.Microcode,
                /* 0xBC */ Exceptions.BadOpcode.Microcode,
                /* 0xBD */ Exceptions.BadOpcode.Microcode,
                /* 0xBE */ Exceptions.BadOpcode.Microcode,
                /* 0xBF */ Exceptions.BadOpcode.Microcode,
                /* 0xC0 */ Exceptions.BadOpcode.Microcode,
                /* 0xC1 */ Instructions.LD_ImmAddr_Reg.Microcode,
                /* 0xC2 */ Exceptions.BadOpcode.Microcode,
                /* 0xC3 */ Instructions.ADD_ImmAddr_Reg.Microcode,
                /* 0xC4 */ Instructions.SUB_ImmAddr_Reg.Microcode,
                /* 0xC5 */ Instructions.MUL_ImmAddr_Reg.Microcode,
                /* 0xC6 */ Instructions.DIV_ImmAddr_Reg.Microcode,
                /* 0xC7 */ Instructions.MOD_ImmAddr_Reg.Microcode,
                /* 0xC8 */ Instructions.AND_ImmAddr_Reg.Microcode,
                /* 0xC9 */ Instructions.OR_ImmAddr_Reg.Microcode,
                /* 0xCA */ Instructions.NOR_ImmAddr_Reg.Microcode,
                /* 0xCB */ Instructions.NAND_ImmAddr_Reg.Microcode,
                /* 0xCC */ Instructions.XOR_ImmAddr_Reg.Microcode,
                /* 0xCD */ Instructions.SHL_ImmAddr_Reg.Microcode,
                /* 0xCE */ Instructions.SHR_ImmAddr_Reg.Microcode,
                /* 0xCF */ Instructions.CMP_ImmAddr_Reg.Microcode,
                /* 0xD0 */ Instructions.TEST_ImmAddr_Reg.Microcode,
                /* 0xD1 */ Exceptions.BadOpcode.Microcode,
                /* 0xD2 */ Exceptions.BadOpcode.Microcode,
                /* 0xD3 */ Exceptions.BadOpcode.Microcode,
                /* 0xD4 */ Exceptions.BadOpcode.Microcode,
                /* 0xD5 */ Exceptions.BadOpcode.Microcode,
                /* 0xD6 */ Instructions.JMP_ImmAddr.Microcode,
                /* 0xD7 */ Instructions.JZ_ImmAddr.Microcode,
                /* 0xD8 */ Instructions.JNZ_ImmAddr.Microcode,
                /* 0xD9 */ Exceptions.BadOpcode.Microcode,
                /* 0xDA */ Exceptions.BadOpcode.Microcode,
                /* 0xDB */ Exceptions.BadOpcode.Microcode,
                /* 0xDC */ Exceptions.BadOpcode.Microcode,
                /* 0xDD */ Exceptions.BadOpcode.Microcode,
                /* 0xDE */ Exceptions.BadOpcode.Microcode,
                /* 0xDF */ Exceptions.BadOpcode.Microcode,
                /* 0xE0 */ Exceptions.BadOpcode.Microcode,
                /* 0xE1 */ Exceptions.BadOpcode.Microcode,
                /* 0xE2 */ Exceptions.BadOpcode.Microcode,
                /* 0xE3 */ Exceptions.BadOpcode.Microcode,
                /* 0xE4 */ Exceptions.BadOpcode.Microcode,
                /* 0xE5 */ Exceptions.BadOpcode.Microcode,
                /* 0xE6 */ Exceptions.BadOpcode.Microcode,
                /* 0xE7 */ Exceptions.BadOpcode.Microcode,
                /* 0xE8 */ Exceptions.BadOpcode.Microcode,
                /* 0xE9 */ Exceptions.BadOpcode.Microcode,
                /* 0xEA */ Exceptions.BadOpcode.Microcode,
                /* 0xEB */ Exceptions.BadOpcode.Microcode,
                /* 0xEC */ Exceptions.BadOpcode.Microcode,
                /* 0xED */ Exceptions.BadOpcode.Microcode,
                /* 0xEE */ Exceptions.BadOpcode.Microcode,
                /* 0xEF */ Exceptions.BadOpcode.Microcode,
                /* 0xF0 */ Exceptions.BadOpcode.Microcode,
                /* 0xF1 */ Exceptions.BadOpcode.Microcode,
                /* 0xF2 */ Exceptions.BadOpcode.Microcode,
                /* 0xF3 */ Exceptions.BadOpcode.Microcode,
                /* 0xF4 */ Exceptions.BadOpcode.Microcode,
                /* 0xF5 */ Exceptions.BadOpcode.Microcode,
                /* 0xF6 */ Exceptions.BadOpcode.Microcode,
                /* 0xF7 */ Exceptions.BadOpcode.Microcode,
                /* 0xF8 */ Exceptions.BadOpcode.Microcode,
                /* 0xF9 */ Exceptions.BadOpcode.Microcode,
                /* 0xFA */ Exceptions.BadOpcode.Microcode,
                /* 0xFB */ Exceptions.BadOpcode.Microcode,
                /* 0xFC */ Exceptions.BadOpcode.Microcode,
                /* 0xFD */ Exceptions.BadOpcode.Microcode,
                /* 0xFE */ Exceptions.BadOpcode.Microcode,
                /* 0xFF */ Exceptions.BadOpcode.Microcode
            };

            MicrocodeMap = new Dictionary<byte, Action[]> {
                { 0x00, Instructions.HALT.Microcode },

                { 0x01, Instructions.LD_RegVal_Reg.Microcode },
                { 0x41, Instructions.LD_ImmVal_Reg.Microcode },
                { 0x81, Instructions.LD_RegAddr_Reg.Microcode },
                { 0xC1, Instructions.LD_ImmAddr_Reg.Microcode },

                { 0x02, Instructions.ST.Microcode },

                { 0x03, Instructions.ADD_RegVal_Reg.Microcode },
                { 0x43, Instructions.ADD_ImmVal_Reg.Microcode },
                { 0x83, Instructions.ADD_RegAddr_Reg.Microcode },
                { 0xC3, Instructions.ADD_ImmAddr_Reg.Microcode },

                { 0x04, Instructions.SUB_RegVal_Reg.Microcode },
                { 0x44, Instructions.SUB_ImmVal_Reg.Microcode },
                { 0x84, Instructions.SUB_RegAddr_Reg.Microcode },
                { 0xC4, Instructions.SUB_ImmAddr_Reg.Microcode },

                { 0x05, Instructions.MUL_RegVal_Reg.Microcode },
                { 0x45, Instructions.MUL_ImmVal_Reg.Microcode },
                { 0x85, Instructions.MUL_RegAddr_Reg.Microcode },
                { 0xC5, Instructions.MUL_ImmAddr_Reg.Microcode },

                { 0x06, Instructions.DIV_RegVal_Reg.Microcode },
                { 0x46, Instructions.DIV_ImmVal_Reg.Microcode },
                { 0x86, Instructions.DIV_RegAddr_Reg.Microcode },
                { 0xC6, Instructions.DIV_ImmAddr_Reg.Microcode },

                { 0x07, Instructions.MOD_RegVal_Reg.Microcode },
                { 0x47, Instructions.MOD_ImmVal_Reg.Microcode },
                { 0x87, Instructions.MOD_RegAddr_Reg.Microcode },
                { 0xC7, Instructions.MOD_ImmAddr_Reg.Microcode },

                { 0x08, Instructions.AND_RegVal_Reg.Microcode },
                { 0x48, Instructions.AND_ImmVal_Reg.Microcode },
                { 0x88, Instructions.AND_RegAddr_Reg.Microcode },
                { 0xC8, Instructions.AND_ImmAddr_Reg.Microcode },

                { 0x09, Instructions.OR_RegVal_Reg.Microcode },
                { 0x49, Instructions.OR_ImmVal_Reg.Microcode },
                { 0x89, Instructions.OR_RegAddr_Reg.Microcode },
                { 0xC9, Instructions.OR_ImmAddr_Reg.Microcode },

                { 0x0A, Instructions.NOR_RegVal_Reg.Microcode },
                { 0x4A, Instructions.NOR_ImmVal_Reg.Microcode },
                { 0x8A, Instructions.NOR_RegAddr_Reg.Microcode },
                { 0xCA, Instructions.NOR_ImmAddr_Reg.Microcode },

                { 0x0B, Instructions.NAND_RegVal_Reg.Microcode },
                { 0x4B, Instructions.NAND_ImmVal_Reg.Microcode },
                { 0x8B, Instructions.NAND_RegAddr_Reg.Microcode },
                { 0xCB, Instructions.NAND_ImmAddr_Reg.Microcode },

                { 0x0C, Instructions.XOR_RegVal_Reg.Microcode },
                { 0x4C, Instructions.XOR_ImmVal_Reg.Microcode },
                { 0x8C, Instructions.XOR_RegAddr_Reg.Microcode },
                { 0xCC, Instructions.XOR_ImmAddr_Reg.Microcode },

                { 0x0D, Instructions.SHL_RegVal_Reg.Microcode },
                { 0x4D, Instructions.SHL_ImmVal_Reg.Microcode },
                { 0x8D, Instructions.SHL_RegAddr_Reg.Microcode },
                { 0xCD, Instructions.SHL_ImmAddr_Reg.Microcode },

                { 0x0E, Instructions.SHR_RegVal_Reg.Microcode },
                { 0x4E, Instructions.SHR_ImmVal_Reg.Microcode },
                { 0x8E, Instructions.SHR_RegAddr_Reg.Microcode },
                { 0xCE, Instructions.SHR_ImmAddr_Reg.Microcode },

                { 0x0F, Instructions.CMP_RegVal_Reg.Microcode },
                { 0x4F, Instructions.CMP_ImmVal_Reg.Microcode },
                { 0x8F, Instructions.CMP_RegAddr_Reg.Microcode },
                { 0xCF, Instructions.CMP_ImmAddr_Reg.Microcode },

                { 0x10, Instructions.TEST_RegVal_Reg.Microcode },
                { 0x50, Instructions.TEST_ImmVal_Reg.Microcode },
                { 0x90, Instructions.TEST_RegAddr_Reg.Microcode },
                { 0xD0, Instructions.TEST_ImmAddr_Reg.Microcode },

                { 0x12, Instructions.INT_RegVal.Microcode },
                { 0x52, Instructions.INT_ImmVal.Microcode },

                { 0x13, Instructions.STIN.Microcode },

                { 0x14, Instructions.OUTR_RegVal_Reg.Microcode },
                { 0x54, Instructions.OUTR_ImmVal_Reg.Microcode },

                { 0x16, Instructions.JMP_RegVal.Microcode },
                { 0x56, Instructions.JMP_ImmVal.Microcode },
                { 0x96, Instructions.JMP_RegAddr.Microcode },
                { 0xD6, Instructions.JMP_ImmAddr.Microcode },

                { 0x17, Instructions.JZ_RegVal.Microcode },
                { 0x57, Instructions.JZ_ImmVal.Microcode },
                { 0x97, Instructions.JZ_RegAddr.Microcode },
                { 0xD7, Instructions.JZ_ImmAddr.Microcode },

                { 0x18, Instructions.JNZ_RegVal.Microcode },
                { 0x58, Instructions.JNZ_ImmVal.Microcode },
                { 0x98, Instructions.JNZ_RegAddr.Microcode },
                { 0xD8, Instructions.JNZ_ImmAddr.Microcode },

                { 0x1D, Instructions.CALL_RegVal.Microcode },
                { 0x5D, Instructions.CALL_ImmVal.Microcode },
                { 0x9D, Instructions.CALL_RegAddr.Microcode },

                { 0x1E, Instructions.OUT_RegVal_Imm.Microcode },

                { 0x1F, Instructions.IN_RegVal_Reg.Microcode },
                { 0x5F, Instructions.IN_ImmVal_Reg.Microcode },

                { 0x20, Instructions.PUSH_RegVal.Microcode },
                { 0x21, Instructions.PUSH_ImmVal.Microcode },

                { 0x22, Instructions.CLR.Microcode },

                { 0x23, Instructions.INC.Microcode },

                { 0x24, Instructions.DEC.Microcode },

                { 0x26, Instructions.POP_RegVal.Microcode },

                { 0x27, Instructions.RET.Microcode },

                { 0xAA, Instructions.NOP.Microcode }
            };
        }
    }

}
