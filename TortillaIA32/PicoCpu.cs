using System;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;

namespace Tortilla {
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
            get { return Value >> 3; }
            set { Value |= (value << 3); }
        }

        public int Offset {
            get { return (B0 & 0x07); }
            set { B0 = (byte)(value & 0x07); }
        }

        public static implicit operator RegValue(UInt64 v) => new RegValue(v);
        public static implicit operator RegValue(UInt32 v) => new RegValue(v);
        public static implicit operator RegValue(UInt16 v) => new RegValue(v);
        public static implicit operator RegValue(byte v) => new RegValue(v);
    }

    public class PicoRegister : Register<UInt64> {
        protected RegValue RegData;

        public override UInt64 Value {
            get { return RegData.Value; }
            set { RegData.Value = value; }
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
            WW = 0b_11111111_11111111_11111111_11111111_11111111_11111111_11111111_11111111,
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

        public SubRegisters SubRegisterMask { get; protected set; } = SubRegisters.WW;

        public override void Enable(BusTypes type) {
            base.Enable(type);
            SubRegisterMask = SubRegisters.WW;
        }

        public override void Set(BusTypes type) {
            base.Set(type);
            SubRegisterMask = SubRegisters.WW;
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
            switch (state) {
            case ClockState.TickEnable:
                if (DataBusEnabled) {
                    DataBus.Value = Value & (UInt64)SubRegisterMask;
                    DataBusEnabled = false;
                }

                if (AddressBusEnabled) {
                    AddressBus.Value = Value & (UInt64)SubRegisterMask;
                    AddressBusEnabled = false;
                }

                if (IOBusEnabled) {
                    IOBus.Value = Value & (UInt64)SubRegisterMask;
                    IOBusEnabled = false;
                }

                break;

            case ClockState.TickSet:
                if (DataBusSet) {
                    Value = DataBus.Value & (UInt64)SubRegisterMask;
                    DataBusSet = false;
                }

                if (AddressBusSet) {
                    Value = AddressBus.Value & (UInt64)SubRegisterMask;
                    AddressBusSet = false;
                }

                if (IOBusSet) {
                    Value = IOBus.Value & (UInt64)SubRegisterMask;
                    IOBusSet = false;
                }

                break;
            }
        }
    }

    public class PicoMotherboard : IMotherboard<UInt64> {
        public PicoMotherboard() {
            Clock = new Clock();
            DataBus = new Bus<UInt64>();
            AddressBus = new Bus<UInt64>();
            IOBus = new Bus<UInt64>();
            Cpu = new PicoCpu(this);
            ConnectComponent(Cpu);

            MemoryModule = new PicoMemoryModule(this);
        }

        public IDataBus<UInt64> DataBus { get; set; }
        public IDataBus<UInt64> AddressBus { get; protected set; }
        public IDataBus<UInt64> IOBus { get; set; }

        public IClock Clock { get; set; }

        public ICpu<UInt64> Cpu { get; set; }

        public PicoMemoryModule MemoryModule { get; set; }

        public UInt32 MemorySize { get { return MemoryModule.MemorySize; } }

        public event EventHandler<string> Debug;
        public event EventHandler PowerOff;
        public event EventHandler<byte> RaiseException;
        public event EventHandler<byte> RaiseInterrupt;

        public void OnDebug(string disasm) {
            Debug?.Invoke(this, disasm);
        }

        public void OnDebug() {
            Debug?.Invoke(this, null);
        }

        public void OnPowerOff() {
            PowerOff?.Invoke(this, null);
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

    public class PicoAlu : BusComponent {
        public PicoAlu(IMotherboard<UInt64> motherboard) {
            TempRegister.DataBus = motherboard.DataBus;
            motherboard.ConnectComponent(TempRegister);
            DataRegister.DataBus = motherboard.DataBus;
            motherboard.ConnectComponent(DataRegister);
            motherboard.ConnectComponent(this);
        }

        public PicoRegister TempRegister { get; set; } = new PicoRegister();
        public PicoRegister DataRegister { get; set; } = new PicoRegister();

        public override void OnTick(ClockState state) {
            switch (state) {
            case ClockState.TickOff:
                break;
            }
        }
    }

    public class PicoMemoryModule : BusComponent {

        public PicoMemoryModule(IMotherboard<UInt64> motherboard) {
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

        public PicoRegister AddressRegister { get; set; } = new PicoRegister();
        public PicoRegister DataRegister { get; set; } = new PicoRegister();

        protected UInt64 Value { get; set; }

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
                    switch (DataRegister.BusDataSize) {
                    case BusDataSizes.Byte:
                        WriteByte(AddressRegister.Value, DataRegister.B0);
                        break;

                    case BusDataSizes.QuarterWord:
                        WriteQuarterWord(AddressRegister.Value, DataRegister.Q0);
                        break;

                    case BusDataSizes.HalfWord:
                        WriteHalfWord(AddressRegister.Value, DataRegister.H0);
                        break;

                    case BusDataSizes.Word:
                        WriteWord(AddressRegister.Value, DataRegister.Value);
                        break;
                    }

                    DataBusEnabled = false;
                }

                if (DataBusSet) {
                    switch (DataRegister.BusDataSize) {
                    case BusDataSizes.Byte:
                    case BusDataSizes.QuarterWord:
                    case BusDataSizes.HalfWord:
                    case BusDataSizes.Word:
                        DataRegister.Value = ReadWord(AddressRegister.Value);
                        break;
                    }

                    DataBusSet = false;
                }
                break;
            }
        }

        UInt64[] Memory { get; set; } = new UInt64[0x3FFF];
        public UInt32 MemorySize { get { return (UInt32)Memory.Length; } }

        protected RegValue CacheAddress;
        protected RegValue CacheData;


        void SetCacheAddress(UInt64 address) {
            // UInt64 addrBase = (address & 0xFFFFFFFF_FFFFFFF8) >> 3;
            UInt64 addrBase = address >> 3;

            if (addrBase != CacheAddress.Base) {
                CacheData.Value = Memory[addrBase];
            }

            CacheAddress.Value = address;
        }

        public UInt64 ReadWord(UInt64 address) {
            SetCacheAddress(address);
            return CacheData.Value;
        }

        public void WriteWord(UInt64 address, UInt64 value) {
            SetCacheAddress(address);
            CacheData.Value = value;
            Memory[CacheAddress.Base] = value;
        }

        public UInt32 ReadHalfWord(UInt64 address) {
            SetCacheAddress(address);
            return (UInt32)((CacheData.Value >> (CacheAddress.Offset * 8)) & 0b_11111111_11111111);
        }

        public void WriteHalfWord(UInt64 address, UInt32 value) {
            SetCacheAddress(address);
            UInt64 temp = ((UInt64)value << (CacheAddress.Offset * 8));
            CacheData.Value |= temp;
            Memory[CacheAddress.Base] = CacheData.Value;
        }

        public UInt16 ReadQuarterWord(UInt64 address) {
            SetCacheAddress(address);
            return (UInt16)((CacheData.Value >> (CacheAddress.Offset * 8)) & 0b_11111111);
        }

        public void WriteQuarterWord(UInt64 address, UInt16 value) {
            SetCacheAddress(address);
            UInt64 temp = ((UInt64)value << (CacheAddress.Offset * 8));
            CacheData.Value |= temp;
            Memory[CacheAddress.Base] = CacheData.Value;
        }

        public byte ReadByte(UInt64 address) {
            SetCacheAddress(address);
            return CacheData[CacheAddress.Offset];
        }

        public void WriteByte(UInt64 address, byte value) {
            SetCacheAddress(address);
            CacheData[CacheAddress.Offset] = value;
            Memory[CacheAddress.Base] = CacheData.Value;
        }
    }

    public class PicoProgramCounter : PicoRegister {
        public PicoProgramCounter(IMotherboard<UInt64> motherboard) {
            AddressBus = motherboard.AddressBus;
            DataBus = motherboard.DataBus;
            IOBus = motherboard.IOBus;
            motherboard.ConnectComponent(this);
        }

        public bool Increment { get; set; } = false;

        public override void OnTick(ClockState state) {
            base.OnTick(state);

            switch (state) {
            case ClockState.TickOff:
                if (Increment) {
                    ++Value;
                    Increment = false;
                }
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


    public class PicoInstructionRegister : PicoRegister {
        public PicoInstructionRegister(PicoMotherboard _motherboard, PicoCpu cpu) {
            Motherboard = _motherboard;
            DataBus = Motherboard.DataBus;
            AddressBus = Motherboard.AddressBus;
            IOBus = Motherboard.IOBus;

            OperandRegister1.AddressBus = Motherboard.AddressBus;
            OperandRegister1.DataBus = Motherboard.DataBus;
            OperandRegister1.IOBus = Motherboard.IOBus;
            Motherboard.ConnectComponent(OperandRegister1);

            CacheAddress.AddressBus = Motherboard.AddressBus;
            CacheAddress.DataBus = Motherboard.DataBus;
            CacheAddress.IOBus = Motherboard.IOBus;
            Motherboard.ConnectComponent(CacheAddress);

            CacheData.AddressBus = Motherboard.AddressBus;
            CacheData.DataBus = Motherboard.DataBus;
            CacheData.IOBus = Motherboard.IOBus;
            Motherboard.ConnectComponent(CacheData);

            TempData.AddressBus = Motherboard.AddressBus;
            TempData.DataBus = Motherboard.DataBus;
            TempData.IOBus = Motherboard.IOBus;
            Motherboard.ConnectComponent(TempData);

            Motherboard.ConnectComponent(this);
            Cpu = cpu;

            ConnectOpCodesToMethods();

            CacheAddress.Value = TempData.Value = 0xFFFFFFFF_FFFFFFF8;
        }

        PicoRegister OperandRegister1 { get; set; } = new PicoRegister();
        PicoRegister TempData { get; set; } = new PicoRegister();
        PicoRegister CacheAddress { get; set; } = new PicoRegister();
        PicoRegister CacheData { get; set; } = new PicoRegister();

        protected delegate void OpCodeDelegate(byte opcode, byte flags);

        protected OpCodeDelegate[] OpCodeMap { get; } = new OpCodeDelegate[256];

        protected virtual void ConnectOpCodesToMethods() {
            System.Reflection.MethodInfo[] methods = this.GetType().GetMethods(
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.IgnoreCase |
                System.Reflection.BindingFlags.InvokeMethod |
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic);

            foreach (System.Reflection.MethodInfo method in methods) {
                object[] attrs = method.GetCustomAttributes(typeof(OpCodeAttribute), true);

                if (attrs != null) {
                    foreach (System.Attribute attr in attrs) {
                        OpCodeAttribute methodAttr = (OpCodeAttribute)attr;

                        if (methodAttr.OpCodes != null) {
                            foreach (byte opCode in methodAttr.OpCodes) {
                                if (OpCodeMap[opCode] != null) {
                                    throw new Exception($"Duplicate handler for opcode {opCode:X2}");
                                }

                                OpCodeMap[opCode] = (OpCodeDelegate)OpCodeDelegate.CreateDelegate(typeof(OpCodeDelegate), this, method);
                            }
                        }
                    }
                }
            }
        }

        PicoMotherboard Motherboard { get; set; }
        PicoCpu Cpu { get; set; }
        byte opcode = 0;
        byte flags = 0;
        public byte Step { get; protected set; }
        public byte Cycle { get; protected set; }
        OpCodeDelegate handler = null;

        public override void OnTick(ClockState state) {
            base.OnTick(state);

            switch (state) {
            case ClockState.TickDecode:
                DecodeInstruction();
                Motherboard.OnDebug();
                break;
            }
        }

        private void DecodeInstruction() {
            if (Step == 0) {
                Cycle = 0;
                handler = null;
                this.Value = 0;
            }

            ++Cycle;
            ++Step;

            if (Step == 1) {
                Cpu.ProgramCounter.Enable(BusTypes.AddressBus);
                Motherboard.MemoryModule.AddressRegister.Set(BusTypes.AddressBus);
                TempData.Set(BusTypes.AddressBus);
                Cpu.ProgramCounter.Increment = true;
                return;
            }

            if (Step == 2) {
                if (TempData.Base == CacheAddress.Base) {
                    CacheAddress.Value = TempData.Value;
                    ++Step;
                    ++Step;
                }
                else {
                    CacheAddress.Value = TempData.Value;
                    Motherboard.MemoryModule.DataRegister.Enable(BusTypes.DataBus);
                    TempData.Set(BusTypes.DataBus);
                    return;
                }
            }

            if (Step == 3) {
                CacheData.Value = TempData.Value;
                ++Step;
            }

            if (Step == 4) { 
                byte src = CacheData[CacheAddress.Offset];
                B0 = src;
                flags = (byte)(src & 0b_0000_0111);
                opcode = (byte)(src >> 2);
                handler = OpCodeMap[opcode];
                ++Step;
            }

            if (Step > 4) {
                handler(opcode, flags);
            }
        }

        [OpCode(0x00)]
        void HLT(byte opcode, byte flags) {
            Step = 0;
            Motherboard.Clock.Stop();
        }

        [OpCode(0x01,0x11)]
        void NOP(byte opcode, byte flags) {
            Step = 0;
        }

        void ReadOperandByte() {
            if (Step == 5) {
                OperandRegister1.Value = 0;
                Cpu.ProgramCounter.Enable(BusTypes.AddressBus);
                Motherboard.MemoryModule.AddressRegister.Set(BusTypes.AddressBus);
                TempData.Set(BusTypes.AddressBus);
                Cpu.ProgramCounter.Increment = true;
                return;
            }

            if (Step == 6) {
                if (TempData.Base == CacheAddress.Base) {
                    CacheAddress.Value = TempData.Value;
                    ++Step;
                    ++Step;
                }
                else {
                    CacheAddress.Value = TempData.Value;
                    Motherboard.MemoryModule.DataRegister.Enable(BusTypes.DataBus);
                    TempData.Set(BusTypes.DataBus);
                    return;
                }
            }

            if (Step == 7) {
                CacheData.Value = TempData.Value;
                ++Step;
                // Fall through
            }

            if (Step == 8) {
                OperandRegister1.B0 = CacheData[CacheAddress.Offset];
                this.B1 = OperandRegister1.B0;
                ++Step;
                // Fall through
            }
        }

        void ReadOperandQuarterWord() {
            ReadOperandByte();

            if (Step == 9) {
                Cpu.ProgramCounter.Enable(BusTypes.AddressBus);
                Motherboard.MemoryModule.AddressRegister.Set(BusTypes.AddressBus);
                TempData.Set(BusTypes.AddressBus);
                Cpu.ProgramCounter.Increment = true;
                return;
            }

            if (Step == 10) {
                if (TempData.Base == CacheAddress.Base) {
                    CacheAddress.Value = TempData.Value;
                    ++Step;
                    ++Step;
                }
                else {
                    CacheAddress.Value = TempData.Value;
                    Motherboard.MemoryModule.DataRegister.Enable(BusTypes.DataBus);
                    TempData.Set(BusTypes.DataBus);
                    return;
                }
            }

            if (Step == 11) {
                CacheData.Value = TempData.Value;
                ++Step;
                // Fall through
            }

            if (Step == 12) {
                OperandRegister1.B1 = CacheData[CacheAddress.Offset];
                this.B2 = OperandRegister1.B1;
                ++Step;
                // Fall through
            }
        }

        public byte InsFlagImmediateOpr = 0b_0000_0100;
        public byte InsFlagImmediate08b = 0b_0000_0100;
        public byte InsFlagImmediate16b = 0b_0000_0101;
        public byte InsFlagImmediate32b = 0b_0000_0110;
        public byte InsFlagImmediate64b = 0b_0000_0111;

        [OpCode(0x02)]
        void LD(byte opcode, byte flags) {

            if (flags == InsFlagImmediate08b) {
                ReadOperandByte();
            }

            Step = 0;
        }

        [OpCode(0x03)]
        void LD_Imm(byte operand, byte flags) {

            if (flags == InsFlagImmediate08b) {
                ReadOperandByte();

                if (Step == 9) {
                    OperandRegister1.Enable(BusTypes.DataBus);
                    Cpu.ARegister.Set(BusTypes.DataBus);
                    Step = 0;
                }

                return;
            }

            if (flags == InsFlagImmediate16b) {
                ReadOperandQuarterWord();

                if (Step == 13) {
                    OperandRegister1.Enable(BusTypes.DataBus);
                    Cpu.ARegister.Set(BusTypes.DataBus);
                    Step = 0;
                }
                return;
            }

            if (flags == InsFlagImmediate32b) {
                // ReadOperandHalfWord();
                Step = 0;
                return;
            }

            if (flags == InsFlagImmediate64b) {
                // ReadOperandWord();
                Step = 0;
                return;
            }

        }

        [OpCode(0x04)]
        void ST(byte opcode, byte flags) {
            Step = 0;
        }

        [OpCode(0x05)]
        void ST_Imm(byte opcode, byte flags) {
            if (flags == InsFlagImmediate08b) {
                ReadOperandByte();

                if (Step == 9) {
                    OperandRegister1.Enable(BusTypes.AddressBus);
                    Cpu.ARegister.Enable(BusTypes.DataBus, BusDataSizes.Byte);
                    Motherboard.MemoryModule.AddressRegister.Set(BusTypes.AddressBus);
                    Motherboard.MemoryModule.DataRegister.Set(BusTypes.DataBus, BusDataSizes.Byte);
                    Step = 0;
                    return;
                }

                return;
            }

            if (flags == InsFlagImmediate16b) {
                ReadOperandQuarterWord();

                if (Step == 13) {
                    // TODO: fault if address mod 2 != 0
                    OperandRegister1.Enable(BusTypes.AddressBus);
                    // TempData.Value = OperandRegister1.Value;
                    // Cpu.ARegister.Value <<= (TempData.Offset * 8);
                    Cpu.ARegister.Enable(BusTypes.DataBus, BusDataSizes.QuarterWord);
                    Motherboard.MemoryModule.AddressRegister.Set(BusTypes.AddressBus);
                    Motherboard.MemoryModule.DataRegister.Set(BusTypes.DataBus, BusDataSizes.QuarterWord);
                    Step = 0;
                    return;
                }

                return;
            }

            if (flags == InsFlagImmediate32b) {
                // ReadOperandByte3();
                Step = 0;
                return;
            }

            if (flags == InsFlagImmediate64b) {
                // ReadOperandByte4();
                Step = 0;
                return;
            }

        }

        [OpCode(0x06)]
        void CAL(byte opcode, byte flags) {
            NOP(opcode, flags);
        }

        [OpCode(0x07)]
        void CAL_Imm(byte opcode, byte flags) {
            NOP(opcode, flags);
        }

        [OpCode(0x08)]
        void ADD(byte opcode, byte flags) {
            NOP(opcode, flags);
        }

        [OpCode(0x09)]
        void ADD_Imm(byte opcode, byte flags) {
            NOP(opcode, flags);
            /*
            ReadOperandByte1();

            switch (Step) {
            case 5:
                UInt64 tmp = Value;
                Value = (tmp + Cpu.ARegister.Value);

                this.Enable(BusTypes.DataBus);
                Cpu.ARegister.Set(BusTypes.DataBus);

                Step = 0;
                break;
            }
            */
        }

        [OpCode(0x0A)]
        void SUB(byte opcode, byte flags) {
            NOP(opcode, flags);
        }

        [OpCode(0x0B)]
        void SUB_Imm(byte opcode, byte flags) {
            NOP(opcode, flags);
        }


        [OpCode(0x0C)]
        void MUL(byte opcode, byte flags) {
            NOP(opcode, flags);
        }

        [OpCode(0x0D)]
        void MUL_Imm(byte opcode, byte flags) {
            NOP(opcode, flags);
        }

        [OpCode(0x0E)]
        void DIV(byte opcode, byte flags) {
            NOP(opcode, flags);
        }

        [OpCode(0x0F)]
        void DIV_Imm(byte opcode, byte flags) {
            NOP(opcode, flags);
        }

        [OpCode(0x10)]
        void INC(byte opcode, byte flags) {
            NOP(opcode, flags);
        }

        [OpCode(0x12)]
        void DEC(byte opcode, byte flags) {
            NOP(opcode, flags);
        }

        [OpCode(0x13)]
        void DEC_Imm(byte opcode, byte flags) {
            NOP(opcode, flags);
        }

        [OpCode(0x14)]
        void AND(byte opcode, byte flags) {
            NOP(opcode, flags);
        }

        [OpCode(0x15)]
        void AND_Imm(byte opcode, byte flags) {
            NOP(opcode, flags);
        }


        [OpCode(0x16)]
        void OR(byte opcode, byte flags) {
            NOP(opcode, flags);
        }

        [OpCode(0x17)]
        void OR_Imm(byte opcode, byte flags) {
            NOP(opcode, flags);
        }

        [OpCode(0x18)]
        void NOR(byte opcode, byte flags) {
            NOP(opcode, flags);
        }

        [OpCode(0x19)]
        void NOR_Imm(byte opcode, byte flags) {
            NOP(opcode, flags);
        }


        [OpCode(0x1A)]
        void NOT(byte opcode, byte flags) {
            NOP(opcode, flags);
        }

        [OpCode(0x1B)]
        void NOT_Imm(byte opcode, byte flags) {
            NOP(opcode, flags);
        }

        [OpCode(0x1C)]
        void SHL(byte opcode, byte flags) {
            NOP(opcode, flags);
        }

        [OpCode(0x1D)]
        void SHL_Imm(byte opcode, byte flags) {
            NOP(opcode, flags);
        }

        [OpCode(0x1E)]
        void SHR(byte opcode, byte flags) {
            NOP(opcode, flags);
        }

        [OpCode(0x1F)]
        void SHR_Imm(byte opcode, byte flags) {
            NOP(opcode, flags);
        }

        [OpCode(0x20)]
        void CMP(byte opcode, byte flags) {
            NOP(opcode, flags);
        }

        [OpCode(0x21)]
        void CMP_Imm(byte opcode, byte flags) {
            NOP(opcode, flags);
        }

        [OpCode(0x22)]
        void JMP(byte opcode, byte flags) {
            NOP(opcode, flags);
        }

        [OpCode(0x23)]
        void JMP_Imm(byte opcode, byte flags) {
            NOP(opcode, flags);
        }

        [OpCode(0x24)]
        void JL(byte opcode, byte flags) {
            NOP(opcode, flags);
        }

        [OpCode(0x25)]
        void JL_Imm(byte opcode, byte flags) {
            NOP(opcode, flags);
        }

        [OpCode(0x26)]
        void JLE(byte opcode, byte flags) {
            NOP(opcode, flags);
        }

        [OpCode(0x27)]
        void JLE_Imm(byte opcode, byte flags) {
            NOP(opcode, flags);
        }

        [OpCode(0x28)]
        void JG(byte opcode, byte flags) {
            NOP(opcode, flags);
        }

        [OpCode(0x29)]
        void JG_Imm(byte opcode, byte flags) {
            NOP(opcode, flags);
        }

        [OpCode(0x2A)]
        void JGE(byte opcode, byte flags) {
            NOP(opcode, flags);
        }

        [OpCode(0x2B)]
        void JGE_Imm(byte opcode, byte flags) {
            NOP(opcode, flags);
        }

        [OpCode(0x2C)]
        void JZ(byte opcode, byte flags) {
            NOP(opcode, flags);
        }

        [OpCode(0x2D)]
        void JZ_Imm(byte opcode, byte flags) {
            NOP(opcode, flags);
        }

        [OpCode(0x2E)]
        void JNZ(byte opcode, byte flags) {
            NOP(opcode, flags);
        }

        [OpCode(0x2F)]
        void JNZ_Imm(byte opcode, byte flags) {
            NOP(opcode, flags);
        }

        [OpCode(0x30)]
        void OUT(byte opcode, byte flags) {
            NOP(opcode, flags);
        }

        [OpCode(0x31)]
        void OUT_Imm(byte opcode, byte flags) {
            NOP(opcode, flags);
        }

        [OpCode(0x32)]
        void IN(byte opcode, byte flags) {
            NOP(opcode, flags);
        }

        [OpCode(0x33)]
        void IN_Imm(byte opcode, byte flags) {
            NOP(opcode, flags);
        }

        [OpCode(0x34)]
        void PUSH(byte opcode, byte flags) {
            NOP(opcode, flags);
        }

        [OpCode(0x35)]
        void PUSH_Imm(byte opcode, byte flags) {
            NOP(opcode, flags);
        }

        [OpCode(0x36)]
        void POP(byte opcode, byte flags) {
            NOP(opcode, flags);
        }

        [OpCode(0x37)]
        void POP_Imm(byte opcode, byte flags) {
            NOP(opcode, flags);
        }




    }

    public class PicoCpu : BusComponent, ICpu<UInt64> {

        public PicoCpu(PicoMotherboard motherboard) {
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

            ProgramCounter = new PicoProgramCounter(Motherboard);

            InstructionRegister = new PicoInstructionRegister(Motherboard, this);

            Motherboard.ConnectComponent(this);
        }

        PicoMotherboard Motherboard { get; set; }

        public bool SingleStep { get; set; }

        public string RegisterDump {
            get {
                var regText = $"PC = {ProgramCounter.Value:X8} IR = {InstructionRegister.Value:X8} Step = {InstructionRegister.Step:X8} Cycle = {InstructionRegister.Cycle:X8} A = {ARegister.Value:X8} B = {BRegister.Value:X8} ";
                return regText;
            }
        }

        public bool IsPowerOn { get; set; }

        public bool IsHalted { get; set; }

        public PicoProgramCounter ProgramCounter { get; protected set; }

        public PicoInstructionRegister InstructionRegister { get; protected set; }

        public PicoRegister ARegister { get; protected set; } = new PicoRegister();

        public PicoRegister BRegister { get; protected set; } = new PicoRegister();

        public PicoRegister CRegister { get; protected set; } = new PicoRegister();

        public PicoRegister DRegister { get; protected set; } = new PicoRegister();

        public PicoRegister ERegister { get; protected set; } = new PicoRegister();

        public PicoRegister FRegister { get; protected set; } = new PicoRegister();

        public PicoRegister InRegister { get; protected set; } = new PicoRegister();

        public PicoRegister SpRegister { get; protected set; } = new PicoRegister();

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
            Motherboard.Clock.Stop();
            IsPowerOn = false;
            Motherboard.OnDebug("Off");
        }

        public void PowerOn() {
            Motherboard.OnDebug("On");
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
