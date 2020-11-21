using System;
using System.CodeDom;
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

        public virtual void Enable(BusTypes type, SubRegisters mask) {
            base.Enable(type);
            EnableSubRegisterMask = mask;
        }

        public virtual void Set(BusTypes type, SubRegisters mask) {
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

        public override void OnTick(ClockState state) {
            var enableMask = (UInt64)EnableSubRegisterMask;
            var enableOffset = OffsetMap[EnableSubRegisterMask];
            var setMask = (UInt64)SetSubRegisterMask;
            var setOffset = OffsetMap[SetSubRegisterMask];

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
                if (DataBusEnabled) {
                    DataBus.Value = (Value & enableMask) >> enableOffset;
                    DataBusEnabled = false;
                }

                if (AddressBusEnabled) {
                    AddressBus.Value = (Value & enableMask) >> enableOffset;
                    AddressBusEnabled = false;
                }

                if (IOBusEnabled) {
                    IOBus.Value = (Value & enableMask) >> enableOffset;
                    IOBusEnabled = false;
                }

                break;

            case ClockState.TickSet:
                var newVal = new UInt64();
                bool set = false;

                if (DataBusSet) {
                    newVal = (DataBus.Value << setOffset) & setMask;
                    set = true;
                    DataBusSet = false;
                }

                if (AddressBusSet) {
                    newVal = (AddressBus.Value << setOffset) & setMask;
                    set = true;
                    AddressBusSet = false;
                }

                if (IOBusSet) {
                    newVal = (IOBus.Value << setOffset) & setMask;
                    set = true;
                    IOBusSet = false;
                }

                if (set) {
                    var notMask = ~setMask;
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
            Clock?.Stop();
            Cpu?.PowerOff();
        }

        public void PowerOn() {
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
        public MaizeRegister PC => MB.Cpu.ProgramCounter;
        public MaizeRegister CS => MB.Cpu.CodeSegment;
        public MaizeRegister SP => MB.Cpu.StackPointer;
        public MaizeRegister BP => MB.Cpu.BasePointer;
        public MaizeAlu Alu => MB.Cpu.Alu;
        public Tortilla.IClock Clock => MB.Clock;

        protected virtual byte Step { 
            get { return Decoder.Step; }
            set { Decoder.Step = value; }
        }

        protected virtual byte Cycle => Decoder.Cycle;
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

        protected virtual bool IsAddress(byte operand) {
            return (operand & OpFlag_Addr) == OpFlag_Addr;
        }

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

        public const byte OpcodeFlag_SrcImm     = 0b_0100_0000;
        public const byte OpcodeFlag_SrcAddr    = 0b_1000_0000;

        public const byte OpFlag_Addr    = 0b_1000_0000;

        public const byte OpFlag_Reg     = 0b_0111_0000;
        public const byte OpFlag_RegA    = 0b_0000_0000;
        public const byte OpFlag_RegB    = 0b_0001_0000;
        public const byte OpFlag_RegC    = 0b_0010_0000;
        public const byte OpFlag_RegD    = 0b_0011_0000;
        public const byte OpFlag_RegE    = 0b_0100_0000;
        public const byte OpFlag_RegI    = 0b_0101_0000;
        public const byte OpFlag_RegS    = 0b_0110_0000;
        public const byte OpFlag_RegF    = 0b_0111_0000;

        public const byte OpFlag_RegSP   = 0b_0110_1100; // S.H0 = stack pointer
        public const byte OpFlag_RegBP   = 0b_0110_1101; // S.H1 = base pointer
        public const byte OpFlag_RegPC   = 0b_0100_1100; // E.H0 = program counter
        public const byte OpFlag_RegCS   = 0b_0100_1101; // E.H1 = code segment
        public const byte OpFlag_RegFL   = 0b_0111_1100; // F.H0 = flags

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

        public Microcode Code { get; set; }

        public virtual void BuildMicrocode() {
            Code = new Microcode { };
        }
    }

    public class InstructionBase<T> : MaizeInstruction where T : MaizeInstruction {
        static InstructionBase() { }

        private static readonly Lazy<T> _instance = new Lazy<T>(() => CreateInstance());

        protected InstructionBase() {
        }

        public static T Instance => _instance.Value;

        public static Microcode GetMicrocode() {
            return Instance.Code;
        }

        private static T CreateInstance() {
            T t = Activator.CreateInstance(typeof(T), true) as T;
            t.BuildMicrocode();
            return t;
        }
    }


    public class MaizeCpu : BusComponent, ICpu<UInt64> {
        public const byte Flag_CarryOut = 0b_0000_0001;
        public const byte Flag_Negative = 0b_0000_0010;
        public const byte Flag_Overflow = 0b_0000_0100;
        public const byte Flag_Parity = 0b_0000_1000;
        public const byte Flag_Zero = 0b_0001_0000;
        public const byte Flag_Sign = 0b_0010_0000;
        public const byte Flag_Reserved = 0b_0100_0000;
        public const byte Flag_Interrupt = 0b_1000_0000;

        public MaizeCpu(MaizeMotherboard motherboard) {
            MB = motherboard;

            ARegister.DataBus = MB.DataBus;
            ARegister.AddressBus = MB.AddressBus;
            ARegister.IOBus = MB.IOBus;
            MB.ConnectComponent(ARegister);

            BRegister.DataBus = MB.DataBus;
            BRegister.AddressBus = MB.AddressBus;
            BRegister.IOBus = MB.IOBus;
            MB.ConnectComponent(BRegister);

            CRegister.DataBus = MB.DataBus;
            CRegister.AddressBus = MB.AddressBus;
            CRegister.IOBus = MB.IOBus;
            MB.ConnectComponent(CRegister);

            DRegister.DataBus = MB.DataBus;
            DRegister.AddressBus = MB.AddressBus;
            DRegister.IOBus = MB.IOBus;
            MB.ConnectComponent(DRegister);

            ERegister.DataBus = MB.DataBus;
            ERegister.AddressBus = MB.AddressBus;
            ERegister.IOBus = MB.IOBus;
            MB.ConnectComponent(ERegister);

            FRegister.DataBus = MB.DataBus;
            FRegister.AddressBus = MB.AddressBus;
            FRegister.IOBus = MB.IOBus;
            MB.ConnectComponent(FRegister);

            SpRegister.DataBus = MB.DataBus;
            SpRegister.AddressBus = MB.AddressBus;
            SpRegister.IOBus = MB.IOBus;
            MB.ConnectComponent(SpRegister);

            // Mapped to E.H0
            ProgramCounter.MapReg = ERegister;
            ProgramCounter.DataBus = MB.DataBus;
            ProgramCounter.AddressBus = MB.AddressBus;
            ProgramCounter.IOBus = MB.IOBus;
            MB.ConnectComponent(ProgramCounter);

            // Mapped to E.H1
            CodeSegment.MapReg = ERegister;
            CodeSegment.DataBus = MB.DataBus;
            CodeSegment.AddressBus = MB.AddressBus;
            CodeSegment.IOBus = MB.IOBus;
            MB.ConnectComponent(CodeSegment);

            // Mapped to S.H0
            StackPointer.MapReg = SpRegister;
            StackPointer.DataBus = MB.DataBus;
            StackPointer.AddressBus = MB.AddressBus;
            StackPointer.IOBus = MB.IOBus;
            MB.ConnectComponent(StackPointer);

            // Mapped to S.H1
            BasePointer.MapReg = SpRegister;
            BasePointer.DataBus = MB.DataBus;
            BasePointer.AddressBus = MB.AddressBus;
            BasePointer.IOBus = MB.IOBus;
            MB.ConnectComponent(BasePointer);

            Decoder = new MaizeDecoder(MB);
            Decoder.InstructionRead += InstructionRegister_InstructionRead;
            Decoder.RegisterMap[MaizeInstruction.OpFlag_RegA] = ARegister;
            Decoder.RegisterMap[MaizeInstruction.OpFlag_RegB] = BRegister;
            Decoder.RegisterMap[MaizeInstruction.OpFlag_RegC] = CRegister;
            Decoder.RegisterMap[MaizeInstruction.OpFlag_RegD] = DRegister;
            Decoder.RegisterMap[MaizeInstruction.OpFlag_RegE] = ERegister;
            Decoder.RegisterMap[MaizeInstruction.OpFlag_RegI] = Decoder;
            Decoder.RegisterMap[MaizeInstruction.OpFlag_RegS] = SpRegister;
            Decoder.RegisterMap[MaizeInstruction.OpFlag_RegF] = FRegister;

            Alu = new MaizeAlu(MB, this);

            MB.ConnectComponent(this);
        }

        public byte Flags {
            get { return FRegister.B1; }
            set { FRegister.B1 = value; }
        }

        /*
        public bool CarryIn {
            get { return (byte)(this.B0 & OpCodeCtrl_CarryIn); }
            set { this.B0 = (byte)((this.B0 & ~OpCodeCtrl_CarryIn) | ((value & 0b_0000_0001) << (OpCodeCtrl_CarryIn / 4))); }
        }
        */

        public bool Negative {
            get { return (Flags & Flag_Negative) == Flag_Negative; }
            set { Flags = (byte)((Flags & ~Flag_Negative) | (value ? Flag_Negative : 0)); }
        }

        public bool Overflow {
            get { return (Flags & Flag_Overflow) == Flag_Overflow; }
            set { Flags = (byte)((Flags & ~Flag_Overflow) | (value ? Flag_Overflow : 0)); }
        }

        public bool Parity {
            get { return (Flags & Flag_Parity) == Flag_Parity; }
            set { Flags = (byte)((Flags & ~Flag_Parity) | (value ? Flag_Parity : 0)); }
        }

        public bool Zero {
            get { return (Flags & Flag_Zero) == Flag_Zero; }
            set { Flags = (byte)((Flags & ~Flag_Zero) | (value ? Flag_Zero : 0)); }
        }

        public bool CarryOut {
            get { return (Flags & Flag_CarryOut) == Flag_CarryOut; }
            set { Flags = (byte)((Flags & ~Flag_CarryOut) | (value ? Flag_CarryOut : 0)); }
        }

        public bool Interrupt {
            get { return (Flags & Flag_Interrupt) == Flag_Interrupt; }
            set { Flags = (byte)((Flags & ~Flag_Interrupt) | (value ? Flag_Interrupt : 0)); }
        }



        private void InstructionRegister_InstructionRead(object sender, Tuple<UInt64, UInt64> e) {
            DecodeInstruction?.Invoke(sender, e);
        }

        MaizeMotherboard MB { get; set; }

        public bool SingleStep { get; set; }

        public string RegisterDump {
            get {
                var regText = $"A={ARegister} B={BRegister} C={CRegister} D={DRegister} E={ERegister} F={FRegister} I={Decoder} S={SpRegister} Step=0x{Decoder.Step:X2} Cycle=0x{Decoder.Cycle:X2}";
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

        public MaizeRegister ARegister { get; protected set; } = new MaizeRegister();
        public MaizeRegister BRegister { get; protected set; } = new MaizeRegister();
        public MaizeRegister CRegister { get; protected set; } = new MaizeRegister();
        public MaizeRegister DRegister { get; protected set; } = new MaizeRegister();
        public MaizeRegister ERegister { get; protected set; } = new MaizeRegister();
        public MaizeRegister FRegister { get; protected set; } = new MaizeRegister();
        public MaizeRegister InRegister { get; protected set; } = new MaizeRegister();
        public MaizeRegister SpRegister { get; protected set; } = new MaizeRegister();
        public MappedRegisterH0 ProgramCounter { get; protected set; } = new MappedRegisterH0();
        public MappedRegisterH1 CodeSegment { get; protected set; } = new MappedRegisterH1();
        public MappedRegisterH0 StackPointer { get; protected set; } = new MappedRegisterH0();
        public MappedRegisterH1 BasePointer { get; protected set; } = new MappedRegisterH1();
     
        public event EventHandler<Tuple<UInt64, UInt64>> DecodeInstruction;

        public void Break() {
            MB.Clock.Stop();
            SingleStep = true;
        }

        public void Continue() {
            IsPowerOn = true;

            if (SingleStep) {
                MB.Clock.Stop();
                MB.Clock.Tick();
            }
            else {
                MB.Clock.Start();

                while (MB.Clock.IsRunning) {
                    MB.Clock.Tick();
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

        public int InterruptID { get; set; }

        public void RaiseInterrupt(int id) {
            // Set interrupt flag
            Interrupt = true;
            InterruptID = id;
            Continue();
        }

        public void Reset() {
            ProgramCounter.Value = 0;
            Decoder.Value = 0;
            IsHalted = false;
        }

        public override void OnTick(ClockState state) {
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

        public void JumpTo(Microcode microcode) {
            ActiveMicrocode = microcode;
            Step = 0;
            handler = ActiveMicrocode[Step];
            handler();
        }

        protected Dictionary<byte, Microcode> MicrocodeMap;
        protected Microcode ActiveMicrocode;

        public event EventHandler<Tuple<UInt64, UInt64>> InstructionRead;

        public override ulong Value { 
            get => base.Value;
            set {
                base.Value = value;
                InstructionRead?.Invoke(this, Tuple.Create(value, MB.Cpu.ProgramCounter.Value - 1));
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

        public byte Step { get; set; }
        public byte Cycle { get; protected set; }
        Action handler = null;

        public IDictionary<byte, MaizeRegister> RegisterMap = new System.Collections.Generic.Dictionary<byte, MaizeRegister>();

        public Microcode ReadOpcodeAndDispatch;
        public Microcode ReadImmediate1Byte;
        public Microcode ReadImmediate2Byte;
        public Microcode ReadImmediate4Byte;
        public Microcode ReadImmediate8Byte;

        public override void OnTick(ClockState state) {
            base.OnTick(state);

            switch (state) {
            case ClockState.TickDecode:
                if (Step >= ActiveMicrocode.Count) {
                    Cycle = 0;
                }

                if (Cycle == 0) {
                    Step = 0;
                    ActiveMicrocode = ReadOpcodeAndDispatch;

                    if (MB.Cpu.Interrupt) {
                        // Vector to interrupt handler for MB.Cpu.InterruptID
                        MB.Cpu.Interrupt = false;
                        
                        if (MB.Cpu.InterruptID == 0) {
                            UInt32 startAddress = MB.MemoryModule.ReadHalfWord(0);
                            MB.Cpu.ProgramCounter.Value = startAddress;
                        }
                        else {
                            UInt32 intAddress = (UInt32)(MB.Cpu.InterruptID * 4);
                            UInt32 startAddress = MB.MemoryModule.ReadHalfWord(intAddress);
                            MB.Cpu.ProgramCounter.Value = startAddress;
                        }
                    }
                }

                handler = ActiveMicrocode[Step];
                handler();
                MB.OnDebug();
                ++Cycle;
                ++Step;

                break;
            }
        }

        protected Stack<Microcode> MicrocodeStack = new Stack<Microcode>();
        protected Stack<byte> StepStack = new Stack<byte>();

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
            ActiveMicrocode = new Microcode {
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

            ReadOpcodeAndDispatch = new Microcode {
                () => {
                    MB.Cpu.ProgramCounter.Enable(BusTypes.AddressBus);
                    MB.MemoryModule.AddressRegister.Set(BusTypes.AddressBus);
                },
                () => {
                    MB.MemoryModule.DataRegister.Enable(BusTypes.DataBus);
                    this.Set(BusTypes.DataBus);
                    this.OperandRegister1.Set(BusTypes.DataBus);
                    MB.Cpu.ProgramCounter.Increment(1);
                },
                () => {
                    var opcode = this.B0;
                    JumpTo(MicrocodeMap[opcode]);
                },
            };

            ReadImmediate1Byte = new Microcode {
                () => {
                    MB.Cpu.ProgramCounter.Enable(BusTypes.AddressBus);
                    MB.MemoryModule.AddressRegister.Set(BusTypes.AddressBus);
                },
                () => {
                    MB.Cpu.ProgramCounter.Increment(1);
                    MB.MemoryModule.DataRegister.Enable(BusTypes.DataBus, SubRegisters.B0);
                    this.Set(BusTypes.DataBus);
                    PopMicrocodeStack();
                }
            };

            ReadImmediate2Byte = new Microcode {
                () => {
                    MB.Cpu.ProgramCounter.Enable(BusTypes.AddressBus);
                    MB.MemoryModule.AddressRegister.Set(BusTypes.AddressBus);
                },
                () => {
                    MB.Cpu.ProgramCounter.Increment(2);
                    MB.MemoryModule.DataRegister.Enable(BusTypes.DataBus, SubRegisters.Q0);
                    this.Set(BusTypes.DataBus);
                    PopMicrocodeStack();
                }
            };

            ReadImmediate4Byte = new Microcode {
                () => {
                    MB.Cpu.ProgramCounter.Enable(BusTypes.AddressBus);
                    MB.MemoryModule.AddressRegister.Set(BusTypes.AddressBus);
                },
                () => {
                    MB.Cpu.ProgramCounter.Increment(4);
                    MB.MemoryModule.DataRegister.Enable(BusTypes.DataBus, SubRegisters.H0);
                    this.Set(BusTypes.DataBus);
                    PopMicrocodeStack();
                }
            };

            ReadImmediate8Byte = new Microcode {
                () => {
                    MB.Cpu.ProgramCounter.Enable(BusTypes.AddressBus);
                    MB.MemoryModule.AddressRegister.Set(BusTypes.AddressBus);
                },
                () => {
                    MB.Cpu.ProgramCounter.Increment(8);
                    MB.MemoryModule.DataRegister.Enable(BusTypes.DataBus, SubRegisters.W0);
                    this.Set(BusTypes.DataBus);
                    PopMicrocodeStack();
                }
            };

            ActiveMicrocode = ReadOpcodeAndDispatch;

            MicrocodeMap = new Dictionary<byte, Microcode> {
                { 0x00, Instruction.HALT.GetMicrocode() },
                { 0x01, Instruction.LD_RegVal_Reg.GetMicrocode() },
                { 0x41, Instruction.LD_ImmVal_Reg.GetMicrocode() },
                { 0x81, Instruction.LD_RegAddr_Reg.GetMicrocode() },
                { 0xC1, Instruction.LD_ImmAddr_Reg.GetMicrocode() },
                { 0x02, Instruction.ST.GetMicrocode() },
                { 0x03, Instruction.ADD_RegVal_Reg.GetMicrocode() },
                { 0x43, Instruction.ADD_ImmVal_Reg.GetMicrocode() },
                { 0x83, Instruction.ADD_RegAddr_Reg.GetMicrocode() },
                { 0xC3, Instruction.ADD_ImmAddr_Reg.GetMicrocode() },
                { 0x04, Instruction.SUB_RegVal_Reg.GetMicrocode() },
                { 0x44, Instruction.SUB_ImmVal_Reg.GetMicrocode() },
                { 0x84, Instruction.SUB_RegAddr_Reg.GetMicrocode() },
                { 0xC4, Instruction.SUB_ImmAddr_Reg.GetMicrocode() },
                { 0x05, Instruction.MUL_RegVal_Reg.GetMicrocode() },
                { 0x45, Instruction.MUL_ImmVal_Reg.GetMicrocode() },
                { 0x85, Instruction.MUL_RegAddr_Reg.GetMicrocode() },
                { 0xC5, Instruction.MUL_ImmAddr_Reg.GetMicrocode() },
                { 0x06, Instruction.DIV_RegVal_Reg.GetMicrocode() },
                { 0x46, Instruction.DIV_ImmVal_Reg.GetMicrocode() },
                { 0x86, Instruction.DIV_RegAddr_Reg.GetMicrocode() },
                { 0xC6, Instruction.DIV_ImmAddr_Reg.GetMicrocode() },
                { 0x07, Instruction.MOD_RegVal_Reg.GetMicrocode() },
                { 0x47, Instruction.MOD_ImmVal_Reg.GetMicrocode() },
                { 0x87, Instruction.MOD_RegAddr_Reg.GetMicrocode() },
                { 0xC7, Instruction.MOD_ImmAddr_Reg.GetMicrocode() },
                { 0x13, Instruction.STI.GetMicrocode() },
                { 0x16, Instruction.JMP_RegVal.GetMicrocode() },
                { 0x56, Instruction.JMP_ImmVal.GetMicrocode() },
                { 0x1D, Instruction.CALL_RegVal.GetMicrocode() },
                { 0x5D, Instruction.CALL_ImmVal.GetMicrocode() },
                { 0x1E, Instruction.OUT_RegVal_Reg.GetMicrocode() },
                { 0x5E, Instruction.OUT_ImmVal_Reg.GetMicrocode() },
                { 0x1F, Instruction.IN_RegVal_Reg.GetMicrocode() },
                { 0x5F, Instruction.IN_ImmVal_Reg.GetMicrocode() },
                { 0x20, Instruction.PUSH_RegVal.GetMicrocode() },
                { 0x21, Instruction.PUSH_ImmVal.GetMicrocode() },
                { 0x22, Instruction.CLR.GetMicrocode() },
                { 0x23, Instruction.INC.GetMicrocode() },
                { 0x24, Instruction.DEC.GetMicrocode() },
                { 0x26, Instruction.POP_RegVal.GetMicrocode() },
                { 0x27, Instruction.RET.GetMicrocode() },
                { 0xAA, Instruction.NOP.GetMicrocode() }
            };
        }
    }

}
