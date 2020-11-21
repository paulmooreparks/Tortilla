﻿using System;
using System.Numerics;
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
            get { return (byte)(this.B0 & OpCodeCtrl_CarryIn); }
            set { this.B0 = (byte)((this.B0 & ~OpCodeCtrl_CarryIn) | ((value & 0b_0000_0001) << (OpCodeCtrl_CarryIn / 4))); }
        }

        public override void OnTick(ClockState state, IBusComponent cpuFlags) {
            base.OnTick(state, cpuFlags);

            switch (state) {
            case ClockState.TickSet:
                ExecuteOperation();
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
                    DestReg.B0 = AND_Byte();
                    break;

                case OpSize_QuarterWord:
                    DestReg.Q0 = AND_QuarterWord();
                    break;

                case OpSize_HalfWord:
                    DestReg.H0 = AND_HalfWord();
                    break;

                case OpSize_Word:
                    DestReg.W0 = AND_Word();
                    break;

                default:
                    goto case OpSize_Word;
                }

                break;

            case OpCode_OR:
                switch (OpCode & OpCodeFlag_Size) {
                case OpSize_Byte:
                    DestReg.B0 = OR_Byte();
                    break;

                case OpSize_QuarterWord:
                    DestReg.Q0 = OR_QuarterWord();
                    break;

                case OpSize_HalfWord:
                    DestReg.H0 = OR_HalfWord();
                    break;

                case OpSize_Word:
                    DestReg.Value = OR_Word();
                    break;

                default:
                    goto case OpSize_Word;
                }

                break;

            case OpCode_NOR:
                switch (OpCode & OpCodeFlag_Size) {
                case OpSize_Byte:
                    DestReg.B0 = NOR_Byte();
                    break;

                case OpSize_QuarterWord:
                    DestReg.Q0 = NOR_QuarterWord();
                    break;

                case OpSize_HalfWord:
                    DestReg.H0 = NOR_HalfWord();
                    break;

                case OpSize_Word:
                    DestReg.Value = NOR_Word();
                    break;

                default:
                    goto case OpSize_Word;
                }

                break;

            case OpCode_NOT:
                switch (OpCode & OpCodeFlag_Size) {
                case OpSize_Byte:
                    DestReg.B0 = NOT_Byte();
                    break;

                case OpSize_QuarterWord:
                    DestReg.Q0 = NOT_QuarterWord();
                    break;

                case OpSize_HalfWord:
                    DestReg.H0 = NOT_HalfWord();
                    break;

                case OpSize_Word:
                    DestReg.Value = NOT_Word();
                    break;

                default:
                    goto case OpSize_Word;
                }

                break;

            case OpCode_NAND:
                switch (OpCode & OpCodeFlag_Size) {
                case OpSize_Byte:
                    DestReg.B0 = NAND_Byte();
                    break;

                case OpSize_QuarterWord:
                    DestReg.Q0 = NAND_QuarterWord();
                    break;

                case OpSize_HalfWord:
                    DestReg.H0 = NAND_HalfWord();
                    break;

                case OpSize_Word:
                    DestReg.Value = NAND_Word();
                    break;

                default:
                    goto case OpSize_Word;
                }

                break;

            case OpCode_XOR:
                switch (OpCode & OpCodeFlag_Size) {
                case OpSize_Byte:
                    DestReg.B0 = XOR_Byte();
                    break;

                case OpSize_QuarterWord:
                    DestReg.Q0 = XOR_QuarterWord();
                    break;

                case OpSize_HalfWord:
                    DestReg.H0 = XOR_HalfWord();
                    break;

                case OpSize_Word:
                    DestReg.Value = XOR_Word();
                    break;

                default:
                    goto case OpSize_Word;
                }

                break;

            case OpCode_SHL:
                switch (OpCode & OpCodeFlag_Size) {
                case OpSize_Byte:
                    DestReg.B0 = SHL_Byte();
                    break;

                case OpSize_QuarterWord:
                    DestReg.Q0 = SHL_QuarterWord();
                    break;

                case OpSize_HalfWord:
                    DestReg.H0 = SHL_HalfWord();
                    break;

                case OpSize_Word:
                    DestReg.Value = SHL_Word();
                    break;

                default:
                    goto case OpSize_Word;
                }

                break;

            case OpCode_SHR:
                switch (OpCode & OpCodeFlag_Size) {
                case OpSize_Byte:
                    DestReg.B0 = SHR_Byte();
                    break;

                case OpSize_QuarterWord:
                    DestReg.Q0 = SHR_QuarterWord();
                    break;

                case OpSize_HalfWord:
                    DestReg.H0 = SHR_HalfWord();
                    break;

                case OpSize_Word:
                    DestReg.Value = SHR_Word();
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

            case OpCode_TEST:
                switch (OpCode & OpCodeFlag_Size) {
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
            Cpu.Zero = (result == 0);
            Cpu.Negative = ((result & 0b_1000_0000) != 0);
            return result;
        }

        UInt16 UpdateFlags(UInt16 result) {
            Cpu.Zero = (result == 0);
            Cpu.Negative = ((Int16)result) < 0;
            return result;
        }

        UInt32 UpdateFlags(UInt32 result) {
            Cpu.Zero = (result == 0);
            Cpu.Negative = ((Int32)result) < 0;
            return result;
        }

        UInt64 UpdateFlags(UInt64 result) {
            Cpu.Zero = (result == 0);
            Cpu.Negative = ((Int64)result) < 0;
            return result;
        }


        private byte ADD_Byte() {
            byte result;

            try {
                result = checked((byte)(DestReg.B0 + SrcReg.B0));
            }
            catch (OverflowException) {
                Cpu.Overflow = true;
                result = unchecked((byte)(DestReg.B0 + SrcReg.B0));
            }

            Cpu.Carry = (result < DestReg.Value);
            return UpdateFlags(result);
        }

        private UInt16 ADD_QuarterWord() {
            UInt16 result;

            try {
                result = checked((ushort)(DestReg.Q0 + SrcReg.Q0));
            }
            catch (OverflowException) {
                Cpu.Overflow = true;
                result = unchecked((ushort)(DestReg.Q0 + SrcReg.Q0));
            }

            Cpu.Carry = (result < DestReg.Value);
            return UpdateFlags(result);
        }

        private UInt32 ADD_HalfWord() {
            UInt32 result;

            try {
                result = checked(DestReg.H0 + SrcReg.H0);
            }
            catch (OverflowException) {
                Cpu.Overflow = true;
                result = unchecked(DestReg.H0 + SrcReg.H0);
            }

            Cpu.Carry = (result < DestReg.Value);
            return UpdateFlags(result);
        }

        private UInt64 ADD_Word() {
            UInt64 result;

            try {
                result = checked(DestReg.Value + SrcReg.Value);
            }
            catch (OverflowException) {
                Cpu.Overflow = true;
                result = unchecked(DestReg.Value + SrcReg.Value);
            }

            Cpu.Carry = (result < DestReg.Value);
            return UpdateFlags(result);
        }


        private byte SUB_Byte() {
            byte result;

            try {
                result = checked((byte)(DestReg.B0 - SrcReg.B0));
            }
            catch (OverflowException) {
                Cpu.Overflow = true;
                result = unchecked((byte)(DestReg.B0 - SrcReg.B0));
            }

            Cpu.Carry = (SrcReg.B0 > DestReg.B0);
            return UpdateFlags(result);
        }

        private UInt16 SUB_QuarterWord() {
            UInt16 result;

            try {
                result = checked((ushort)(DestReg.Q0 - SrcReg.Q0));
            }
            catch (OverflowException) {
                Cpu.Overflow = true;
                result = unchecked((ushort)(DestReg.Q0 - SrcReg.Q0));
            }

            Cpu.Carry = (SrcReg.Q0 > DestReg.Q0);
            return UpdateFlags(result);
        }

        private UInt32 SUB_HalfWord() {
            UInt32 result;

            try {
                result = checked(DestReg.H0 - SrcReg.H0);
            }
            catch (OverflowException) {
                Cpu.Overflow = true;
                result = unchecked(DestReg.H0 - SrcReg.H0);
            }

            Cpu.Carry = (SrcReg.H0 > DestReg.H0);
            return UpdateFlags(result);
        }

        private UInt64 SUB_Word() {
            UInt64 result;

            try {
                result = checked(DestReg.Value - SrcReg.Value);
            }
            catch (OverflowException) {
                Cpu.Overflow = true;
                result = unchecked(DestReg.Value - SrcReg.Value);
            }

            Cpu.Carry = (SrcReg.W0 > DestReg.W0);
            return UpdateFlags(result);
        }


        private byte INC_Byte() {
            byte result;

            try {
                result = checked((byte)(DestReg.B0 + 1));
            }
            catch (OverflowException) {
                Cpu.Overflow = true;
                result = unchecked((byte)(DestReg.B0 + 1));
            }

            Cpu.Carry = (result < DestReg.Value);
            return UpdateFlags(result);
        }

        private UInt16 INC_QuarterWord() {
            UInt16 result;

            try {
                result = checked((ushort)(DestReg.Q0 + 1));
            }
            catch (OverflowException) {
                Cpu.Overflow = true;
                result = unchecked((ushort)(DestReg.Q0 + 1));
            }

            Cpu.Carry = (result < DestReg.Value);
            return UpdateFlags(result);
        }

        private UInt32 INC_HalfWord() {
            UInt32 result;

            try {
                result = checked(DestReg.H0 + 1);
            }
            catch (OverflowException) {
                Cpu.Overflow = true;
                result = unchecked(DestReg.H0 + 1);
            }

            Cpu.Carry = (result < DestReg.Value);
            return UpdateFlags(result);
        }

        private UInt64 INC_Word() {
            UInt64 result;

            try {
                result = checked(DestReg.Value + 1);
            }
            catch (OverflowException) {
                Cpu.Overflow = true;
                result = unchecked(DestReg.Value + 1);
            }

            Cpu.Carry = (result < DestReg.Value);
            return UpdateFlags(result);
        }


        private byte DEC_Byte() {
            byte result;

            try {
                result = checked((byte)(DestReg.B0 - 1));
            }
            catch (OverflowException) {
                Cpu.Overflow = true;
                result = unchecked((byte)(DestReg.B0 - 1));
            }

            Cpu.Carry = (result < DestReg.Value);
            return UpdateFlags(result);
        }

        private UInt16 DEC_QuarterWord() {
            UInt16 result;

            try {
                result = checked((ushort)(DestReg.Q0 - 1));
            }
            catch (OverflowException) {
                Cpu.Overflow = true;
                result = unchecked((ushort)(DestReg.Q0 - 1));
            }

            Cpu.Carry = (result < DestReg.Value);
            return UpdateFlags(result);
        }

        private UInt32 DEC_HalfWord() {
            UInt32 result;

            try {
                result = checked(DestReg.H0 - 1);
            }
            catch (OverflowException) {
                Cpu.Overflow = true;
                result = unchecked(DestReg.H0 - 1);
            }

            Cpu.Carry = (result < DestReg.Value);
            return UpdateFlags(result);
        }

        private UInt64 DEC_Word() {
            UInt64 result;

            try {
                result = checked(DestReg.Value - 1);
            }
            catch (OverflowException) {
                Cpu.Overflow = true;
                result = unchecked(DestReg.Value - 1);
            }

            Cpu.Carry = (result < DestReg.Value);
            return UpdateFlags(result);
        }


        private byte MUL_Byte() {
            UInt16 result = (UInt16)(DestReg.B0 * SrcReg.B0);
            Cpu.Overflow = result > (int)byte.MaxValue;
            Cpu.Carry = result > byte.MaxValue;
            return UpdateFlags((byte)result);
        }

        private UInt16 MUL_QuarterWord() {
            UInt32 result = (UInt32)(DestReg.Q0 * SrcReg.Q0);
            Cpu.Overflow = result > Int16.MaxValue;
            Cpu.Carry = result > UInt16.MaxValue;
            return UpdateFlags((UInt16)result);
        }

        private UInt32 MUL_HalfWord() {
            UInt64 result = (UInt64)(DestReg.Q0 * SrcReg.Q0);
            Cpu.Overflow = result > Int32.MaxValue;
            Cpu.Carry = result > UInt32.MaxValue;
            return UpdateFlags((UInt32)result);
        }

        private UInt64 MUL_Word() {
            BigInteger result = (BigInteger)(DestReg.Q0 * SrcReg.Q0);
            Cpu.Overflow = result > Int64.MaxValue;
            Cpu.Carry = result > UInt64.MaxValue;
            return UpdateFlags((UInt64)result);
        }


        private byte DIV_Byte() {
            byte result;

            try {
                result = checked((byte)(DestReg.B0 / SrcReg.B0));
            }
            catch (OverflowException) {
                Cpu.Overflow = true;
                result = unchecked((byte)(DestReg.B0 / SrcReg.B0));
            }

            return UpdateFlags(result);
        }

        private UInt16 DIV_QuarterWord() {
            UInt16 result;

            try {
                result = checked((ushort)(DestReg.Q0 / SrcReg.Q0));
            }
            catch (OverflowException) {
                Cpu.Overflow = true;
                result = unchecked((ushort)(DestReg.Q0 / SrcReg.Q0));
            }

            return UpdateFlags(result);
        }

        private UInt32 DIV_HalfWord() {
            UInt32 result;

            try {
                result = checked(DestReg.H0 / SrcReg.H0);
            }
            catch (OverflowException) {
                Cpu.Overflow = true;
                result = unchecked(DestReg.H0 / SrcReg.H0);
            }

            return UpdateFlags(result);
        }

        private UInt64 DIV_Word() {
            UInt64 result;

            try {
                result = checked(DestReg.Value / SrcReg.Value);
            }
            catch (OverflowException) {
                Cpu.Overflow = true;
                result = unchecked(DestReg.Value / SrcReg.Value);
            }

            return UpdateFlags(result);
        }


        private byte MOD_Byte() {
            byte result;

            try {
                result = checked((byte)(DestReg.B0 % SrcReg.B0));
            }
            catch (OverflowException) {
                Cpu.Overflow = true;
                result = unchecked((byte)(DestReg.B0 % SrcReg.B0));
            }

            return UpdateFlags(result);
        }

        private UInt16 MOD_QuarterWord() {
            UInt16 result;

            try {
                result = checked((ushort)(DestReg.Q0 % SrcReg.Q0));
            }
            catch (OverflowException) {
                Cpu.Overflow = true;
                result = unchecked((ushort)(DestReg.Q0 % SrcReg.Q0));
            }

            return UpdateFlags(result);
        }

        private UInt32 MOD_HalfWord() {
            UInt32 result;

            try {
                result = checked(DestReg.H0 % SrcReg.H0);
            }
            catch (OverflowException) {
                Cpu.Overflow = true;
                result = unchecked(DestReg.H0 % SrcReg.H0);
            }

            return UpdateFlags(result);
        }

        private UInt64 MOD_Word() {
            UInt64 result;

            try {
                result = checked(DestReg.Value % SrcReg.Value);
            }
            catch (OverflowException) {
                Cpu.Overflow = true;
                result = unchecked(DestReg.Value % SrcReg.Value);
            }

            Cpu.Carry = (result < SrcReg.Value);
            return UpdateFlags(result);
        }


        private byte AND_Byte() {
            byte result = (byte)(DestReg.B0 & SrcReg.B0);
            return UpdateFlags(result);
        }

        private UInt16 AND_QuarterWord() {
            UInt16 result = (ushort)(DestReg.Q0 & SrcReg.Q0);
            return UpdateFlags(result);
        }

        private UInt32 AND_HalfWord() {
            UInt32 result = DestReg.H0 & SrcReg.H0;
            return UpdateFlags(result);
        }

        private UInt64 AND_Word() {
            UInt64 result = DestReg.Value & SrcReg.Value;
            return UpdateFlags(result);
        }


        private byte OR_Byte() {
            byte result = (byte)(DestReg.B0 | SrcReg.B0);
            return UpdateFlags(result);
        }

        private UInt16 OR_QuarterWord() {
            UInt16 result = (ushort)(DestReg.Q0 | SrcReg.Q0);
            return UpdateFlags(result);
        }

        private UInt32 OR_HalfWord() {
            UInt32 result = DestReg.H0 | SrcReg.H0;
            return UpdateFlags(result);
        }

        private UInt64 OR_Word() {
            UInt64 result = DestReg.Value | SrcReg.Value;
            return UpdateFlags(result);
        }


        private byte NOR_Byte() {
            byte result = (byte)~(DestReg.B0 | SrcReg.B0);
            return UpdateFlags(result);
        }

        private UInt16 NOR_QuarterWord() {
            UInt16 result = (ushort)~(DestReg.Q0 | SrcReg.Q0);
            return UpdateFlags(result);
        }

        private UInt32 NOR_HalfWord() {
            UInt32 result = ~(DestReg.H0 | SrcReg.H0);
            return UpdateFlags(result);
        }

        private UInt64 NOR_Word() {
            UInt64 result = ~(DestReg.Value | SrcReg.Value);
            return UpdateFlags(result);
        }


        private byte NOT_Byte() {
            byte result = (byte)~(DestReg.B0); 
            return UpdateFlags(result);
        }

        private UInt16 NOT_QuarterWord() {
            UInt16 result = (ushort)~(DestReg.Q0);
            return UpdateFlags(result);
        }

        private UInt32 NOT_HalfWord() {
            UInt32 result = ~(DestReg.H0);
            return UpdateFlags(result);
        }

        private UInt64 NOT_Word() {
            UInt64 result = ~(DestReg.Value);
            return UpdateFlags(result);
        }


        private byte NAND_Byte() {
            byte result = (byte)~(DestReg.B0 & SrcReg.B0);
            return UpdateFlags(result);
        }

        private UInt16 NAND_QuarterWord() {
            UInt16 result = (ushort)~(DestReg.Q0 & SrcReg.Q0);
            return UpdateFlags(result);
        }

        private UInt32 NAND_HalfWord() {
            UInt32 result = ~(DestReg.H0 & SrcReg.H0);
            return UpdateFlags(result);
        }

        private UInt64 NAND_Word() {
            UInt64 result = ~(DestReg.Value & SrcReg.Value);
            return UpdateFlags(result);
        }


        private byte XOR_Byte() {
            byte result = (byte)(DestReg.B0 ^ SrcReg.B0);
            return UpdateFlags(result);
        }

        private UInt16 XOR_QuarterWord() {
            UInt16 result = (ushort)(DestReg.Q0 ^ SrcReg.Q0);
            return UpdateFlags(result);
        }

        private UInt32 XOR_HalfWord() {
            UInt32 result = (DestReg.H0 ^ SrcReg.H0);
            return UpdateFlags(result);
        }

        private UInt64 XOR_Word() {
            UInt64 result = (DestReg.Value ^ SrcReg.Value);
            return UpdateFlags(result);
        }


        private byte SHL_Byte() {
            byte result = (byte)(DestReg.B0 << SrcReg.B0);
            return UpdateFlags(result);
        }

        private UInt16 SHL_QuarterWord() {
            UInt16 result = (ushort)(DestReg.Q0 << SrcReg.B0);
            return UpdateFlags(result);
        }

        private UInt32 SHL_HalfWord() {
            UInt32 result = (DestReg.H0 << SrcReg.B0);
            return UpdateFlags(result);
        }

        private UInt64 SHL_Word() {
            UInt64 result = (DestReg.Value << SrcReg.B0);
            return UpdateFlags(result);
        }


        private byte SHR_Byte() {
            byte result = (byte)(DestReg.B0 >> SrcReg.B0);
            return UpdateFlags(result);
        }

        private UInt16 SHR_QuarterWord() {
            UInt16 result = (ushort)(DestReg.Q0 >> SrcReg.B0);
            return UpdateFlags(result);
        }

        private UInt32 SHR_HalfWord() {
            UInt32 result = (DestReg.H0 >> SrcReg.B0);
            return UpdateFlags(result);
        }

        private UInt64 SHR_Word() {
            UInt64 result = (DestReg.Value >> SrcReg.B0);
            return UpdateFlags(result);
        }

    }

}