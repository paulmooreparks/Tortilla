using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Tortilla;

namespace Maize {
    public enum SubRegisterMask : UInt64 {
        B0 = 0b_00000000_00000000_00000000_00000000_00000000_00000000_00000000_11111111,
        B1 = 0b_00000000_00000000_00000000_00000000_00000000_00000000_11111111_00000000,
        B2 = 0b_00000000_00000000_00000000_00000000_00000000_11111111_00000000_00000000,
        B3 = 0b_00000000_00000000_00000000_00000000_11111111_00000000_00000000_00000000,
        B4 = 0b_00000000_00000000_00000000_11111111_00000000_00000000_00000000_00000000,
        B5 = 0b_00000000_00000000_11111111_00000000_00000000_00000000_00000000_00000000,
        B6 = 0b_00000000_11111111_00000000_00000000_00000000_00000000_00000000_00000000,
        B7 = 0b_11111111_00000000_00000000_00000000_00000000_00000000_00000000_00000000,
        Q0 = 0b_00000000_00000000_00000000_00000000_00000000_00000000_11111111_11111111,
        Q1 = 0b_00000000_00000000_00000000_00000000_11111111_11111111_00000000_00000000,
        Q2 = 0b_00000000_00000000_11111111_11111111_00000000_00000000_00000000_00000000,
        Q3 = 0b_11111111_11111111_00000000_00000000_00000000_00000000_00000000_00000000,
        H0 = 0b_00000000_00000000_00000000_00000000_11111111_11111111_11111111_11111111,
        H1 = 0b_11111111_11111111_11111111_11111111_00000000_00000000_00000000_00000000,
        W0 = 0b_11111111_11111111_11111111_11111111_11111111_11111111_11111111_11111111,
    }

    public class Bus<DataType> : Tortilla.IDataBus<DataType> {
        public DataType Value { get; set; }
        public override string ToString() {
            return $"0x{Value:X16}";
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct RegValue {
        public RegValue(UInt64 init) : this() {
            W0 = init;
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

        [FieldOffset(0)] public UInt64 W0;
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
                return (byte)(W0 >> shift);
            }
            set {
                int shift = i * 8;
                UInt64 newValue = value;
                newValue <<= shift;
                W0 |= newValue;
            }
        }

        public static implicit operator RegValue(UInt64 v) => new RegValue(v);
        public static implicit operator RegValue(UInt32 v) => new RegValue(v);
        public static implicit operator RegValue(UInt16 v) => new RegValue(v);
        public static implicit operator RegValue(byte v) => new RegValue(v);

        public override string ToString() {
            return $"0x{W0:X16}";
        }
    }

    public class MaizeRegister : Tortilla.IBusComponent {
        public event Action<IBusComponent> RequestTickExecute;
        public event Action<IBusComponent> RequestTickUpdate;
        public event Action<IBusComponent> RequestTickEnableToAddressBus;
        public event Action<IBusComponent> RequestTickEnableToDataBus;
        public event Action<IBusComponent> RequestTickEnableToIOBus;
        public event Action<IBusComponent> RequestTickSetFromAddressBus;
        public event Action<IBusComponent> RequestTickSetFromDataBus;
        public event Action<IBusComponent> RequestTickSetFromIOBus;
        public event Action<IBusComponent> OnRegisterTickStore;
        public event Action<IBusComponent> OnRegisterTickLoad;

        public void RegisterTickExecute() {
            RequestTickExecute?.Invoke(this);
        }

        protected void RegisterTickUpdate() {
            RequestTickUpdate?.Invoke(this);
        }

        protected void RegisterTickEnableToAddressBus() {
            RequestTickEnableToAddressBus?.Invoke(this);
        }

        protected void RegisterTickEnableToDataBus() {
            RequestTickEnableToDataBus?.Invoke(this);
        }

        protected void RegisterTickEnableToIOBus() {
            RequestTickEnableToIOBus?.Invoke(this);
        }

        protected void RegisterTickSetFromAddressBus() {
            RequestTickSetFromAddressBus?.Invoke(this);
        }

        protected void RegisterTickSetFromDataBus() {
            RequestTickSetFromDataBus?.Invoke(this);
        }

        protected void RegisterTickSetFromIOBus() {
            RequestTickSetFromIOBus?.Invoke(this);
        }

        protected void RegisterTickStore() {
            OnRegisterTickStore?.Invoke(this);
        }

        protected void RegisterTickLoad() {
            OnRegisterTickLoad?.Invoke(this);
        }

        public RegValue RegData;

        public UInt64 PrivilegeFlags = 0;
        public UInt64 PrivilegeMask = 0;
        public IDataBus<UInt64> DataBus = null;
        public IDataBus<UInt64> AddressBus = null;
        public IDataBus<UInt64> IOBus = null;

        protected int[] OffsetMap = new int[] {
            0,
            8,
            16,
            24,
            32,
            40,
            48,
            56,
            0,
            16,
            32,
            48,
            0,
            32,
            0
        };

        public SubRegister SetSubRegister = SubRegister.W0;
        public int SetOffset = 0;
        public SubRegisterMask SetSubRegisterMask = SubRegisterMask.W0;

        public SubRegister EnableSubRegister = SubRegister.W0;
        public int EnableOffset = 0;
        public SubRegisterMask EnableSubRegisterMask = SubRegisterMask.W0;

        public void EnableToAddressBus(SubRegister subReg) {
            EnableSubRegister = subReg;
            EnableSubRegisterMask = MaizeInstruction.SubRegisterMaskMap[(int)EnableSubRegister];
            EnableOffset = OffsetMap[(int)EnableSubRegister];
            RequestTickEnableToAddressBus?.Invoke(this);
        }

        public void EnableToDataBus(SubRegister subReg) {
            EnableSubRegister = subReg;
            EnableSubRegisterMask = MaizeInstruction.SubRegisterMaskMap[(int)EnableSubRegister];
            EnableOffset = OffsetMap[(int)EnableSubRegister];
            RequestTickEnableToDataBus?.Invoke(this);
        }

        public void EnableToIOBus(SubRegister subReg) {
            EnableSubRegister = subReg;
            EnableSubRegisterMask = MaizeInstruction.SubRegisterMaskMap[(int)EnableSubRegister];
            EnableOffset = OffsetMap[(int)EnableSubRegister];
            RequestTickEnableToIOBus?.Invoke(this);
        }


        public void SetFromAddressBus(SubRegister subReg) {
            SetSubRegister = subReg;
            SetSubRegisterMask = MaizeInstruction.SubRegisterMaskMap[(int)SetSubRegister];
            SetOffset = OffsetMap[(int)SetSubRegister];
            RequestTickSetFromAddressBus?.Invoke(this);
        }

        public void SetFromDataBus(SubRegister subReg) {
            SetSubRegister = subReg;
            SetSubRegisterMask = MaizeInstruction.SubRegisterMaskMap[(int)SetSubRegister];
            SetOffset = OffsetMap[(int)SetSubRegister];
            RequestTickSetFromDataBus?.Invoke(this);
        }

        public void SetFromIOBus(SubRegister subReg) {
            SetSubRegister = subReg;
            SetSubRegisterMask = MaizeInstruction.SubRegisterMaskMap[(int)SetSubRegister];
            SetOffset = OffsetMap[(int)SetSubRegister];
            RequestTickSetFromIOBus?.Invoke(this);
        }


        protected UInt64 IncrementCount = 0;
        protected UInt64 DecrementCount = 0;

        public void Increment() {
            IncrementCount += 1;
            RequestTickUpdate?.Invoke(this);
        }

        public void Increment(Int64 count) {
            IncrementCount += (UInt64)count;
            RequestTickUpdate?.Invoke(this);
        }

        public void Decrement() {
            DecrementCount += 1;
            RequestTickUpdate?.Invoke(this);
        }

        public void Decrement(Int64 count) {
            DecrementCount += (UInt64)count;
            RequestTickUpdate?.Invoke(this);
        }

        public void PrivilegeCheck(IBusComponent cpuFlags) {
            if ((PrivilegeMask & (UInt64)SetSubRegisterMask) != 0) {
                var regFlags = cpuFlags as MaizeRegister;
                if ((PrivilegeFlags & regFlags.RegData.W0) != PrivilegeFlags) {
                    throw new Exception("Privilege exception");
                }
            }
        }

        public virtual void OnTickUpdate(IBusComponent cpuFlags) {
            RegData.W0 += IncrementCount;
            IncrementCount = 0;
            RegData.W0 -= DecrementCount;
            DecrementCount = 0;
        }

        public virtual void OnTickEnableToAddressBus(IBusComponent cpuFlags) {
            AddressBus.Value = (RegData.W0 & (UInt64)EnableSubRegisterMask) >> EnableOffset;
        }

        public virtual void OnTickEnableToDataBus(IBusComponent cpuFlags) {
            DataBus.Value = (RegData.W0 & (UInt64)EnableSubRegisterMask) >> EnableOffset;
        }

        public virtual void OnTickEnableToIOBus(IBusComponent cpuFlags) {
            IOBus.Value = (RegData.W0 & (UInt64)EnableSubRegisterMask) >> EnableOffset;
        }

        public virtual void OnTickSetFromAddressBus(IBusComponent cpuFlags) {
            PrivilegeCheck(cpuFlags);
            RegData.W0 = (~(UInt64)SetSubRegisterMask & RegData.W0) | (AddressBus.Value << SetOffset) & (UInt64)SetSubRegisterMask;
        }

        public virtual void OnTickSetFromDataBus(IBusComponent cpuFlags) {
            PrivilegeCheck(cpuFlags);
            RegData.W0 = (~(UInt64)SetSubRegisterMask & RegData.W0) | (DataBus.Value << SetOffset) & (UInt64)SetSubRegisterMask;
        }

        public virtual void OnTickSetFromIOBus(IBusComponent cpuFlags) {
            PrivilegeCheck(cpuFlags);
            RegData.W0 = (~(UInt64)SetSubRegisterMask & RegData.W0) | (IOBus.Value << SetOffset) & (UInt64)SetSubRegisterMask;
        }

        public virtual void OnTickLoad(IBusComponent cpuFlags) {
        }

        public virtual void OnTickStore(IBusComponent cpuFlags) {
        }

        public virtual void OnTickExecute(IBusComponent cpuFlags) {
        }

        public override string ToString() {
            return $"0x{RegData.W0:X16}";
        }
    }

    public class Clock : IClock {
        public Clock() {
        }

        public bool IsRunning { get; protected set; } = false;

        public void Start() {
            IsRunning = true;
        }

        public void Initialize() {
            IBusComponent[] tmp = new IBusComponent[compNextIndex];
            TickEvents = new Action<IBusComponent>[compNextIndex];

            for (var i = 0; i < compNextIndex; ++i) {
                var component = compList[i];
                tmp[i] = compList[i];
            }

            compList = tmp;
        }

        public void RegisterTickExecute(IBusComponent comp) {
            TickEvents[TickEventsCount] = comp.OnTickExecute;
            ++TickEventsCount;
        }

        private void RegisterTickUpdate(IBusComponent comp) {
            TickEvents[TickEventsCount] = comp.OnTickUpdate;
            ++TickEventsCount;
        }

        private void RegisterTickEnableToAddressBus(IBusComponent comp) {
            TickEvents[TickEventsCount] = comp.OnTickEnableToAddressBus;
            ++TickEventsCount;
        }

        private void RegisterTickEnableToDataBus(IBusComponent comp) {
            TickEvents[TickEventsCount] = comp.OnTickEnableToDataBus;
            ++TickEventsCount;
        }

        private void RegisterTickEnableToIOBus(IBusComponent comp) {
            TickEvents[TickEventsCount] = comp.OnTickEnableToIOBus;
            ++TickEventsCount;
        }

        private void RegisterTickSetFromAddressBus(IBusComponent comp) {
            TickEvents[TickEventsCount] = comp.OnTickSetFromAddressBus;
            ++TickEventsCount;
        }

        private void RegisterTickSetFromDataBus(IBusComponent comp) {
            TickEvents[TickEventsCount] = comp.OnTickSetFromDataBus;
            ++TickEventsCount;
        }

        private void RegisterTickSetFromIOBus(IBusComponent comp) {
            TickEvents[TickEventsCount] = comp.OnTickSetFromIOBus;
            ++TickEventsCount;
        }

        private void RegisterTickStore(IBusComponent comp) {
            TickEvents[TickEventsCount] = comp.OnTickStore;
            ++TickEventsCount;
        }

        private void RegisterTickLoad(IBusComponent comp) {
            TickEvents[TickEventsCount] = comp.OnTickLoad;
            ++TickEventsCount;
        }

        protected Action<IBusComponent>[] TickEvents = null;
        protected int TickEventsCount = 0;
        protected int TickExecuteCount = 0;

        public void Tick(IBusComponent cpuFlags) {
            for (var i = 0; i < TickEventsCount; ++i) {
                TickEvents[i](cpuFlags);
            }

            TickEventsCount = 0;
        }

        public void Stop() {
            IsRunning = false;
        }

        protected IBusComponent[] compList = new IBusComponent[32];
        protected int compNextIndex = 0;

        public void ConnectComponent(IBusComponent component) {
            foreach (var c in compList) {
                if (c == component) {
                    return;
                }
            }

            if (compNextIndex == compList.Length) {
                IBusComponent[] tmp = new IBusComponent[compNextIndex * 2];
                compList.CopyTo(tmp, 0);
                compList = tmp;
            }

            component.RequestTickExecute += RegisterTickExecute;
            component.RequestTickUpdate += RegisterTickUpdate;
            component.RequestTickEnableToAddressBus += RegisterTickEnableToAddressBus;
            component.RequestTickEnableToDataBus += RegisterTickEnableToDataBus;
            component.RequestTickEnableToIOBus += RegisterTickEnableToIOBus;
            component.RequestTickSetFromAddressBus += RegisterTickSetFromAddressBus;
            component.RequestTickSetFromDataBus += RegisterTickSetFromDataBus;
            component.RequestTickSetFromIOBus += RegisterTickSetFromIOBus;
            component.OnRegisterTickLoad += RegisterTickLoad;
            component.OnRegisterTickStore += RegisterTickStore;

            compList[compNextIndex] = component;
            ++compNextIndex;
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
            PowerModule = new MaizePowerModule(this);

            MaizeInstruction.WireUp(this);
        }

        public void PowerOff() {
            Clock?.Stop();
            Cpu?.PowerOff();
        }

        public void PowerOn() {
            Clock.Initialize();
            Cpu.PowerOn();
        }

        public void EnableDebug(bool isEnabled) {
            if (Cpu is null) {
                return;
            }

            if (isEnabled) {
                // Cpu.Decoder.DecoderValueAssignActions = Cpu.Decoder.DecoderValueAssignDebug;
            }
            else {
                // Cpu.Decoder.DecoderValueAssignActions = Cpu.Decoder.DecoderValueAssign;
            }
        }

        public IDataBus<UInt64> DataBus { get; protected set; }
        public IDataBus<UInt64> AddressBus { get; protected set; }
        public IDataBus<UInt64> IOBus { get; protected set; }

        public Clock Clock { get; protected set; }

        public MaizeCpu Cpu;
        public MaizeMemoryModule MemoryModule;
        public MaizePowerModule PowerModule;

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

        public void RaiseInterrupt(UInt64 id) {
            Cpu.RaiseInterrupt(id);
        }

        public byte ReadByte(UInt64 address) {
            return MemoryModule.ReadByte(address);
        }

        public void WriteByte(UInt64 address, byte value) {
            MemoryModule.WriteByte(address, value);
        }

        public void EnablePortIO(UInt64 address) {
            IBusComponent component = deviceTable[address];
            component?.EnableToIOBus(SubRegister.W0);
        }

        public void SetPortAddress(UInt64 address) {
            IBusComponent component = deviceTable[address];
            component?.SetFromAddressBus(SubRegister.W0);
        }

        public void SetPortIO(UInt64 address) {
            IBusComponent component = deviceTable[address];
            component?.SetFromIOBus(SubRegister.W0);
        }

        protected Dictionary<UInt64, IBusComponent> deviceTable = new();

        public void ConnectComponent(IBusComponent component) {
            Clock.ConnectComponent(component);
        }

        public void ConnectDevice(IBusComponent component, UInt64 address) {
            Clock.ConnectComponent(component);
            deviceTable[address] = component;
        }

        public UInt64 ConnectInterrupt(IBusComponent component, UInt64 address) {
            return address;
        }
    }

    public class MaizePowerModule : MaizeRegister {
        public MaizePowerModule(IMotherboard<UInt64> motherboard) {
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

    public class MaizeMemoryModule : MaizeRegister {

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

            CacheAddress.W0 = 0xFFFFFFFF_FFFFFFFF;
            CacheBase = CacheAddress.W0 >> 8;

            AddressRegister.RequestTickSetFromAddressBus += AddressRegisterSet;
            AddressRegister.RequestTickSetFromDataBus += AddressRegisterSet;
            AddressRegister.RequestTickSetFromIOBus += AddressRegisterSet;
            DataRegister.RequestTickSetFromAddressBus += DataRegisterSet;
            DataRegister.RequestTickSetFromDataBus += DataRegisterSet;
            DataRegister.RequestTickSetFromIOBus += DataRegisterSet;
        }

        private void AddressRegisterSet(IBusComponent comp) {
            RegisterTickLoad();
        }

        private void DataRegisterSet(IBusComponent comp) {
            RegisterTickStore();
        }

        public MaizeRegister AddressRegister = new MaizeRegister();
        public MaizeRegister DataRegister = new MaizeRegister();

        public override void OnTickStore(IBusComponent cpuFlags) {
            switch (DataRegister.SetSubRegisterMask) {
            case SubRegisterMask.B0:
                WriteByte(AddressRegister.RegData.W0, DataRegister.RegData.B0);
                break;

            case SubRegisterMask.B1:
                WriteByte(AddressRegister.RegData.W0, DataRegister.RegData.B1);
                break;

            case SubRegisterMask.B2:
                WriteByte(AddressRegister.RegData.W0, DataRegister.RegData.B2);
                break;

            case SubRegisterMask.B3:
                WriteByte(AddressRegister.RegData.W0, DataRegister.RegData.B3);
                break;

            case SubRegisterMask.B4:
                WriteByte(AddressRegister.RegData.W0, DataRegister.RegData.B4);
                break;

            case SubRegisterMask.B5:
                WriteByte(AddressRegister.RegData.W0, DataRegister.RegData.B5);
                break;

            case SubRegisterMask.B6:
                WriteByte(AddressRegister.RegData.W0, DataRegister.RegData.B6);
                break;

            case SubRegisterMask.B7:
                WriteByte(AddressRegister.RegData.W0, DataRegister.RegData.B7);
                break;

            case SubRegisterMask.Q0:
                WriteQuarterWord(AddressRegister.RegData.W0, DataRegister.RegData.Q0);
                break;

            case SubRegisterMask.Q1:
                WriteQuarterWord(AddressRegister.RegData.W0, DataRegister.RegData.Q1);
                break;

            case SubRegisterMask.Q2:
                WriteQuarterWord(AddressRegister.RegData.W0, DataRegister.RegData.Q2);
                break;

            case SubRegisterMask.Q3:
                WriteQuarterWord(AddressRegister.RegData.W0, DataRegister.RegData.Q3);
                break;

            case SubRegisterMask.H0:
                WriteHalfWord(AddressRegister.RegData.W0, DataRegister.RegData.H0);
                break;

            case SubRegisterMask.H1:
                WriteHalfWord(AddressRegister.RegData.W0, DataRegister.RegData.H1);
                break;

            case SubRegisterMask.W0:
                WriteWord(AddressRegister.RegData.W0, DataRegister.RegData.W0);
                break;
            }
        }

        public override void OnTickLoad(IBusComponent cpuFlags) {
            DataRegister.RegData.W0 = ReadWord(AddressRegister.RegData.W0);
        }

        RegValue tmp = 0;

        public UInt64 ReadWord(UInt64 address) {
            tmp.B0 = ReadByte(address);
            tmp.B1 = ReadByte(++address);
            tmp.B2 = ReadByte(++address);
            tmp.B3 = ReadByte(++address);
            tmp.B4 = ReadByte(++address);
            tmp.B5 = ReadByte(++address);
            tmp.B6 = ReadByte(++address);
            tmp.B7 = ReadByte(++address);
            return tmp.W0;
        }

        public void WriteWord(UInt64 address, UInt64 value) {
            tmp.W0 = value;
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
            tmp.B0 = ReadByte(address);
            tmp.B1 = ReadByte(++address);
            tmp.B2 = ReadByte(++address);
            tmp.B3 = ReadByte(++address);
            return tmp.H0;
        }

        public void WriteHalfWord(UInt64 address, UInt32 value) {
            tmp.H0 = value;
            WriteByte(address, tmp.B0);
            WriteByte(++address, tmp.B1);
            WriteByte(++address, tmp.B2);
            WriteByte(++address, tmp.B3);
        }

        public UInt16 ReadQuarterWord(UInt64 address) {
            tmp.B0 = ReadByte(address);
            tmp.B1 = ReadByte(++address);
            return tmp.Q0;
        }

        public void WriteQuarterWord(UInt64 address, UInt16 value) {
            tmp.Q0 = value;
            WriteByte(address, tmp.B0);
            WriteByte(++address, tmp.B1);
        }

        public byte ReadByte(UInt64 address) {
            SetCacheAddress(address);
            return Cache[CacheAddress.B0];
        }

        public void WriteByte(UInt64 address, byte value) {
            SetCacheAddress(address);
            Cache[CacheAddress.B0] = value;
        }

        void SetCacheAddress(UInt64 address) {
            var addressBase = address >> 8;

            if (addressBase != CacheBase) {
                if (!MemoryMap.ContainsKey(addressBase)) {
                    MemoryMap[addressBase] = new byte[0x100];
                }

                Cache = MemoryMap[addressBase];
                CacheBase = addressBase;
            }

            CacheAddress.W0 = address;
        }

        protected byte[] Cache = null;
        protected Dictionary<UInt64, byte[]> MemoryMap = new();

        public UInt32 MemorySize => (UInt32)MemoryMap.Count() * 0x100;

        protected RegValue CacheAddress;
        protected UInt64 CacheBase;
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
        public MaizeInstruction() {
        }

        public static MaizeMotherboard MB;
        public static MaizeCpu Cpu;
        public static MaizeDecoder Decoder;
        public static MaizeRegister OperandRegister1;
        public static MaizeRegister OperandRegister2;
        public static MaizeRegister OperandRegister3;
        public static MaizeRegister OperandRegister4;
        public static MaizeMemoryModule MemoryModule;
        public static MaizeRegister SrcReg;
        public static MaizeRegister DestReg;
        public static MaizeRegister F;
        public static MaizeRegister I;
        public static MaizeRegister P;
        public static MaizeRegister S;
        public static MaizeAlu Alu;

        // public static MaizeRegister A = Cpu.A;
        // public static MaizeRegister B = Cpu.B;
        // public static MaizeRegister C = Cpu.C;
        // public static MaizeRegister D = Cpu.D;
        // public static MaizeRegister E = Cpu.E;
        // public static MaizeRegister G = Cpu.G;
        // public static MaizeRegister H = Cpu.H;
        // public static MaizeRegister J = Cpu.J;
        // public static MaizeRegister K = Cpu.K;
        // public static MaizeRegister L = Cpu.L;
        // public static MaizeRegister M = Cpu.M;
        // public static MaizeRegister Z = Cpu.Z;
        // public static Tortilla.IClock Clock = MB.Clock;

        internal static void WireUp(MaizeMotherboard motherboard) {
            MB = motherboard;
            MemoryModule = MB.MemoryModule;
            Cpu = MB.Cpu;
            F = Cpu.F;
            I = Cpu.I;
            P = Cpu.P;
            S = Cpu.S;
            Alu = Cpu.Alu;
            Decoder = Cpu.Decoder;
            SrcReg = null;
            DestReg = null;
            OperandRegister1 = Decoder.OperandRegister1;
            OperandRegister2 = Decoder.OperandRegister2;
            OperandRegister3 = Decoder.OperandRegister3;
            OperandRegister4 = Decoder.OperandRegister4;
        }


        protected byte SrcImmSizeFlag => (byte)(OperandRegister1.RegData.B1 & OpFlag_ImmSize);
        protected int SrcImmSize => 1 << SrcImmSizeFlag;
        protected byte SrcRegisterFlag => (byte)(OperandRegister1.RegData.B1 & OpFlag_Reg);
        protected byte SrcSubRegisterFlag => (byte)(OperandRegister1.RegData.B1 & OpFlag_SubReg);
        protected byte DestImmSizeFlag => (byte)(OperandRegister1.RegData.B2 & OpFlag_ImmSize);
        protected int DestImmSize => 1 << DestImmSizeFlag;
        protected byte DestRegisterFlag => (byte)(OperandRegister1.RegData.B2 & OpFlag_Reg);
        protected byte DestSubRegisterFlag => (byte)(OperandRegister1.RegData.B2 & OpFlag_SubReg);

        public byte? Opcode = null;
        public virtual string Mnemonic { get; } = string.Empty;

        public const byte OpcodeFlag = 0b_1100_0000;
        public const byte OpcodeFlag_SrcImm = 0b_0100_0000;
        public const byte OpcodeFlag_SrcAddr = 0b_1000_0000;

        public const byte OpFlag_Reg = 0b_1111_0000;
        public const byte OpFlag_RegA = 0b_0000_0000;
        public const byte OpFlag_RegB = 0b_0001_0000;
        public const byte OpFlag_RegC = 0b_0010_0000;
        public const byte OpFlag_RegD = 0b_0011_0000;
        public const byte OpFlag_RegE = 0b_0100_0000;
        public const byte OpFlag_RegG = 0b_0101_0000;
        public const byte OpFlag_RegH = 0b_0110_0000;
        public const byte OpFlag_RegJ = 0b_0111_0000;
        public const byte OpFlag_RegK = 0b_1000_0000;
        public const byte OpFlag_RegL = 0b_1001_0000;
        public const byte OpFlag_RegM = 0b_1010_0000;
        public const byte OpFlag_RegZ = 0b_1011_0000;
        public const byte OpFlag_RegF = 0b_1100_0000;
        public const byte OpFlag_RegI = 0b_1101_0000;
        public const byte OpFlag_RegP = 0b_1110_0000;
        public const byte OpFlag_RegS = 0b_1111_0000;

        public const byte OpFlag_RegSP = 0b_1111_1100; // S.H0 = stack pointer
        public const byte OpFlag_RegBP = 0b_1111_1101; // S.H1 = base pointer
        public const byte OpFlag_RegPC = 0b_1110_1100; // P.H0 = program counter
        public const byte OpFlag_RegCS = 0b_1110_1101; // P.H1 = program segment
        public const byte OpFlag_RegFL = 0b_1100_1100; // F.H0 = flags

        public const byte OpFlag_SubReg = 0b_0000_1111;
        public const byte OpFlag_RegB0 = 0b_0000_0000;
        public const byte OpFlag_RegB1 = 0b_0000_0001;
        public const byte OpFlag_RegB2 = 0b_0000_0010;
        public const byte OpFlag_RegB3 = 0b_0000_0011;
        public const byte OpFlag_RegB4 = 0b_0000_0100;
        public const byte OpFlag_RegB5 = 0b_0000_0101;
        public const byte OpFlag_RegB6 = 0b_0000_0110;
        public const byte OpFlag_RegB7 = 0b_0000_0111;
        public const byte OpFlag_RegQ0 = 0b_0000_1000;
        public const byte OpFlag_RegQ1 = 0b_0000_1001;
        public const byte OpFlag_RegQ2 = 0b_0000_1010;
        public const byte OpFlag_RegQ3 = 0b_0000_1011;
        public const byte OpFlag_RegH0 = 0b_0000_1100;
        public const byte OpFlag_RegH1 = 0b_0000_1101;
        public const byte OpFlag_RegW0 = 0b_0000_1110;

        public const byte OpFlag_ImmSize = 0b_0000_0111;
        public const byte OpFlag_Imm08b = 0b_0000_0000;
        public const byte OpFlag_Imm16b = 0b_0000_0001;
        public const byte OpFlag_Imm32b = 0b_0000_0010;
        public const byte OpFlag_Imm64b = 0b_0000_0011;

        public const byte OpFlag_ImmRes01 = 0b_0100_0000;
        public const byte OpFlag_ImmRes02 = 0b_0101_0000;
        public const byte OpFlag_ImmRes03 = 0b_0110_0000;
        public const byte OpFlag_ImmRes04 = 0b_0111_0000;

        protected static SubRegisterMask[] ImmSizeSubRegMaskMap = new SubRegisterMask[] {
            SubRegisterMask.B0,
            SubRegisterMask.Q0,
            SubRegisterMask.H0,
            SubRegisterMask.W0
        };

        protected static SubRegister[] ImmSizeSubRegMap = new SubRegister[] {
            SubRegister.B0,
            SubRegister.Q0,
            SubRegister.H0,
            SubRegister.W0
        };

        public static SubRegisterMask[] SubRegisterMaskMap = new SubRegisterMask[] {
            SubRegisterMask.B0,
            SubRegisterMask.B1,
            SubRegisterMask.B2,
            SubRegisterMask.B3,
            SubRegisterMask.B4,
            SubRegisterMask.B5,
            SubRegisterMask.B6,
            SubRegisterMask.B7,
            SubRegisterMask.Q0,
            SubRegisterMask.Q1,
            SubRegisterMask.Q2,
            SubRegisterMask.Q3,
            SubRegisterMask.H0,
            SubRegisterMask.H1,
            SubRegisterMask.W0
        };

        protected static int[] SizeMap = new int[] {
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            2,
            2,
            2,
            2,
            4,
            4,
            8
        };

        protected static byte[] AluOpSizeMap = new byte[] {
            MaizeAlu.OpSize_Byte,
            MaizeAlu.OpSize_Byte,
            MaizeAlu.OpSize_Byte,
            MaizeAlu.OpSize_Byte,
            MaizeAlu.OpSize_Byte,
            MaizeAlu.OpSize_Byte,
            MaizeAlu.OpSize_Byte,
            MaizeAlu.OpSize_Byte,
            MaizeAlu.OpSize_QuarterWord,
            MaizeAlu.OpSize_QuarterWord,
            MaizeAlu.OpSize_QuarterWord,
            MaizeAlu.OpSize_QuarterWord,
            MaizeAlu.OpSize_HalfWord,
            MaizeAlu.OpSize_HalfWord,
            MaizeAlu.OpSize_Word
        };

        public Action[] Code = null;

        public virtual void BuildMicrocode() {
            Code = new Action[] { };
        }

        public override string ToString() {
            string paramBytes = "";
            StringBuilder text = new StringBuilder($"${P.RegData.H0:X8}: {$"{Mnemonic}",-42} ; {Opcode:X2} {paramBytes}");
            return text.ToString();
        }
    }

    public class InstructionBase<T> : MaizeInstruction where T : MaizeInstruction {
        static InstructionBase() { }

        private static readonly Lazy<T> _instance = new Lazy<T>(() => CreateInstance());

        protected InstructionBase() {
        }

        public static T Instance = _instance.Value;

        public static Action[] GetMicrocode() {
            return Instance.Code;
        }

        public static Action[] Microcode = Instance.Code;

        private static T CreateInstance() {
            T t = Activator.CreateInstance(typeof(T), true) as T;
            t.BuildMicrocode();
            return t;
        }
    }


    public class MaizeCpu : MaizeRegister, ICpu<UInt64> {
        public const UInt64 Flag_CarryOut =         0b_00000000_00000000_00000000_00000000_00000000_00000000_00000000_00000001;
        public const UInt64 Flag_Negative =         0b_00000000_00000000_00000000_00000000_00000000_00000000_00000000_00000010;
        public const UInt64 Flag_Overflow =         0b_00000000_00000000_00000000_00000000_00000000_00000000_00000000_00000100;
        public const UInt64 Flag_Parity =           0b_00000000_00000000_00000000_00000000_00000000_00000000_00000000_00001000;
        public const UInt64 Flag_Zero =             0b_00000000_00000000_00000000_00000000_00000000_00000000_00000000_00010000;
        public const UInt64 Flag_Sign =             0b_00000000_00000000_00000000_00000000_00000000_00000000_00000000_00100000;
        public const UInt64 Flag_Reserved =         0b_00000000_00000000_00000000_00000000_00000000_00000000_00000000_01000000;

        public const UInt64 Flag_Privilege =        0b_00000000_00000000_00000000_00000001_00000000_00000000_00000000_00000000;
        public const UInt64 Flag_InterruptEnabled = 0b_00000000_00000000_00000000_00000010_00000000_00000000_00000000_00000000;
        public const UInt64 Flag_InterruptSet =     0b_00000000_00000000_00000000_00000100_00000000_00000000_00000000_00000000;

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
            F.PrivilegeMask = (UInt64)SubRegisterMask.H1;
            F.DataBus = MB.DataBus;
            F.AddressBus = MB.AddressBus;
            F.IOBus = MB.IOBus;
            MB.ConnectComponent(F);

            P.PrivilegeFlags = Flag_Privilege;
            P.PrivilegeMask = (UInt64)SubRegisterMask.H1;
            P.DataBus = MB.DataBus;
            P.AddressBus = MB.AddressBus;
            P.IOBus = MB.IOBus;
            MB.ConnectComponent(P);

            S.DataBus = MB.DataBus;
            S.AddressBus = MB.AddressBus;
            S.IOBus = MB.IOBus;
            MB.ConnectComponent(S);

            Decoder = new MaizeDecoder(MB);
            // Decoder.InstructionRead += InstructionRegister_InstructionRead;
            Decoder.RegisterMap[MaizeInstruction.OpFlag_RegA >> 4] = A;
            Decoder.RegisterMap[MaizeInstruction.OpFlag_RegB >> 4] = B;
            Decoder.RegisterMap[MaizeInstruction.OpFlag_RegC >> 4] = C;
            Decoder.RegisterMap[MaizeInstruction.OpFlag_RegD >> 4] = D;
            Decoder.RegisterMap[MaizeInstruction.OpFlag_RegE >> 4] = E;
            Decoder.RegisterMap[MaizeInstruction.OpFlag_RegG >> 4] = G;
            Decoder.RegisterMap[MaizeInstruction.OpFlag_RegH >> 4] = H;
            Decoder.RegisterMap[MaizeInstruction.OpFlag_RegJ >> 4] = J;
            Decoder.RegisterMap[MaizeInstruction.OpFlag_RegK >> 4] = K;
            Decoder.RegisterMap[MaizeInstruction.OpFlag_RegL >> 4] = L;
            Decoder.RegisterMap[MaizeInstruction.OpFlag_RegM >> 4] = M;
            Decoder.RegisterMap[MaizeInstruction.OpFlag_RegZ >> 4] = Z;
            Decoder.RegisterMap[MaizeInstruction.OpFlag_RegF >> 4] = F;
            Decoder.RegisterMap[MaizeInstruction.OpFlag_RegI >> 4] = Decoder;
            Decoder.RegisterMap[MaizeInstruction.OpFlag_RegP >> 4] = P;
            Decoder.RegisterMap[MaizeInstruction.OpFlag_RegS >> 4] = S;

            I = Decoder;

            P.RegData.H0 = 0x0000_1000; // Start address
            P.RegData.H1 = 0x0000_0000; // Start segment

            Alu = new MaizeAlu(MB, this);

            MB.ConnectComponent(this);
            MB.ConnectDevice(this, 0x00);
        }

        public UInt64 Flags {
            get { return F.RegData.W0; }
            set { F.RegData.W0 = value; }
        }

        /*
        public bool CarryIn {
            get { return (this.B0 & OpCodeCtrl_CarryIn); }
            set { this.B0 = ((this.B0 & ~OpCodeCtrl_CarryIn) | ((value & 0b_0000_0001) << (OpCodeCtrl_CarryIn / 4))); }
        }
        */

        public bool NegativeFlag {
            get { return (F.RegData.W0 & Flag_Negative) == Flag_Negative; }
            set { F.RegData.W0 = ((F.RegData.W0 & ~Flag_Negative) | (value ? Flag_Negative : 0)); }
        }

        public bool OverflowFlag {
            get { return (F.RegData.W0 & Flag_Overflow) == Flag_Overflow; }
            set { F.RegData.W0 = ((F.RegData.W0 & ~Flag_Overflow) | (value ? Flag_Overflow : 0)); }
        }

        public bool ZeroFlag {
            get { return (F.RegData.W0 & Flag_Zero) == Flag_Zero; }
            set { F.RegData.W0 = ((F.RegData.W0 & ~Flag_Zero) | (value ? Flag_Zero : 0)); }
        }

        public bool CarryFlag {
            get { return (F.RegData.W0 & Flag_CarryOut) == Flag_CarryOut; }
            set { F.RegData.W0 = ((F.RegData.W0 & ~Flag_CarryOut) | (value ? Flag_CarryOut : 0)); }
        }

        public bool InterruptEnabledFlag {
            get { return (F.RegData.W0 & Flag_InterruptEnabled) == Flag_InterruptEnabled; }
            set { F.RegData.W0 = ((F.RegData.W0 & ~Flag_InterruptEnabled) | (value ? Flag_InterruptEnabled : 0)); }
        }

        public bool InterruptSetFlag {
            get { return (F.RegData.W0 & Flag_InterruptSet) == Flag_InterruptSet; }
            set { F.RegData.W0 = ((F.RegData.W0 & ~Flag_InterruptSet) | (value ? Flag_InterruptSet : 0)); }
        }

        public bool PrivilegeFlag {
            get { return (F.RegData.W0 & Flag_Privilege) == Flag_Privilege; }
            set { F.RegData.W0 = ((F.RegData.W0 & ~Flag_Privilege) | (value ? Flag_Privilege : 0)); }
        }

        private void InstructionRegister_InstructionRead(object sender, Tuple<UInt64, UInt64> e) {
            DecodeInstruction?.Invoke(sender, e);
        }

        MaizeMotherboard MB = null;

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

        public MaizeAlu Alu;
        public MaizeDecoder Decoder;

        public MaizeRegister A = new MaizeRegister();
        public MaizeRegister B = new MaizeRegister();
        public MaizeRegister C = new MaizeRegister();
        public MaizeRegister D = new MaizeRegister();
        public MaizeRegister E = new MaizeRegister();
        public MaizeRegister G = new MaizeRegister();
        public MaizeRegister H = new MaizeRegister();
        public MaizeRegister J = new MaizeRegister();
        public MaizeRegister K = new MaizeRegister();
        public MaizeRegister L = new MaizeRegister();
        public MaizeRegister M = new MaizeRegister();
        public MaizeRegister Z = new MaizeRegister();
        public MaizeRegister F = new MaizeRegister();
        public MaizeRegister I = null; 
        public MaizeRegister P = new MaizeRegister();
        public MaizeRegister S = new MaizeRegister();
     
        public event EventHandler<Tuple<UInt64, UInt64>> DecodeInstruction;

        public void Break() {
            MB.Clock.Stop();
            SingleStep = true;
        }

        AutoResetEvent ClockStopEvent { get; } = new(false);
        AutoResetEvent ClockEvent { get; } = new(false);

        public void Run() {
            IsPowerOn = true;

            while (IsPowerOn) {
                if (SingleStep) {
                    MB.Clock.Stop();
                    MB.Clock.RegisterTickExecute(MB.Cpu.Decoder);
                    MB.Clock.Tick(F);
                }
                else {
                    MB.Clock.Start();

                    while (MB.Clock.IsRunning) {
                        MB.Clock.RegisterTickExecute(MB.Cpu.Decoder);
                        MB.Clock.Tick(F);
                    }
                }

                if (IsPowerOn) {
                    ClockEvent.WaitOne();
                }
            }
        }

        public void PowerOff() {
            IsPowerOn = false;
            MB.Clock.Stop();
            ClockEvent.Set();
        }

        public void PowerOn() {
            IsPowerOn = true;
            PrivilegeFlag = true;
            InterruptEnabledFlag = true;
            Run();
        }

        public Queue<UInt64> InterruptQueue = new();

        public void RaiseInterrupt(UInt64 id) {
            // Set interrupt flag
            InterruptQueue.Enqueue(id);
            InterruptSetFlag = true;
            
            if (!MB.Clock.IsRunning) {
                MB.Clock.Start();
                ClockEvent.Set();
            }
        }

        public void Reset() {
            P.RegData.H0 = 0;
            Decoder.RegData.W0 = 0;
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

        public void JumpTo(MaizeInstruction instruction) {
            ActiveInstruction = instruction;
            Step = 0;
            ActiveInstruction.Code[Step]();
        }


        // public event EventHandler<Tuple<UInt64, UInt64>> InstructionRead;

        public MaizeRegister OperandRegister1 = new MaizeRegister();
        public MaizeRegister OperandRegister2 = new MaizeRegister();
        public MaizeRegister OperandRegister3 = new MaizeRegister();
        public MaizeRegister OperandRegister4 = new MaizeRegister();
        public MaizeRegister SrcReg = null;
        public MaizeRegister DestReg = null;

        MaizeMotherboard MB = null;
        public Tortilla.IClock Clock => MB.Clock;

        public int Step = 0;
        public int Cycle = 0;

        public MaizeRegister[] RegisterMap = new MaizeRegister[0x16];

        public MaizeInstruction[] InstructionArray;
        protected MaizeInstruction ActiveInstruction;
        public Action[] DecoderValueAssignActions; 

        public override void OnTickExecute(IBusComponent cpuFlags) {
            if (Cycle == 0) {
                Step = 0;
                ActiveInstruction = Core.ReadOpcodeAndDispatch.Instance;

                if (   (MB.Cpu.F.RegData.W0 & MaizeCpu.Flag_InterruptSet) == MaizeCpu.Flag_InterruptSet
                    && (MB.Cpu.F.RegData.W0 & MaizeCpu.Flag_InterruptEnabled) == MaizeCpu.Flag_InterruptEnabled)
                {
                    MB.Cpu.InterruptSetFlag = false;
                    var interruptID = MB.Cpu.InterruptQueue.Dequeue();

                    if (interruptID != 0) {
                        MB.Cpu.S.RegData.H0 -= 8;
                        MB.MemoryModule.WriteWord(MB.Cpu.S.RegData.H0, MB.Cpu.F.RegData.W0);
                        MB.Cpu.S.RegData.H0 -= 8;
                        MB.MemoryModule.WriteWord(MB.Cpu.S.RegData.H0, MB.Cpu.P.RegData.W0);
                    }

                    // Vector to interrupt handler for interrupt ID
                    UInt32 intAddress = (UInt32)(interruptID * 4);
                    UInt32 startAddress = MB.MemoryModule.ReadHalfWord(intAddress);
                    MB.Cpu.P.RegData.H0 = startAddress;
                }
            }

            ActiveInstruction.Code[Step]();
            ++Step;

            if (Step >= ActiveInstruction.Code.Length) {
                Cycle = 0;
                return;
            }

            ++Cycle;
        }

        public void ReadAndEnableImmediate(int size) {
            PushMicrocodeStack();

            switch (size) {
            case 1:
                ActiveInstruction = Core.ReadImmediate1Byte.Instance;
                break;
            case 2:
                ActiveInstruction = Core.ReadImmediate2Byte.Instance;
                break;
            case 4:
                ActiveInstruction = Core.ReadImmediate4Byte.Instance;
                break;
            case 8:
                ActiveInstruction = Core.ReadImmediate8Byte.Instance;
                break;
            }

            ActiveInstruction.Code[Step]();
        }

        int stackIndex = 0;

        // TODO: Get rid of the ugly magic number (0x0F) in the next two arrays
        MaizeInstruction[] MicrocodeStack = new MaizeInstruction[0x0F];
        int[] StepStack = new int[0x0F];

        public void PushMicrocodeStack() {
            MicrocodeStack[stackIndex] = ActiveInstruction;
            StepStack[stackIndex] = Step;
            ++stackIndex;
            Step = 0;
        }

        public void PopMicrocodeStack() {
            --stackIndex;
            ActiveInstruction = MicrocodeStack[stackIndex];
            Step = StepStack[stackIndex];
            ++Step;
            // TODO: This is a problem if there isn't another step to execute! I'm making a big assumption here.
            ActiveInstruction.Code[Step]();
        }

        protected void BuildMicrocode() {

            ActiveInstruction = Core.ReadOpcodeAndDispatch.Instance;
            DecoderValueAssignActions = Core.DecoderValueAssign.Microcode;
            // DecoderValueAssignActions = Instructions.DecoderValueAssignDebug.Microcode;

            InstructionArray = new MaizeInstruction[] {
                /* 0x00 */ Instructions.HALT.Instance,
                /* 0x01 */ Instructions.LD_RegVal_Reg.Instance,
                /* 0x02 */ Instructions.STIM.Instance,
                /* 0x03 */ Instructions.ADD_RegVal_Reg.Instance,
                /* 0x04 */ Instructions.SUB_RegVal_Reg.Instance,
                /* 0x05 */ Instructions.MUL_RegVal_Reg.Instance,
                /* 0x06 */ Instructions.DIV_RegVal_Reg.Instance,
                /* 0x07 */ Instructions.MOD_RegVal_Reg.Instance,
                /* 0x08 */ Instructions.AND_RegVal_Reg.Instance,
                /* 0x09 */ Instructions.OR_RegVal_Reg.Instance,
                /* 0x0A */ Instructions.NOR_RegVal_Reg.Instance,
                /* 0x0B */ Instructions.NAND_RegVal_Reg.Instance,
                /* 0x0C */ Instructions.XOR_RegVal_Reg.Instance,
                /* 0x0D */ Instructions.SHL_RegVal_Reg.Instance,
                /* 0x0E */ Instructions.SHR_RegVal_Reg.Instance,
                /* 0x0F */ Instructions.CMP_RegVal_Reg.Instance,
                /* 0x10 */ Instructions.TEST_RegVal_Reg.Instance,
                /* 0x11 */ Exceptions.BadOpcode.Instance,
                /* 0x12 */ Instructions.INT_RegVal.Instance,
                /* 0x13 */ Instructions.ST.Instance,
                /* 0x14 */ Instructions.OUTR_RegVal_Reg.Instance,
                /* 0x15 */ Exceptions.BadOpcode.Instance,
                /* 0x16 */ Instructions.JMP_RegVal.Instance,
                /* 0x17 */ Instructions.JZ_RegVal.Instance,
                /* 0x18 */ Instructions.JNZ_RegVal.Instance,
                /* 0x19 */ Exceptions.BadOpcode.Instance,
                /* 0x1A */ Exceptions.BadOpcode.Instance,
                /* 0x1B */ Exceptions.BadOpcode.Instance,
                /* 0x1C */ Exceptions.BadOpcode.Instance,
                /* 0x1D */ Instructions.CALL_RegVal.Instance,
                /* 0x1E */ Instructions.OUT_RegVal_Imm.Instance,
                /* 0x1F */ Instructions.IN_RegVal_Reg.Instance,
                /* 0x20 */ Instructions.PUSH_RegVal.Instance,
                /* 0x21 */ Instructions.PUSH_ImmVal.Instance,
                /* 0x22 */ Instructions.CLR.Instance,
                /* 0x23 */ Instructions.INC.Instance,
                /* 0x24 */ Instructions.DEC.Instance,
                /* 0x25 */ Exceptions.BadOpcode.Instance,
                /* 0x26 */ Instructions.POP_RegVal.Instance,
                /* 0x27 */ Instructions.RET.Instance,
                /* 0x28 */ Instructions.IRET.Instance,
                /* 0x29 */ Instructions.STI.Instance,
                /* 0x2A */ Instructions.CLI.Instance,
                /* 0x2B */ Exceptions.BadOpcode.Instance,
                /* 0x2C */ Exceptions.BadOpcode.Instance,
                /* 0x2D */ Exceptions.BadOpcode.Instance,
                /* 0x2E */ Exceptions.BadOpcode.Instance,
                /* 0x2F */ Exceptions.BadOpcode.Instance,
                /* 0x30 */ Exceptions.BadOpcode.Instance,
                /* 0x29 */ Instructions.STC.Instance,
                /* 0x2A */ Instructions.CLC.Instance,
                /* 0x33 */ Exceptions.BadOpcode.Instance,
                /* 0x34 */ Exceptions.BadOpcode.Instance,
                /* 0x35 */ Exceptions.BadOpcode.Instance,
                /* 0x36 */ Exceptions.BadOpcode.Instance,
                /* 0x37 */ Exceptions.BadOpcode.Instance,
                /* 0x38 */ Exceptions.BadOpcode.Instance,
                /* 0x39 */ Exceptions.BadOpcode.Instance,
                /* 0x3A */ Exceptions.BadOpcode.Instance,
                /* 0x3B */ Exceptions.BadOpcode.Instance,
                /* 0x3C */ Exceptions.BadOpcode.Instance,
                /* 0x3D */ Exceptions.BadOpcode.Instance,
                /* 0x3E */ Exceptions.BadOpcode.Instance,
                /* 0x3F */ Exceptions.BadOpcode.Instance,
                /* 0x40 */ Exceptions.BadOpcode.Instance,
                /* 0x41 */ Instructions.LD_ImmVal_Reg.Instance,
                /* 0x42 */ Exceptions.BadOpcode.Instance,
                /* 0x43 */ Instructions.ADD_ImmVal_Reg.Instance,
                /* 0x44 */ Instructions.SUB_ImmVal_Reg.Instance,
                /* 0x45 */ Instructions.MUL_ImmVal_Reg.Instance,
                /* 0x46 */ Instructions.DIV_ImmVal_Reg.Instance,
                /* 0x47 */ Instructions.MOD_ImmVal_Reg.Instance,
                /* 0x48 */ Instructions.AND_ImmVal_Reg.Instance,
                /* 0x49 */ Instructions.OR_ImmVal_Reg.Instance,
                /* 0x4A */ Instructions.NOR_ImmVal_Reg.Instance,
                /* 0x4B */ Instructions.NAND_ImmVal_Reg.Instance,
                /* 0x4C */ Instructions.XOR_ImmVal_Reg.Instance,
                /* 0x4D */ Instructions.SHL_ImmVal_Reg.Instance,
                /* 0x4E */ Instructions.SHR_ImmVal_Reg.Instance,
                /* 0x4F */ Instructions.CMP_ImmVal_Reg.Instance,
                /* 0x50 */ Instructions.TEST_ImmVal_Reg.Instance,
                /* 0x51 */ Exceptions.BadOpcode.Instance,
                /* 0x52 */ Instructions.INT_ImmVal.Instance,
                /* 0x53 */ Exceptions.BadOpcode.Instance,
                /* 0x54 */ Instructions.OUTR_ImmVal_Reg.Instance,
                /* 0x55 */ Exceptions.BadOpcode.Instance,
                /* 0x56 */ Instructions.JMP_ImmVal.Instance,
                /* 0x57 */ Instructions.JZ_ImmVal.Instance,
                /* 0x58 */ Instructions.JNZ_ImmVal.Instance,
                /* 0x59 */ Exceptions.BadOpcode.Instance,
                /* 0x5A */ Exceptions.BadOpcode.Instance,
                /* 0x5B */ Exceptions.BadOpcode.Instance,
                /* 0x5C */ Exceptions.BadOpcode.Instance,
                /* 0x5D */ Instructions.CALL_ImmVal.Instance,
                /* 0x5E */ Exceptions.BadOpcode.Instance,
                /* 0x5F */ Instructions.IN_ImmVal_Reg.Instance,
                /* 0x60 */ Exceptions.BadOpcode.Instance,
                /* 0x61 */ Exceptions.BadOpcode.Instance,
                /* 0x62 */ Exceptions.BadOpcode.Instance,
                /* 0x63 */ Exceptions.BadOpcode.Instance,
                /* 0x64 */ Exceptions.BadOpcode.Instance,
                /* 0x65 */ Exceptions.BadOpcode.Instance,
                /* 0x66 */ Exceptions.BadOpcode.Instance,
                /* 0x67 */ Exceptions.BadOpcode.Instance,
                /* 0x68 */ Exceptions.BadOpcode.Instance,
                /* 0x69 */ Exceptions.BadOpcode.Instance,
                /* 0x6A */ Exceptions.BadOpcode.Instance,
                /* 0x6B */ Exceptions.BadOpcode.Instance,
                /* 0x6C */ Exceptions.BadOpcode.Instance,
                /* 0x6D */ Exceptions.BadOpcode.Instance,
                /* 0x6E */ Exceptions.BadOpcode.Instance,
                /* 0x6F */ Exceptions.BadOpcode.Instance,
                /* 0x70 */ Exceptions.BadOpcode.Instance,
                /* 0x71 */ Exceptions.BadOpcode.Instance,
                /* 0x72 */ Exceptions.BadOpcode.Instance,
                /* 0x73 */ Exceptions.BadOpcode.Instance,
                /* 0x74 */ Exceptions.BadOpcode.Instance,
                /* 0x75 */ Exceptions.BadOpcode.Instance,
                /* 0x76 */ Exceptions.BadOpcode.Instance,
                /* 0x77 */ Exceptions.BadOpcode.Instance,
                /* 0x78 */ Exceptions.BadOpcode.Instance,
                /* 0x79 */ Exceptions.BadOpcode.Instance,
                /* 0x7A */ Exceptions.BadOpcode.Instance,
                /* 0x7B */ Exceptions.BadOpcode.Instance,
                /* 0x7C */ Exceptions.BadOpcode.Instance,
                /* 0x7D */ Exceptions.BadOpcode.Instance,
                /* 0x7E */ Exceptions.BadOpcode.Instance,
                /* 0x7F */ Exceptions.BadOpcode.Instance,
                /* 0x80 */ Exceptions.BadOpcode.Instance,
                /* 0x81 */ Instructions.LD_RegAddr_Reg.Instance,
                /* 0x82 */ Exceptions.BadOpcode.Instance,
                /* 0x83 */ Instructions.ADD_RegAddr_Reg.Instance,
                /* 0x84 */ Instructions.SUB_RegAddr_Reg.Instance,
                /* 0x85 */ Instructions.MUL_RegAddr_Reg.Instance,
                /* 0x86 */ Instructions.DIV_RegAddr_Reg.Instance,
                /* 0x87 */ Instructions.MOD_RegAddr_Reg.Instance,
                /* 0x88 */ Instructions.AND_RegAddr_Reg.Instance,
                /* 0x89 */ Instructions.OR_RegAddr_Reg.Instance,
                /* 0x8A */ Instructions.NOR_RegAddr_Reg.Instance,
                /* 0x8B */ Instructions.NAND_RegAddr_Reg.Instance,
                /* 0x8C */ Instructions.XOR_RegAddr_Reg.Instance,
                /* 0x8D */ Instructions.SHL_RegAddr_Reg.Instance,
                /* 0x8E */ Instructions.SHR_RegAddr_Reg.Instance,
                /* 0x8F */ Instructions.CMP_RegAddr_Reg.Instance,
                /* 0x90 */ Instructions.TEST_RegAddr_Reg.Instance,
                /* 0x91 */ Exceptions.BadOpcode.Instance,
                /* 0x92 */ Exceptions.BadOpcode.Instance,
                /* 0x93 */ Exceptions.BadOpcode.Instance,
                /* 0x94 */ Exceptions.BadOpcode.Instance,
                /* 0x95 */ Exceptions.BadOpcode.Instance,
                /* 0x96 */ Instructions.JMP_RegAddr.Instance,
                /* 0x97 */ Instructions.JZ_RegAddr.Instance,
                /* 0x98 */ Instructions.JNZ_RegAddr.Instance,
                /* 0x99 */ Exceptions.BadOpcode.Instance,
                /* 0x9A */ Exceptions.BadOpcode.Instance,
                /* 0x9B */ Exceptions.BadOpcode.Instance,
                /* 0x9C */ Exceptions.BadOpcode.Instance,
                /* 0x9D */ Instructions.CALL_RegAddr.Instance,
                /* 0x9E */ Exceptions.BadOpcode.Instance,
                /* 0x9F */ Exceptions.BadOpcode.Instance,
                /* 0xA0 */ Exceptions.BadOpcode.Instance,
                /* 0xA1 */ Exceptions.BadOpcode.Instance,
                /* 0xA2 */ Exceptions.BadOpcode.Instance,
                /* 0xA3 */ Exceptions.BadOpcode.Instance,
                /* 0xA4 */ Exceptions.BadOpcode.Instance,
                /* 0xA5 */ Exceptions.BadOpcode.Instance,
                /* 0xA6 */ Exceptions.BadOpcode.Instance,
                /* 0xA7 */ Exceptions.BadOpcode.Instance,
                /* 0xA8 */ Exceptions.BadOpcode.Instance,
                /* 0xA9 */ Exceptions.BadOpcode.Instance,
                /* 0xAA */ Instructions.NOP.Instance,
                /* 0xAB */ Exceptions.BadOpcode.Instance,
                /* 0xAC */ Exceptions.BadOpcode.Instance,
                /* 0xAD */ Exceptions.BadOpcode.Instance,
                /* 0xAE */ Exceptions.BadOpcode.Instance,
                /* 0xAF */ Exceptions.BadOpcode.Instance,
                /* 0xB0 */ Exceptions.BadOpcode.Instance,
                /* 0xB1 */ Exceptions.BadOpcode.Instance,
                /* 0xB2 */ Exceptions.BadOpcode.Instance,
                /* 0xB3 */ Exceptions.BadOpcode.Instance,
                /* 0xB4 */ Exceptions.BadOpcode.Instance,
                /* 0xB5 */ Exceptions.BadOpcode.Instance,
                /* 0xB6 */ Exceptions.BadOpcode.Instance,
                /* 0xB7 */ Exceptions.BadOpcode.Instance,
                /* 0xB8 */ Exceptions.BadOpcode.Instance,
                /* 0xB9 */ Exceptions.BadOpcode.Instance,
                /* 0xBA */ Exceptions.BadOpcode.Instance,
                /* 0xBB */ Exceptions.BadOpcode.Instance,
                /* 0xBC */ Exceptions.BadOpcode.Instance,
                /* 0xBD */ Exceptions.BadOpcode.Instance,
                /* 0xBE */ Exceptions.BadOpcode.Instance,
                /* 0xBF */ Exceptions.BadOpcode.Instance,
                /* 0xC0 */ Exceptions.BadOpcode.Instance,
                /* 0xC1 */ Instructions.LD_ImmAddr_Reg.Instance,
                /* 0xC2 */ Exceptions.BadOpcode.Instance,
                /* 0xC3 */ Instructions.ADD_ImmAddr_Reg.Instance,
                /* 0xC4 */ Instructions.SUB_ImmAddr_Reg.Instance,
                /* 0xC5 */ Instructions.MUL_ImmAddr_Reg.Instance,
                /* 0xC6 */ Instructions.DIV_ImmAddr_Reg.Instance,
                /* 0xC7 */ Instructions.MOD_ImmAddr_Reg.Instance,
                /* 0xC8 */ Instructions.AND_ImmAddr_Reg.Instance,
                /* 0xC9 */ Instructions.OR_ImmAddr_Reg.Instance,
                /* 0xCA */ Instructions.NOR_ImmAddr_Reg.Instance,
                /* 0xCB */ Instructions.NAND_ImmAddr_Reg.Instance,
                /* 0xCC */ Instructions.XOR_ImmAddr_Reg.Instance,
                /* 0xCD */ Instructions.SHL_ImmAddr_Reg.Instance,
                /* 0xCE */ Instructions.SHR_ImmAddr_Reg.Instance,
                /* 0xCF */ Instructions.CMP_ImmAddr_Reg.Instance,
                /* 0xD0 */ Instructions.TEST_ImmAddr_Reg.Instance,
                /* 0xD1 */ Exceptions.BadOpcode.Instance,
                /* 0xD2 */ Exceptions.BadOpcode.Instance,
                /* 0xD3 */ Exceptions.BadOpcode.Instance,
                /* 0xD4 */ Exceptions.BadOpcode.Instance,
                /* 0xD5 */ Exceptions.BadOpcode.Instance,
                /* 0xD6 */ Instructions.JMP_ImmAddr.Instance,
                /* 0xD7 */ Instructions.JZ_ImmAddr.Instance,
                /* 0xD8 */ Instructions.JNZ_ImmAddr.Instance,
                /* 0xD9 */ Exceptions.BadOpcode.Instance,
                /* 0xDA */ Exceptions.BadOpcode.Instance,
                /* 0xDB */ Exceptions.BadOpcode.Instance,
                /* 0xDC */ Exceptions.BadOpcode.Instance,
                /* 0xDD */ Exceptions.BadOpcode.Instance,
                /* 0xDE */ Exceptions.BadOpcode.Instance,
                /* 0xDF */ Exceptions.BadOpcode.Instance,
                /* 0xE0 */ Exceptions.BadOpcode.Instance,
                /* 0xE1 */ Exceptions.BadOpcode.Instance,
                /* 0xE2 */ Exceptions.BadOpcode.Instance,
                /* 0xE3 */ Exceptions.BadOpcode.Instance,
                /* 0xE4 */ Exceptions.BadOpcode.Instance,
                /* 0xE5 */ Exceptions.BadOpcode.Instance,
                /* 0xE6 */ Exceptions.BadOpcode.Instance,
                /* 0xE7 */ Exceptions.BadOpcode.Instance,
                /* 0xE8 */ Exceptions.BadOpcode.Instance,
                /* 0xE9 */ Exceptions.BadOpcode.Instance,
                /* 0xEA */ Exceptions.BadOpcode.Instance,
                /* 0xEB */ Exceptions.BadOpcode.Instance,
                /* 0xEC */ Exceptions.BadOpcode.Instance,
                /* 0xED */ Exceptions.BadOpcode.Instance,
                /* 0xEE */ Exceptions.BadOpcode.Instance,
                /* 0xEF */ Exceptions.BadOpcode.Instance,
                /* 0xF0 */ Exceptions.BadOpcode.Instance,
                /* 0xF1 */ Exceptions.BadOpcode.Instance,
                /* 0xF2 */ Exceptions.BadOpcode.Instance,
                /* 0xF3 */ Exceptions.BadOpcode.Instance,
                /* 0xF4 */ Exceptions.BadOpcode.Instance,
                /* 0xF5 */ Exceptions.BadOpcode.Instance,
                /* 0xF6 */ Exceptions.BadOpcode.Instance,
                /* 0xF7 */ Exceptions.BadOpcode.Instance,
                /* 0xF8 */ Exceptions.BadOpcode.Instance,
                /* 0xF9 */ Exceptions.BadOpcode.Instance,
                /* 0xFA */ Exceptions.BadOpcode.Instance,
                /* 0xFB */ Exceptions.BadOpcode.Instance,
                /* 0xFC */ Exceptions.BadOpcode.Instance,
                /* 0xFD */ Exceptions.BadOpcode.Instance,
                /* 0xFE */ Exceptions.BadOpcode.Instance,
                /* 0xFF */ Exceptions.BadOpcode.Instance
            };

            for (int i = 0; i < InstructionArray.Length; ++i) {
                var instruction = InstructionArray[i];

                if (instruction is not null) {
                    if (instruction.Opcode is null) {
                        instruction.Opcode = (byte)i;
                    }
                    else if (instruction != Exceptions.BadOpcode.Instance) {
                        throw new Exception($"Instruction already assigned to opcode {instruction.Opcode}");
                    }
                }
            }


        }
    }

}
