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
        byte ReadPort8(UInt16 address);
        void WritePort8(UInt16 address, byte value);
        UInt16 ReadPort16(UInt16 address);
        void WritePort16(UInt16 address, UInt16 value);
        void PowerOff();
    }

    public interface ICpu {
        void PowerOn(IHardware hardware);
        void PowerOff();
        void Reset();
        void RaiseInterrupt(byte id);
        int ClockRate { get; }
        void Break();
        bool SingleStep { get; set; }
        void Continue();
        string RegisterDump { get; }
        bool IsPowerOn { get; }
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
        ManualResetEvent shutdownEvent = new ManualResetEvent(true);
        ManualResetEvent runEvent = new ManualResetEvent(false);

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

        const int AX_INDEX = 0;
        const int CX_INDEX = 1;
        const int DX_INDEX = 2;
        const int BX_INDEX = 3;
        const int SP_INDEX = 4;
        const int BP_INDEX = 5;
        const int SI_INDEX = 6;
        const int DI_INDEX = 7;

        protected UInt32 EAX {
            get { return generalRegisters[EAX_INDEX]; }
            set { generalRegisters[EAX_INDEX] = value; }
        }

        protected UInt32 ECX {
            get { return generalRegisters[ECX_INDEX]; }
            set { generalRegisters[ECX_INDEX] = value; }
        }

        protected UInt32 EDX {
            get { return generalRegisters[EDX_INDEX]; }
            set { generalRegisters[EDX_INDEX] = value; }
        }

        protected UInt32 EBX {
            get { return generalRegisters[EBX_INDEX]; }
            set { generalRegisters[EBX_INDEX] = value; }
        }

        protected UInt32 ESP {
            get { return generalRegisters[ESP_INDEX]; }
            set { generalRegisters[ESP_INDEX] = value; }
        }

        protected UInt32 EBP {
            get { return generalRegisters[EBP_INDEX]; }
            set { generalRegisters[EBP_INDEX] = value; }
        }

        protected UInt32 ESI {
            get { return generalRegisters[ESI_INDEX]; }
            set { generalRegisters[ESI_INDEX] = value; }
        }

        protected UInt32 EDI {
            get { return generalRegisters[EDI_INDEX]; }
            set { generalRegisters[EDI_INDEX] = value; }
        }


        const int ES_INDEX = 0;
        const int CS_INDEX = 1;
        const int SS_INDEX = 2;
        const int DS_INDEX = 3;
        const int FS_INDEX = 4;
        const int GS_INDEX = 5;

        protected UInt16 ES {
            get { return segmentRegisters[ES_INDEX]; }
            set { segmentRegisters[ES_INDEX] = value; }
        }

        protected UInt16 CS {
            get { return segmentRegisters[CS_INDEX]; }
            set { segmentRegisters[CS_INDEX] = value; }
        }

        protected UInt16 SS {
            get { return segmentRegisters[SS_INDEX]; }
            set { segmentRegisters[SS_INDEX] = value; }
        }

        protected UInt16 DS {
            get { return segmentRegisters[DS_INDEX]; }
            set { segmentRegisters[DS_INDEX] = value; }
        }

        protected UInt16 FS {
            get { return segmentRegisters[FS_INDEX]; }
            set { segmentRegisters[FS_INDEX] = value; }
        }

        protected UInt16 GS {
            get { return segmentRegisters[GS_INDEX]; }
            set { segmentRegisters[GS_INDEX] = value; }
        }


        protected UInt32 EFLAGS {
            get { return statusRegisters[0]; }
            set { statusRegisters[0] = value; }
        }

        protected UInt32 EIP {
            get { return statusRegisters[1]; }
            set { statusRegisters[1] = value; }
        }

        protected UInt32 CR0 { get; set; } = 0;
        protected UInt32 CR1 { get; set; } = 0;
        protected UInt32 CR2 { get; set; } = 0;
        protected UInt32 CR3 { get; set; } = 0;
        protected UInt32 CR4 { get; set; } = 0;

        protected UInt32 DR0 { get; set; } = 0;
        protected UInt32 DR1 { get; set; } = 0;
        protected UInt32 DR2 { get; set; } = 0;
        protected UInt32 DR3 { get; set; } = 0;
        protected UInt32 DR4 { get; set; } = 0;
        protected UInt32 DR5 { get; set; } = 0;
        protected UInt32 DR6 { get; set; } = 0;
        protected UInt32 DR7 { get; set; } = 0;


        protected UInt16 AX {
            get { return (UInt16)(EAX & 0xFFFF); }
            set { EAX = (UInt32)(value & 0xFFFF); }
        }

        protected byte AL {
            get { return (byte)(EAX & 0x00FF); }
            set { EAX = (UInt32)(value & 0x00FF); }
        }

        protected byte AH {
            get { return (byte)(EAX & 0xFF00); }
            set { EAX = (UInt32)(value & 0xFF00); }
        }

        protected UInt16 BX {
            get { return (UInt16)(EBX & 0xFFFF); }
            set { EBX = (UInt32)(value & 0xFFFF); }
        }

        protected byte BL {
            get { return (byte)(EBX & 0x00FF); }
            set { EBX = (UInt32)(value & 0x00FF); }
        }

        protected byte BH {
            get { return (byte)(EBX & 0xFF00); }
            set { EBX = (UInt32)(value & 0xFF00); }
        }

        protected UInt16 CX {
            get { return (UInt16)(ECX & 0xFFFF); }
            set { ECX = (UInt32)(value & 0xFFFF); }
        }

        protected byte CL {
            get { return (byte)(ECX & 0x00FF); }
            set { ECX = (UInt32)(value & 0x00FF); }
        }

        protected byte CH {
            get { return (byte)(ECX & 0xFF00); }
            set { ECX = (UInt32)(value & 0xFF00); }
        }

        protected UInt16 DX {
            get { return (UInt16)(EDX & 0xFFFF); }
            set { EDX = (UInt32)(value & 0xFFFF); }
        }

        protected byte DL {
            get { return (byte)(EDX & 0x00FF); }
            set { EDX = (UInt32)(value & 0x00FF); }
        }

        protected byte DH {
            get { return (byte)(EDX & 0xFF00); }
            set { EDX = (UInt32)(value & 0xFF00); }
        }

        protected UInt16 BP {
            get { return (UInt16)(EBP & 0xFFFF); }
            set { EBP = (UInt32)(value & 0xFFFF); }
        }

        protected UInt16 SI {
            get { return (UInt16)(ESI & 0xFFFF); }
            set { ESI = (UInt32)(value & 0xFFFF); }
        }

        protected UInt16 DI {
            get { return (UInt16)(EDI & 0xFFFF); }
            set { EDI = (UInt32)(value & 0xFFFF); }
        }

        protected UInt16 SP {
            get { return (UInt16)(ESP & 0xFFFF); }
            set { ESP = (UInt32)(value & 0xFFFF); }
        }

        protected UInt32 TF {
            get { return (EFLAGS & 0x00000100) >> 8; }
            set { EFLAGS = (UInt32)(EFLAGS & ~(1 << 8) | (value << 8)); }
        }

        protected UInt32 CF {
            get { return (EFLAGS & 0x00000001); }
            set { EFLAGS = (UInt32)(EFLAGS & ~(1 << 0) | (value << 0)); }
        }

        protected UInt32 PF {
            get { return (EFLAGS & 0x00000004) >> 2; }
            set { EFLAGS = (UInt32)(EFLAGS & ~(1 << 2) | (value << 2)); }
        }

        protected UInt32 AF {
            get { return (EFLAGS & 0x00000010) >> 4; }
            set { EFLAGS = (UInt32)(EFLAGS & ~(1 << 4) | (value << 4)); }
        }

        protected UInt32 ZF {
            get { return (EFLAGS & 0x00000040) >> 6; }
            set { EFLAGS = (UInt32)(EFLAGS & ~(1 << 6) | (value << 6)); }
        }

        protected UInt32 SF {
            get { return (EFLAGS & 0x00000080) >> 7; }
            set { EFLAGS = (UInt32)(EFLAGS & ~(1 << 7) | (value << 7)); }
        }

        protected UInt32 IF {
            get { return (EFLAGS & 0x00000200) >> 9; }
            set { EFLAGS = (UInt32)(EFLAGS & ~(1 << 9) | (value << 9)); }
        }

        protected UInt32 DF {
            get { return (EFLAGS & 0x00000400) >> 10; }
            set { EFLAGS = (UInt32)(EFLAGS & ~(1 << 10) | (value << 10)); }
        }

        protected UInt32 OF {
            get { return (EFLAGS & 0x00000800) >> 11; }
            set { EFLAGS = (UInt32)(EFLAGS & ~(1 << 11) | (value << 11)); }
        }

        protected UInt32 IOPL {
            get { return (EFLAGS & 0x00003000) >> 12; }
            set { EFLAGS = (UInt32)(EFLAGS & ~(1 << 12) | (value << 12)); }
        }

        protected UInt32 NT {
            get { return (EFLAGS & 0x00004000) >> 14; }
            set { EFLAGS = (UInt32)(EFLAGS & ~(1 << 14) | (value << 14)); }
        }

        protected UInt32 RF {
            get { return (EFLAGS & 0x00010000) >> 16; }
            set { EFLAGS = (UInt32)(EFLAGS & ~(1 << 16) | (value << 16)); }
        }

        protected UInt32 VM {
            get { return (EFLAGS & 0x00020000) >> 17; }
            set { EFLAGS = (UInt32)(EFLAGS & ~(1 << 17) | (value << 17)); }
        }

        protected UInt32 PE {
            get { return (CR0 & 0x00000001); }
            set { CR0 = (UInt32)(CR0 & ~(1 << 0) | (value << 0)); }
        }

        enum Instruction {
            ADD, PUSH, POP, OR, ADC, SBB, AND, DAA, SUB, DAS, XOR, AAA, CMP, AAS,
            INC, DEC, PUSHA, POPA, BOUND, ARPL, IMUL, INSB, INSW, OUTSB, OUTSW, 
            JO, JNO, JB, JNB, JZ, JNZ, JBE, JA, JS, JNS, JP, JNP, JL, JNL, JLE, JNLE,
            TEST, XCHG, MOV, LEA, NOP, CBW, CWD, CALL, WAIT, PUSHF, POPF, SAHF, LAHF,
            MOVSB, MOVSW, CMPSB, CMPSW, STOSB, STOSW, LODSB, LODSW, SCASB, SCASW, 
            RETN, LES, LDS, ENTER, LEAVE, RETF, INT3, INT, INTO, IRET, 
            AAM, AAD, SALC, XLAT, ESC, LOOPNZ, LOOP, JCXZ, IN, OUT, JMP, INT1, HLT,
            CMC, CLC, STC, CLI, STI, CLD, STD
        }

        public IHardware Hardware { get; set; }

        public int ClockRate { get; set; } = 10000000; // 10MHz

        public int Cycles { get; set; } = 0;




        protected delegate void OpCodeDelegate();
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


        // Timer timer = new Timer()

        bool Tick() {
            --Cycles;
            return Cycles != 0;
        }

        string[] regArray8 = { "AL", "CL", "DL", "BL", "AH", "CH", "DH", "BH" };
        string[] regArray16 = { "AX", "CX", "DX", "BX", "SP", "BP", "SI", "DI" };
        string[] regArray32 = { "EAX", "ECX", "EDX", "EBX", "ESP", "EBP", "ESI", "EDI" };
        string[] segArray = { "ES", "CS", "SS", "DS", "FS", "GS" };

        int Flags { get; set; }

        const int REAL_MODE = 0x01;
        const int ADDR_SIZE_32 = 0x02;
        const int OPERAND_SIZE_32 = 0x04;

        const int DEFAULT_SEGMENT_SELECT = DS_INDEX;
        int segSelect = DEFAULT_SEGMENT_SELECT;

        string _dbgAddress = string.Empty;
        string _dbgImm = string.Empty;
        string _dbgSegSelect = string.Empty;

        public void Break() {
            if (!IsPowerOn || IsHalted) {
                return;
            }

            TF = 1;
            runEvent.Reset();
        }

        public void Step() {
            if (!IsPowerOn || IsHalted) {
                return;
            }

            runEvent.Set();
        }

        public void Continue() {
            if (!IsPowerOn) {
                return;
            }

            runEvent.Set();
        }

        void ClearStatusRegisters() {
            for (var i = 0; i < statusRegisters.Length; ++i) {
                statusRegisters[i] = 0;
            }
        }

        void ClearGeneralRegisters() {
            for (var i = 0; i < generalRegisters.Length; ++i) {
                generalRegisters[i] = 0;
            }
        }

        void ClearSegmentRegisters() {
            for (var i = 0; i < segmentRegisters.Length; ++i) {
                segmentRegisters[i] = 0;
            }
        }

        public bool IsPowerOn { get; protected set; }

        protected bool IsHalted {
            get {
                return !interruptEvent.WaitOne(0);
            }
        }

        public void PowerOn(IHardware hardware) {
            if (IsPowerOn) {
                return;
            }

            shutdownEvent.WaitOne();
            new Thread(new ParameterizedThreadStart(Run)).Start(hardware);
        }

        public void PowerOff() {
            IsPowerOn = false;
            interruptEvent.Set();
        }

        public void Reset() {
            PowerOff();

            shutdownEvent.WaitOne();
            PowerOn(Hardware);
        }

        protected void Run(object o) {
            try {
                if (o == null) {
                    throw new Exception("Null hardware object reference");
                }

                IsPowerOn = true;
                shutdownEvent.Reset();
                Hardware = (IHardware)o;

                var temp = TF;
                ClearGeneralRegisters();
                ClearSegmentRegisters();
                ClearStatusRegisters();
                EFLAGS = 0;
                Flags = REAL_MODE;
                CS = 0xF000;
                EIP = 0xFFF0;
                SS = CS;
                TF = temp;

                while (IsPowerOn) {
                    if (TF == 1) {
                        runEvent.Reset();
                    }

                    var oldPE = PE;
                    segSelect = DEFAULT_SEGMENT_SELECT;
                    ExecuteInstruction();

                    /* For now, restore flags to REAL_MODE after every instruction, until I get 
                    protected mode implemented. */
                    Flags = REAL_MODE;
                    // TODO: Protected mode

                    if (!IsPowerOn) {
                        interruptEvent.Set();
                        return;
                    }

                    if (TF == 1) {
                        runEvent.WaitOne();
                    }

                    if (interruptEvent.WaitOne(0)) {
                        DbgIns("Interrupt event");
                    }
                }
            }
            finally {
                shutdownEvent.Set();
                interruptEvent.Reset();
            }
        }

        protected void ExecuteInstruction() {
            _dbgAddress = $"{(CS << 4) + EIP:X8}";
            _dbgSegSelect = string.Empty;
            _dbgLastOperand = string.Empty;
            DecodeOpcode(ReadImm8());
        }

        protected void DecodeOpcode(byte opcode) {
            OpCodeDelegate handler = OpCodeMap[opcode];

            if (handler != null) {
                handler();
            }
            else {
                DbgIns($"#GP(0) Unknown instruction: {opcode:X}");
                Hardware.RaiseException(0);
                Hlt();
            }
        }

        UInt32 lastResult = 0;
#if false
        UInt32 lastAddResult = 0;
        UInt32 lastOp1 = 0;
        UInt32 lastOp2 = 0;
        UInt32 lastOpSize = 0;
#endif

        string _dbgLastOperand = string.Empty;

        UInt32[] parityTable = new UInt32[256] {
            0,0,0,1,0,1,1,0,0,1,1,0,1,0,0,1,
            0,1,1,0,1,0,0,1,1,0,0,1,0,1,1,0,
            0,1,1,0,1,0,0,1,1,0,0,1,0,1,1,0,
            1,0,0,1,0,1,1,0,0,1,1,0,1,0,0,1,
            0,1,1,0,1,0,0,1,1,0,0,1,0,1,1,0,
            1,0,0,1,0,1,1,0,0,1,1,0,1,0,0,1,
            1,0,0,1,0,1,1,0,0,1,1,0,1,0,0,1,
            0,1,1,0,1,0,0,1,1,0,0,1,0,1,1,0,
            0,1,1,0,1,0,0,1,1,0,0,1,0,1,1,0,
            1,0,0,1,0,1,1,0,0,1,1,0,1,0,0,1,
            1,0,0,1,0,1,1,0,0,1,1,0,1,0,0,1,
            0,1,1,0,1,0,0,1,1,0,0,1,0,1,1,0,
            1,0,0,1,0,1,1,0,0,1,1,0,1,0,0,1,
            0,1,1,0,1,0,0,1,1,0,0,1,0,1,1,0,
            0,1,1,0,1,0,0,1,1,0,0,1,0,1,1,0,
            1,0,0,1,0,1,1,0,0,1,1,0,1,0,0,1
        };

        private void SetFlags(UInt32 dest, UInt32 source, UInt64 result, int size) {
            var dsign = (dest >> (size - 1)) & 0x01;
            var ssign = (source >> (size - 1)) & 0x01;
            var rsign = (result >> (size - 1)) & 0x01;

            CF = (UInt32)((result >> size) & 0x01);
            OF = (UInt32)((rsign != dsign && rsign != ssign) ? 1 : 0);

            if (result.Equals(UInt64.MinValue)) {
                ZF = 1;
            }
            else {
                ZF = 0;
            }

            SF = (UInt32)(((Int32)lastResult < 0) ? 1 : 0);
            PF = parityTable[lastResult & 0xFF];
        }

        private void SetFlags8(byte dest, byte source, UInt16 result) {
            SetFlags(dest, source, result, 8);
        }

        private void SetFlags16(UInt16 dest, UInt16 source, UInt32 result) {
            SetFlags(dest, source, result, 16);
        }

        private void SetFlags32(UInt32 dest, UInt32 source, UInt64 result) {
            SetFlags(dest, source, result, 32);
        }

        byte Move8(ref byte op1, ref byte op2) {
            byte result = (byte)(op2 & 0xFF);
            SetFlags8(op1, op2, result);
            return result;
        }

        UInt16 Move16(ref UInt16 op1, ref UInt16 op2) {
            UInt16 result = (UInt16)(op2 & 0xFFFF);
            SetFlags16(op1, op2, result);
            return result;
        }

        UInt32 Move32(ref UInt32 op1, ref UInt32 op2) {
            UInt32 result = op2;
            SetFlags32(op1, op2, result);
            return result;
        }

        protected byte Subtract8(byte op1, byte op2) {
            UInt16 result = (UInt16)(op1 - op2);
            lastResult = (UInt16)(result & 0x000000FF);
            op1 = (byte)lastResult;
            SetFlags8(op1, op2, result);
            return op1;
        }

        protected UInt16 Subtract16(UInt16 op1, UInt16 op2) {
            UInt32 result = (UInt32)(op1 - op2);
            lastResult = (UInt32)(result & 0x0000FFFF);
            op1 = (UInt16)lastResult;
            SetFlags16(op1, op2, result);
            return op1;
        }

        protected UInt32 Subtract32(UInt32 op1, UInt32 op2) {
            UInt64 result = (UInt64)(op1 - op2);
            lastResult = (UInt32)(result & 0x00000000FFFFFFFF);
            op1 = (UInt32)lastResult;
            SetFlags32(op1, op2, result);
            return op1;
        }

        protected byte SubtractByref8(ref byte op1, ref byte op2) {
            op1 = Subtract8(op1, op2);
            return op1;
        }

        protected UInt16 SubtractByref16(ref UInt16 op1, ref UInt16 op2) {
            op1 = Subtract16(op1, op2);
            return op1;
        }

        protected UInt32 SubtractByref32(ref UInt32 op1, ref UInt32 op2) {
            op1 = Subtract32(op1, op2);
            return op1;
        }

        protected byte Compare8(ref byte op1, ref byte op2) {
            return Subtract8(op1, op2);
        }

        protected UInt16 Compare16(ref UInt16 op1, ref UInt16 op2) {
            return Subtract16(op1, op2);
        }

        protected UInt32 Compare32(ref UInt32 op1, ref UInt32 op2) {
            return Subtract32(op1, op2);
        }

        protected byte Add8(byte op1, byte op2) {
            UInt16 result = (UInt16)(op1 + op2);
            lastResult = (UInt16)(result & 0x000000FF);
            SetFlags8(op1, op2, result);
            return (byte)(lastResult);
        }

        protected UInt16 Add16(UInt16 op1, UInt16 op2) {
            UInt32 result = (UInt32)(op1 + op2);
            lastResult = (UInt32)(result & 0x0000FFFF);
            SetFlags16(op1, op2, result);
            return (UInt16)(lastResult);
        }

        protected UInt32 Add32(UInt32 op1, UInt32 op2) {
            UInt64 result = (UInt64)(op1 + op2);
            lastResult = (UInt32)(result & 0x00000000FFFFFFFF);
            SetFlags32(op1, op2, result);
            return lastResult;
        }

        protected byte AddByref8(ref byte op1, ref byte op2) {
            op1 = Add8(op1, op2);
            return op1;
        }

        protected UInt16 AddByref16(ref UInt16 op1, ref UInt16 op2) {
            op1 = Add16(op1, op2);
            return op1;
        }

        protected UInt32 AddByref32(ref UInt32 op1, ref UInt32 op2) {
            op1 = Add32(op1, op2);
            return op1;
        }

        byte And8(ref byte op1, ref byte op2) {
            byte result = (byte)(op1 & op2);
            SetFlags8(op1, op2, result);
            OF = 0;
            CF = 0;
            return result;
        }

        UInt16 And16(ref UInt16 op1, ref UInt16 op2) {
            UInt16 result = (UInt16)(op1 & op2);
            SetFlags16(op1, op2, result);
            OF = 0;
            CF = 0;
            return result;
        }

        UInt32 And32(ref UInt32 op1, ref UInt32 op2) {
            UInt32 result = (op1 & op2);
            SetFlags32(op1, op2, result);
            OF = 0;
            CF = 0;
            return result;
        }

        byte Or8(ref byte op1, ref byte op2) {
            byte result = (byte)(op1 | op2);
            SetFlags8(op1, op2, result);
            OF = 0;
            CF = 0;
            return result;
        }

        UInt16 Or16(ref UInt16 op1, ref UInt16 op2) {
            UInt16 result = (UInt16)(op1 | op2);
            SetFlags16(op1, op2, result);
            OF = 0;
            CF = 0;
            return result;
        }

        UInt32 Or32(ref UInt32 op1, ref UInt32 op2) {
            UInt32 result = (op1 | op2);
            SetFlags32(op1, op2, result);
            OF = 0;
            CF = 0;
            return result;
        }

        byte Xor8(ref byte op1, ref byte op2) {
            byte result = (byte)(op1 ^ op2);
            SetFlags8(op1, op2, result);
            OF = 0;
            CF = 0;
            return result;
        }

        UInt16 Xor16(ref UInt16 op1, ref UInt16 op2) {
            UInt16 result = (UInt16)(op1 ^ op2);
            SetFlags16(op1, op2, result);
            OF = 0;
            CF = 0;
            return result;
        }

        UInt32 Xor32(ref UInt32 op1, ref UInt32 op2) {
            UInt32 result = (op1 ^ op2);
            SetFlags32(op1, op2, result);
            OF = 0;
            CF = 0;
            return result;
        }

        byte Xchg8(ref byte op1, ref byte op2) {
            byte temp = op2;
            op2 = op1;
            op1 = temp;
            return temp;
        }

        UInt16 Xchg16(ref UInt16 op1, ref UInt16 op2) {
            UInt16 result = (UInt16)(op1 ^ op2);
            SetFlags16(op1, op2, result);
            OF = 0;
            CF = 0;
            return result;
        }

        UInt32 Xchg32(ref UInt32 op1, ref UInt32 op2) {
            UInt32 result = (op1 ^ op2);
            SetFlags32(op1, op2, result);
            OF = 0;
            CF = 0;
            return result;
        }

        byte Not8(ref byte op1, ref byte op2) {
            op1 = (byte)(~op1);
            return op1;
        }

        UInt16 Not16(ref UInt16 op1, ref UInt16 op2) {
            op1 = (UInt16)(~op1);
            return op1;
        }

        UInt32 Not32(ref UInt32 op1, ref UInt32 op2) {
            op1 = (~op1);
            return op1;
        }

        UInt32 Jb() {
            byte offset = ReadImm8();
            UInt32 targetIP = EIP;

            if ((offset & 0x80) == 0) {
                targetIP = targetIP + (offset);
            }
            else {
                targetIP = (UInt32)(targetIP - (-offset & 0xFF));
            }

            return targetIP;
        }

        UInt32 Jz16() {
            UInt16 offset = ReadImm16();
            UInt32 targetIP = EIP;

            if ((offset & 0x8000) == 0) {
                targetIP = targetIP + (offset);
            }
            else {
                targetIP = (UInt32)(targetIP - (-offset & 0xffff) + 2);
            }

            return targetIP;
        }

        UInt32 Jz32() {
            UInt32 offset = ReadImm8();
            UInt32 targetIP = EIP;

            if ((offset & 0x8000) == 0) {
                targetIP = targetIP + (offset);
            }
            else {
                targetIP = (UInt32)(targetIP - (-offset & 0xFFFFFFFF) + 2);
            }

            return targetIP;
        }

        /**********************************************************************
        ModRm utilities
        **********************************************************************/
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
                _dbgLastOperand = $"[{regArray32[sib.index]}{_dbgLastOperand}]";
            }
            else {
                var scale = 1 << sib.scale;
                var dbgScale = string.Empty;

                if (scale > 1) {
                    dbgScale = $" * {scale}";
                }

                value = (UInt32)(generalRegisters[sib.bse] + generalRegisters[sib.bse] * scale + displacement);
                _dbgLastOperand = $"[{regArray32[sib.bse]} + {regArray32[sib.index]}{dbgScale}{_dbgLastOperand}]";
            }

            return value;
        }

        protected UInt16 Read16Mod00(int sel) {
            UInt16 value = 0;

            switch (sel) {
                case 0x00:
                    value = (UInt16)(BX + SI);
                    _dbgLastOperand = "[BX + SI]";
                    break;

                case 0x01:
                    value = (UInt16)(BX + DI);
                    _dbgLastOperand = "[BX + DI]";
                    break;

                case 0x02:
                    value = (UInt16)(BP + SI);
                    _dbgLastOperand = "[BP + SI]";
                    break;

                case 0x03:
                    value = (UInt16)(BP + DI);
                    _dbgLastOperand = "[BP + DI]";
                    break;

                case 0x04:
                    value = (UInt16)(SI);
                    _dbgLastOperand = "[SI]";
                    break;

                case 0x05:
                    value = (UInt16)(DI);
                    if (segSelect != DEFAULT_SEGMENT_SELECT) {
                        segSelect = ES_INDEX;
                    }
                    _dbgLastOperand = "[DI]";
                    break;

                case 0x06:
                    value = ReadImm16();
                    _dbgLastOperand = $"0x{value:X4}";
                    break;

                case 0x07:
                    value = (UInt16)(BX);
                    _dbgLastOperand = "[BX]";
                    break;
            }

            return value;
        }

        protected UInt32 Read32Mod00(int sel) {
            UInt32 value = 0;

            if (sel == 0x06) {
                value = ReadImm16();
                _dbgLastOperand = $"0x{value:X4}";
                return value;
            }

            switch (sel) {
                case EAX_INDEX:
                    value = (UInt32)(EAX);
                    _dbgLastOperand = $"[EAX{_dbgLastOperand}]";
                    break;

                case ECX_INDEX:
                    value = (UInt32)(ECX);
                    _dbgLastOperand = $"[ECX{_dbgLastOperand}]";
                    break;

                case EDX_INDEX:
                    value = (UInt32)(EDX);
                    _dbgLastOperand = $"[EDX{_dbgLastOperand}]";
                    break;

                case EBX_INDEX:
                    value = (UInt32)(EBX);
                    _dbgLastOperand = $"[EBX{_dbgLastOperand}]";
                    break;

                case 0x04: {
                        SIB sib = ReadSIB();

                        byte displacement = ReadImm8();

                        if (displacement > 0) {
                            _dbgLastOperand = $" + 0x{displacement:X2}";
                        }

                        value = CalcSib(sib, displacement);
                    }
                    break;

                case EBP_INDEX:
                    value = (UInt32)(EBP);
                    _dbgLastOperand = $"[EBP{_dbgLastOperand}]";
                    break;

                case EDI_INDEX:
                    value = (UInt32)(EDI);
                    _dbgLastOperand = $"[EDI{_dbgLastOperand}]";
                    break;
            }

            return value;
        }

        protected UInt16 Read16Mod01(int sel) {
            UInt16 value = 0;
            byte displacement = ReadImm8();

            if (displacement > 0) {
                _dbgLastOperand = $" + 0x{displacement:X2}";
            }

            switch (sel) {
                case 0x00:
                    value = (UInt16)(BX + SI + displacement);
                    _dbgLastOperand = $"[BX + SI{_dbgLastOperand}]";
                    break;

                case 0x01:
                    value = (UInt16)(BX + DI + displacement);
                    _dbgLastOperand = $"[BX + DI{_dbgLastOperand}]";
                    break;

                case 0x02:
                    value = (UInt16)(BP + SI + displacement);
                    _dbgLastOperand = $"[BP + SI{_dbgLastOperand}]";
                    break;

                case 0x03:
                    value = (UInt16)(BP + DI + displacement);
                    _dbgLastOperand = $"[BP + DI{_dbgLastOperand}]";
                    break;

                case 0x04:
                    value = (UInt16)(SI + displacement);
                    _dbgLastOperand = $"[SI{_dbgLastOperand}]";
                    break;

                case 0x05:
                    value = (UInt16)(DI + displacement);
                    if (segSelect != DEFAULT_SEGMENT_SELECT) {
                        segSelect = ES_INDEX;
                    }
                    _dbgLastOperand = $"[DI{_dbgLastOperand}]";
                    break;

                case 0x06:
                    value = (UInt16)(BP + displacement);
                    if (segSelect != DEFAULT_SEGMENT_SELECT) {
                        segSelect = SP_INDEX;
                    }
                    _dbgLastOperand = $"[BP{_dbgLastOperand}]";
                    break;

                case 0x07:
                    value = (UInt16)(BX + displacement);
                    _dbgLastOperand = $"[BX{_dbgLastOperand}]";
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
                _dbgLastOperand = $" + 0x{displacement:X2}";
            }

            switch (sel) {
                case 0x00:
                    value = (UInt32)(EAX + displacement);
                    _dbgLastOperand = $"[EAX{_dbgLastOperand}]";
                    break;

                case 0x01:
                    value = (UInt32)(ECX + displacement);
                    _dbgLastOperand = $"[ECX{_dbgLastOperand}]";
                    break;

                case 0x02:
                    value = (UInt32)(EDX + displacement);
                    _dbgLastOperand = $"[EDX{_dbgLastOperand}]";
                    break;

                case 0x03:
                    value = (UInt32)(EBX + displacement);
                    _dbgLastOperand = $"[EBX{_dbgLastOperand}]";
                    break;

                case 0x04:
                    value = CalcSib(sib, displacement);
                    break;

                case 0x05:
                    value = (UInt32)(EBP + displacement);
                    if (segSelect != DEFAULT_SEGMENT_SELECT) {
                        segSelect = SP_INDEX;
                    }
                    _dbgLastOperand = $"[EBP{_dbgLastOperand}]";
                    break;

                case 0x06:
                    value = (UInt32)(ESI + displacement);
                    _dbgLastOperand = $"[ESI{_dbgLastOperand}]";
                    break;

                case 0x07:
                    value = (UInt32)(EDI + displacement);
                    if (segSelect != DEFAULT_SEGMENT_SELECT) {
                        segSelect = ES_INDEX;
                    }
                    _dbgLastOperand = $"[EDI{_dbgLastOperand}]";
                    break;
            }

            return value;
        }

        protected UInt16 Read16Mod02(int sel) {
            UInt16 value = 0;
            UInt16 displacement = ReadImm16();

            if (displacement > 0) {
                _dbgLastOperand = $" + 0x{displacement:X4}";
            }

            switch (sel) {
                case 0x00:
                    value = (UInt16)(BX + SI + displacement);
                    _dbgLastOperand = $"[BX + SI{_dbgLastOperand}]";
                    break;

                case 0x01:
                    value = (UInt16)(BX + DI + displacement);
                    _dbgLastOperand = $"[BX + DI{_dbgLastOperand}]";
                    break;

                case 0x02:
                    value = (UInt16)(BP + SI + displacement);
                    _dbgLastOperand = $"[BP + SI{_dbgLastOperand}]";
                    break;

                case 0x03:
                    value = (UInt16)(BP + DI + displacement);
                    _dbgLastOperand = $"[BP + DI{_dbgLastOperand}]";
                    break;

                case 0x04:
                    value = (UInt16)(SI + displacement);
                    _dbgLastOperand = $"[SI{_dbgLastOperand}]";
                    break;

                case 0x05:
                    value = (UInt16)(DI + displacement);
                    if (segSelect != DEFAULT_SEGMENT_SELECT) {
                        segSelect = ES_INDEX;
                    }
                    _dbgLastOperand = $"[DI]{_dbgLastOperand}";
                    break;

                case 0x06:
                    value = (UInt16)(BP + displacement);
                    if (segSelect != DEFAULT_SEGMENT_SELECT) {
                        segSelect = SP_INDEX;
                    }
                    _dbgLastOperand = $"[BP{_dbgLastOperand}]";
                    break;

                case 0x07:
                    value = (UInt16)(BX + displacement);
                    _dbgLastOperand = $"[BX{_dbgLastOperand}]";
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
                _dbgLastOperand = $" + 0x{displacement:X8}";
            }

            switch (sel) {
                case 0x00:
                    value = (UInt32)(EAX + displacement);
                    _dbgLastOperand = $"[EAX{_dbgLastOperand}]";
                    break;

                case 0x01:
                    value = (UInt32)(ECX + displacement);
                    _dbgLastOperand = $"[ECX{_dbgLastOperand}]";
                    break;

                case 0x02:
                    value = (UInt32)(EDX + displacement);
                    _dbgLastOperand = $"[EDX{_dbgLastOperand}]";
                    break;

                case 0x03:
                    value = (UInt32)(EBX + displacement);
                    _dbgLastOperand = $"[EBX{_dbgLastOperand}]";
                    break;

                case 0x04:
                    value = CalcSib(sib, displacement);
                    break;

                case 0x05:
                    value = (UInt32)(EBP + displacement);
                    if (segSelect != DEFAULT_SEGMENT_SELECT) {
                        segSelect = SP_INDEX;
                    }
                    _dbgLastOperand = $"[EBP{_dbgLastOperand}]";
                    break;

                case 0x06:
                    value = (UInt32)(ESI + displacement);
                    _dbgLastOperand = $"[ESI{_dbgLastOperand}]";
                    break;

                case 0x07:
                    value = (UInt32)(EDI + displacement);
                    if (segSelect != DEFAULT_SEGMENT_SELECT) {
                        segSelect = ES_INDEX;
                    }
                    _dbgLastOperand = $"[EDI{_dbgLastOperand}]";
                    break;
            }

            return value;
        }

        private UInt32 GetRegMask8(int index) {
            UInt32 mask = 0;

            switch (index) {
                case 0x00: // AL
                    mask = 0x000000FF;
                    break;

                case 0x01: // CL
                    mask = 0x000000FF;
                    break;

                case 0x02: // DL
                    mask = 0x000000FF;
                    break;

                case 0x03: // BL
                    mask = 0x000000FF;
                    break;

                case 0x04: // AH
                    mask = 0x0000FF00;
                    break;

                case 0x05: // CH
                    mask = 0x0000FF00;
                    break;

                case 0x06: // DH
                    mask = 0x0000FF00;
                    break;

                case 0x07: // BH
                    mask = 0x0000FF00;
                    break;
            }

            return mask;
        }

        protected UInt32 GetRegMaskRm8(ModRm modrm) {
            return GetRegMask8(modrm.rm);
        }

        protected UInt32 GetRegMaskReg8(ModRm modrm) {
            return GetRegMask8(modrm.reg);
        }

        protected ModRm ReadModRm() {
            return new ModRm(ReadImm8());
        }

        /**********************************************************************
        SIB utilities
        **********************************************************************/

        protected SIB ReadSIB() {
            return new SIB(ReadImm8());
        }

        /**********************************************************************
        Memory read/write utilities
        **********************************************************************/

        protected byte ReadImm8() {
            byte value = Read8((UInt32)(CS << 4) + EIP);
            DbgImm(value);
            ++EIP;
            return value;
        }

        protected UInt16 ReadImm16() {
            UInt16 value = Read16((UInt32)(CS << 4) + EIP);
            DbgImm(value);
            EIP += 2;
            return value;
        }

        protected UInt32 ReadImm32() {
            UInt32 value = Read32((UInt32)(CS << 4) + EIP);
            DbgImm(value);
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


        /**********************************************************************
        Debug output utilities
        **********************************************************************/

        void DbgImm(byte opcode) {
            _dbgImm += $"{opcode:X2} ";
        }

        void DbgImm(UInt16 opcode) {
            _dbgImm += $"{opcode & 0x00FF:X2} {(opcode >> 8) & 0x00FF:X2} ";
        }

        void DbgImm(UInt32 opcode) {
            _dbgImm += $"{opcode & 0x000000FF:X2} {(opcode >> 8) & 0x000000FF:X2} {(opcode >> 16) & 0x000000FF:X2} {(opcode >> 24) & 0x000000FF:X2} ";
        }

        void DbgIns(string s) {
            Hardware.Debug($"{_dbgAddress} {_dbgImm,-21} {s}", this);
            _dbgImm = string.Empty;
        }


        /*********************************************************************
        Common instruction execution patterns
        *********************************************************************/

        delegate R InstructionOperation<R, Op1, Op2>(ref Op1 op1, ref Op2 op2);

        void Increg(int regIndex) {
            var tempCF = CF;

            if ((Flags & ADDR_SIZE_32) != 0) {
                generalRegisters[regIndex] = Add32(generalRegisters[regIndex], 1);
                DbgIns($"{Instruction.INC} {regArray32[regIndex]}");
            }
            else {
                generalRegisters[regIndex] = Add16((UInt16)(generalRegisters[regIndex] & 0x0000FFFF), 1);
                DbgIns($"{Instruction.INC} {regArray16[regIndex]}");
            }

            CF = tempCF;
        }

        void Decreg(int regIndex) {
            var tempCF = CF;

            if ((Flags & ADDR_SIZE_32) != 0) {
                generalRegisters[regIndex] = Subtract32(generalRegisters[regIndex], 1);
                DbgIns($"{Instruction.DEC} {regArray32[regIndex]}");
            }
            else {
                generalRegisters[regIndex] = Subtract16((UInt16)(generalRegisters[regIndex] & 0x0000FFFF), 1);
                DbgIns($"{Instruction.DEC} {regArray16[regIndex]}");
            }

            CF = tempCF;
        }

        void EbGb(Instruction instruction, InstructionOperation<byte, byte, byte> operation) {
            Cycles = 1;
            byte op1 = 0;
            byte op2 = 0;
            var modrm = ReadModRm();

            if (modrm.mod == 0x03) {
                var op1Index = modrm.rm;
                var op1Mask = GetRegMask8(op1Index);
                op1 = (byte)(generalRegisters[op1Index] & op1Mask);

                var op2Index = modrm.reg;
                var op2Mask = GetRegMask8(op2Index);
                op2 = (byte)(generalRegisters[op2Index] & op2Mask);

                generalRegisters[op1Index] = operation(ref op1, ref op2);

                DbgIns($"{instruction} {regArray8[op1Index]}, {regArray8[op2Index]}");
            }
            else {
                UInt32 address = Read16ModRm(modrm);
                address = CalcOffset32(address);
                op1 = Read8(address);

                int op2Index = modrm.reg;
                UInt32 op2Mask = GetRegMask8(op2Index);
                op2 = (byte)(generalRegisters[op2Index] & op2Mask);

                Write8(address, operation(ref op1, ref op2));

                DbgIns($"{instruction} {_dbgSegSelect}{_dbgLastOperand}, {regArray8[op2Index]}");
            }
        }

        void EbIb(Instruction instruction, InstructionOperation<byte, byte, byte> operation) {
            Cycles = 1;
            byte op1 = 0;
            byte op2 = 0;
            var modrm = ReadModRm();

            if (modrm.mod == 0x03) {
                var op1Index = modrm.rm;
                var op1Mask = GetRegMask8(op1Index);
                op1 = (byte)(generalRegisters[op1Index] & op1Mask);
                op2 = (byte)ReadImm8();

                generalRegisters[op1Index] = operation(ref op1, ref op2);

                DbgIns($"{instruction} {regArray8[op1Index]}, {op2:X2}");
            }
            else {
                UInt32 address = Read16ModRm(modrm);
                address = CalcOffset32(address);
                op1 = Read8(address);
                op2 = (byte)ReadImm8();

                Write8(address, operation(ref op1, ref op2));

                DbgIns($"{instruction} {_dbgSegSelect}{_dbgLastOperand}, {op2:X2}");
            }
        }

        void EvIv(Instruction instruction, InstructionOperation<UInt16, UInt16, UInt16> operation16, InstructionOperation<UInt32, UInt32, UInt32> operation32) {
            Cycles = 1;
            var modrm = ReadModRm();

            if (modrm.mod == 0x03) {
                var op1Index = modrm.rm;

                if ((Flags & ADDR_SIZE_32) != 0) {
                    UInt32 op1 = generalRegisters[op1Index];
                    UInt32 op2 = ReadImm32();

                    generalRegisters[op1Index] = operation32(ref op1, ref op2);

                    DbgIns($"{instruction} {regArray32[op1Index]}, {op2:X8}");
                }
                else {
                    UInt16 op1 = (UInt16)(generalRegisters[op1Index] & 0x0000FFFF);
                    UInt16 op2 = ReadImm16();

                    generalRegisters[op1Index] = operation16(ref op1, ref op2);

                    DbgIns($"{instruction} {regArray16[op1Index]}, {op2:X4}");
                }
            }
            else {
                UInt32 address = 0;

                if ((Flags & ADDR_SIZE_32) != 0) {
                    address = Read32ModRm(modrm);
                }
                else {
                    address = Read16ModRm(modrm);
                }

                string src = string.Empty;

                if ((Flags & OPERAND_SIZE_32) != 0) {
                    address = CalcOffset32(address);
                    UInt32 op1 = Read32(address);
                    UInt32 op2 = ReadImm32();
                    src = $"{op2:X8}";

                    Write32(address, operation32(ref op1, ref op2));
                }
                else {
                    address = CalcOffset32(address);
                    UInt16 op1 = Read16(address);
                    UInt16 op2 = ReadImm16();
                    src = $"{op2:X4}";

                    Write16(address, operation16(ref op1, ref op2));
                }

                DbgIns($"{instruction} {_dbgSegSelect}{_dbgLastOperand}, {src}");
            }
        }

        void EvGv(Instruction instruction, InstructionOperation<UInt16, UInt16, UInt16> operation16, InstructionOperation<UInt32, UInt32, UInt32> operation32) {
            Cycles = 1;
            var modrm = ReadModRm();

            if (modrm.mod == 0x03) {
                var op1Index = modrm.rm;
                var op2Index = modrm.reg;

                if ((Flags & ADDR_SIZE_32) != 0) {
                    UInt32 op1 = generalRegisters[op1Index];
                    UInt32 op2 = generalRegisters[op2Index];

                    generalRegisters[op1Index] = operation32(ref op1, ref op2);

                    DbgIns($"{instruction} {regArray32[op1Index]}, {regArray32[op2Index]}");
                }
                else {
                    UInt16 op1 = (UInt16)(generalRegisters[op1Index] & 0x0000FFFF);
                    UInt16 op2 = (UInt16)(generalRegisters[op2Index] & 0x0000FFFF);

                    generalRegisters[op1Index] = operation16(ref op1, ref op2);

                    DbgIns($"{instruction} {regArray16[op1Index]}, {regArray16[op2Index]}");
                }
            }
            else {
                UInt32 address = 0;

                if ((Flags & ADDR_SIZE_32) != 0) {
                    address = Read32ModRm(modrm);
                }
                else {
                    address = Read16ModRm(modrm);
                }

                string src = string.Empty;

                if ((Flags & OPERAND_SIZE_32) != 0) {
                    address = CalcOffset32(address);
                    UInt32 op1 = Read32(address);
                    UInt32 op2 = generalRegisters[modrm.reg];
                    src = regArray32[modrm.reg];

                    Write32(address, operation32(ref op1, ref op2));
                }
                else {
                    address = CalcOffset32(address);
                    UInt16 op1 = Read16(address);
                    UInt16 op2 = (UInt16)(generalRegisters[modrm.reg] & 0x0000ffff);
                    src = regArray16[modrm.reg];

                    Write16(address, operation16(ref op1, ref op2));
                }

                DbgIns($"{instruction} {_dbgSegSelect}{_dbgLastOperand}, {src}");
            }
        }

        void GbEb(Instruction instruction, InstructionOperation<byte, byte, byte> operation) {
            Cycles = 1;
            byte op1 = 0;
            byte op2 = 0;
            var modrm = ReadModRm();
            var op1Index = modrm.reg;

            if (modrm.mod == 0x03) {
                var op1Mask = GetRegMask8(op1Index);
                op1 = (byte)(generalRegisters[op1Index] & op1Mask);

                var op2Index = modrm.rm;
                var op2Mask = GetRegMask8(op2Index);
                op2 = (byte)(generalRegisters[op2Index] & op2Mask);

                generalRegisters[op1Index] = operation(ref op1, ref op2);

                DbgIns($"{instruction} {regArray8[op1Index]}, {regArray8[op2Index]}");
            }
            else {
                UInt32 address = Read16ModRm(modrm);
                address = CalcOffset32(address);
                op2 = Read8(address);

                UInt32 op1Mask = GetRegMask8(op1Index);
                op1 = (byte)(generalRegisters[op1Index] & op1Mask);

                generalRegisters[op1Index] = operation(ref op1, ref op2);

                DbgIns($"{instruction} {regArray8[op1Index]}, {_dbgSegSelect}{_dbgLastOperand}");
            }
        }

        void GvEv(Instruction instruction, InstructionOperation<UInt16, UInt16, UInt16> operation16, InstructionOperation<UInt32, UInt32, UInt32> operation32) {
            Cycles = 1;
            var modrm = ReadModRm();

            if (modrm.mod == 0x03) {
                var op1Index = modrm.reg;
                var op2Index = modrm.rm;

                if ((Flags & ADDR_SIZE_32) != 0) {
                    UInt32 op1 = generalRegisters[op1Index];
                    UInt32 op2 = generalRegisters[op2Index];

                    generalRegisters[op1Index] = operation32(ref op1, ref op2);

                    DbgIns($"{instruction} {regArray32[op1Index]}, {regArray32[op2Index]}");
                }
                else {
                    UInt16 op1 = (UInt16)(generalRegisters[op1Index] & 0x0000FFFF);
                    UInt16 op2 = (UInt16)(generalRegisters[op2Index] & 0x0000FFFF);

                    generalRegisters[op1Index] = operation16(ref op1, ref op2);

                    DbgIns($"{instruction} {regArray32[op1Index]}, {regArray32[op2Index]}");
                }
            }
            else {
                UInt32 address = 0;

                if ((Flags & ADDR_SIZE_32) != 0) {
                    address = Read32ModRm(modrm);
                }
                else {
                    address = Read16ModRm(modrm);
                }

                string dest = string.Empty;

                if ((Flags & OPERAND_SIZE_32) != 0) {
                    address = CalcOffset32(address);
                    UInt32 op1 = generalRegisters[modrm.reg];
                    UInt32 op2 = Read32(address);
                    dest = regArray32[modrm.reg];

                    generalRegisters[modrm.reg] = operation32(ref op1, ref op2);
                }
                else {
                    address = CalcOffset32(address);
                    UInt16 op1 = (UInt16)(generalRegisters[modrm.reg] & 0x0000ffff);
                    UInt16 op2 = Read16(address);
                    dest = regArray16[modrm.reg];

                    generalRegisters[modrm.reg] = operation16(ref op1, ref op2);
                }

                DbgIns($"{instruction} {_dbgSegSelect}{dest}, {_dbgLastOperand}");
            }
        }

        void GwLIb(Instruction instruction, int regIndex, InstructionOperation<byte, byte, byte> operation) {
            byte op2 = ReadImm8();
            byte temp = (byte)(generalRegisters[regIndex] & 0x000000FF);
            generalRegisters[regIndex] = operation(ref temp, ref op2);
            DbgIns($"{instruction} {regArray8[regIndex]}, {op2:X2}");
        }

        void GwHIb(Instruction instruction, int regIndex, InstructionOperation<byte, byte, byte> operation) {
            byte op2 = ReadImm8();
            byte temp = (byte)(generalRegisters[regIndex] & 0x0000FF00);
            byte result = operation(ref temp, ref op2);
            generalRegisters[regIndex] |= (UInt32)(result << 8);
            DbgIns($"{instruction} {regArray8[regIndex + 4]}, {op2:X2}");
        }

        void GvIv(Instruction instruction, int regIndex, InstructionOperation<UInt16, UInt16, UInt16> operation16, InstructionOperation<UInt32, UInt32, UInt32> operation32) {
            string dest = string.Empty;
            string src = string.Empty;

            if ((Flags & OPERAND_SIZE_32) != 0) {
                UInt32 op2 = ReadImm32();
                operation32(ref generalRegisters[regIndex], ref op2);
                dest = regArray32[EAX_INDEX];
                src = $"0x{op2:X8}";
            }
            else {
                UInt16 op2 = ReadImm16();
                UInt16 temp = (UInt16)(generalRegisters[regIndex] & 0x0000FFFF);
                generalRegisters[regIndex] = operation16(ref temp, ref op2);
                dest = regArray16[regIndex];
                src = $"0x{op2:X4}";
            }

            DbgIns($"{instruction} {_dbgSegSelect}{dest}, {src}");
        }

        /*********************************************************************
        OpCodes
        *********************************************************************/

        [OpCode(0x00)]
        void AddEbGb() {
            EbGb(Instruction.ADD, AddByref8);
        }

        [OpCode(0x01)]
        void AddEvGv() {
            EvGv(Instruction.ADD, AddByref16, AddByref32);
        }

        [OpCode(0x02)]
        void AddGbEb() {
            GbEb(Instruction.ADD, AddByref8);
        }

        [OpCode(0x03)]
        void AddGvEv() {
            GvEv(Instruction.ADD, AddByref16, AddByref32);
        }

        [OpCode(0x04)]
        void AddALIb() {
            GwLIb(Instruction.ADD, AX_INDEX, AddByref8);
        }

        [OpCode(0x05)]
        void AddeAXIv() {
            GvIv(Instruction.ADD, EAX_INDEX, AddByref16, AddByref32);
        }

        [OpCode(0x06)]
        void PushES() {
            Push16(ES);
            DbgIns("PUSH ES");
        }

        [OpCode(0x07)]
        void PopES() {
            ES = Pop16();
            DbgIns("POP ES");
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

        [OpCode(0x17)]
        void PopSS() {
            SS = Pop16();
            DbgIns("POP SS");
        }

        [OpCode(0x1E)]
        void PushDS() {
            Push16(DS);
            DbgIns("PUSH DS");
        }

        [OpCode(0x1F)]
        void PopDS() {
            DS = Pop16();
            DbgIns("POP DS");
        }

        [OpCode(0x20)]
        void AndEbGb() {
            EbGb(Instruction.AND, And8);
        }

        [OpCode(0x21)]
        void AndEvGv() {
            EvGv(Instruction.AND, And16, And32);
        }

        [OpCode(0x22)]
        void AndGbEb() {
            GbEb(Instruction.AND, And8);
        }

        [OpCode(0x23)]
        void AndGvEv() {
            GvEv(Instruction.AND, And16, And32);
        }

        [OpCode(0x24)]
        void AndALIb() {
            GwLIb(Instruction.AND, AX_INDEX, And8);
        }

        [OpCode(0x25)]
        void AndeAXIv() {
            GvIv(Instruction.AND, EAX_INDEX, And16, And32);
        }

        [OpCode(0x26)]
        void ESPrefix() {
            segSelect = ES_INDEX;
            _dbgSegSelect = "ES:";
            DecodeOpcode(ReadImm8());
        }

        [OpCode(0x28)]
        void SubEbGb() {
            EbGb(Instruction.SUB, SubtractByref8);
        }

        [OpCode(0x29)]
        void SubEvGv() {
            EvGv(Instruction.SUB, SubtractByref16, SubtractByref32);
        }

        [OpCode(0x2A)]
        void SubGbEb() {
            GbEb(Instruction.SUB, SubtractByref8);
        }

        [OpCode(0x2B)]
        void SubGvEv() {
            GvEv(Instruction.SUB, SubtractByref16, SubtractByref32);
        }

        [OpCode(0x2C)]
        void SubALIb() {
            GwLIb(Instruction.SUB, AX_INDEX, SubtractByref8);
        }

        [OpCode(0x2D)]
        void SubeAXIv() {
            GvIv(Instruction.SUB, EAX_INDEX, SubtractByref16, SubtractByref32);
        }

        [OpCode(0x2E)]
        void CSPrefix() {
            segSelect = CS_INDEX;
            _dbgSegSelect = "CS:";
            DecodeOpcode(ReadImm8());
        }

        [OpCode(0x30)]
        void XorEbGb() {
            EbGb(Instruction.XOR, Xor8);
        }

        [OpCode(0x31)]
        void XorEvGv() {
            EvGv(Instruction.XOR, Xor16, Xor32);
        }

        [OpCode(0x32)]
        void XorGbEb() {
            GbEb(Instruction.XOR, Xor8);
        }

        [OpCode(0x33)]
        void XorGvEv() {
            GvEv(Instruction.XOR, Xor16, Xor32);
        }

        [OpCode(0x34)]
        void XorALIb() {
            GwLIb(Instruction.XOR, AX_INDEX, Xor8);
        }

        [OpCode(0x35)]
        void XoreAXIv() {
            GvIv(Instruction.XOR, EAX_INDEX, Xor16, Xor32);
        }

        [OpCode(0x36)]
        void SSPrefix() {
            segSelect = SS_INDEX;
            _dbgSegSelect = "SS:";
            DecodeOpcode(ReadImm8());
        }

        [OpCode(0x38)]
        void CmpEbGb() {
            EbGb(Instruction.SUB, Compare8);
        }

        [OpCode(0x39)]
        void CmpEvGv() {
            EvGv(Instruction.SUB, Compare16, Compare32);
        }

        [OpCode(0x3A)]
        void CmpGbEb() {
            GbEb(Instruction.SUB, Compare8);
        }

        [OpCode(0x3B)]
        void CmpGvEv() {
            GvEv(Instruction.SUB, Compare16, Compare32);
        }

        [OpCode(0x3C)]
        void CmpALIb() {
            GwLIb(Instruction.SUB, AX_INDEX, Compare8);
        }

        [OpCode(0x3D)]
        void CmpeAXIv() {
            GvIv(Instruction.SUB, EAX_INDEX, Compare16, Compare32);
        }


        [OpCode(0x3E)]
        void DSPrefix() {
            segSelect = DS_INDEX;
            _dbgSegSelect = "DS:";
            DecodeOpcode(ReadImm8());
        }

        [OpCode(0x40)]
        void IncAX() {
            Increg(EAX_INDEX);
        }

        [OpCode(0x41)]
        void IncCX() {
            Increg(ECX_INDEX);
        }

        [OpCode(0x42)]
        void IncDX() {
            Increg(EDX_INDEX);
        }

        [OpCode(0x43)]
        void IncBX() {
            Increg(EBX_INDEX);
        }

        [OpCode(0x44)]
        void IncSP() {
            Increg(ESP_INDEX);
        }

        [OpCode(0x45)]
        void IncBP() {
            Increg(EBX_INDEX);
        }

        [OpCode(0x46)]
        void IncSI() {
            Increg(ESI_INDEX);
        }

        [OpCode(0x47)]
        void IncDI() {
            Increg(EDI_INDEX);
        }

        [OpCode(0x48)]
        void DecAX() {
            Decreg(EAX_INDEX);
        }

        [OpCode(0x49)]
        void DecCX() {
            Decreg(ECX_INDEX);
        }

        [OpCode(0x4A)]
        void DecDX() {
            Decreg(EDX_INDEX);
        }

        [OpCode(0x4B)]
        void DecBX() {
            Decreg(EBX_INDEX);
        }

        [OpCode(0x4C)]
        void DecSP() {
            Decreg(ESP_INDEX);
        }

        [OpCode(0x4D)]
        void DecBP() {
            Decreg(EBP_INDEX);
        }

        [OpCode(0x4E)]
        void DecSI() {
            Decreg(ESI_INDEX);
        }

        [OpCode(0x4F)]
        void DecDI() {
            Decreg(EDI_INDEX);
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

        [OpCode(0x60)]
        void PushAll() {
            if ((Flags & OPERAND_SIZE_32) != 0) {
                var temp = ESP;
                Push32(EAX);
                Push32(ECX);
                Push32(EDX);
                Push32(EBX);
                Push32(temp);
                Push32(EBP);
                Push32(ESI);
                Push32(EDI);
            }
            else {
                var temp = SP;
                Push16(AX);
                Push16(CX);
                Push16(DX);
                Push16(BX);
                Push16(temp);
                Push16(BP);
                Push16(SI);
                Push16(DI);
            }

            DbgIns("PUSHA");
        }

        [OpCode(0x61)]
        void PopAll() {
            if ((Flags & OPERAND_SIZE_32) != 0) {
                EDI = Pop32();
                ESI = Pop32();
                EBP = Pop32();
                ESP += 4;
                EBX = Pop32();
                EDX = Pop32();
                ECX = Pop32();
                EAX = Pop32();
            }
            else {
                DI = Pop16();
                SI = Pop16();
                BP = Pop16();
                ESP += 2;
                BX = Pop16();
                DX = Pop16();
                CX = Pop16();
                AX = Pop16();
            }

            DbgIns("POPA");
        }

        [OpCode(0x64)]
        void FSPrefix() {
            segSelect = FS_INDEX;
            _dbgSegSelect = "FS:";
            DecodeOpcode(ReadImm8());
        }

        [OpCode(0x65)]
        void GSPrefix() {
            segSelect = GS_INDEX;
            _dbgSegSelect = "GS:";
            DecodeOpcode(ReadImm8());
        }

        [OpCode(0x66)]
        void OperandSize() {
            Flags ^= OPERAND_SIZE_32;
            DecodeOpcode(ReadImm8());
        }

        [OpCode(0x67)]
        void AddressSize() {
            Flags ^= ADDR_SIZE_32;
            DecodeOpcode(ReadImm8());
        }

        [OpCode(0x68)]
        void PushIv() {
            if ((Flags & OPERAND_SIZE_32) != 0) {
                var value = ReadImm32();
                Push32(value);
                DbgIns($"PUSH 0x{value:X8}");
            }
            else {
                var value = ReadImm16();
                Push16(value);
                DbgIns($"PUSH 0x{value:X4}");
            }
        }

        [OpCode(0x6A)]
        void PushIb() {
            var value = ReadImm8();
            Push8(value);
            DbgIns($"PUSH 0x{value:X2}");
        }

        [OpCode(0x70)]
        void JO () {
            var targetIP = Jb();

            if (OF == 1) {
                EIP = targetIP;                
            }

            DbgIns($"JO 0x{(CS << 4) + targetIP:X8}");
        }

        [OpCode(0x71)]
        void JNO() {
            var targetIP = Jb();

            if (OF == 0) {
                EIP = targetIP;
            }

            DbgIns($"JNO 0x{(CS << 4) + targetIP:X8}");
        }

        [OpCode(0x72)]
        void JC() {
            var targetIP = Jb();

            if (CF == 1) {
                EIP = targetIP;
            }

            DbgIns($"JC 0x{(CS << 4) + targetIP:X8}");
        }

        [OpCode(0x73)]
        void JAE() {
            var targetIP = Jb();

            if (CF == 0) {
                EIP = targetIP;
            }

            DbgIns($"JAE 0x{(CS << 4) + targetIP:X8}");
        }

        [OpCode(0x74)]
        void JZ() {
            var targetIP = Jb();

            if (ZF == 1) {
                EIP = targetIP;
            }

            DbgIns($"JZ 0x{(CS << 4) + targetIP:X8}");
        }

        [OpCode(0x75)]
        void JNZ() {
            var targetIP = Jb();

            if (ZF == 0) {
                EIP = targetIP;
            }

            DbgIns($"JNZ 0x{(CS << 4) + targetIP:X8}");
        }

        [OpCode(0x76)]
        void JNA() {
            var targetIP = Jb();

            if (CF == 1 || ZF == 1) {
                EIP = targetIP;
            }

            DbgIns($"JNA 0x{(CS << 4) + targetIP:X8}");
        }

        [OpCode(0x77)]
        void JA() {
            var targetIP = Jb();

            if (CF == 0 && ZF == 0) {
                EIP = targetIP;
            }

            DbgIns($"JA 0x{(CS << 4) + targetIP:X8}");
        }

        [OpCode(0x78)]
        void JS() {
            var targetIP = Jb();

            if (SF == 1) {
                EIP = targetIP;
            }

            DbgIns($"JS 0x{(CS << 4) + targetIP:X8}");
        }

        [OpCode(0x79)]
        void JNS() {
            var targetIP = Jb();

            if (SF == 0) {
                EIP = targetIP;
            }

            DbgIns($"JNS 0x{(CS << 4) + targetIP:X8}");
        }

        [OpCode(0x7A)]
        void JP() {
            var targetIP = Jb();

            if (PF == 1) {
                EIP = targetIP;
            }

            DbgIns($"JP 0x{(CS << 4) + targetIP:X8}");
        }

        [OpCode(0x7B)]
        void JNP() {
            var targetIP = Jb();

            if (PF == 0) {
                EIP = targetIP;
            }

            DbgIns($"JNP 0x{(CS << 4) + targetIP:X8}");
        }

        [OpCode(0x7C)]
        void JL() {
            var targetIP = Jb();

            if (SF != OF) {
                EIP = targetIP;
            }

            DbgIns($"JL 0x{(CS << 4) + targetIP:X8}");
        }

        [OpCode(0x7D)]
        void JGE() {
            var targetIP = Jb();

            if (SF == OF) {
                EIP = targetIP;
            }

            DbgIns($"JGE 0x{(CS << 4) + targetIP:X8}");
        }

        [OpCode(0x7E)]
        void JLE() {
            var targetIP = Jb();

            if (ZF == 1 || SF != OF) {
                EIP = targetIP;
            }

            DbgIns($"JLE 0x{(CS << 4) + targetIP:X8}");
        }

        [OpCode(0x7F)]
        void JG() {
            var targetIP = Jb();

            if (ZF == 0 && SF == OF) {
                EIP = targetIP;
            }

            DbgIns($"JG 0x{(CS << 4) + targetIP:X8}");
        }


        [OpCode(0x88)]
        void MovEbGb() {
            EbGb(Instruction.MOV, Move8);
        }

        [OpCode(0x89)]
        void MovEvGv() {
            EvGv(Instruction.MOV, Move16, Move32);
        }

        [OpCode(0x8A)]
        void MovGbEb() {
            GbEb(Instruction.MOV, Move8);
        }

        [OpCode(0x8B)]
        void MovGvEv() {
            GvEv(Instruction.MOV, Move16, Move32);
        }

        [OpCode(0x8C)]
        void MovEwSw() {
            Cycles = 1;
            var modrm = ReadModRm();

            if (modrm.mod == 0x03) {
                generalRegisters[modrm.rm] = segmentRegisters[modrm.reg];
                DbgIns($"MOV {regArray16[modrm.rm]}, {segArray[modrm.reg]}");
            }
            else {
                if ((Flags & ADDR_SIZE_32) != 0) {
                    UInt32 address = Read32ModRm(modrm);
                    Write32(CalcOffset32(address), segmentRegisters[modrm.reg]);
                }
                else {
                    UInt16 address = Read16ModRm(modrm);
                    Write16(CalcOffset32(address), segmentRegisters[modrm.reg]);
                }

                DbgIns($"MOV {_dbgSegSelect}{_dbgLastOperand}, {segArray[modrm.reg]}");
            }
        }

        [OpCode(0x8D)]
        void LeaGvM() {
            Cycles = 1;
            var modrm = ReadModRm();

            if ((Flags & OPERAND_SIZE_32) != 0) {
                UInt32 value = Read32ModRm(modrm);
                generalRegisters[modrm.reg] = CalcOffset32(value);
            }
            else {
                UInt16 value = Read16ModRm(modrm);
                generalRegisters[modrm.reg] = (UInt32)(CalcOffset16(value) & 0x0000FFFF);
            }

            DbgIns($"LEA {regArray16[modrm.reg]}, {_dbgSegSelect}{_dbgLastOperand}");
        }

        [OpCode(0x8e)]
        void MovSwEw() {
            Cycles = 1;
            var modrm = ReadModRm();

            if (modrm.mod == 0x03) {
                segmentRegisters[modrm.reg] = (UInt16)(generalRegisters[modrm.rm] & 0x0000FFFF);
                DbgIns($"MOV {segArray[modrm.reg]}, {regArray16[modrm.rm]}");
            }
            else {
                if ((Flags & ADDR_SIZE_32) != 0) {
                    UInt32 value = Read32ModRm(modrm);
                    segmentRegisters[modrm.reg] = (UInt16)(CalcOffset32(value) & 0x0000FFFF);
                }
                else {
                    UInt16 value = Read16ModRm(modrm);
                    segmentRegisters[modrm.reg] = CalcOffset16(value);
                }

                DbgIns($"MOV {segArray[modrm.reg]}, {_dbgSegSelect}{_dbgLastOperand}");
            }

            if (modrm.reg == SS_INDEX) {
                /* Run next instruction without handling interrupts */
                DecodeOpcode(ReadImm8());
            }
        }

        [OpCode(0x90)]
        void Nop() {
            Cycles = 1;
            DbgIns("NOP");
        }

        [OpCode(0xB0)]
        void MovALIb() {
            GwLIb(Instruction.MOV, AX_INDEX, Move8);
        }

        [OpCode(0xB1)]
        void MovCLIb() {
            GwLIb(Instruction.MOV, CX_INDEX, Move8);
        }

        [OpCode(0xB2)]
        void MovDLIb() {
            GwLIb(Instruction.MOV, DX_INDEX, Move8);
        }

        [OpCode(0xB3)]
        void MovBLIb() {
            GwLIb(Instruction.MOV, BX_INDEX, Move8);
        }

        [OpCode(0xB4)]
        void MovAHIb() {
            GwHIb(Instruction.MOV, AX_INDEX, Move8);
        }

        [OpCode(0xB5)]
        void MovCHIb() {
            GwHIb(Instruction.MOV, CX_INDEX, Move8);
        }

        [OpCode(0xB6)]
        void MovDHIb() {
            GwHIb(Instruction.MOV, DX_INDEX, Move8);
        }

        [OpCode(0xB7)]
        void MovBHIb() {
            GwHIb(Instruction.MOV, BX_INDEX, Move8);
        }

        [OpCode(0xB8)]
        void MoveeAXIv() {
            GvIv(Instruction.MOV, EAX_INDEX, Move16, Move32);
        }

        [OpCode(0xB9)]
        void MovCXIv() {
            GvIv(Instruction.MOV, ECX_INDEX, Move16, Move32);
        }

        [OpCode(0xBA)]
        void MovDXIv() {
            GvIv(Instruction.MOV, EDX_INDEX, Move16, Move32);
        }

        [OpCode(0xBB)]
        void MovBXIv() {
            GvIv(Instruction.MOV, EBX_INDEX, Move16, Move32);
        }

        [OpCode(0xBC)]
        void MoveSPIv() {
            GvIv(Instruction.MOV, ESP_INDEX, Move16, Move32);
        }

        [OpCode(0xBD)]
        void MoveBPIv() {
            GvIv(Instruction.MOV, EBP_INDEX, Move16, Move32);
        }

        [OpCode(0xBE)]
        void MoveSIIv() {
            GvIv(Instruction.MOV, ESI_INDEX, Move16, Move32);
        }

        [OpCode(0xBF)]
        void MoveDIIv() {
            GvIv(Instruction.MOV, EDI_INDEX, Move16, Move32);
        }

        [OpCode(0xCC)]
        void Int3() {
            --EIP;
            DbgIns($"Debug break at address {EIP:X4}");
            interruptEvent.WaitOne();
            DbgIns("INT3 continuing");
        }

        [OpCode(0xC6)]
        void MovEbIb() {
            EbIb(Instruction.MOV, Move8);
        }

        [OpCode(0xC7)]
        void MovEvIv() {
            EvIv(Instruction.MOV, Move16, Move32);
        }

        [OpCode(0xCD)]
        void IntImm8() {
            byte id = ReadImm8();
            Push16((UInt16)(EFLAGS & 0x0000FFFF));

            IF = 0;
            // TF = 0;
            AF = 0;

            UInt32 address = (UInt32)(id * 4);
            var newip = Read16(address);
            var newcs = Read16(address + 2);
            Push16(CS);
            Push16((UInt16)(EIP & 0x0000FFFF));
            CS = newcs;
            EIP = newip;
            DbgIns($"INT 0x{id:X2}");
        }

        [OpCode(0xCF)]
        void IRet() {
            EIP = Pop16();
            CS = Pop16();
            EFLAGS = Pop16();
            DbgIns("IRET");
        }

        [OpCode(0xE9)]
        void JmpJz() {
            if ((Flags & ADDR_SIZE_32) != 0) {
                EIP = Jz32();
            }
            else {
                EIP = Jz16();
            }

            DbgIns($"JMP 0x{(CS << 4) + EIP:X8}");
        }

        [OpCode(0xEA)]
        void JmpAp() {
            if ((Flags & OPERAND_SIZE_32) != 0) {
                // TODO: This won't work
                UInt16 segment = ReadImm16();
                UInt32 offset = ReadImm32();
                EIP = offset;
                DbgIns($"JMP 0x{offset:X12}");
            }
            else {
                UInt32 offset = ReadImm32();
                EIP = offset;
                DbgIns($"JMP 0x{offset:X8}");
            }
        }

        [OpCode(0xEB)]
        void Jmpb() {
            EIP = Jb();
            DbgIns($"JMP 0x{(CS << 4) + EIP:X8}");
        }

        [OpCode(0xF4)]
        void Hlt() {
            DbgIns("HLT");
            interruptEvent.WaitOne();
            Hardware.Debug("CPU awake", this);
        }

        [OpCode(0xF8)]
        void Clc() {
            CF = 0;
            DbgIns("CLC");
        }

        [OpCode(0XF9)]
        void Stc() {
            CF = 1;
            DbgIns("STC");
        }

        [OpCode(0xFA)]
        void Cli() {
            IF = 0;
            DbgIns("CLI");
        }

        [OpCode(0xFB)]
        void Sti() {
            IF = 1;
            DbgIns("STI");
        }

        [OpCode(0xFC)]
        void Cld() {
            DF = 0;
            DbgIns("CLD");
        }

        [OpCode(0xFD)]
        void Std() {
            DF = 1;
            DbgIns("STD");
        }

        [OpCode(0xF6)]
        void OpcodeF6() {
            var modrm = ReadModRm();
            switch (modrm.reg) {
                /* TEST */
                case 0:
                case 1:
                    DbgIns("TEST not implemented");
                    Hardware.RaiseException(0);
                    Hlt();
                    break;

                case 2:
                    DbgIns("NOT not implemented");
                    Hardware.RaiseException(0);
                    Hlt();
                    break;

                case 3:
                    DbgIns("NEG not implemented");
                    Hardware.RaiseException(0);
                    Hlt();
                    break;

                /* MUL */
                case 4: 
                    {
                        byte op2;
                        var op2Index = modrm.rm;

                        if (modrm.mod == 0x03) {
                            op2 = (byte)(generalRegisters[op2Index] & 0x000000FF);
                            _dbgLastOperand = regArray8[op2Index];
                        }
                        else {
                            UInt32 address = Read32ModRm(modrm);
                            op2 = Read8(address);
                        }

                        byte op1 = AL;
                        AX = (UInt16)(op1 * op2);

                        DbgIns($"MUL {_dbgLastOperand}");
                    }
                    break;

                case 5:
                    DbgIns("IMUL not implemented");
                    Hardware.RaiseException(0);
                    Hlt();
                    break;

                case 6:
                    DbgIns("DIV not implemented");
                    Hardware.RaiseException(0);
                    Hlt();
                    break;

                case 7:
                    DbgIns("IDIV not implemented");
                    Hardware.RaiseException(0);
                    Hlt();
                    break;

            }
        }

        [OpCode(0xF7)]
        void OpcodeF7() {
            var modrm = ReadModRm();
            switch (modrm.reg) {
                /* TEST */
                case 0:
                case 1:
                    DbgIns("TEST not implemented");
                    Hardware.RaiseException(0);
                    Hlt();
                    break;

                case 2:
                    DbgIns("NOT not implemented");
                    Hardware.RaiseException(0);
                    Hlt();
                    break;

                case 3:
                    DbgIns("NEG not implemented");
                    Hardware.RaiseException(0);
                    Hlt();
                    break;

                /* MUL */
                case 4: 
                    {
                        UInt32 op2 = 0;
                        var op2Index = modrm.rm;

                        if (modrm.mod == 3) {
                            if ((Flags & ADDR_SIZE_32) != 0) {
                                op2 = generalRegisters[op2Index];
                                _dbgLastOperand = regArray32[op2Index];
                            }
                            else {
                                op2 = (UInt16)(generalRegisters[op2Index] & 0x0000FFFF);
                                _dbgLastOperand = regArray16[op2Index];
                            }
                        }
                        else {
                            UInt32 address = 0;

                            if ((Flags & ADDR_SIZE_32) != 0) {
                                address = Read32ModRm(modrm);
                                op2 = Read32(address);
                            }
                            else {
                                address = Read16ModRm(modrm);
                                op2 = Read16(address);
                            }
                        }

                        if ((Flags & ADDR_SIZE_32) != 0) {
                            var op1 = AX;
                            UInt64 result = op1 * op2;
                            EAX = (UInt32)(result & 0x00000000FFFFFFFF);
                            EDX = (UInt32)(result & 0xFFFFFFFF00000000);

                            DbgIns($"MUL {_dbgLastOperand}");
                        }
                        else {
                            UInt16 op1 = AX;
                            UInt32 result = (UInt16)(op1 * op2);
                            AX = (UInt16)(result & 0x0000FFFF);
                            DX = (UInt16)(result & 0xFFFF0000);

                            DbgIns($"MUL {_dbgLastOperand}");
                        }
                    }
                    break;

                case 5:
                    DbgIns("IMUL not implemented");
                    Hardware.RaiseException(0);
                    Hlt();
                    break;

                case 6:
                    DbgIns("DIV not implemented");
                    Hardware.RaiseException(0);
                    Hlt();
                    break;

                case 7:
                    DbgIns("IDIV not implemented");
                    Hardware.RaiseException(0);
                    Hlt();
                    break;
            }
        }

        [OpCode(0xFF)]
        void OpcodeFF() {
            var modrm = ReadModRm();
            switch (modrm.reg) {
                case 6:
                    var value = ReadImm16();
                    Push16(value);
                    DbgIns($"PUSH 0x{value:X4}");
                    break;
            }
        }

        public string RegisterDump {
            get {
                var regText = $"EAX = {EAX:X8} EBX = {EBX:X8} ECX = {ECX:X8} EDX = {EDX:X8} ESI = {ESI:X8} EDI = {EDI:X8} EIP = {EIP:X8} ESP = {ESP:X8} EBP = {EBP:X8} EFLAGS = {EFLAGS:X4}\r\n\r\nCS = {CS:X4} DS = {DS:X4} ES = {ES:X4} SS = {SS:X4} FS = {FS:X4} GS = {GS:X4}\r\n\r\nCF = {CF} PF = {PF} AF = {AF} ZF = {ZF} SF = {SF} DF = {DF} OF = {OF}, TF = {TF}";
                return regText;
            }
        }

        public bool SingleStep {
            get => TF == 1;
            set {
                if (value) {
                    TF = 1;
                    runEvent.Set();
                }
                else {
                    TF = 0;
                    runEvent.Reset();
                }
            }
        }
    }
}
