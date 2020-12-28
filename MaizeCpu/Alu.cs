using System;
using System.Numerics;
using Tortilla;

/* One thing that you'll notice upon reading the code, especially if you have any familiarity with C# and 
with .NET in general, is that I don't use a lot of properties, but rather fields. I did this for speed and 
simplicity. Inside a CPU, and in assembly code in general, there's not a lot of information hiding, though 
there is encapsulation to some degree. My primary concern in this code is not to write enterprise-grade 
interfaces, but rather to get raw speed wherever I can. I'm sacrificing some sacred OO principles in pursuit 
of performance, and inside the ugly innards of the CPU implementation, it's probably worth it. */

namespace Maize {
    public class Alu : Register {
        public Alu(Motherboard _motherboard, Cpu _cpu) {
            Motherboard = _motherboard;
            Cpu = _cpu;

            DestReg.AddressBus = Motherboard.AddressBus;
            DestReg.DataBus = Motherboard.DataBus;
            DestReg.IOBus = Motherboard.IOBus;
            Motherboard.ConnectComponent(DestReg);

            SrcReg.AddressBus = Motherboard.AddressBus;
            SrcReg.DataBus = Motherboard.DataBus;
            SrcReg.IOBus = Motherboard.IOBus;
            Motherboard.ConnectComponent(SrcReg);

            this.DataBus = Motherboard.DataBus;
            Motherboard.ConnectComponent(this);

            RequestTickSetFromAddressBus += OnRequestTickSet;
            RequestTickSetFromDataBus += OnRequestTickSet;
            RequestTickSetFromIOBus += OnRequestTickSet;
        }

        private void OnRequestTickSet(IBusComponent obj) {
            RegisterTickExecute();
        }

        Motherboard Motherboard { get; set; }
        Cpu Cpu { get; set; }

        public Register DestReg { get; set; } = new Register();
        public Register SrcReg { get; set; } = new Register();

        public const byte OpCode_ADD  = 0b_0000_0000;
        public const byte OpCode_SUB  = 0b_0000_0001;
        public const byte OpCode_MUL  = 0b_0000_0010;
        public const byte OpCode_DIV  = 0b_0000_0011;
        public const byte OpCode_MOD  = 0b_0000_0100;
        public const byte OpCode_INC  = 0b_0000_0101;
        public const byte OpCode_DEC  = 0b_0000_0110;
        public const byte OpCode_AND  = 0b_0000_0111;
        public const byte OpCode_OR   = 0b_0000_1000;
        public const byte OpCode_NOR  = 0b_0000_1001;
        public const byte OpCode_NOT  = 0b_0000_1010;
        public const byte OpCode_NAND = 0b_0000_1011;
        public const byte OpCode_XOR  = 0b_0000_1100;
        public const byte OpCode_SHL  = 0b_0000_1101;
        public const byte OpCode_SHR  = 0b_0000_1110;
        public const byte OpCode_CMP  = 0b_0000_1111;
        public const byte OpCode_TEST = 0b_0001_0000;

        public const byte OpSize_Byte =          0b_0000_0000;
        public const byte OpSize_QuarterWord =   0b_0001_0000;
        public const byte OpSize_HalfWord =      0b_0010_0000;
        public const byte OpSize_Word =          0b_0011_0000;

        public const byte OpCodeCtrl_CarryIn =   0b_1000_0000;

        protected const byte OpCodeFlag_Code =  0b_0000_1111;
        protected const byte OpCodeFlag_Size =  0b_0011_0000;
        protected const byte OpCodeFlag_Ctrl =  0b_1100_0000;

        public byte CarryIn {
            get { return (byte)(this.RegData.B0 & OpCodeCtrl_CarryIn); }
            set { this.RegData.B0 = (byte)((this.RegData.B0 & ~OpCodeCtrl_CarryIn) | ((value & 0b_0000_0001) << (OpCodeCtrl_CarryIn / 4))); }
        }

        public override void OnTickExecute(IBusComponent cpuFlags) {
            switch (RegData.B0 & OpCodeFlag_Code) {
            case OpCode_ADD:
                switch (RegData.B0 & OpCodeFlag_Size) {
                case OpSize_Byte:
                    DestReg.RegData.B0 = ADD_Byte();
                    break;

                case OpSize_QuarterWord:
                    DestReg.RegData.Q0 = ADD_QuarterWord();
                    break;

                case OpSize_HalfWord:
                    DestReg.RegData.H0 = ADD_HalfWord();
                    break;

                case OpSize_Word:
                    DestReg.RegData.W0 = ADD_Word();
                    break;

                default:
                    goto case OpSize_Word;
                }

                break;

            case OpCode_SUB:
                switch (RegData.B0 & OpCodeFlag_Size) {
                case OpSize_Byte:
                    DestReg.RegData.B0 = SUB_Byte();
                    break;

                case OpSize_QuarterWord:
                    DestReg.RegData.Q0 = SUB_QuarterWord();
                    break;

                case OpSize_HalfWord:
                    DestReg.RegData.H0 = SUB_HalfWord();
                    break;

                case OpSize_Word:
                    DestReg.RegData.W0 = SUB_Word();
                    break;

                default:
                    goto case OpSize_Word;
                }

                break;

            case OpCode_MUL:
                switch (RegData.B0 & OpCodeFlag_Size) {
                case OpSize_Byte:
                    DestReg.RegData.B0 = MUL_Byte();
                    break;

                case OpSize_QuarterWord:
                    DestReg.RegData.Q0 = MUL_QuarterWord();
                    break;

                case OpSize_HalfWord:
                    DestReg.RegData.H0 = MUL_HalfWord();
                    break;

                case OpSize_Word:
                    DestReg.RegData.W0 = MUL_Word();
                    break;

                default:
                    goto case OpSize_Word;
                }

                break;

            case OpCode_DIV:
                switch (RegData.B0 & OpCodeFlag_Size) {
                case OpSize_Byte:
                    DestReg.RegData.B0 = DIV_Byte();
                    break;

                case OpSize_QuarterWord:
                    DestReg.RegData.Q0 = DIV_QuarterWord();
                    break;

                case OpSize_HalfWord:
                    DestReg.RegData.H0 = DIV_HalfWord();
                    break;

                case OpSize_Word:
                    DestReg.RegData.W0 = DIV_Word();
                    break;

                default:
                    goto case OpSize_Word;
                }

                break;

            case OpCode_MOD:
                switch (RegData.B0 & OpCodeFlag_Size) {
                case OpSize_Byte:
                    DestReg.RegData.B0 = MOD_Byte();
                    break;

                case OpSize_QuarterWord:
                    DestReg.RegData.Q0 = MOD_QuarterWord();
                    break;

                case OpSize_HalfWord:
                    DestReg.RegData.H0 = MOD_HalfWord();
                    break;

                case OpSize_Word:
                    DestReg.RegData.W0 = MOD_Word();
                    break;

                default:
                    goto case OpSize_Word;
                }

                break;

            case OpCode_INC:
                switch (RegData.B0 & OpCodeFlag_Size) {
                case OpSize_Byte:
                    DestReg.RegData.B0 = INC_Byte();
                    break;

                case OpSize_QuarterWord:
                    DestReg.RegData.Q0 = INC_QuarterWord();
                    break;

                case OpSize_HalfWord:
                    DestReg.RegData.H0 = INC_HalfWord();
                    break;

                case OpSize_Word:
                    DestReg.RegData.W0 = INC_Word();
                    break;

                default:
                    goto case OpSize_Word;
                }

                break;

            case OpCode_DEC:
                switch (RegData.B0 & OpCodeFlag_Size) {
                case OpSize_Byte:
                    DestReg.RegData.B0 = DEC_Byte();
                    break;

                case OpSize_QuarterWord:
                    DestReg.RegData.Q0 = DEC_QuarterWord();
                    break;

                case OpSize_HalfWord:
                    DestReg.RegData.H0 = DEC_HalfWord();
                    break;

                case OpSize_Word:
                    DestReg.RegData.W0 = DEC_Word();
                    break;

                default:
                    goto case OpSize_Word;
                }

                break;

            case OpCode_AND:
                switch (RegData.B0 & OpCodeFlag_Size) {
                case OpSize_Byte:
                    DestReg.RegData.B0 = AND_Byte();
                    break;

                case OpSize_QuarterWord:
                    DestReg.RegData.Q0 = AND_QuarterWord();
                    break;

                case OpSize_HalfWord:
                    DestReg.RegData.H0 = AND_HalfWord();
                    break;

                case OpSize_Word:
                    DestReg.RegData.W0 = AND_Word();
                    break;

                default:
                    goto case OpSize_Word;
                }

                break;

            case OpCode_OR:
                switch (RegData.B0 & OpCodeFlag_Size) {
                case OpSize_Byte:
                    DestReg.RegData.B0 = OR_Byte();
                    break;

                case OpSize_QuarterWord:
                    DestReg.RegData.Q0 = OR_QuarterWord();
                    break;

                case OpSize_HalfWord:
                    DestReg.RegData.H0 = OR_HalfWord();
                    break;

                case OpSize_Word:
                    DestReg.RegData.W0 = OR_Word();
                    break;

                default:
                    goto case OpSize_Word;
                }

                break;

            case OpCode_NOR:
                switch (RegData.B0 & OpCodeFlag_Size) {
                case OpSize_Byte:
                    DestReg.RegData.B0 = NOR_Byte();
                    break;

                case OpSize_QuarterWord:
                    DestReg.RegData.Q0 = NOR_QuarterWord();
                    break;

                case OpSize_HalfWord:
                    DestReg.RegData.H0 = NOR_HalfWord();
                    break;

                case OpSize_Word:
                    DestReg.RegData.W0 = NOR_Word();
                    break;

                default:
                    goto case OpSize_Word;
                }

                break;

            case OpCode_NOT:
                switch (RegData.B0 & OpCodeFlag_Size) {
                case OpSize_Byte:
                    DestReg.RegData.B0 = NOT_Byte();
                    break;

                case OpSize_QuarterWord:
                    DestReg.RegData.Q0 = NOT_QuarterWord();
                    break;

                case OpSize_HalfWord:
                    DestReg.RegData.H0 = NOT_HalfWord();
                    break;

                case OpSize_Word:
                    DestReg.RegData.W0 = NOT_Word();
                    break;

                default:
                    goto case OpSize_Word;
                }

                break;

            case OpCode_NAND:
                switch (RegData.B0 & OpCodeFlag_Size) {
                case OpSize_Byte:
                    DestReg.RegData.B0 = NAND_Byte();
                    break;

                case OpSize_QuarterWord:
                    DestReg.RegData.Q0 = NAND_QuarterWord();
                    break;

                case OpSize_HalfWord:
                    DestReg.RegData.H0 = NAND_HalfWord();
                    break;

                case OpSize_Word:
                    DestReg.RegData.W0 = NAND_Word();
                    break;

                default:
                    goto case OpSize_Word;
                }

                break;

            case OpCode_XOR:
                switch (RegData.B0 & OpCodeFlag_Size) {
                case OpSize_Byte:
                    DestReg.RegData.B0 = XOR_Byte();
                    break;

                case OpSize_QuarterWord:
                    DestReg.RegData.Q0 = XOR_QuarterWord();
                    break;

                case OpSize_HalfWord:
                    DestReg.RegData.H0 = XOR_HalfWord();
                    break;

                case OpSize_Word:
                    DestReg.RegData.W0 = XOR_Word();
                    break;

                default:
                    goto case OpSize_Word;
                }

                break;

            case OpCode_SHL:
                switch (RegData.B0 & OpCodeFlag_Size) {
                case OpSize_Byte:
                    DestReg.RegData.B0 = SHL_Byte();
                    break;

                case OpSize_QuarterWord:
                    DestReg.RegData.Q0 = SHL_QuarterWord();
                    break;

                case OpSize_HalfWord:
                    DestReg.RegData.H0 = SHL_HalfWord();
                    break;

                case OpSize_Word:
                    DestReg.RegData.W0 = SHL_Word();
                    break;

                default:
                    goto case OpSize_Word;
                }

                break;

            case OpCode_SHR:
                switch (RegData.B0 & OpCodeFlag_Size) {
                case OpSize_Byte:
                    DestReg.RegData.B0 = SHR_Byte();
                    break;

                case OpSize_QuarterWord:
                    DestReg.RegData.Q0 = SHR_QuarterWord();
                    break;

                case OpSize_HalfWord:
                    DestReg.RegData.H0 = SHR_HalfWord();
                    break;

                case OpSize_Word:
                    DestReg.RegData.W0 = SHR_Word();
                    break;

                default:
                    goto case OpSize_Word;
                }

                break;

            case OpCode_CMP:
                switch (RegData.B0 & OpCodeFlag_Size) {
                case OpSize_Byte:
                    SUB_Byte();
                    break;

                case OpSize_QuarterWord:
                    SUB_QuarterWord();
                    break;

                case OpSize_HalfWord:
                    SUB_HalfWord();
                    break;

                case OpSize_Word:
                    SUB_Word();
                    break;

                default:
                    goto case OpSize_Word;
                }

                break;

            case OpCode_TEST:
                switch (RegData.B0 & OpCodeFlag_Size) {
                case OpSize_Byte:
                    AND_Byte();
                    break;

                case OpSize_QuarterWord:
                    AND_QuarterWord();
                    break;

                case OpSize_HalfWord:
                    AND_HalfWord();
                    break;

                case OpSize_Word:
                    AND_Word();
                    break;

                default:
                    goto case OpSize_Word;
                }

                break;
            }
        }

        byte UpdateFlags(byte result) {
            Cpu.ZeroFlag = (result == 0);
            Cpu.NegativeFlag = ((result & 0b_1000_0000) != 0);
            return result;
        }

        UInt16 UpdateFlags(UInt16 result) {
            Cpu.ZeroFlag = (result == 0);
            Cpu.NegativeFlag = ((Int16)result) < 0;
            return result;
        }

        UInt32 UpdateFlags(UInt32 result) {
            Cpu.ZeroFlag = (result == 0);
            Cpu.NegativeFlag = ((Int32)result) < 0;
            return result;
        }

        UInt64 UpdateFlags(UInt64 result) {
            Cpu.ZeroFlag = (result == 0);
            Cpu.NegativeFlag = ((Int64)result) < 0;
            return result;
        }


        private byte ADD_Byte() {
            byte result;

            try {
                result = checked((byte)(DestReg.RegData.B0 + SrcReg.RegData.B0));
            }
            catch (OverflowException) {
                Cpu.OverflowFlag = true;
                result = unchecked((byte)(DestReg.RegData.B0 + SrcReg.RegData.B0));
            }

            Cpu.CarryFlag = (result < DestReg.RegData.W0);
            return UpdateFlags(result);
        }

        private UInt16 ADD_QuarterWord() {
            UInt16 result;

            try {
                result = checked((ushort)(DestReg.RegData.Q0 + SrcReg.RegData.Q0));
            }
            catch (OverflowException) {
                Cpu.OverflowFlag = true;
                result = unchecked((ushort)(DestReg.RegData.Q0 + SrcReg.RegData.Q0));
            }

            Cpu.CarryFlag = (result < DestReg.RegData.W0);
            return UpdateFlags(result);
        }

        private UInt32 ADD_HalfWord() {
            UInt32 result;

            try {
                result = checked(DestReg.RegData.H0 + SrcReg.RegData.H0);
            }
            catch (OverflowException) {
                Cpu.OverflowFlag = true;
                result = unchecked(DestReg.RegData.H0 + SrcReg.RegData.H0);
            }

            Cpu.CarryFlag = (result < DestReg.RegData.W0);
            return UpdateFlags(result);
        }

        private UInt64 ADD_Word() {
            UInt64 result;

            try {
                result = checked(DestReg.RegData.W0 + SrcReg.RegData.W0);
            }
            catch (OverflowException) {
                Cpu.OverflowFlag = true;
                result = unchecked(DestReg.RegData.W0 + SrcReg.RegData.W0);
            }

            Cpu.CarryFlag = (result < DestReg.RegData.W0);
            return UpdateFlags(result);
        }


        private byte SUB_Byte() {
            byte result;

            try {
                result = checked((byte)(DestReg.RegData.B0 - SrcReg.RegData.B0));
            }
            catch (OverflowException) {
                Cpu.OverflowFlag = true;
                result = unchecked((byte)(DestReg.RegData.B0 - SrcReg.RegData.B0));
            }

            Cpu.CarryFlag = (SrcReg.RegData.B0 > DestReg.RegData.B0);
            return UpdateFlags(result);
        }

        private UInt16 SUB_QuarterWord() {
            UInt16 result;

            try {
                result = checked((ushort)(DestReg.RegData.Q0 - SrcReg.RegData.Q0));
            }
            catch (OverflowException) {
                Cpu.OverflowFlag = true;
                result = unchecked((ushort)(DestReg.RegData.Q0 - SrcReg.RegData.Q0));
            }

            Cpu.CarryFlag = (SrcReg.RegData.Q0 > DestReg.RegData.Q0);
            return UpdateFlags(result);
        }

        private UInt32 SUB_HalfWord() {
            UInt32 result;

            try {
                result = checked(DestReg.RegData.H0 - SrcReg.RegData.H0);
            }
            catch (OverflowException) {
                Cpu.OverflowFlag = true;
                result = unchecked(DestReg.RegData.H0 - SrcReg.RegData.H0);
            }

            Cpu.CarryFlag = (SrcReg.RegData.H0 > DestReg.RegData.H0);
            return UpdateFlags(result);
        }

        private UInt64 SUB_Word() {
            UInt64 result;

            try {
                result = checked(DestReg.RegData.W0 - SrcReg.RegData.W0);
            }
            catch (OverflowException) {
                Cpu.OverflowFlag = true;
                result = unchecked(DestReg.RegData.W0 - SrcReg.RegData.W0);
            }

            Cpu.CarryFlag = (SrcReg.RegData.W0 > DestReg.RegData.W0);
            return UpdateFlags(result);
        }


        private byte INC_Byte() {
            byte result;

            try {
                result = checked((byte)(DestReg.RegData.B0 + 1));
            }
            catch (OverflowException) {
                Cpu.OverflowFlag = true;
                result = unchecked((byte)(DestReg.RegData.B0 + 1));
            }

            Cpu.CarryFlag = (result < DestReg.RegData.W0);
            return UpdateFlags(result);
        }

        private UInt16 INC_QuarterWord() {
            UInt16 result;

            try {
                result = checked((ushort)(DestReg.RegData.Q0 + 1));
            }
            catch (OverflowException) {
                Cpu.OverflowFlag = true;
                result = unchecked((ushort)(DestReg.RegData.Q0 + 1));
            }

            Cpu.CarryFlag = (result < DestReg.RegData.W0);
            return UpdateFlags(result);
        }

        private UInt32 INC_HalfWord() {
            UInt32 result;

            try {
                result = checked(DestReg.RegData.H0 + 1);
            }
            catch (OverflowException) {
                Cpu.OverflowFlag = true;
                result = unchecked(DestReg.RegData.H0 + 1);
            }

            Cpu.CarryFlag = (result < DestReg.RegData.W0);
            return UpdateFlags(result);
        }

        private UInt64 INC_Word() {
            UInt64 result;

            try {
                result = checked(DestReg.RegData.W0 + 1);
            }
            catch (OverflowException) {
                Cpu.OverflowFlag = true;
                result = unchecked(DestReg.RegData.W0 + 1);
            }

            Cpu.CarryFlag = (result < DestReg.RegData.W0);
            return UpdateFlags(result);
        }


        private byte DEC_Byte() {
            byte result;

            try {
                result = checked((byte)(DestReg.RegData.B0 - 1));
            }
            catch (OverflowException) {
                Cpu.OverflowFlag = true;
                result = unchecked((byte)(DestReg.RegData.B0 - 1));
            }

            Cpu.CarryFlag = (result < DestReg.RegData.W0);
            return UpdateFlags(result);
        }

        private UInt16 DEC_QuarterWord() {
            UInt16 result;

            try {
                result = checked((ushort)(DestReg.RegData.Q0 - 1));
            }
            catch (OverflowException) {
                Cpu.OverflowFlag = true;
                result = unchecked((ushort)(DestReg.RegData.Q0 - 1));
            }

            Cpu.CarryFlag = (result < DestReg.RegData.W0);
            return UpdateFlags(result);
        }

        private UInt32 DEC_HalfWord() {
            UInt32 result;

            try {
                result = checked(DestReg.RegData.H0 - 1);
            }
            catch (OverflowException) {
                Cpu.OverflowFlag = true;
                result = unchecked(DestReg.RegData.H0 - 1);
            }

            Cpu.CarryFlag = (result < DestReg.RegData.W0);
            return UpdateFlags(result);
        }

        private UInt64 DEC_Word() {
            UInt64 result;

            try {
                result = checked(DestReg.RegData.W0 - 1);
            }
            catch (OverflowException) {
                Cpu.OverflowFlag = true;
                result = unchecked(DestReg.RegData.W0 - 1);
            }

            Cpu.CarryFlag = (result < DestReg.RegData.W0);
            return UpdateFlags(result);
        }


        private byte MUL_Byte() {
            UInt16 result = (UInt16)(DestReg.RegData.B0 * SrcReg.RegData.B0);
            Cpu.OverflowFlag = result > (int)byte.MaxValue;
            Cpu.CarryFlag = result > byte.MaxValue;
            return UpdateFlags((byte)result);
        }

        private UInt16 MUL_QuarterWord() {
            UInt32 result = (UInt32)(DestReg.RegData.Q0 * SrcReg.RegData.Q0);
            Cpu.OverflowFlag = result > Int16.MaxValue;
            Cpu.CarryFlag = result > UInt16.MaxValue;
            return UpdateFlags((UInt16)result);
        }

        private UInt32 MUL_HalfWord() {
            UInt64 result = (UInt64)(DestReg.RegData.Q0 * SrcReg.RegData.Q0);
            Cpu.OverflowFlag = result > Int32.MaxValue;
            Cpu.CarryFlag = result > UInt32.MaxValue;
            return UpdateFlags((UInt32)result);
        }

        private UInt64 MUL_Word() {
            BigInteger result = (BigInteger)(DestReg.RegData.Q0 * SrcReg.RegData.Q0);
            Cpu.OverflowFlag = result > Int64.MaxValue;
            Cpu.CarryFlag = result > UInt64.MaxValue;
            return UpdateFlags((UInt64)result);
        }


        private byte DIV_Byte() {
            byte result;

            try {
                result = checked((byte)(DestReg.RegData.B0 / SrcReg.RegData.B0));
            }
            catch (OverflowException) {
                Cpu.OverflowFlag = true;
                result = unchecked((byte)(DestReg.RegData.B0 / SrcReg.RegData.B0));
            }
            catch (DivideByZeroException) {
                result = DestReg.RegData.B0;
                Motherboard.RaiseException(0x00);
            }

            return UpdateFlags(result);
        }

        private UInt16 DIV_QuarterWord() {
            UInt16 result;

            try {
                result = checked((ushort)(DestReg.RegData.Q0 / SrcReg.RegData.Q0));
            }
            catch (OverflowException) {
                Cpu.OverflowFlag = true;
                result = unchecked((ushort)(DestReg.RegData.Q0 / SrcReg.RegData.Q0));
            }
            catch (DivideByZeroException) {
                result = DestReg.RegData.Q0;
                Motherboard.RaiseException(0x00);
            }

            return UpdateFlags(result);
        }

        private UInt32 DIV_HalfWord() {
            UInt32 result;

            try {
                result = checked(DestReg.RegData.H0 / SrcReg.RegData.H0);
            }
            catch (OverflowException) {
                Cpu.OverflowFlag = true;
                result = unchecked(DestReg.RegData.H0 / SrcReg.RegData.H0);
            }
            catch (DivideByZeroException) {
                result = DestReg.RegData.H0;
                Motherboard.RaiseException(0x00);
            }

            return UpdateFlags(result);
        }

        private UInt64 DIV_Word() {
            UInt64 result;

            try {
                result = checked(DestReg.RegData.W0 / SrcReg.RegData.W0);
            }
            catch (OverflowException) {
                Cpu.OverflowFlag = true;
                result = unchecked(DestReg.RegData.W0 / SrcReg.RegData.W0);
            }
            catch (DivideByZeroException) {
                result = DestReg.RegData.W0;
                Motherboard.RaiseException(0x00);
            }

            return UpdateFlags(result);
        }


        private byte MOD_Byte() {
            byte result;

            try {
                result = checked((byte)(DestReg.RegData.B0 % SrcReg.RegData.B0));
            }
            catch (OverflowException) {
                Cpu.OverflowFlag = true;
                result = unchecked((byte)(DestReg.RegData.B0 % SrcReg.RegData.B0));
            }
            catch (DivideByZeroException) {
                result = DestReg.RegData.B0;
                Motherboard.RaiseException(0x00);
            }

            return UpdateFlags(result);
        }

        private UInt16 MOD_QuarterWord() {
            UInt16 result;

            try {
                result = checked((ushort)(DestReg.RegData.Q0 % SrcReg.RegData.Q0));
            }
            catch (OverflowException) {
                Cpu.OverflowFlag = true;
                result = unchecked((ushort)(DestReg.RegData.Q0 % SrcReg.RegData.Q0));
            }
            catch (DivideByZeroException) {
                result = DestReg.RegData.Q0;
                Motherboard.RaiseException(0x00);
            }

            return UpdateFlags(result);
        }

        private UInt32 MOD_HalfWord() {
            UInt32 result;

            try {
                result = checked(DestReg.RegData.H0 % SrcReg.RegData.H0);
            }
            catch (OverflowException) {
                Cpu.OverflowFlag = true;
                result = unchecked(DestReg.RegData.H0 % SrcReg.RegData.H0);
            }
            catch (DivideByZeroException) {
                result = DestReg.RegData.H0;
                Motherboard.RaiseException(0x00);
            }

            return UpdateFlags(result);
        }

        private UInt64 MOD_Word() {
            UInt64 result;

            try {
                result = checked(DestReg.RegData.W0 % SrcReg.RegData.W0);
            }
            catch (OverflowException) {
                Cpu.OverflowFlag = true;
                result = unchecked(DestReg.RegData.W0 % SrcReg.RegData.W0);
            }
            catch (DivideByZeroException) {
                result = DestReg.RegData.W0;
                Motherboard.RaiseException(0x00);
            }

            Cpu.CarryFlag = (result < SrcReg.RegData.W0);
            return UpdateFlags(result);
        }


        private byte AND_Byte() {
            byte result = (byte)(DestReg.RegData.B0 & SrcReg.RegData.B0);
            return UpdateFlags(result);
        }

        private UInt16 AND_QuarterWord() {
            UInt16 result = (ushort)(DestReg.RegData.Q0 & SrcReg.RegData.Q0);
            return UpdateFlags(result);
        }

        private UInt32 AND_HalfWord() {
            UInt32 result = DestReg.RegData.H0 & SrcReg.RegData.H0;
            return UpdateFlags(result);
        }

        private UInt64 AND_Word() {
            UInt64 result = DestReg.RegData.W0 & SrcReg.RegData.W0;
            return UpdateFlags(result);
        }


        private byte OR_Byte() {
            byte result = (byte)(DestReg.RegData.B0 | SrcReg.RegData.B0);
            return UpdateFlags(result);
        }

        private UInt16 OR_QuarterWord() {
            UInt16 result = (ushort)(DestReg.RegData.Q0 | SrcReg.RegData.Q0);
            return UpdateFlags(result);
        }

        private UInt32 OR_HalfWord() {
            UInt32 result = DestReg.RegData.H0 | SrcReg.RegData.H0;
            return UpdateFlags(result);
        }

        private UInt64 OR_Word() {
            UInt64 result = DestReg.RegData.W0 | SrcReg.RegData.W0;
            return UpdateFlags(result);
        }


        private byte NOR_Byte() {
            byte result = (byte)~(DestReg.RegData.B0 | SrcReg.RegData.B0);
            return UpdateFlags(result);
        }

        private UInt16 NOR_QuarterWord() {
            UInt16 result = (ushort)~(DestReg.RegData.Q0 | SrcReg.RegData.Q0);
            return UpdateFlags(result);
        }

        private UInt32 NOR_HalfWord() {
            UInt32 result = ~(DestReg.RegData.H0 | SrcReg.RegData.H0);
            return UpdateFlags(result);
        }

        private UInt64 NOR_Word() {
            UInt64 result = ~(DestReg.RegData.W0 | SrcReg.RegData.W0);
            return UpdateFlags(result);
        }


        private byte NOT_Byte() {
            byte result = (byte)~(DestReg.RegData.B0); 
            return UpdateFlags(result);
        }

        private UInt16 NOT_QuarterWord() {
            UInt16 result = (ushort)~(DestReg.RegData.Q0);
            return UpdateFlags(result);
        }

        private UInt32 NOT_HalfWord() {
            UInt32 result = ~(DestReg.RegData.H0);
            return UpdateFlags(result);
        }

        private UInt64 NOT_Word() {
            UInt64 result = ~(DestReg.RegData.W0);
            return UpdateFlags(result);
        }


        private byte NAND_Byte() {
            byte result = (byte)~(DestReg.RegData.B0 & SrcReg.RegData.B0);
            return UpdateFlags(result);
        }

        private UInt16 NAND_QuarterWord() {
            UInt16 result = (ushort)~(DestReg.RegData.Q0 & SrcReg.RegData.Q0);
            return UpdateFlags(result);
        }

        private UInt32 NAND_HalfWord() {
            UInt32 result = ~(DestReg.RegData.H0 & SrcReg.RegData.H0);
            return UpdateFlags(result);
        }

        private UInt64 NAND_Word() {
            UInt64 result = ~(DestReg.RegData.W0 & SrcReg.RegData.W0);
            return UpdateFlags(result);
        }


        private byte XOR_Byte() {
            byte result = (byte)(DestReg.RegData.B0 ^ SrcReg.RegData.B0);
            return UpdateFlags(result);
        }

        private UInt16 XOR_QuarterWord() {
            UInt16 result = (ushort)(DestReg.RegData.Q0 ^ SrcReg.RegData.Q0);
            return UpdateFlags(result);
        }

        private UInt32 XOR_HalfWord() {
            UInt32 result = (DestReg.RegData.H0 ^ SrcReg.RegData.H0);
            return UpdateFlags(result);
        }

        private UInt64 XOR_Word() {
            UInt64 result = (DestReg.RegData.W0 ^ SrcReg.RegData.W0);
            return UpdateFlags(result);
        }


        private byte SHL_Byte() {
            byte result = (byte)(DestReg.RegData.B0 << SrcReg.RegData.B0);
            return UpdateFlags(result);
        }

        private UInt16 SHL_QuarterWord() {
            UInt16 result = (ushort)(DestReg.RegData.Q0 << SrcReg.RegData.B0);
            return UpdateFlags(result);
        }

        private UInt32 SHL_HalfWord() {
            UInt32 result = (DestReg.RegData.H0 << SrcReg.RegData.B0);
            return UpdateFlags(result);
        }

        private UInt64 SHL_Word() {
            UInt64 result = (DestReg.RegData.W0 << SrcReg.RegData.B0);
            return UpdateFlags(result);
        }


        private byte SHR_Byte() {
            byte result = (byte)(DestReg.RegData.B0 >> SrcReg.RegData.B0);
            return UpdateFlags(result);
        }

        private UInt16 SHR_QuarterWord() {
            UInt16 result = (ushort)(DestReg.RegData.Q0 >> SrcReg.RegData.B0);
            return UpdateFlags(result);
        }

        private UInt32 SHR_HalfWord() {
            UInt32 result = (DestReg.RegData.H0 >> SrcReg.RegData.B0);
            return UpdateFlags(result);
        }

        private UInt64 SHR_Word() {
            UInt64 result = (DestReg.RegData.W0 >> SrcReg.RegData.B0);
            return UpdateFlags(result);
        }

    }

}
