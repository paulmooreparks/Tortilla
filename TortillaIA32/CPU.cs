using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tortilla {
    public interface IHardware {
        byte Read8(UInt32 address);
        void Write8(UInt32 address, byte value);
        void Debug(string disasm, object o);
        void RaiseException(byte id);
        void RaiseInterrupt(byte id);
        void PowerOff();
    }

    public interface ICpu {
        void Run(IHardware hardware);
        void RaiseInterrupt(byte id);
        int ClockRate { get; }
        void PowerOff();
    }

    public interface ICpuMode {
        void Decode(byte opCode);

        byte ReadImm8();
        UInt16 ReadImm16();
        UInt32 ReadImm32();

        byte Read8(UInt32 Address);
        UInt16 Read16(UInt32 Address);
        UInt32 Read32(UInt32 Address);

        void Write8(UInt32 Address, byte Value);
        void Write16(UInt32 Address, UInt16 value);
        void Write32(UInt32 Address, UInt32 value);

        void Push8(byte value);
        void Push16(UInt16 value);
        void Push32(UInt32 value);

        byte Pop8();
        UInt16 Pop16();
        UInt32 Pop32();
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


    public class IA32 : ICpu {
        public IA32() {
            ConnectOpCodesToMethods();
            EFLAGS = 0x00000010;
        }

        protected UInt32[] generalRegisters = new UInt32[8];
        protected UInt16[] segmentRegisters = new UInt16[6];
        protected UInt32[] statusRegisters = new UInt32[2];

        AutoResetEvent interruptEvent = new AutoResetEvent(false);
        ManualResetEvent powerEvent = new ManualResetEvent(false);

        public void RaiseInterrupt(byte id) {
            interruptEvent.Set();
        }

        const int EAX_INDEX = 0;
        const int ECX_INDEX = 1;
        const int EDX_INDEX = 2;
        const int EBX_INDEX = 3;
        const int ESP_INDEX = 4;
        const int EBP_INDEX = 5;
        const int ESI_INDEX = 6;
        const int EDI_INDEX = 7;

        public UInt32 EAX {
            get { return generalRegisters[EAX_INDEX]; }
            set { generalRegisters[EAX_INDEX] = value; }
        }

        public UInt32 ECX {
            get { return generalRegisters[ECX_INDEX]; }
            set { generalRegisters[ECX_INDEX] = value; }
        }

        public UInt32 EDX {
            get { return generalRegisters[EDX_INDEX]; }
            set { generalRegisters[EDX_INDEX] = value; }
        }

        public UInt32 EBX {
            get { return generalRegisters[EBX_INDEX]; }
            set { generalRegisters[EBX_INDEX] = value; }
        }

        public UInt32 ESP {
            get { return generalRegisters[ESP_INDEX]; }
            set { generalRegisters[ESP_INDEX] = value; }
        }

        public UInt32 EBP {
            get { return generalRegisters[EBP_INDEX]; }
            set { generalRegisters[EBP_INDEX] = value; }
        }

        public UInt32 ESI {
            get { return generalRegisters[ESI_INDEX]; }
            set { generalRegisters[ESI_INDEX] = value; }
        }

        public UInt32 EDI {
            get { return generalRegisters[EDI_INDEX]; }
            set { generalRegisters[EDI_INDEX] = value; }
        }


        const int ES_INDEX = 0;
        const int CS_INDEX = 1;
        const int SS_INDEX = 2;
        const int DS_INDEX = 3;
        const int FS_INDEX = 4;
        const int GS_INDEX = 5;

        public UInt16 ES {
            get { return segmentRegisters[ES_INDEX]; }
            set { segmentRegisters[ES_INDEX] = value; }
        }

        public UInt16 CS {
            get { return segmentRegisters[CS_INDEX]; }
            set { segmentRegisters[CS_INDEX] = value; }
        }

        public UInt16 SS {
            get { return segmentRegisters[SS_INDEX]; }
            set { segmentRegisters[SS_INDEX] = value; }
        }

        public UInt16 DS {
            get { return segmentRegisters[DS_INDEX]; }
            set { segmentRegisters[DS_INDEX] = value; }
        }

        public UInt16 FS {
            get { return segmentRegisters[FS_INDEX]; }
            set { segmentRegisters[FS_INDEX] = value; }
        }

        public UInt16 GS {
            get { return segmentRegisters[GS_INDEX]; }
            set { segmentRegisters[GS_INDEX] = value; }
        }


        public UInt32 EFLAGS {
            get { return statusRegisters[0]; }
            set { statusRegisters[0] = value; }
        }

        public UInt32 EIP {
            get { return statusRegisters[1]; }
            set { statusRegisters[1] = value; }
        }

        public UInt32 CR0 { get; set; } = 0;
        public UInt32 CR1 { get; set; } = 0;
        public UInt32 CR2 { get; set; } = 0;
        public UInt32 CR3 { get; set; } = 0;
        public UInt32 CR4 { get; set; } = 0;

        public UInt16 AX {
            get { return (UInt16)(EAX & 0xFFFF); }
            set { EAX = (UInt32)(value & 0xFFFF); }
        }

        public byte AL {
            get { return (byte)(EAX & 0x00FF); }
            set { EAX = (UInt32)(value & 0x00FF); }
        }

        public byte AH {
            get { return (byte)(EAX & 0xFF00); }
            set { EAX = (UInt32)(value & 0xFF00); }
        }

        public UInt16 BX {
            get { return (UInt16)(EBX & 0xFFFF); }
            set { EBX = (UInt32)(value & 0xFFFF); }
        }

        public byte BL {
            get { return (byte)(EBX & 0x00FF); }
            set { EBX = (UInt32)(value & 0x00FF); }
        }

        public byte BH {
            get { return (byte)(EBX & 0xFF00); }
            set { EBX = (UInt32)(value & 0xFF00); }
        }

        public UInt16 CX {
            get { return (UInt16)(ECX & 0xFFFF); }
            set { ECX = (UInt32)(value & 0xFFFF); }
        }

        public byte CL {
            get { return (byte)(ECX & 0x00FF); }
            set { ECX = (UInt32)(value & 0x00FF); }
        }

        public byte CH {
            get { return (byte)(ECX & 0xFF00); }
            set { ECX = (UInt32)(value & 0xFF00); }
        }

        public UInt16 DX {
            get { return (UInt16)(EDX & 0xFFFF); }
            set { EDX = (UInt32)(value & 0xFFFF); }
        }

        public byte DL {
            get { return (byte)(EDX & 0x00FF); }
            set { EDX = (UInt32)(value & 0x00FF); }
        }

        public byte DH {
            get { return (byte)(EDX & 0xFF00); }
            set { EDX = (UInt32)(value & 0xFF00); }
        }

        public UInt16 BP {
            get { return (UInt16)(EBP & 0xFFFF); }
            set { EBP = (UInt32)(value & 0xFFFF); }
        }

        public UInt16 SI {
            get { return (UInt16)(ESI & 0xFFFF); }
            set { ESI = (UInt32)(value & 0xFFFF); }
        }

        public UInt16 DI {
            get { return (UInt16)(EDI & 0xFFFF); }
            set { EDI = (UInt32)(value & 0xFFFF); }
        }

        public UInt16 SP {
            get { return (UInt16)(ESP & 0xFFFF); }
            set { ESP = (UInt32)(value & 0xFFFF); }
        }


        public UInt32 CF {
            get { return EFLAGS & 0x00000001; }
            set { EFLAGS = (value & 0x01); }
        }

        public UInt32 PF {
            get { return EFLAGS & 0x00000004; }
            set { EFLAGS |= (value & 0x01) << 2; }
        }

        public UInt32 AF {
            get { return EFLAGS & 0x00000010; }
            set { EFLAGS |= (value & 0x01) << 4; }
        }

        public UInt32 ZF {
            get { return EFLAGS & 0x00000040; }
            set { EFLAGS |= (value & 0x01) << 6; }
        }

        public UInt32 SF {
            get { return EFLAGS & 0x00000080; }
            set { EFLAGS |= (value & 0x01) << 7; }
        }

        public UInt32 TF {
            get { return EFLAGS & 0x00000100; }
            set { EFLAGS |= (value & 0x01) << 8; }
        }

        public UInt32 IF {
            get { return EFLAGS & 0x00000200; }
            set { EFLAGS |= (value & 0x01) << 9; }
        }

        public UInt32 DF {
            get { return EFLAGS & 0x00000400; }
            set { EFLAGS |= (value & 0x01) << 10; }
        }

        public UInt32 OF {
            get { return EFLAGS & 0x00000800; }
            set { EFLAGS |= (value & 0x01) << 11; }
        }

        public UInt32 IOPL {
            get { return EFLAGS & 0x00003000; }
            set { EFLAGS |= (value & 0x11) << 12; }
        }

        public UInt32 NT {
            get { return EFLAGS & 0x00004000; }
            set { EFLAGS |= (value & 0x01) << 14; }
        }

        public UInt32 RF {
            get { return EFLAGS & 0x00010000; }
            set { EFLAGS |= (value & 0x01) << 16; }
        }

        public UInt32 VM {
            get { return EFLAGS & 0x00020000; }
            set { EFLAGS |= (value & 0x01) << 17; }
        }

        public UInt32 PE {
            get { return CR0 & 0x00000001; }
            set { CR0 |= (value & 0x01); }
        }

        public IHardware Hardware { get; set; }

        public int ClockRate { get; set; } = 10000000; // 10MHz

        public int Cycles { get; set; } = 0;




        protected delegate void OpCodeDelegate();
        protected Dictionary<byte, OpCodeDelegate> OpCodeMap { get; } = new Dictionary<byte, OpCodeDelegate>();

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
                                OpCodeMap[opCode] = (OpCodeDelegate)OpCodeDelegate.CreateDelegate(typeof(OpCodeDelegate), this, method);
                            }
                        }
                    }
                }
            }
        }


        // Timer timer = new Timer()

        bool Tick() {
            --Cycles;
            return Cycles != 0;
        }

        string[] regArray16 = { "AX", "CX", "DX", "BX", "SP", "BP", "SI", "DI" };
        string[] regArray32 = { "EAX", "ECX", "EDX", "EBX", "ESP", "EBP", "ESI", "EDI" };
        string[] segArray = { "ES", "CS", "SS", "DS", "FS", "GS" };

        int Flags { get; set; }

        const int REAL_MODE = 0x01;
        const int ADDR_SIZE_OVERRIDE = 0x02;
        const int REG_SIZE_OVERRIDE = 0x04;

        const int DEFAULT_SEGMENT_SELECT = GS_INDEX;
        int segSelect = DEFAULT_SEGMENT_SELECT;

        string dbgAddress = "";
        string dbgSegSelect = "";

        public void Run(IHardware hardware) {
            Hardware = hardware;
            Flags = 0;
            CS = 0xF000;
            EIP = 0xFFF0;
            SS = CS;

            for (;;) {
                var oldPE = PE;
                segSelect = DEFAULT_SEGMENT_SELECT;
                dbgAddress = string.Format("{0:X8}", (CS << 4) + EIP);
                dbgSegSelect = "";
                Decode(ReadImm8());

                Flags = 0;

                if (oldPE != PE) {
                    if (PE == 1) {
                        Flags |= 1;
                    }
                    else {
                        Flags &= ~(1 << 0);
                    }
                }

                if (powerEvent.WaitOne(0)) {
                    return;
                }
            }
        }

        protected void Decode(byte opCode) {
            OpCodeDelegate handler = null;

            if (OpCodeMap.TryGetValue(opCode, out handler) && handler != null) {
                handler();
            }
            else {
                DbgIns(string.Format("#GP(0) Unknown instruction: {0:X}", opCode));
                Hardware.RaiseException(0);
                Hlt();
            }
        }

        UInt32 lastResult = 0;
        UInt32 lastAddResult = 0;
        UInt32 lastOp1 = 0;
        UInt32 lastOp2 = 0;
        UInt32 lastOpSize = 0;

        string dbgLastOperand = string.Empty;

        protected UInt16 Subtract16(UInt16 dest, UInt16 source) {
            lastAddResult = dest;
            lastOp2 = source;
            lastOp1 = lastResult = (UInt32)((dest - source) & 0x0000FFFF);
            lastOpSize = 32;
            return (UInt16)(lastResult & 0x0000FFFF);
        }

        protected UInt32 Subtract32(UInt32 dest, UInt32 source) {
            lastAddResult = dest;
            lastOp2 = source;
            lastOp1 = lastResult = dest - source;
            lastOpSize = 32;
            return lastResult;
        }

        protected UInt16 Read16ModRm(ModRm modrm) {
            UInt16 value = 0;

            switch (modrm.mod) {
                case 0x00:
                    value = Read16Mod00(modrm.rm);
                    break;

                case 0x01:
                    value = Read16Mod01(modrm.rm);
                    break;

                case 0x02:
                    value = Read16Mod02(modrm.rm);
                    break;
            }

            return value;
        }

        protected UInt16 Read16ModReg(ModRm modrm) {
            UInt16 value = 0;

            switch (modrm.mod) {
                case 0x00:
                    value = Read16Mod00(modrm.reg);
                    break;

                case 0x01:
                    value = Read16Mod01(modrm.reg);
                    break;

                case 0x02:
                    value = Read16Mod02(modrm.reg);
                    break;
            }

            return value;
        }

        protected UInt32 Read32ModRm(ModRm modrm) {
            UInt32 value = 0;

            switch (modrm.mod) {
                case 0x00:
                    value = Read32Mod00(modrm.rm);
                    break;

                case 0x01:
                    value = Read32Mod01(modrm.rm);
                    break;

                case 0x02:
                    value = Read32Mod02(modrm.rm);
                    break;
            }

            return value;
        }

        protected UInt32 Read32ModReg(ModRm modrm) {
            UInt32 value = 0;

            switch (modrm.mod) {
                case 0x00:
                    value = Read32Mod00(modrm.reg);
                    break;

                case 0x01:
                    value = Read32Mod01(modrm.reg);
                    break;

                case 0x02:
                    value = Read32Mod02(modrm.reg);
                    break;
            }

            return value;
        }

        private uint CalcSib(SIB sib, UInt32 displacement) {
            uint value;
            if (sib.index == 0x04) {
                value = (UInt32)(generalRegisters[sib.index] + displacement);
                dbgLastOperand = string.Format("[{0}{1}]", regArray32[sib.index], dbgLastOperand);
            }
            else {
                var scale = 1 << sib.scale;
                var dbgScale = "";

                if (scale > 1) {
                    dbgScale = string.Format(" * {0}", scale);
                }

                value = (UInt32)(generalRegisters[sib.bse] + generalRegisters[sib.bse] * scale + displacement);
                dbgLastOperand = string.Format("[{0} + {1}{2}{3}]", regArray32[sib.bse], regArray32[sib.index], dbgScale, dbgLastOperand);
            }

            return value;
        }

        protected UInt16 Read16Mod00(int sel) {
            UInt16 value = 0;

            switch (sel) {
                case 0x00:
                    value = (UInt16)(BX + SI);
                    dbgLastOperand = "[BX + SI]";
                    break;

                case 0x01:
                    value = (UInt16)(BX + DI);
                    dbgLastOperand = "[BX + DI]";
                    break;

                case 0x02:
                    value = (UInt16)(BP + SI);
                    dbgLastOperand = "[BP + SI]";
                    break;

                case 0x03:
                    value = (UInt16)(BP + DI);
                    dbgLastOperand = "[BP + DI]";
                    break;

                case 0x04:
                    value = (UInt16)(SI);
                    dbgLastOperand = "[SI]";
                    break;

                case 0x05:
                    value = (UInt16)(DI);
                    dbgLastOperand = "[DI]";
                    break;

                case 0x06:
                    value = ReadImm16();
                    dbgLastOperand = string.Format("0x{0:X4}", value);
                    break;

                case 0x07:
                    value = (UInt16)(BX);
                    dbgLastOperand = "[BX]";
                    break;
            }

            return value;
        }

        protected UInt32 Read32Mod00(int sel) {
            UInt32 value = 0;

            if (sel == 0x06) {
                value = ReadImm16();
                dbgLastOperand = string.Format("0x{0:X4}", value);
                return value;
            }

            SIB sib = new SIB();

            if (sel == 0x04) {
                sib = ReadSIB();
            }

            byte displacement = ReadImm8();

            if (displacement > 0) {
                dbgLastOperand = string.Format(" + 0x{0:X2}", displacement);
            }

            switch (sel) {
                case 0x00:
                    value = (UInt32)(EAX);
                    dbgLastOperand = string.Format("[EAX{0}]", dbgLastOperand);
                    break;

                case 0x01:
                    value = (UInt32)(ECX);
                    dbgLastOperand = string.Format("[ECX{0}]", dbgLastOperand);
                    break;

                case 0x02:
                    value = (UInt32)(EDX);
                    dbgLastOperand = string.Format("[EDX{0}]", dbgLastOperand);
                    break;

                case 0x03:
                    value = (UInt32)(EBX);
                    dbgLastOperand = string.Format("[EBX{0}]", dbgLastOperand);
                    break;

                case 0x04:
                    value = CalcSib(sib, displacement);
                    break;

                case 0x05:
                    value = (UInt32)(EBP);
                    dbgLastOperand = string.Format("[EBP{0}]", dbgLastOperand);
                    break;

                case 0x07:
                    value = (UInt32)(EDI);
                    dbgLastOperand = string.Format("[EDI{0}]", dbgLastOperand);
                    break;
            }

            return value;
        }

        protected UInt16 Read16Mod01(int sel) {
            UInt16 value = 0;
            byte displacement = ReadImm8();

            if (displacement > 0) {
                dbgLastOperand = string.Format(" + 0x{0:X2}", displacement);
            }

            switch (sel) {
                case 0x00:
                    value = (UInt16)(BX + SI + displacement);
                    dbgLastOperand = string.Format("[BX + SI{0}]", dbgLastOperand);
                    break;

                case 0x01:
                    value = (UInt16)(BX + DI + displacement);
                    dbgLastOperand = string.Format("[BX + DI{0}]", dbgLastOperand);
                    break;

                case 0x02:
                    value = (UInt16)(BP + SI + displacement);
                    dbgLastOperand = string.Format("[BP + SI{0}]", dbgLastOperand);
                    break;

                case 0x03:
                    value = (UInt16)(BP + DI + displacement);
                    dbgLastOperand = string.Format("[BP + DI{0}]", dbgLastOperand);
                    break;

                case 0x04:
                    value = (UInt16)(SI + displacement);
                    dbgLastOperand = string.Format("[SI{0}]", dbgLastOperand);
                    break;

                case 0x05:
                    value = (UInt16)(DI + displacement);
                    dbgLastOperand = string.Format("[DI{0}]", dbgLastOperand);
                    break;

                case 0x06:
                    value = (UInt16)(BP + displacement);
                    dbgLastOperand = string.Format("[BP{0}]", dbgLastOperand);
                    break;

                case 0x07:
                    value = (UInt16)(BX + displacement);
                    dbgLastOperand = string.Format("[BX{0}]", dbgLastOperand);
                    break;
            }

            return value;
        }

        protected UInt32 Read32Mod01(int sel) {
            UInt32 value = 0;
            SIB sib = new SIB();

            if (sel == 0x04) {
                sib = ReadSIB();
            }

            byte displacement = ReadImm8();

            if (displacement > 0) {
                dbgLastOperand = string.Format(" + 0x{0:X2}", displacement);
            }

            switch (sel) {
                case 0x00:
                    value = (UInt32)(EAX + displacement);
                    dbgLastOperand = string.Format("[EAX{0}]", dbgLastOperand);
                    break;

                case 0x01:
                    value = (UInt32)(ECX + displacement);
                    dbgLastOperand = string.Format("[ECX{0}]", dbgLastOperand);
                    break;

                case 0x02:
                    value = (UInt32)(EDX + displacement);
                    dbgLastOperand = string.Format("[EDX{0}]", dbgLastOperand);
                    break;

                case 0x03:
                    value = (UInt32)(EBX + displacement);
                    dbgLastOperand = string.Format("[EBX{0}]", dbgLastOperand);
                    break;

                case 0x04:
                    value = CalcSib(sib, displacement);
                    break;

                case 0x05:
                    value = (UInt32)(EBP + displacement);
                    dbgLastOperand = string.Format("[EBP{0}]", dbgLastOperand);
                    break;

                case 0x06:
                    value = (UInt32)(ESI + displacement);
                    dbgLastOperand = string.Format("[ESI{0}]", dbgLastOperand);
                    break;

                case 0x07:
                    value = (UInt32)(EDI + displacement);
                    dbgLastOperand = string.Format("[EDI{0}]", dbgLastOperand);
                    break;
            }

            return value;
        }

        protected UInt16 Read16Mod02(int sel) {
            UInt16 value = 0;
            UInt16 displacement = ReadImm16();

            if (displacement > 0) {
                dbgLastOperand = string.Format(" + 0x{0:X4}", displacement);
            }

            switch (sel) {
                case 0x00:
                    value = (UInt16)(BX + SI + displacement);
                    dbgLastOperand = string.Format("[BX + SI{0}]", dbgLastOperand);
                    break;

                case 0x01:
                    value = (UInt16)(BX + DI + displacement);
                    dbgLastOperand = string.Format("[BX + DI{0}]", dbgLastOperand);
                    break;

                case 0x02:
                    value = (UInt16)(BP + SI + displacement);
                    dbgLastOperand = string.Format("[BP + SI{0}]", dbgLastOperand);
                    break;

                case 0x03:
                    value = (UInt16)(BP + DI + displacement);
                    dbgLastOperand = string.Format("[BP + DI{0}]", dbgLastOperand);
                    break;

                case 0x04:
                    value = (UInt16)(SI + displacement);
                    dbgLastOperand = string.Format("[SI{0}]", dbgLastOperand);
                    break;

                case 0x05:
                    value = (UInt16)(DI + displacement);
                    dbgLastOperand = string.Format("[DI]{0}", dbgLastOperand);
                    break;

                case 0x06:
                    value = (UInt16)(BP + displacement);
                    dbgLastOperand = string.Format("[BP{0}]", dbgLastOperand);
                    break;

                case 0x07:
                    value = (UInt16)(BX + displacement);
                    dbgLastOperand = string.Format("[BX{0}]", dbgLastOperand);
                    break;
            }

            return value;
        }

        protected UInt32 Read32Mod02(int sel) {
            UInt32 value = 0;
            SIB sib = new SIB();

            if (sel == 0x04) {
                sib = ReadSIB();
            }

            UInt32 displacement = ReadImm8();

            if (displacement > 0) {
                dbgLastOperand = string.Format(" + 0x{0:X8}", displacement);
            }

            switch (sel) {
                case 0x00:
                    value = (UInt32)(EAX + displacement);
                    dbgLastOperand = string.Format("[EAX{0}]", dbgLastOperand);
                    break;

                case 0x01:
                    value = (UInt32)(ECX + displacement);
                    dbgLastOperand = string.Format("[ECX{0}]", dbgLastOperand);
                    break;

                case 0x02:
                    value = (UInt32)(EDX + displacement);
                    dbgLastOperand = string.Format("[EDX{0}]", dbgLastOperand);
                    break;

                case 0x03:
                    value = (UInt32)(EBX + displacement);
                    dbgLastOperand = string.Format("[EBX{0}]", dbgLastOperand);
                    break;

                case 0x04:
                    value = CalcSib(sib, displacement);
                    break;

                case 0x05:
                    value = (UInt32)(EBP + displacement);
                    dbgLastOperand = string.Format("[EBP{0}]", dbgLastOperand);
                    break;

                case 0x06:
                    value = (UInt32)(ESI + displacement);
                    dbgLastOperand = string.Format("[ESI{0}]", dbgLastOperand);
                    break;

                case 0x07:
                    value = (UInt32)(EDI + displacement);
                    dbgLastOperand = string.Format("[EDI{0}]", dbgLastOperand);
                    break;
            }

            return value;
        }

        protected ModRm ReadModRm() {
            return new ModRm(ReadImm8());
        }

        protected SIB ReadSIB() {
            return new SIB(ReadImm8());
        }

        protected byte ReadImm8() {
            byte value = Read8((UInt32)(CS << 4) + EIP);
            ++EIP;
            return value;
        }

        protected UInt16 ReadImm16() {
            UInt16 value = Read16((UInt32)(CS << 4) + EIP);
            EIP += 2;
            return value;
        }

        protected UInt32 ReadImm32() {
            UInt32 value = Read32((UInt32)(CS << 4) + EIP);
            EIP += 4;
            return value;
        }

        protected byte Read8(UInt32 address) {
            return Hardware.Read8(address);
        }

        protected UInt16 Read16(UInt32 address) {
            UInt16 value = Read8(address);
            value |= (UInt16)(Read8(address + 1) << 8);
            return value;
        }

        protected UInt32 Read32(UInt32 address) {
            UInt32 value = Read16(address);
            value |= (UInt32)(Read16(address + 2) << 16);
            return value;
        }

        protected void Write8(UInt32 address, byte value) {
            Hardware.Write8(address, value);
        }

        protected void Write16(UInt32 address, UInt16 value) {
            Write8(address + 0, (byte)(value & 0x00FF));
            Write8(address + 1, (byte)((value & 0xFF00) >> 8));
        }

        protected void Write32(UInt32 address, UInt32 value) {
            Write16(address + 0, (UInt16)(value & 0x0000FFFF));
            Write16(address + 2, (UInt16)((value & 0xFFFF0000) >> 16));
        }

        protected void Push8(byte value) {
            SP -= 2;
            Write16((UInt32)(SS << 4) + ESP, value);
        }

        protected void Push16(UInt16 value) {
            SP -= 2;
            Write16((UInt32)(SS << 4) + ESP, value);
        }

        protected void Push32(UInt32 value) {
            ESP -= 4;
            Write32((UInt32)(SS << 4) + ESP, value);
        }

        protected byte Pop8() {
            var value = Read8((UInt32)(SS << 4) + ESP);
            SP += 2;
            return value;
        }

        protected UInt16 Pop16() {
            var value = Read16((UInt32)(SS << 4) + ESP);
            SP += 2;
            return value;
        }

        protected UInt32 Pop32() {
            var value = Read32((UInt32)(SS << 4) + ESP);
            ESP += 4;
            return value;
        }

        public void PowerOff() {
            interruptEvent.Set();
            powerEvent.Set();
        }

        protected struct ModRm {
            public ModRm(byte modrm) {
                mod = modrm >> 6 & 3;
                reg = modrm >> 3 & 7;
                rm = modrm & 7;
            }

            public int mod { get; }
            public int reg { get; }
            public int rm { get; }
        }

        protected struct SIB {
            public SIB(byte sib) {
                scale = sib >> 6 & 3;
                index = sib >> 3 & 7;
                bse = sib & 7;
            }

            public int scale { get; }
            public int index { get; }
            public int bse { get; }
        }

        UInt16 CalcOffset16(UInt16 offset) {
            return (UInt16)((segmentRegisters[segSelect] << 4) + offset);
        }

        UInt32 CalcOffset32(UInt32 offset) {
            return (UInt32)(segmentRegisters[segSelect] << 4) + offset;
        }

        void DbgIns(string s) {
            Hardware.Debug(string.Format("{0} {1}", dbgAddress, s), this);
        }


        /*********************************************************************
        OpCodes
        *********************************************************************/

        [OpCode(0x06)]
        void PushES() {
            Push16(ES);
            DbgIns("PUSH ES");
        }


        [OpCode(0x0e)]
        void PushCS() {
            Push16(CS);
            DbgIns("PUSH CS");
        }

        [OpCode(0x0f)]
        void TwoByteOpCode() {
            byte op = ReadImm8();

            switch (op) {
                case 0xA0:
                    Push16(FS);
                    DbgIns("PUSH FS");
                    break;

                case 0xa1:
                    FS = Pop16();
                    DbgIns("POP FS");
                    break;

                case 0xa8:
                    Push16(GS);
                    DbgIns("PUSH GS");
                    break;

                case 0xa9:
                    GS = Pop16();
                    DbgIns("POP GS");
                    break;
            }
        }



        [OpCode(0x16)]
        void PushSS() {
            Push16(SS);
            DbgIns("PUSH SS");
        }

        [OpCode(0x1E)]
        void PushDS() {
            Push16(DS);
            DbgIns("PUSH DS");
        }

        [OpCode(0x26)]
        void ESPrefix() {
            segSelect = ES_INDEX;
            dbgSegSelect = "ES:";
            Decode(ReadImm8());
        }

        [OpCode(0x2E)]
        void CSPrefix() {
            segSelect = CS_INDEX;
            dbgSegSelect = "CS:";
            Decode(ReadImm8());
        }

        // XOR
        [OpCode(0x30)]
        void XorEbGb() {
            Cycles = 1;
            var modrm = ReadModRm();

            if (modrm.mod == 0x03) {
                generalRegisters[modrm.rm] ^= generalRegisters[modrm.reg] & 0x000000FF;
                DbgIns(string.Format("XOR {0}, {1}", regArray16[modrm.rm], regArray16[modrm.reg]));
            }
            else {
                UInt32 address = 0;
                address = Read16ModRm(modrm);

                byte value = 0;
                value = (byte)(generalRegisters[modrm.reg] & 0x000000ff);
                address = CalcOffset32(address);
                var dvalue = Read8(address);
                Write8(address, (byte)(value ^ dvalue));

                DbgIns(string.Format("XOR {0}{1}, {2}", dbgSegSelect, dbgLastOperand, regArray16[modrm.reg]));
            }
        }

        // XOR Ev, Gv
        [OpCode(0x31)]
        void XorEvGv() {
            Cycles = 1;
            var modrm = ReadModRm();

            if (modrm.mod == 0x03) {
                if ((Flags & ADDR_SIZE_OVERRIDE) != 0) {
                    generalRegisters[modrm.rm] ^= generalRegisters[modrm.reg];
                    DbgIns(string.Format("XOR {0}, {1}", regArray32[modrm.rm], regArray32[modrm.reg]));
                }
                else {
                    generalRegisters[modrm.rm] ^= generalRegisters[modrm.reg] & 0x0000FFFF;
                    DbgIns(string.Format("XOR {0}, {1}", regArray16[modrm.rm], regArray16[modrm.reg]));
                }
            }
            else {
                UInt32 address = 0;

                if ((Flags & ADDR_SIZE_OVERRIDE) != 0) {
                    address = Read32ModRm(modrm);
                }
                else {
                    address = Read16ModRm(modrm);
                }

                string src = "";

                if ((Flags & REG_SIZE_OVERRIDE) != 0) {
                    UInt32 value = 0;
                    value = generalRegisters[modrm.reg];
                    address = CalcOffset32(address);
                    UInt32 dvalue = Read32(address);
                    Write32(address, dvalue ^ value);
                    src = regArray32[modrm.reg];
                }
                else {
                    UInt16 value = 0;
                    value = (UInt16)(generalRegisters[modrm.reg] & 0x0000ffff);
                    address = CalcOffset32(address);
                    var dvalue = Read16(address);
                    Write16(address, (UInt16)(dvalue ^ value));
                    src = regArray16[modrm.reg];
                }

                DbgIns(string.Format("XOR {0}{1}, {2}", dbgSegSelect, dbgLastOperand, src));
            }
        }

        // XOR Gb, Eb
        [OpCode(0x32)]
        void XorGbEb() {
            Cycles = 1;
            var modrm = ReadModRm();

            if (modrm.mod == 0x03) {
                generalRegisters[modrm.reg] ^= generalRegisters[modrm.rm] & 0x000000FF;
                DbgIns(string.Format("MOV {0}, {1}", regArray16[modrm.reg], regArray16[modrm.rm]));
            }
            else {
                UInt32 address = 0;
                address = Read16ModReg(modrm);

                byte value = 0;
                value = (byte)(generalRegisters[modrm.rm] & 0x000000ff);
                address = CalcOffset32(address);
                var dvalue = Read8(address);
                Write8(address, (byte)(dvalue ^ value));

                DbgIns(string.Format("MOV {0}{1}, {2}", dbgSegSelect, dbgLastOperand, regArray16[modrm.rm]));
            }
        }

        // XOR Gv, Ev
        [OpCode(0x33)]
        void XorGvEv() {
            Cycles = 1;
            var modrm = ReadModRm();

            if (modrm.mod == 0x03) {
                if ((Flags & ADDR_SIZE_OVERRIDE) != 0) {
                    generalRegisters[modrm.reg] ^= generalRegisters[modrm.rm];
                    DbgIns(string.Format("MOV {0}, {1}", regArray32[modrm.reg], regArray32[modrm.rm]));
                }
                else {
                    generalRegisters[modrm.reg] ^= generalRegisters[modrm.rm] & 0x0000FFFF;
                    DbgIns(string.Format("MOV {0}, {1}", regArray16[modrm.reg], regArray16[modrm.rm]));
                }
            }
            else {
                UInt32 address = 0;
                string dest = "";

                if ((Flags & ADDR_SIZE_OVERRIDE) != 0) {
                    address = generalRegisters[modrm.reg];
                    dest = regArray32[modrm.reg];
                }
                else {
                    address = generalRegisters[modrm.reg] & 0x0000FFFF;
                    dest = regArray16[modrm.reg];
                }

                if ((Flags & REG_SIZE_OVERRIDE) != 0) {
                    UInt32 value = Read32ModRm(modrm);
                    UInt32 dvalue = Read32(address);
                    Write32(address, dvalue ^ CalcOffset32(value));
                }
                else {
                    UInt16 value = Read16ModRm(modrm);
                    UInt16 dvalue = Read16(address);
                    Write16(address, (UInt16)(dvalue ^ CalcOffset16(value)));
                }

                DbgIns(string.Format("XOR {0}, {1}{2}", dest, dbgSegSelect, dbgLastOperand));
            }
        }

        [OpCode(0x36)]
        void SSPrefix() {
            segSelect = SS_INDEX;
            dbgSegSelect = "SS:";
            Decode(ReadImm8());
        }

        // SUB AX, imm16
        // SUB EAX, imm32
        [OpCode(0x2D)]
        void SubAXIv() {
            if ((Flags & ADDR_SIZE_OVERRIDE) != 0) {
                UInt32 value = ReadImm32();
                EAX = Subtract32(EAX, value);
                DbgIns(string.Format("SUB EAX, {0:X8}", value));
            }
            else {
                UInt16 value = ReadImm16();
                AX = Subtract16(AX, value);
                DbgIns(string.Format("SUB AX, {0:X4}", value));
            }
        }

        // CMP AX, imm16
        // CMP EAX, imm32
        [OpCode(0x3D)]
        void CmpAXIv() {
            if ((Flags & ADDR_SIZE_OVERRIDE) != 0) {
                UInt32 value = ReadImm32();
                /*EAX = */
                Subtract32(EAX, value);
                DbgIns(string.Format("CMP EAX, {0:X8}", value));
            }
            else {
                UInt16 value = ReadImm16();
                /*AX = */
                Subtract16(AX, value);
                DbgIns(string.Format("CMP AX, {0:X4}", value));
            }
        }

        [OpCode(0x3E)]
        void DSPrefix() {
            segSelect = DS_INDEX;
            dbgSegSelect = "DS:";
            Decode(ReadImm8());
        }

        // INC
        [OpCode(0x40)]
        void IncAX() {
            if ((Flags & ADDR_SIZE_OVERRIDE) != 0) {
                ++EAX;
                DbgIns("INC EAX");
            }
            else {
                ++AX;
                DbgIns("INC AX");
            }
        }

        // INC
        [OpCode(0x41)]
        void IncCX() {
            if ((Flags & ADDR_SIZE_OVERRIDE) != 0) {
                ++ECX;
                DbgIns("INC ECX");
            }
            else {
                ++CX;
                DbgIns("INC CX");
            }
        }

        // INC
        [OpCode(0x42)]
        void IncDX() {
            if ((Flags & ADDR_SIZE_OVERRIDE) != 0) {
                ++EDX;
                DbgIns("INC EDX");
            }
            else {
                ++DX;
                DbgIns("INC DX");
            }
        }

        // INC
        [OpCode(0x43)]
        void IncBX() {
            if ((Flags & ADDR_SIZE_OVERRIDE) != 0) {
                ++EBX;
                DbgIns("INC EBX");
            }
            else {
                ++BX;
                DbgIns("INC BX");
            }
        }

        // INC
        [OpCode(0x44)]
        void IncSP() {
            if ((Flags & ADDR_SIZE_OVERRIDE) != 0) {
                ++ESP;
                DbgIns("INC ESP");
            }
            else {
                ++SP;
                DbgIns("INC SP");
            }
        }

        // INC
        [OpCode(0x45)]
        void IncBP() {
            if ((Flags & ADDR_SIZE_OVERRIDE) != 0) {
                ++EBP;
                DbgIns("INC EBP");
            }
            else {
                ++BP;
                DbgIns("INC BP");
            }
        }

        // INC
        [OpCode(0x46)]
        void IncSI() {
            if ((Flags & ADDR_SIZE_OVERRIDE) != 0) {
                ++ESI;
                DbgIns("INC ESI");
            }
            else {
                ++SI;
                DbgIns("INC SI");
            }
        }

        // INC
        [OpCode(0x47)]
        void IncDI() {
            if ((Flags & ADDR_SIZE_OVERRIDE) != 0) {
                ++EDI;
                DbgIns("INC EDI");
            }
            else {
                ++DI;
                DbgIns("INC DI");
            }
        }

        // DEC
        [OpCode(0x48)]
        void DecAX() {
            if ((Flags & ADDR_SIZE_OVERRIDE) != 0) {
                --EAX;
                DbgIns("DEC EAX");
            }
            else {
                --AX;
                DbgIns("DEC AX");
            }
        }

        // DEC
        [OpCode(0x49)]
        void DecCX() {
            if ((Flags & ADDR_SIZE_OVERRIDE) != 0) {
                --ECX;
                DbgIns("DEC ECX");
            }
            else {
                --CX;
                DbgIns("DEC CX");
            }
        }

        // DEC
        [OpCode(0x4A)]
        void DecDX() {
            if ((Flags & ADDR_SIZE_OVERRIDE) != 0) {
                --EDX;
                DbgIns("DEC EDX");
            }
            else {
                --DX;
                DbgIns("DEC DX");
            }
        }

        // DEC
        [OpCode(0x4B)]
        void DecBX() {
            if ((Flags & ADDR_SIZE_OVERRIDE) != 0) {
                --EBX;
                DbgIns("DEC EBX");
            }
            else {
                --BX;
                DbgIns("DEC BX");
            }
        }

        // DEC
        [OpCode(0x4C)]
        void DecSP() {
            if ((Flags & ADDR_SIZE_OVERRIDE) != 0) {
                --ESP;
                DbgIns("DEC ESP");
            }
            else {
                --SP;
                DbgIns("DEC SP");
            }
        }

        // DEC
        [OpCode(0x4D)]
        void DecBP() {
            if ((Flags & ADDR_SIZE_OVERRIDE) != 0) {
                --EBP;
                DbgIns("DEC EBP");
            }
            else {
                --BP;
                DbgIns("DEC BP");
            }
        }

        // DEC
        [OpCode(0x4E)]
        void DecSI() {
            if ((Flags & ADDR_SIZE_OVERRIDE) != 0) {
                --ESI;
                DbgIns("DEC ESI");
            }
            else {
                --SI;
                DbgIns("DEC SI");
            }
        }

        // DEC
        [OpCode(0x4F)]
        void DecDI() {
            if ((Flags & ADDR_SIZE_OVERRIDE) != 0) {
                --EDI;
                DbgIns("DEC EDI");
            }
            else {
                --DI;
                DbgIns("DEC DI");
            }
        }

        [OpCode(0x50)]
        void PushAX() {
            Push16(AX);
            DbgIns("PUSH AX");
        }

        [OpCode(0x51)]
        void PushCX() {
            Push16(CX);
            DbgIns("PUSH CX");
        }

        [OpCode(0x52)]
        void PushDX() {
            Push16(DX);
            DbgIns("PUSH DX");
        }

        [OpCode(0x53)]
        void PushBX() {
            Push16(BX);
            DbgIns("PUSH BX");
        }

        [OpCode(0x54)]
        void PushSP() {
            Push16(SP);
            DbgIns("PUSH SP");
        }

        [OpCode(0x55)]
        void PushBP() {
            Push16(BP);
            DbgIns("PUSH BP");
        }

        [OpCode(0x56)]
        void PushSI() {
            Push16(SI);
            DbgIns("PUSH SI");
        }

        [OpCode(0x57)]
        void PushDI() {
            Push16(DI);
            DbgIns("PUSH DI");
        }

        [OpCode(0x58)]
        void PopAX() {
            AX = Pop16();
            DbgIns("POP AX");
        }

        [OpCode(0x59)]
        void PopCX() {
            CX = Pop16();
            DbgIns("POP CX");
        }

        [OpCode(0x5A)]
        void PopDX() {
            DX = Pop16();
            DbgIns("POP DX");
        }

        [OpCode(0x5B)]
        void PopBX() {
            BX = Pop16();
            DbgIns("POP BX");
        }

        [OpCode(0x5c)]
        void PopSP() {
            SP = Pop16();
            DbgIns("POP SP");
        }

        [OpCode(0x5d)]
        void PopBP() {
            BP = Pop16();
            DbgIns("POP BP");
        }

        [OpCode(0x5e)]
        void PopSI() {
            SI = Pop16();
            DbgIns("POP SI");
        }

        [OpCode(0x5f)]
        void PopDI() {
            DI = Pop16();
            DbgIns("POP DI");
        }

        [OpCode(0x64)]
        void FSPrefix() {
            segSelect = FS_INDEX;
            dbgSegSelect = "FS:";
            Decode(ReadImm8());
        }

        [OpCode(0x65)]
        void GSPrefix() {
            segSelect = GS_INDEX;
            dbgSegSelect = "GS:";
            Decode(ReadImm8());
        }

        [OpCode(0x66)]
        void OperandSize() {
            Flags |= REG_SIZE_OVERRIDE;
            Decode(ReadImm8());
        }

        [OpCode(0x67)]
        void AddressSize() {
            Flags |= ADDR_SIZE_OVERRIDE;
            Decode(ReadImm8());
        }

        // PUSH imm16
        [OpCode(0x68)]
        void PushIv() {
            var value = ReadImm16();
            Push16(value);
            DbgIns(string.Format("PUSH 0x{0:X4}", value));
        }

        // PUSH imm8
        [OpCode(0x6A)]
        void PushIb() {
            var value = ReadImm8();
            Push8(value);
            DbgIns(string.Format("PUSH 0x{0:X2}", value));
        }

        // MOV Eb, Gb
        [OpCode(0x88)]
        void MovEbGb() {
            Cycles = 1;
            var modrm = ReadModRm();

            if (modrm.mod == 0x03) {
                generalRegisters[modrm.rm] = generalRegisters[modrm.reg] & 0x000000FF;
                DbgIns(string.Format("MOV {0}, {1}", regArray16[modrm.rm], regArray16[modrm.reg]));
            }
            else {
                UInt32 address = 0;
                address = Read16ModRm(modrm);

                byte value = 0;
                value = (byte)(generalRegisters[modrm.reg] & 0x000000ff);
                address = CalcOffset32(address);
                Write8(address, value);

                DbgIns(string.Format("MOV {0}{1}, {2}", dbgSegSelect, dbgLastOperand, regArray16[modrm.reg]));
            }
        }

        // MOV Ev, Gv
        [OpCode(0x89)]
        void MovEvGv() {
            Cycles = 1;
            var modrm = ReadModRm();

            if (modrm.mod == 0x03) {
                if ((Flags & ADDR_SIZE_OVERRIDE) != 0) {
                    generalRegisters[modrm.rm] = generalRegisters[modrm.reg];
                    DbgIns(string.Format("MOV {0}, {1}", regArray32[modrm.rm], regArray32[modrm.reg]));
                }
                else {
                    generalRegisters[modrm.rm] = generalRegisters[modrm.reg] & 0x0000FFFF;
                    DbgIns(string.Format("MOV {0}, {1}", regArray16[modrm.rm], regArray16[modrm.reg]));
                }
            }
            else {
                UInt32 address = 0;

                if ((Flags & ADDR_SIZE_OVERRIDE) != 0) {
                    address = Read32ModRm(modrm);
                }
                else {
                    address = Read16ModRm(modrm);
                }

                string src = "";

                if ((Flags & REG_SIZE_OVERRIDE) != 0) {
                    UInt32 value = generalRegisters[modrm.reg];
                    Write32(CalcOffset32(address), value);
                    src = regArray32[modrm.reg];
                }
                else {
                    UInt16 value = (UInt16)(generalRegisters[modrm.reg] & 0x0000ffff);
                    address = CalcOffset32(address);
                    Write16(address, value);
                    src = regArray16[modrm.reg];
                }

                DbgIns(string.Format("MOV {0}{1}, {2}", dbgSegSelect, dbgLastOperand, src));
            }
        }

        // MOV Gb, Eb
        [OpCode(0x8A)]
        void MovGbEb() {
            Cycles = 1;
            var modrm = ReadModRm();

            if (modrm.mod == 0x03) {
                generalRegisters[modrm.reg] = generalRegisters[modrm.rm] & 0x000000FF;
                DbgIns(string.Format("MOV {0}, {1}", regArray16[modrm.reg], regArray16[modrm.rm]));
            }
            else {
                UInt32 address = 0;
                address = Read16ModReg(modrm);

                byte value = 0;
                value = (byte)(generalRegisters[modrm.rm] & 0x000000ff);
                address = CalcOffset32(address);
                Write8(address, value);

                DbgIns(string.Format("MOV {0}{1}, {2}", dbgSegSelect, dbgLastOperand, regArray16[modrm.rm]));
            }
        }

        // MOV Gv, Ev
        [OpCode(0x8B)]
        void MovGvEv() {
            Cycles = 1;
            var modrm = ReadModRm();

            if (modrm.mod == 0x03) {
                if ((Flags & ADDR_SIZE_OVERRIDE) != 0) {
                    generalRegisters[modrm.reg] = generalRegisters[modrm.rm];
                    DbgIns(string.Format("MOV {0}, {1}", regArray32[modrm.reg], regArray32[modrm.rm]));
                }
                else {
                    generalRegisters[modrm.reg] = generalRegisters[modrm.rm] & 0x0000FFFF;
                    DbgIns(string.Format("MOV {0}, {1}", regArray16[modrm.reg], regArray16[modrm.rm]));
                }
            }
            else {
                string dest = "";

                if ((Flags & ADDR_SIZE_OVERRIDE) != 0) {
                    UInt32 value = Read32ModRm(modrm);
                    if ((Flags & REG_SIZE_OVERRIDE) != 0) {
                        generalRegisters[modrm.reg] = value;
                        dest = regArray32[modrm.reg];
                    }
                    else {
                        generalRegisters[modrm.reg] |= value & 0x0000FFFF;
                        dest = regArray16[modrm.reg];
                    }
                }
                else {
                    UInt16 value = Read16ModRm(modrm);
                    if ((Flags & REG_SIZE_OVERRIDE) != 0) {
                        generalRegisters[modrm.reg] = value;
                        dest = regArray32[modrm.reg];
                    }
                    else {
                        generalRegisters[modrm.reg] |= value;
                        dest = regArray16[modrm.reg];
                    }
                }

                DbgIns(string.Format("MOV {0}, {1}{2}", dest, dbgSegSelect, dbgLastOperand));
            }
        }

        [OpCode(0x8C)]
        void MovEwSw() {
            Cycles = 1;
            var modrm = ReadModRm();

            if (modrm.mod == 0x03) {
                generalRegisters[modrm.rm] = segmentRegisters[modrm.reg];
                DbgIns(string.Format("MOV {0}, {1}", regArray16[modrm.rm], segArray[modrm.reg]));
            }
            else {
                if ((Flags & ADDR_SIZE_OVERRIDE) != 0) {
                    UInt32 address = Read32ModRm(modrm);
                    Write32(CalcOffset32(address), segmentRegisters[modrm.reg]);
                }
                else {
                    UInt16 address = Read16ModRm(modrm);
                    Write16(CalcOffset32(address), segmentRegisters[modrm.reg]);
                }

                DbgIns(string.Format("MOV {0}, {1}{2}", segArray[modrm.reg], dbgSegSelect, dbgLastOperand));
            }
        }

        // LEA Gv, M
        [OpCode(0x8D)]
        void LeaGvM() {
            Cycles = 1;
            var modrm = ReadModRm();

            if ((Flags & REG_SIZE_OVERRIDE) != 0) {
                UInt32 value = Read32ModRm(modrm);
                generalRegisters[modrm.reg] = CalcOffset32(value);
            }
            else {
                UInt16 value = Read16ModRm(modrm);
                generalRegisters[modrm.reg] = (UInt32)(CalcOffset16(value) & 0x0000FFFF);
            }

            DbgIns(string.Format("LEA {0}, {1}{2}", regArray16[modrm.reg], dbgSegSelect, dbgLastOperand));
        }

        [OpCode(0x8e)]
        void MovSwEw() {
            Cycles = 1;
            var modrm = ReadModRm();

            if (modrm.mod == 0x03) {
                segmentRegisters[modrm.reg] = (UInt16)(generalRegisters[modrm.rm] & 0x0000FFFF);
                DbgIns(string.Format("MOV {0}, {1}", segArray[modrm.reg], regArray16[modrm.rm]));
            }
            else {
                if ((Flags & ADDR_SIZE_OVERRIDE) != 0) {
                    UInt32 value = Read32ModRm(modrm);
                    segmentRegisters[modrm.reg] = (UInt16)(CalcOffset32(value) & 0x0000FFFF);
                }
                else {
                    UInt16 value = Read16ModRm(modrm);
                    segmentRegisters[modrm.reg] = CalcOffset16(value);
                }

                DbgIns(string.Format("MOV {0}, {1}{2}", segArray[modrm.reg], dbgSegSelect, dbgLastOperand));
            }

            if (modrm.reg == SS_INDEX) {
                /* Run next instruction without handling interrupts */
                Decode(ReadImm8());
            }
        }

        // NOP
        [OpCode(0x90)]
        void Nop() {
            Cycles = 1;
            DbgIns("NOP");
        }

        // MOV r/m8,r8
        [OpCode(0xB8)]
        void MovRM8R8() {
            Cycles = 1;
            if ((Flags & REG_SIZE_OVERRIDE) != 0) {
                EAX = ReadImm32();
                DbgIns(string.Format("MOV EAX, 0x{0:X8}", EAX));
            }
            else {
                AX = ReadImm16();
                DbgIns(string.Format("MOV AX, 0x{0:X4}", AX));
            }
        }

        [OpCode(0xB9)]
        void MovCXIv() {
            Cycles = 1;
            if ((Flags & REG_SIZE_OVERRIDE) != 0) {
                ECX = ReadImm32();
                DbgIns(string.Format("MOV ECX, 0x{0:X8}", ECX));
            }
            else {
                CX = ReadImm16();
                DbgIns(string.Format("MOV CX, 0x{0:X4}", CX));
            }
        }

        [OpCode(0xBA)]
        void MovDXIv() {
            Cycles = 1;
            if ((Flags & REG_SIZE_OVERRIDE) != 0) {
                DX = ReadImm16();
                DbgIns(string.Format("MOV DX, 0x{0:X4}", DX));
            }
            else {
                DX = ReadImm16();
                DbgIns(string.Format("MOV DX, 0x{0:X4}", DX));
            }
        }

        [OpCode(0xBB)]
        void MovBXIv() {
            Cycles = 1;
            if ((Flags & REG_SIZE_OVERRIDE) != 0) {
                EBX = ReadImm32();
                DbgIns(string.Format("MOV EBX, 0x{0:X8}", EBX));
            }
            else {
                BX = ReadImm16();
                DbgIns(string.Format("MOV BX, 0x{0:X4}", BX));
            }
        }

        // MOV Ev, Iv
        [OpCode(0xC7)]
        void MovEvIv() {
            Cycles = 1;
            var modrm = ReadModRm();

            if (modrm.mod == 0x03) {
                if ((Flags & ADDR_SIZE_OVERRIDE) != 0) {
                    generalRegisters[modrm.rm] = ReadImm32();
                    DbgIns(string.Format("MOV {0}, {1:X8}", regArray32[modrm.rm], generalRegisters[modrm.rm]));
                }
                else {
                    generalRegisters[modrm.rm] = ReadImm16();
                    DbgIns(string.Format("MOV {0}, {1:X4}", regArray16[modrm.rm], generalRegisters[modrm.rm]));
                }
            }
            else {
                UInt32 address = 0;

                if ((Flags & ADDR_SIZE_OVERRIDE) != 0) {
                    address = Read32ModRm(modrm);
                }
                else {
                    address = Read16ModRm(modrm);
                }

                string src = "";
                string size = "";

                if ((Flags & REG_SIZE_OVERRIDE) != 0) {
                    UInt32 value = ReadImm32();
                    Write32(CalcOffset32(address), value);
                    src = string.Format("0x{0:X8}", value);
                    size = "DWORD PTR";
                }
                else {
                    UInt16 value = ReadImm16();
                    address = CalcOffset32(address);
                    Write16(address, value);
                    src = string.Format("0x{0:X4}", value);
                    size = "WORD PTR";
                }

                DbgIns(string.Format("MOV {3} {0}{1}, {2}", dbgSegSelect, dbgLastOperand, src, size));
            }
        }

        // INT imm8
        [OpCode(0xCD)]
        void IntImm8() {
            Cycles = 37;
            var id = ReadImm8();
            IF = 0;
            TF = 0;
            Hardware.RaiseInterrupt(id);
            DbgIns(string.Format("INT 0x{0:X2}", id));
        }

        // HLT
        [OpCode(0xF4)]
        void Hlt() {
            DbgIns("HLT");
            interruptEvent.WaitOne();
            Hardware.Debug("CPU awake", this);
        }

        // JMP b
        [OpCode(0xEB)]
        void Jmpb() {
            var o = ReadImm8();

            if ((o & 0x80) == 0) {
                EIP = EIP + (o);
            }
            else {
                EIP = (UInt32)(EIP - (-o & 0xFF));
            }

            DbgIns(string.Format("JMP 0x{0:X8}", (CS << 4) + EIP));
        }

        // JMP w
        [OpCode(0xE9)]
        void Jmpw() {
            if ((Flags & ADDR_SIZE_OVERRIDE) != 0) {
                var o = ReadImm32();

                if ((o & 0x8000) == 0) {
                    EIP = EIP + (o);
                }
                else {
                    EIP = (UInt32)(EIP - (-o & 0xFFFFFFFF) + 2);
                }
            }
            else {
                var o = ReadImm16();

                if ((o & 0x8000) == 0) {
                    EIP = EIP + (o);
                }
                else {
                    EIP = (UInt32)(EIP - (-o & 0xffff) + 2);
                }
            }

            DbgIns(string.Format("JMP 0x{0:X8}", (CS << 4) + EIP));
        }

        // CLI
        [OpCode(0xFA)]
        void Cli() {
            Cycles = 3;
            IF = 0;
            DbgIns("CLI");
        }

        [OpCode(0xFF)]
        void OpcodeFF() {
            var modrm = ReadModRm();
            switch (modrm.reg) {
                case 6:
                    var value = ReadImm16();
                    Push16(value);
                    DbgIns(string.Format("PUSH 0x{0:X4}", value));
                    break;
            }
        }
    }
}
