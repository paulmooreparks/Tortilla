using System;
using Tortilla;

namespace Maize {
    public class MaizeAlu : MaizeRegister {
        public MaizeAlu(MaizeMotherboard _motherboard, MaizeCpu _cpu) {
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
        }

        MaizeMotherboard Motherboard { get; set; }
        MaizeCpu Cpu { get; set; }

        public MaizeRegister DestReg { get; set; } = new MaizeRegister();
        public MaizeRegister SrcReg { get; set; } = new MaizeRegister();

        public byte OpCode {
            get { return this.B0; }
            set { this.B0 = value; }
        }

        public const byte OpCode_ADD =  0b_0000_0000;
        public const byte OpCode_SUB =  0b_0000_0001;
        public const byte OpCode_MUL =  0b_0000_0010;
        public const byte OpCode_DIV =  0b_0000_0011;
        public const byte OpCode_MOD =  0b_0000_0100;
        public const byte OpCode_INC =  0b_0000_0101;
        public const byte OpCode_DEC =  0b_0000_0110;
        public const byte OpCode_AND =  0b_0000_0111;
        public const byte OpCode_OR =   0b_0000_1000;
        public const byte OpCode_NOR =  0b_0000_1001;
        public const byte OpCode_NOT =  0b_0000_1010;
        public const byte OpCode_NAND = 0b_0000_1011;
        public const byte OpCode_XOR =  0b_0000_1100;
        public const byte OpCode_SHL =  0b_0000_1101;
        public const byte OpCode_SHR =  0b_0000_1110;
        public const byte OpCode_CMP =  0b_0000_1111;

        public const byte OpSize_Byte =          0b_0000_0000;
        public const byte OpSize_QuarterWord =   0b_0001_0000;
        public const byte OpSize_HalfWord =      0b_0010_0000;
        public const byte OpSize_Word =          0b_0011_0000;

        public const byte OpCodeCtrl_CarryIn =   0b_1000_0000;

        protected const byte OpCodeFlag_Code =  0b_0000_1111;
        protected const byte OpCodeFlag_Size =  0b_0011_0000;
        protected const byte OpCodeFlag_Ctrl =  0b_1100_0000;

        public byte Flags {
            get { return Cpu.FRegister.B1; }
            set { Cpu.FRegister.B1 = value; }
        }

        protected const byte Flag_CarryOut =    0b_0000_0001;
        protected const byte Flag_Negative =    0b_0000_0010;
        protected const byte Flag_Overflow =    0b_0000_0100;
        protected const byte Flag_Parity =      0b_0000_1000;
        protected const byte Flag_Zero =        0b_0001_0000;
        protected const byte Flag_Sign =        0b_0010_0000;

        public byte CarryIn {
            get { return (byte)(this.B0 & OpCodeCtrl_CarryIn); }
            set { this.B0 = (byte)((this.B0 & ~OpCodeCtrl_CarryIn) | ((value & 0b_0000_0001) << (OpCodeCtrl_CarryIn / 2))); }
        }

        public byte Negative {
            get { return (byte)(Flags & Flag_Negative); }
            set { Flags = (byte)((Flags & ~Flag_Negative) | ((value & 0b_0000_0001) << (Flag_Negative / 2))); }
        }

        public byte Overflow {
            get { return (byte)(Flags & Flag_Overflow); }
            set { Flags = (byte)((Flags & ~Flag_Overflow) | ((value & 0b_0000_0001) << (Flag_Overflow / 2))); }
        }

        public byte Parity {
            get { return (byte)(Flags & Flag_Parity); }
            set { Flags = (byte)((Flags & ~Flag_Parity) | ((value & 0b_0000_0001) << (Flag_Parity / 2))); }
        }

        public byte Zero {
            get { return (byte)(Flags & Flag_Zero); }
            set { Flags = (byte)((Flags & ~Flag_Zero) | ((value & 0b_0000_0001) << (Flag_Zero / 2))); }
        }

        public byte CarryOut {
            get { return (byte)(Flags & Flag_CarryOut); }
            set { Flags = (byte)((Flags & ~Flag_CarryOut) | ((value & 0b_0000_0001) << (Flag_CarryOut / 2))); }
        }



        public override void OnTick(ClockState state) {
            base.OnTick(state);

            switch (state) {
            case ClockState.TickEnable:
                break;

            case ClockState.TickSet:
                ExecuteOperation();
                break;

            case ClockState.TickOff:
                break;

            }
        }

        void ExecuteOperation() {
            switch (OpCode & OpCodeFlag_Code) {
            case OpCode_ADD:
                switch (OpCode & OpCodeFlag_Size) {
                case OpSize_Byte:
                    DestReg.B0 = ADD_Byte();
                    break;

                case OpSize_QuarterWord:
                    DestReg.Q0 = ADD_QuarterWord();
                    break;

                case OpSize_HalfWord:
                    DestReg.H0 = ADD_HalfWord();
                    break;

                case OpSize_Word:
                    DestReg.Value = ADD_Word();
                    break;

                default:
                    goto case OpSize_Word;
                }

                break;

            case OpCode_SUB:
                switch (OpCode & OpCodeFlag_Size) {
                case OpSize_Byte:
                    DestReg.B0 = SUB_Byte();
                    break;

                case OpSize_QuarterWord:
                    DestReg.Q0 = SUB_QuarterWord();
                    break;

                case OpSize_HalfWord:
                    DestReg.H0 = SUB_HalfWord();
                    break;

                case OpSize_Word:
                    DestReg.Value = SUB_Word();
                    break;

                default:
                    goto case OpSize_Word;
                }

                break;

            case OpCode_MUL:
                switch (OpCode & OpCodeFlag_Size) {
                case OpSize_Byte:
                    DestReg.B0 = MUL_Byte();
                    break;

                case OpSize_QuarterWord:
                    DestReg.Q0 = MUL_QuarterWord();
                    break;

                case OpSize_HalfWord:
                    DestReg.H0 = MUL_HalfWord();
                    break;

                case OpSize_Word:
                    DestReg.Value = MUL_Word();
                    break;

                default:
                    goto case OpSize_Word;
                }

                break;

            case OpCode_DIV:
                switch (OpCode & OpCodeFlag_Size) {
                case OpSize_Byte:
                    DestReg.B0 = DIV_Byte();
                    break;

                case OpSize_QuarterWord:
                    DestReg.Q0 = DIV_QuarterWord();
                    break;

                case OpSize_HalfWord:
                    DestReg.H0 = DIV_HalfWord();
                    break;

                case OpSize_Word:
                    DestReg.Value = DIV_Word();
                    break;

                default:
                    goto case OpSize_Word;
                }

                break;

            case OpCode_MOD:
                switch (OpCode & OpCodeFlag_Size) {
                case OpSize_Byte:
                    DestReg.B0 = MOD_Byte();
                    break;

                case OpSize_QuarterWord:
                    DestReg.Q0 = MOD_QuarterWord();
                    break;

                case OpSize_HalfWord:
                    DestReg.H0 = MOD_HalfWord();
                    break;

                case OpSize_Word:
                    DestReg.Value = MOD_Word();
                    break;

                default:
                    goto case OpSize_Word;
                }

                break;

            case OpCode_INC:
                switch (OpCode & OpCodeFlag_Size) {
                case OpSize_Byte:
                    DestReg.B0 = INC_Byte();
                    break;

                case OpSize_QuarterWord:
                    DestReg.Q0 = INC_QuarterWord();
                    break;

                case OpSize_HalfWord:
                    DestReg.H0 = INC_HalfWord();
                    break;

                case OpSize_Word:
                    DestReg.Value = INC_Word();
                    break;

                default:
                    goto case OpSize_Word;
                }

                break;

            case OpCode_DEC:
                switch (OpCode & OpCodeFlag_Size) {
                case OpSize_Byte:
                    DestReg.B0 = DEC_Byte();
                    break;

                case OpSize_QuarterWord:
                    DestReg.Q0 = DEC_QuarterWord();
                    break;

                case OpSize_HalfWord:
                    DestReg.H0 = DEC_HalfWord();
                    break;

                case OpSize_Word:
                    DestReg.Value = DEC_Word();
                    break;

                default:
                    goto case OpSize_Word;
                }

                break;

            case OpCode_AND:
                switch (OpCode & OpCodeFlag_Size) {
                case OpSize_Byte:
                    DestReg.B0 = (byte)(DestReg.B0 & SrcReg.B0);
                    break;

                case OpSize_QuarterWord:
                    DestReg.Q0 = (ushort)(DestReg.Q0 & SrcReg.Q0);
                    break;

                case OpSize_HalfWord:
                    DestReg.H0 = (DestReg.H0 & SrcReg.H0);
                    break;

                case OpSize_Word:
                    DestReg.Value = (DestReg.Value & SrcReg.Value);
                    break;

                default:
                    goto case OpSize_Word;
                }

                break;

            case OpCode_OR:
                switch (OpCode & OpCodeFlag_Size) {
                case OpSize_Byte:
                    DestReg.B0 = (byte)(DestReg.B0 | SrcReg.B0);
                    break;

                case OpSize_QuarterWord:
                    DestReg.Q0 = (ushort)(DestReg.Q0 | SrcReg.Q0);
                    break;

                case OpSize_HalfWord:
                    DestReg.H0 = (DestReg.H0 | SrcReg.H0);
                    break;

                case OpSize_Word:
                    DestReg.Value = (DestReg.Value | SrcReg.Value);
                    break;

                default:
                    goto case OpSize_Word;
                }

                break;

            case OpCode_NOR:
                switch (OpCode & OpCodeFlag_Size) {
                case OpSize_Byte:
                    DestReg.B0 = (byte)~(DestReg.B0 | SrcReg.B0);
                    break;

                case OpSize_QuarterWord:
                    DestReg.Q0 = (ushort)~(DestReg.Q0 | SrcReg.Q0);
                    break;

                case OpSize_HalfWord:
                    DestReg.H0 = ~(DestReg.H0 | SrcReg.H0);
                    break;

                case OpSize_Word:
                    DestReg.Value = ~(DestReg.Value | SrcReg.Value);
                    break;

                default:
                    goto case OpSize_Word;
                }

                break;

            case OpCode_NOT:
                switch (OpCode & OpCodeFlag_Size) {
                case OpSize_Byte:
                    DestReg.B0 = (byte)~(DestReg.B0);
                    break;

                case OpSize_QuarterWord:
                    DestReg.Q0 = (ushort)~(DestReg.Q0);
                    break;

                case OpSize_HalfWord:
                    DestReg.H0 = ~(DestReg.H0);
                    break;

                case OpSize_Word:
                    DestReg.Value = ~(DestReg.Value);
                    break;

                default:
                    goto case OpSize_Word;
                }

                break;

            case OpCode_NAND:
                switch (OpCode & OpCodeFlag_Size) {
                case OpSize_Byte:
                    DestReg.B0 = (byte)~(DestReg.B0 & SrcReg.B0);
                    break;

                case OpSize_QuarterWord:
                    DestReg.Q0 = (ushort)~(DestReg.Q0 & SrcReg.Q0);
                    break;

                case OpSize_HalfWord:
                    DestReg.H0 = ~(DestReg.H0 & SrcReg.H0);
                    break;

                case OpSize_Word:
                    DestReg.Value = ~(DestReg.Value & SrcReg.Value);
                    break;

                default:
                    goto case OpSize_Word;
                }

                break;

            case OpCode_XOR:
                switch (OpCode & OpCodeFlag_Size) {
                case OpSize_Byte:
                    DestReg.B0 = (byte)(DestReg.B0 ^ SrcReg.B0);
                    break;

                case OpSize_QuarterWord:
                    DestReg.Q0 = (ushort)(DestReg.Q0 ^ SrcReg.Q0);
                    break;

                case OpSize_HalfWord:
                    DestReg.H0 = (DestReg.H0 ^ SrcReg.H0);
                    break;

                case OpSize_Word:
                    DestReg.Value = (DestReg.Value ^ SrcReg.Value);
                    break;

                default:
                    goto case OpSize_Word;
                }

                break;

            case OpCode_SHL:
                switch (OpCode & OpCodeFlag_Size) {
                case OpSize_Byte:
                    DestReg.B0 = (byte)(DestReg.B0 << SrcReg.B0);
                    break;

                case OpSize_QuarterWord:
                    DestReg.Q0 = (ushort)(DestReg.Q0 << SrcReg.B0);
                    break;

                case OpSize_HalfWord:
                    DestReg.H0 = (DestReg.H0 << SrcReg.B0);
                    break;

                case OpSize_Word:
                    DestReg.Value = (DestReg.Value << SrcReg.B0);
                    break;

                default:
                    goto case OpSize_Word;
                }

                break;

            case OpCode_SHR:
                switch (OpCode & OpCodeFlag_Size) {
                case OpSize_Byte:
                    DestReg.B0 = (byte)(DestReg.B0 >> SrcReg.B0);
                    break;

                case OpSize_QuarterWord:
                    DestReg.Q0 = (ushort)(DestReg.Q0 >> SrcReg.B0);
                    break;

                case OpSize_HalfWord:
                    DestReg.H0 = (DestReg.H0 >> SrcReg.B0);
                    break;

                case OpSize_Word:
                    DestReg.Value = (DestReg.Value >> SrcReg.B0);
                    break;

                default:
                    goto case OpSize_Word;
                }

                break;

            case OpCode_CMP:
                switch (OpCode & OpCodeFlag_Size) {
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
            }

            UpdateFlags();
        }

        void UpdateFlags() {

        }

        private UInt64 DEC_Word() {
            UInt64 result;

            try {
                result = checked(DestReg.Value - 1);
            }
            catch (OverflowException) {
                Overflow = 1;
                result = unchecked(DestReg.Value - 1);
            }

            return result;
        }

        private UInt32 DEC_HalfWord() {
            UInt32 result;

            try {
                result = checked(DestReg.H0 - 1);
            }
            catch (OverflowException) {
                Overflow = 1;
                result = unchecked(DestReg.H0 - 1);
            }

            return result;
        }

        private UInt16 DEC_QuarterWord() {
            UInt16 result;

            try {
                result = checked((ushort)(DestReg.Q0 - 1));
            }
            catch (OverflowException) {
                Overflow = 1;
                result = unchecked((ushort)(DestReg.Q0 - 1));
            }

            return result;
        }

        private byte DEC_Byte() {
            byte result;

            try {
                result = checked((byte)(DestReg.B0 - 1));
            }
            catch (OverflowException) {
                Overflow = 1;
                result = unchecked((byte)(DestReg.B0 - 1));
            }

            return result;
        }


        private UInt64 INC_Word() {
            UInt64 result;

            try {
                result = checked(DestReg.Value + 1);
            }
            catch (OverflowException) {
                Overflow = 1;
                result = unchecked(DestReg.Value + 1);
            }

            return result;
        }

        private UInt32 INC_HalfWord() {
            UInt32 result;

            try {
                result = checked(DestReg.H0 + 1);
            }
            catch (OverflowException) {
                Overflow = 1;
                result = unchecked(DestReg.H0 + 1);
            }

            return result;
        }

        private UInt16 INC_QuarterWord() {
            UInt16 result;

            try {
                result = checked((ushort)(DestReg.Q0 + 1));
            }
            catch (OverflowException) {
                Overflow = 1;
                result = unchecked((ushort)(DestReg.Q0 + 1));
            }

            return result;
        }

        private byte INC_Byte() {
            byte result;

            try {
                result = checked((byte)(DestReg.B0 + 1));
            }
            catch (OverflowException) {
                Overflow = 1;
                result = unchecked((byte)(DestReg.B0 + 1));
            }

            return result;
        }

        private UInt64 MOD_Word() {
            UInt64 result;

            try {
                result = checked(DestReg.Value % SrcReg.Value);
            }
            catch (OverflowException) {
                Overflow = 1;
                result = unchecked(DestReg.Value % SrcReg.Value);
            }

            return result;
        }

        private UInt32 MOD_HalfWord() {
            UInt32 result;

            try {
                result = checked(DestReg.H0 % SrcReg.H0);
            }
            catch (OverflowException) {
                Overflow = 1;
                result = unchecked(DestReg.H0 % SrcReg.H0);
            }

            return result;
        }

        private UInt16 MOD_QuarterWord() {
            UInt16 result;

            try {
                result = checked((ushort)(DestReg.Q0 % SrcReg.Q0));
            }
            catch (OverflowException) {
                Overflow = 1;
                result = unchecked((ushort)(DestReg.Q0 % SrcReg.Q0));
            }

            return result;
        }

        private byte MOD_Byte() {
            byte result;

            try {
                result = checked((byte)(DestReg.B0 % SrcReg.B0));
            }
            catch (OverflowException) {
                Overflow = 1;
                result = unchecked((byte)(DestReg.B0 % SrcReg.B0));
            }

            return result;
        }




        private UInt64 DIV_Word() {
            UInt64 result;

            try {
                result = checked(DestReg.Value / SrcReg.Value);
            }
            catch (OverflowException) {
                Overflow = 1;
                result = unchecked(DestReg.Value / SrcReg.Value);
            }

            return result;
        }

        private UInt32 DIV_HalfWord() {
            UInt32 result;

            try {
                result = checked(DestReg.H0 / SrcReg.H0);
            }
            catch (OverflowException) {
                Overflow = 1;
                result = unchecked(DestReg.H0 / SrcReg.H0);
            }

            return result;
        }

        private UInt16 DIV_QuarterWord() {
            UInt16 result;

            try {
                result = checked((ushort)(DestReg.Q0 / SrcReg.Q0));
            }
            catch (OverflowException) {
                Overflow = 1;
                result = unchecked((ushort)(DestReg.Q0 / SrcReg.Q0));
            }

            return result;
        }

        private byte DIV_Byte() {
            byte result;

            try {
                result = checked((byte)(DestReg.B0 / SrcReg.B0));
            }
            catch (OverflowException) {
                Overflow = 1;
                result = unchecked((byte)(DestReg.B0 / SrcReg.B0));
            }

            return result;
        }


        private UInt64 MUL_Word() {
            UInt64 result;

            try {
                result = checked(DestReg.Value * SrcReg.Value);
            }
            catch (OverflowException) {
                Overflow = 1;
                result = unchecked(DestReg.Value * SrcReg.Value);
            }

            return result;
        }

        private UInt32 MUL_HalfWord() {
            UInt32 result;

            try {
                result = checked(DestReg.H0 * SrcReg.H0);
            }
            catch (OverflowException) {
                Overflow = 1;
                result = unchecked(DestReg.H0 * SrcReg.H0);
            }

            return result;
        }

        private UInt16 MUL_QuarterWord() {
            UInt16 result;

            try {
                result = checked((ushort)(DestReg.Q0 * SrcReg.Q0));
            }
            catch (OverflowException) {
                Overflow = 1;
                result = unchecked((ushort)(DestReg.Q0 * SrcReg.Q0));
            }

            return result;
        }

        private byte MUL_Byte() {
            byte result;

            try {
                result = checked((byte)(DestReg.B0 * SrcReg.B0));
            }
            catch (OverflowException) {
                Overflow = 1;
                result = unchecked((byte)(DestReg.B0 * SrcReg.B0));
            }

            return result;
        }

        private UInt64 SUB_Word() {
            UInt64 result;

            try {
                result = checked(DestReg.Value - SrcReg.Value);
            }
            catch (OverflowException) {
                Overflow = 1;
                result = unchecked(DestReg.Value - SrcReg.Value);
            }

            return result;
        }

        private UInt32 SUB_HalfWord() {
            UInt32 result;

            try {
                result = checked(DestReg.H0 - SrcReg.H0);
            }
            catch (OverflowException) {
                Overflow = 1;
                result = unchecked(DestReg.H0 - SrcReg.H0);
            }

            return result;
        }

        private UInt16 SUB_QuarterWord() {
            UInt16 result;

            try {
                result = checked((ushort)(DestReg.Q0 - SrcReg.Q0));
            }
            catch (OverflowException) {
                Overflow = 1;
                result = unchecked((ushort)(DestReg.Q0 - SrcReg.Q0));
            }

            return result;
        }

        private byte SUB_Byte() {
            byte result;

            try {
                result = checked((byte)(DestReg.B0 - SrcReg.B0));
            }
            catch (OverflowException) {
                Overflow = 1;
                result = unchecked((byte)(DestReg.B0 - SrcReg.B0));
            }

            return result;
        }

        private UInt64 ADD_Word() {
            UInt64 result;

            try {
                result = checked(DestReg.Value + SrcReg.Value);
            }
            catch (OverflowException) {
                Overflow = 1;
                result = unchecked(DestReg.Value + SrcReg.Value);
            }

            return result;
        }

        private UInt32 ADD_HalfWord() {
            UInt32 result;

            try {
                result = checked(DestReg.H0 + SrcReg.H0);
            }
            catch (OverflowException) {
                Overflow = 1;
                result = unchecked(DestReg.H0 + SrcReg.H0);
            }

            return result;
        }

        private UInt16 ADD_QuarterWord() {
            UInt16 result;

            try {
                result = checked((ushort)(DestReg.Q0 + SrcReg.Q0));
            }
            catch (OverflowException) {
                Overflow = 1;
                result = unchecked((ushort)(DestReg.Q0 + SrcReg.Q0));
            }

            return result;
        }

        private byte ADD_Byte() {
            byte result;

            try {
                result = checked((byte)(DestReg.B0 + SrcReg.B0));
            }
            catch (OverflowException) {
                Overflow = 1;
                result = unchecked((byte)(DestReg.B0 + SrcReg.B0));
            }

            return result;
        }

    }

}
