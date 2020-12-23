using System;
using Tortilla;
using System.Runtime.InteropServices;

/* One thing that you'll notice upon reading the code, especially if you have any familiarity with C# and 
with .NET in general, is that I don't use a lot of properties, but rather fields. I did this for speed and 
simplicity. Inside a CPU, and in assembly code in general, there's not a lot of information hiding, though 
there is encapsulation to some degree. My primary concern in this code is not to write enterprise-grade 
interfaces, but rather to get raw speed wherever I can. I'm sacrificing some sacred OO principles in pursuit 
of performance, and inside the ugly innards of the CPU implementation, it's probably worth it. */

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

    public class Register : Tortilla.IBusComponent {
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
            EnableSubRegisterMask = Instruction.SubRegisterMaskMap[(int)EnableSubRegister];
            EnableOffset = OffsetMap[(int)EnableSubRegister];
            RequestTickEnableToAddressBus?.Invoke(this);
        }

        public void EnableToDataBus(SubRegister subReg) {
            EnableSubRegister = subReg;
            EnableSubRegisterMask = Instruction.SubRegisterMaskMap[(int)EnableSubRegister];
            EnableOffset = OffsetMap[(int)EnableSubRegister];
            RequestTickEnableToDataBus?.Invoke(this);
        }

        public void EnableToIOBus(SubRegister subReg) {
            EnableSubRegister = subReg;
            EnableSubRegisterMask = Instruction.SubRegisterMaskMap[(int)EnableSubRegister];
            EnableOffset = OffsetMap[(int)EnableSubRegister];
            RequestTickEnableToIOBus?.Invoke(this);
        }


        public void SetFromAddressBus(SubRegister subReg) {
            SetSubRegister = subReg;
            SetSubRegisterMask = Instruction.SubRegisterMaskMap[(int)SetSubRegister];
            SetOffset = OffsetMap[(int)SetSubRegister];
            RequestTickSetFromAddressBus?.Invoke(this);
        }

        public void SetFromDataBus(SubRegister subReg) {
            SetSubRegister = subReg;
            SetSubRegisterMask = Instruction.SubRegisterMaskMap[(int)SetSubRegister];
            SetOffset = OffsetMap[(int)SetSubRegister];
            RequestTickSetFromDataBus?.Invoke(this);
        }

        public void SetFromIOBus(SubRegister subReg) {
            SetSubRegister = subReg;
            SetSubRegisterMask = Instruction.SubRegisterMaskMap[(int)SetSubRegister];
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
                var regFlags = cpuFlags as Register;
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

}
