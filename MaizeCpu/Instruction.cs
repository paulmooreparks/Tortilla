using System;
using System.Text;
using Tortilla;

namespace Maize {
    public class Instruction {
        public Instruction() {
        }

        public static Motherboard MB;
        public static Cpu Cpu;
        public static Decoder Decoder;
        public static Register OperandRegister1;
        public static Register OperandRegister2;
        public static Register OperandRegister3;
        public static Register OperandRegister4;
        public static MemoryModule MemoryModule;
        public static Register SrcReg;
        public static Register DestReg;
        public static Register F;
        public static Register I;
        public static Register P;
        public static Register S;
        public static Alu Alu;

        // public static Register A = Cpu.A;
        // public static Register B = Cpu.B;
        // public static Register C = Cpu.C;
        // public static Register D = Cpu.D;
        // public static Register E = Cpu.E;
        // public static Register G = Cpu.G;
        // public static Register H = Cpu.H;
        // public static Register J = Cpu.J;
        // public static Register K = Cpu.K;
        // public static Register L = Cpu.L;
        // public static Register M = Cpu.M;
        // public static Register Z = Cpu.Z;
        // public static Tortilla.IClock Clock = MB.Clock;

        internal static void WireUp(Motherboard motherboard) {
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
            Alu.OpSize_Byte,
            Alu.OpSize_Byte,
            Alu.OpSize_Byte,
            Alu.OpSize_Byte,
            Alu.OpSize_Byte,
            Alu.OpSize_Byte,
            Alu.OpSize_Byte,
            Alu.OpSize_Byte,
            Alu.OpSize_QuarterWord,
            Alu.OpSize_QuarterWord,
            Alu.OpSize_QuarterWord,
            Alu.OpSize_QuarterWord,
            Alu.OpSize_HalfWord,
            Alu.OpSize_HalfWord,
            Alu.OpSize_Word
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

    public class InstructionBase<T> : Instruction where T : Instruction {
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


}
