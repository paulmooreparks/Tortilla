using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using Tortilla;

namespace Maize {
    public class Cpu : Register, ICpu<UInt64> {
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

        public Cpu(Motherboard motherboard) {
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

            Decoder = new Decoder(MB);
            // Decoder.InstructionRead += InstructionRegister_InstructionRead;
            Decoder.RegisterMap[Instruction.OpFlag_RegA >> 4] = A;
            Decoder.RegisterMap[Instruction.OpFlag_RegB >> 4] = B;
            Decoder.RegisterMap[Instruction.OpFlag_RegC >> 4] = C;
            Decoder.RegisterMap[Instruction.OpFlag_RegD >> 4] = D;
            Decoder.RegisterMap[Instruction.OpFlag_RegE >> 4] = E;
            Decoder.RegisterMap[Instruction.OpFlag_RegG >> 4] = G;
            Decoder.RegisterMap[Instruction.OpFlag_RegH >> 4] = H;
            Decoder.RegisterMap[Instruction.OpFlag_RegJ >> 4] = J;
            Decoder.RegisterMap[Instruction.OpFlag_RegK >> 4] = K;
            Decoder.RegisterMap[Instruction.OpFlag_RegL >> 4] = L;
            Decoder.RegisterMap[Instruction.OpFlag_RegM >> 4] = M;
            Decoder.RegisterMap[Instruction.OpFlag_RegZ >> 4] = Z;
            Decoder.RegisterMap[Instruction.OpFlag_RegF >> 4] = F;
            Decoder.RegisterMap[Instruction.OpFlag_RegI >> 4] = Decoder;
            Decoder.RegisterMap[Instruction.OpFlag_RegP >> 4] = P;
            Decoder.RegisterMap[Instruction.OpFlag_RegS >> 4] = S;

            I = Decoder;

            P.RegData.H0 = 0x0000_1000; // Start address
            P.RegData.H1 = 0x0000_0000; // Start segment

            Alu = new Alu(MB, this);

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

        Motherboard MB = null;

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

        public Alu Alu;
        public Decoder Decoder;

        public Register A = new Register();
        public Register B = new Register();
        public Register C = new Register();
        public Register D = new Register();
        public Register E = new Register();
        public Register G = new Register();
        public Register H = new Register();
        public Register J = new Register();
        public Register K = new Register();
        public Register L = new Register();
        public Register M = new Register();
        public Register Z = new Register();
        public Register F = new Register();
        public Register I = null; 
        public Register P = new Register();
        public Register S = new Register();
     
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

    public class Decoder : Register {
        public Decoder(Motherboard _motherboard) {
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

        public void JumpTo(Instruction instruction) {
            ActiveInstruction = instruction;
            Step = 0;
            ActiveInstruction.Code[Step]();
        }


        // public event EventHandler<Tuple<UInt64, UInt64>> InstructionRead;

        public Register OperandRegister1 = new Register();
        public Register OperandRegister2 = new Register();
        public Register OperandRegister3 = new Register();
        public Register OperandRegister4 = new Register();
        public Register SrcReg = null;
        public Register DestReg = null;

        Motherboard MB = null;
        public Tortilla.IClock Clock => MB.Clock;

        public int Step = 0;
        public int Cycle = 0;

        public Register[] RegisterMap = new Register[0x16];

        public Instruction[] InstructionArray;
        protected Instruction ActiveInstruction;

        public override void OnTickExecute(IBusComponent cpuFlags) {
            if (Cycle == 0) {
                Step = 0;
                ActiveInstruction = Core.ReadOpcodeAndDispatch.Instance;

                if (   (MB.Cpu.F.RegData.W0 & Cpu.Flag_InterruptSet) == Cpu.Flag_InterruptSet
                    && (MB.Cpu.F.RegData.W0 & Cpu.Flag_InterruptEnabled) == Cpu.Flag_InterruptEnabled)
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
        Instruction[] MicrocodeStack = new Instruction[0x0F];
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

            InstructionArray = new Instruction[] {
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
                        throw new Exception($"Instruction already assigned to opcode {instruction.Opcode:X2}");
                    }
                }
            }


        }
    }

}
