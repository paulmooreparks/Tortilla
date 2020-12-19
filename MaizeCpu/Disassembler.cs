using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maize {
    public class Disassembler : Tortilla.IDissasember<UInt64, UInt64> {
        public Disassembler() {
            OpCodes = new Dictionary<int, (string, OpCodeDelegate)> {
                { 0x00, ("HALT", NoParams) },
                { 0x01, ("LD"  , SrcImmReg_DestReg) },
                { 0x41, ("LD"  , SrcImmReg_DestReg) },
                { 0x81, ("LD"  , SrcImmReg_DestReg) },
                { 0xC1, ("LD"  , SrcImmReg_DestReg) },
                { 0x02, ("STIM"  , SrcImmReg_DestImmAddr) },
                { 0x42, ("STIM"  , SrcImmReg_DestImmAddr) },
                { 0x03, ("ADD" , SrcImmReg_DestReg) },
                { 0x43, ("ADD" , SrcImmReg_DestReg) },
                { 0x83, ("ADD" , SrcImmReg_DestReg) },
                { 0xC3, ("ADD" , SrcImmReg_DestReg) },
                { 0x04, ("SUB" , SrcImmReg_DestReg) },
                { 0x44, ("SUB" , SrcImmReg_DestReg) },
                { 0x84, ("SUB" , SrcImmReg_DestReg) },
                { 0xC4, ("SUB" , SrcImmReg_DestReg) },
                { 0x05, ("MUL" , SrcImmReg_DestReg) },
                { 0x45, ("MUL" , SrcImmReg_DestReg) },
                { 0x85, ("MUL" , SrcImmReg_DestReg) },
                { 0xC5, ("MUL" , SrcImmReg_DestReg) },
                { 0x06, ("DIV" , SrcImmReg_DestReg) },
                { 0x46, ("DIV" , SrcImmReg_DestReg) },
                { 0x86, ("DIV" , SrcImmReg_DestReg) },
                { 0xC6, ("DIV" , SrcImmReg_DestReg) },
                { 0x07, ("MOD" , SrcImmReg_DestReg) },
                { 0x47, ("MOD" , SrcImmReg_DestReg) },
                { 0x87, ("MOD" , SrcImmReg_DestReg) },
                { 0xC7, ("MOD" , SrcImmReg_DestReg) },
                { 0x08, ("AND" , SrcImmReg_DestReg) },
                { 0x48, ("AND" , SrcImmReg_DestReg) },
                { 0x88, ("AND" , SrcImmReg_DestReg) },
                { 0xC8, ("AND" , SrcImmReg_DestReg) },
                { 0x09, ("OR"  , SrcImmReg_DestReg) },
                { 0x49, ("OR"  , SrcImmReg_DestReg) },
                { 0x89, ("OR"  , SrcImmReg_DestReg) },
                { 0xC9, ("OR"  , SrcImmReg_DestReg) },
                { 0x0A, ("NOR" , SrcImmReg_DestReg) },
                { 0x4A, ("NOR" , SrcImmReg_DestReg) },
                { 0x8A, ("NOR" , SrcImmReg_DestReg) },
                { 0xCA, ("NOR" , SrcImmReg_DestReg) },
                { 0x0B, ("NAND", SrcImmReg_DestReg) },
                { 0x4B, ("NAND", SrcImmReg_DestReg) },
                { 0x8B, ("NAND", SrcImmReg_DestReg) },
                { 0xCB, ("NAND", SrcImmReg_DestReg) },
                { 0x0C, ("XOR" , SrcImmReg_DestReg) },
                { 0x4C, ("XOR" , SrcImmReg_DestReg) },
                { 0x8C, ("XOR" , SrcImmReg_DestReg) },
                { 0xCC, ("XOR" , SrcImmReg_DestReg) },
                { 0x0D, ("SHL" , SrcImmReg_DestReg) },
                { 0x4D, ("SHL" , SrcImmReg_DestReg) },
                { 0x8D, ("SHL" , SrcImmReg_DestReg) },
                { 0xCD, ("SHL" , SrcImmReg_DestReg) },
                { 0x0E, ("SHR" , SrcImmReg_DestReg) },
                { 0x4E, ("SHR" , SrcImmReg_DestReg) },
                { 0x8E, ("SHR" , SrcImmReg_DestReg) },
                { 0xCE, ("SHR" , SrcImmReg_DestReg) },
                { 0x0F, ("CMP" , SrcImmReg_DestReg) },
                { 0x4F, ("CMP" , SrcImmReg_DestReg) },
                { 0x8F, ("CMP" , SrcImmReg_DestReg) },
                { 0xCF, ("CMP" , SrcImmReg_DestReg) },
                { 0x10, ("TEST", SrcImmReg_DestReg) },
                { 0x50, ("TEST", SrcImmReg_DestReg) },
                { 0x90, ("TEST", SrcImmReg_DestReg) },
                { 0xD0, ("TEST", SrcImmReg_DestReg) },
                { 0x11, ("PUSH", SrcImmReg_DestReg) },
                { 0x51, ("PUSH", SrcImmReg_DestReg) },
                { 0x12, ("INT" , UnaryReg) },
                { 0x52, ("INT" , UnaryImm) },
                { 0x13, ("ST" , SrcImmReg_DestReg) },
                { 0x53, ("ST" , SrcImmReg_DestReg) },
                { 0x14, ("OUTR" , SrcImmReg_DestReg) },
                { 0x54, ("OUTR" , SrcImmReg_DestReg) },
                { 0x16, ("JMP" , UnaryReg) },
                { 0x56, ("JMP" , UnaryImm) },
                { 0x96, ("JMP" , UnaryReg) },
                { 0xD6, ("JMP" , UnaryImm) },
                { 0x17, ("JZ"  , UnaryReg) },
                { 0x57, ("JZ"  , UnaryImm) },
                { 0x97, ("JZ"  , UnaryReg) },
                { 0xD7, ("JZ"  , UnaryImm) },
                { 0x18, ("JNZ" , UnaryReg) },
                { 0x58, ("JNZ" , UnaryImm) },
                { 0x98, ("JNZ" , UnaryReg) },
                { 0xD8, ("JNZ" , UnaryImm) },
                { 0x19, ("JLT" , UnaryReg) },
                { 0x59, ("JLT" , UnaryImm) },
                { 0x99, ("JLT" , UnaryReg) },
                { 0xD9, ("JLT" , UnaryImm) },
                { 0x1A, ("JLE" , UnaryReg) },
                { 0x5A, ("JLE" , UnaryImm) },
                { 0x9A, ("JLE" , UnaryReg) },
                { 0xDA, ("JLE" , UnaryImm) },
                { 0x1B, ("JGT" , UnaryReg) },
                { 0x5B, ("JGT" , UnaryImm) },
                { 0x9B, ("JGT" , UnaryReg) },
                { 0xDB, ("JGT" , UnaryImm) },
                { 0x1C, ("JGE" , UnaryReg) },
                { 0x5C, ("JGE" , UnaryImm) },
                { 0x9C, ("JGE" , UnaryReg) },
                { 0xDC, ("JGE" , UnaryImm) },
                { 0x1D, ("CALL", UnaryReg) },
                { 0x5D, ("CALL", UnaryImm) },
                { 0x9D, ("CALL", UnaryReg) },
                { 0xDD, ("CALL", UnaryImm) },
                { 0x1E, ("OUT" , SrcImmReg_DestImm) },
                { 0x5E, ("OUT" , SrcImmReg_DestImm) },
                { 0x9E, ("OUT" , SrcImmReg_DestImm) },
                { 0xDE, ("OUT" , SrcImmReg_DestImm) },
                { 0x1F, ("IN"  , SrcImmReg_DestReg) },
                { 0x5F, ("IN"  , SrcImmReg_DestReg) },
                { 0x9F, ("IN"  , SrcImmReg_DestReg) },
                { 0xDF, ("IN"  , SrcImmReg_DestReg) },
                { 0x20, ("PUSH", UnaryReg) },
                { 0x21, ("PUSH", UnaryImm) },
                { 0x22, ("CLR" , UnaryReg) },
                { 0x23, ("INC" , UnaryReg) },
                { 0x24, ("DEC" , UnaryReg) },
                { 0x25, ("NOT" , UnaryReg) },
                { 0x26, ("POP" , UnaryReg) },
                { 0x27, ("RET" , NoParams) },
                { 0x28, ("IRET" , NoParams) },
                { 0x29, ("STI" , NoParams) },
                { 0x30, ("CLI" , NoParams) },
                { 0x31, ("STC" , NoParams) },
                { 0x32, ("CLC" , NoParams) },
                { 0xAA, ("NOP" , NoParams) },
                { 0xFF, ("BRK" , NoParams) }
            };
        }

        protected OpCodeDelegate handler = null;
        RegValue address;
        RegValue input;
        byte opcode;
        // byte flags;
        RegValue? operand1;
        RegValue? operand2;
        RegValue? operand3;
        RegValue? operand4;

        protected delegate DecodeResult OpCodeDelegate(out string text);

        public enum DecodeResult {
            Invalid = 0xFF,
            Partial = 1,
            ReadBytes1 = 1,
            ReadBytes2 = 2,
            ReadBytes3 = 3,
            ReadBytes4 = 4,
            ReadBytes5 = 5,
            ReadBytes6 = 6,
            ReadBytes7 = 7,
            ReadBytes8 = 8,
            Complete = 0
        }

        protected DecodeResult LastResult { get; set; } = DecodeResult.Complete;

        public int Decode(ulong value, ulong addressInit, out string text) {
            text = null;

            if (addressInit == 0xFFFFFFFF_FFFFFFF8) {
                return 0;
            }

            string retval = null;
            input = value;

            switch (LastResult) {
            case DecodeResult.Complete:
            case DecodeResult.Invalid:
                operand3 = null;
                operand4 = null;
                address = addressInit;
                opcode = input.B0;
                operand1 = input.B1;
                operand2 = input.B2;

                if (OpCodes.ContainsKey(opcode)) {
                    handler = OpCodes[opcode].Item2;

                    if (handler != null) {
                        LastResult = handler(out retval);
                        text = retval;
                    }
                    else {
                        text = $"${address.H0:X8}: {"unknown",-42} ; {opcode:X2}";
                        return (int)DecodeResult.Complete;
                    }
                }
                else {
                    text = $"${address.H0:X8}: {"unknown",-42} ; {opcode:X2}";
                    return (int)DecodeResult.Complete;
                }

                return (int)LastResult;

            case DecodeResult.ReadBytes1:
            case DecodeResult.ReadBytes2:
            case DecodeResult.ReadBytes3:
            case DecodeResult.ReadBytes4:
            case DecodeResult.ReadBytes5:
            case DecodeResult.ReadBytes6:
            case DecodeResult.ReadBytes7:
            case DecodeResult.ReadBytes8:
                if (operand1 == null) {
                    operand1 = value;
                }
                else if (operand2 == null) {
                    operand2 = value;
                }
                else if (operand3 == null) {
                    operand3 = value;
                }
                else if (operand4 == null) {
                    operand4 = value;
                }

                if (handler != null) {
                    LastResult = handler(out retval);
                }

                text = retval;
                return (int)LastResult;
            }

            return 0;
        }

        protected Dictionary<int, string> RegName = new Dictionary<int, string> {
            { MaizeInstruction.OpFlag_RegA, "A" },
            { MaizeInstruction.OpFlag_RegB, "B" },
            { MaizeInstruction.OpFlag_RegC, "C" },
            { MaizeInstruction.OpFlag_RegD, "D" },
            { MaizeInstruction.OpFlag_RegE, "E" },
            { MaizeInstruction.OpFlag_RegG, "G" },
            { MaizeInstruction.OpFlag_RegH, "H" },
            { MaizeInstruction.OpFlag_RegJ, "J" },
            { MaizeInstruction.OpFlag_RegK, "K" },
            { MaizeInstruction.OpFlag_RegL, "L" },
            { MaizeInstruction.OpFlag_RegM, "M" },
            { MaizeInstruction.OpFlag_RegZ, "Z" },
            { MaizeInstruction.OpFlag_RegF, "F" },
            { MaizeInstruction.OpFlag_RegI, "I" },
            { MaizeInstruction.OpFlag_RegP, "P" },
            { MaizeInstruction.OpFlag_RegS, "S" }
        };

        protected Dictionary<int, string> SubRegName = new Dictionary<int, string> {
            { MaizeInstruction.OpFlag_RegB0, ".B0" },
            { MaizeInstruction.OpFlag_RegB1, ".B1" },
            { MaizeInstruction.OpFlag_RegB2, ".B2" },
            { MaizeInstruction.OpFlag_RegB3, ".B3" },
            { MaizeInstruction.OpFlag_RegB4, ".B4" },
            { MaizeInstruction.OpFlag_RegB5, ".B5" },
            { MaizeInstruction.OpFlag_RegB6, ".B6" },
            { MaizeInstruction.OpFlag_RegB7, ".B7" },
            { MaizeInstruction.OpFlag_RegQ0, ".Q0" },
            { MaizeInstruction.OpFlag_RegQ1, ".Q1" },
            { MaizeInstruction.OpFlag_RegQ2, ".Q2" },
            { MaizeInstruction.OpFlag_RegQ3, ".Q3" },
            { MaizeInstruction.OpFlag_RegH0, ".H0" },
            { MaizeInstruction.OpFlag_RegH1, ".H1" },
            { MaizeInstruction.OpFlag_RegW0, "" }
        };

        protected Dictionary<int, (string, OpCodeDelegate)> OpCodes;

        DecodeResult NoParams(out string text) {
            text = FormatOutput(address, opcode, $"", "");
            return DecodeResult.Complete;
        }

        [OpCode(
            0x01, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0x10, 0x1E, 0x1F, 
            0x41, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4A, 0x4B, 0x4C, 0x4D, 0x4E, 0x4F, 0x50, 0x5E, 0x5F, 
            0x81, 0x83, 0x84, 0x85, 0x86, 0x87, 0x88, 0x89, 0x8A, 0x8B, 0x8C, 0x8D, 0x8E, 0x8F, 0x90, 0x9E, 0x9F,
            0xC1, 0xC3, 0xC4, 0xC5, 0xC6, 0xC7, 0xC8, 0xC9, 0xCA, 0xCB, 0xCC, 0xCD, 0xCE, 0xCF, 0xD0, 0xDE, 0xDF,
            0x13)]
        DecodeResult SrcImmReg_DestReg(out string text) {
            text = null;

            if (operand1 == null) {
                return DecodeResult.Partial;
            }

            if (operand2 == null) {
                return DecodeResult.Partial;
            }

            bool srcIsAddress = ((opcode & MaizeInstruction.OpcodeFlag_SrcAddr) == MaizeInstruction.OpcodeFlag_SrcAddr);
            // bool destIsAddress = ((operand2?.B0 & MaizeInstruction.OpFlag_Addr) == MaizeInstruction.OpFlag_Addr);
            bool srcIsImmediate = ((opcode & MaizeInstruction.OpcodeFlag_SrcImm) == MaizeInstruction.OpcodeFlag_SrcImm);

            if (srcIsImmediate && operand3 == null) {
                return DecodeResult.Partial;
            }
            
            string srcImmBytes = "";
            string destImmBytes = "";

            string srcReg = "";
            string destReg = "";

            string srcImm = "";
            string destImm = "";

            if (srcIsImmediate) {
                switch (operand1?.B0 & MaizeInstruction.OpFlag_ImmSize) {
                case MaizeInstruction.OpFlag_Imm08b:
                    srcImmBytes = $"{operand3?.B0:X2} ";
                    srcImm = $"{(srcIsAddress ? "@" : "")}${operand3?.B0:X2}";
                    break;

                case MaizeInstruction.OpFlag_Imm16b:
                    srcImmBytes = $"{operand3?.B0:X2} {operand3?.B1:X2} ";
                    srcImm = $"{(srcIsAddress ? "@" : "")}${operand3?.B1:X2}{operand3?.B0:X2}";
                    break;

                case MaizeInstruction.OpFlag_Imm32b:
                    srcImmBytes = $"{operand3?.B0:X2} {operand3?.B1:X2} {operand3?.B2:X2} {operand3?.B3:X2} ";
                    srcImm = $"{(srcIsAddress ? "@" : "")}${operand3?.B3:X2}{operand3?.B2:X2}{operand3?.B1:X2}{operand3?.B0:X2}";
                    break;

                case MaizeInstruction.OpFlag_Imm64b:
                    srcImmBytes = $"{operand3?.B0:X2} {operand3?.B1:X2} {operand3?.B2:X2} {operand3?.B3:X2} {operand3?.B4:X2} {operand3?.B5:X2} {operand3?.B6:X2} {operand3?.B7:X2} ";
                    srcImm = $"{(srcIsAddress ? "@" : "")}${operand3?.B7:X2}{operand3?.B6:X2}{operand3?.B5:X2}{operand3?.B4:X2}{operand3?.B3:X2}{operand3?.B2:X2}{operand3?.B1:X2}{operand3?.B0:X2}";
                    break;
                }

            }
            else {
                // Source operand is register
                int srcRegNameFlag = (int)(operand1?.B0 & MaizeInstruction.OpFlag_Reg);
                int srcSubregNameFlag = (int)(operand1?.B0 & MaizeInstruction.OpFlag_SubReg);

                srcReg = $"{(srcIsAddress ? "@" : "")}{RegName[srcRegNameFlag]}{SubRegName[srcSubregNameFlag]}";
            }

            int destRegNameFlag = (int)(operand2?.B0 & MaizeInstruction.OpFlag_Reg);
            int destSubregNameFlag = (int)(operand2?.B0 & MaizeInstruction.OpFlag_SubReg);
            destReg = $"{RegName[destRegNameFlag]}{SubRegName[destSubregNameFlag]}";

            text = FormatOutput(address, opcode, 
                $"{srcImm}{srcReg} {destImm}{destReg}", 
                $"{operand1?.B0:X2} {operand2?.B0:X2} {srcImmBytes}{destImmBytes}");

            return DecodeResult.Complete;
        }


        DecodeResult SrcImmReg_DestImm(out string text) {
            text = null;

            if (operand1 == null) {
                return DecodeResult.Partial;
            }

            if (operand2 == null) {
                return DecodeResult.Partial;
            }

            if (operand3 == null) {
                return DecodeResult.Partial;
            }

            bool srcIsAddress = ((opcode & MaizeInstruction.OpcodeFlag_SrcImm) == MaizeInstruction.OpcodeFlag_SrcImm);
            bool srcIsImmediate = ((opcode & MaizeInstruction.OpcodeFlag_SrcImm) == MaizeInstruction.OpcodeFlag_SrcImm);

            string srcImmBytes = "";
            string destImmBytes = "";

            string srcReg = "";
            string destReg = "";

            string srcImm = "";
            string destImm = "";

            if (srcIsImmediate) {
                switch (operand1?.B0 & MaizeInstruction.OpFlag_ImmSize) {
                case MaizeInstruction.OpFlag_Imm08b:
                    srcImmBytes = $"{operand3?.B0:X2} ";
                    srcImm = $"{(srcIsAddress ? "@" : "")}${operand3?.B0:X2}";
                    break;

                case MaizeInstruction.OpFlag_Imm16b:
                    srcImmBytes = $"{operand3?.B0:X2} {operand3?.B1:X2} ";
                    srcImm = $"{(srcIsAddress ? "@" : "")}${operand3?.B1:X2}{operand3?.B0:X2}";
                    break;

                case MaizeInstruction.OpFlag_Imm32b:
                    srcImmBytes = $"{operand3?.B0:X2} {operand3?.B1:X2} {operand3?.B2:X2} {operand3?.B3:X2} ";
                    srcImm = $"{(srcIsAddress ? "@" : "")}${operand3?.B3:X2}{operand3?.B2:X2}{operand3?.B1:X2}{operand3?.B0:X2}";
                    break;

                case MaizeInstruction.OpFlag_Imm64b:
                    srcImmBytes = $"{operand3?.B0:X2} {operand3?.B1:X2} {operand3?.B2:X2} {operand3?.B3:X2} {operand3?.B4:X2} {operand3?.B5:X2} {operand3?.B6:X2} {operand3?.B7:X2} ";
                    srcImm = $"{(srcIsAddress ? "@" : "")}${operand3?.B7:X2}{operand3?.B6:X2}{operand3?.B5:X2}{operand3?.B4:X2}{operand3?.B3:X2}{operand3?.B2:X2}{operand3?.B1:X2}{operand3?.B0:X2}";
                    break;
                }

            }
            else {
                // Source operand is register
                int srcRegNameFlag = (int)(operand1?.B0 & MaizeInstruction.OpFlag_Reg);
                int srcSubregNameFlag = (int)(operand1?.B0 & MaizeInstruction.OpFlag_SubReg);

                srcReg = $"{(srcIsAddress ? "@" : "")}{RegName[srcRegNameFlag]}{SubRegName[srcSubregNameFlag]}";
            }

            switch (operand2?.B0 & MaizeInstruction.OpFlag_ImmSize) {
            case MaizeInstruction.OpFlag_Imm08b:
                destImmBytes = $"{operand3?.B0:X2} ";
                destImm = $"${operand3?.B0:X2}";
                break;

            case MaizeInstruction.OpFlag_Imm16b:
                destImmBytes = $"{operand3?.B0:X2} {operand3?.B1:X2} ";
                destImm = $"${operand3?.B1:X2}{operand3?.B0:X2}";
                break;

            case MaizeInstruction.OpFlag_Imm32b:
                destImmBytes = $"{operand3?.B0:X2} {operand3?.B1:X2} {operand3?.B2:X2} {operand3?.B3:X2} ";
                destImm = $"${operand3?.B3:X2}{operand3?.B2:X2}{operand3?.B1:X2}{operand3?.B0:X2}";
                break;

            case MaizeInstruction.OpFlag_Imm64b:
                destImmBytes = $"{operand3?.B0:X2} {operand3?.B1:X2} {operand3?.B2:X2} {operand3?.B3:X2} {operand3?.B4:X2} {operand3?.B5:X2} {operand3?.B6:X2} {operand3?.B7:X2} ";
                destImm = $"${operand3?.B7:X2}{operand3?.B6:X2}{operand3?.B5:X2}{operand3?.B4:X2}{operand3?.B3:X2}{operand3?.B2:X2}{operand3?.B1:X2}{operand3?.B0:X2}";
                break;
            }

            text = FormatOutput(address, opcode,
                $"{srcImm}{srcReg} {destImm}{destReg}",
                $"{operand1?.B0:X2} {operand2?.B0:X2} {srcImmBytes}{destImmBytes}");

            return DecodeResult.Complete;
        }


        DecodeResult SrcImmReg_DestImmAddr(out string text) {
            text = null;

            if (operand1 == null) {
                return DecodeResult.Partial;
            }

            if (operand2 == null) {
                return DecodeResult.Partial;
            }

            if (operand3 == null) {
                return DecodeResult.Partial;
            }

            bool srcIsAddress = ((opcode & MaizeInstruction.OpcodeFlag_SrcImm) == MaizeInstruction.OpcodeFlag_SrcImm);
            bool srcIsImmediate = ((opcode & MaizeInstruction.OpcodeFlag_SrcImm) == MaizeInstruction.OpcodeFlag_SrcImm);

            string srcImmBytes = "";
            string destImmBytes = "";

            string srcReg = "";
            string destReg = "";

            string srcImm = "";
            string destImm = "";

            if (srcIsImmediate) {
                switch (operand1?.B0 & MaizeInstruction.OpFlag_ImmSize) {
                case MaizeInstruction.OpFlag_Imm08b:
                    srcImmBytes = $"{operand3?.B0:X2} ";
                    srcImm = $"{(srcIsAddress ? "@" : "")}${operand3?.B0:X2}";
                    break;

                case MaizeInstruction.OpFlag_Imm16b:
                    srcImmBytes = $"{operand3?.B0:X2} {operand3?.B1:X2} ";
                    srcImm = $"{(srcIsAddress ? "@" : "")}${operand3?.B1:X2}{operand3?.B0:X2}";
                    break;

                case MaizeInstruction.OpFlag_Imm32b:
                    srcImmBytes = $"{operand3?.B0:X2} {operand3?.B1:X2} {operand3?.B2:X2} {operand3?.B3:X2} ";
                    srcImm = $"{(srcIsAddress ? "@" : "")}${operand3?.B3:X2}{operand3?.B2:X2}{operand3?.B1:X2}{operand3?.B0:X2}";
                    break;

                case MaizeInstruction.OpFlag_Imm64b:
                    srcImmBytes = $"{operand3?.B0:X2} {operand3?.B1:X2} {operand3?.B2:X2} {operand3?.B3:X2} {operand3?.B4:X2} {operand3?.B5:X2} {operand3?.B6:X2} {operand3?.B7:X2} ";
                    srcImm = $"{(srcIsAddress ? "@" : "")}${operand3?.B7:X2}{operand3?.B6:X2}{operand3?.B5:X2}{operand3?.B4:X2}{operand3?.B3:X2}{operand3?.B2:X2}{operand3?.B1:X2}{operand3?.B0:X2}";
                    break;
                }

            }
            else {
                // Source operand is register
                int srcRegNameFlag = (int)(operand1?.B0 & MaizeInstruction.OpFlag_Reg);
                int srcSubregNameFlag = (int)(operand1?.B0 & MaizeInstruction.OpFlag_SubReg);

                srcReg = $"{(srcIsAddress ? "@" : "")}{RegName[srcRegNameFlag]}{SubRegName[srcSubregNameFlag]}";
            }

            switch (operand2?.B0 & MaizeInstruction.OpFlag_ImmSize) {
            case MaizeInstruction.OpFlag_Imm08b:
                destImmBytes = $"{operand3?.B0:X2} ";
                destImm = $"@${operand3?.B0:X2}";
                break;

            case MaizeInstruction.OpFlag_Imm16b:
                destImmBytes = $"{operand3?.B0:X2} {operand3?.B1:X2} ";
                destImm = $"@${operand3?.B1:X2}{operand3?.B0:X2}";
                break;

            case MaizeInstruction.OpFlag_Imm32b:
                destImmBytes = $"{operand3?.B0:X2} {operand3?.B1:X2} {operand3?.B2:X2} {operand3?.B3:X2} ";
                destImm = $"@${operand3?.B3:X2}{operand3?.B2:X2}{operand3?.B1:X2}{operand3?.B0:X2}";
                break;

            case MaizeInstruction.OpFlag_Imm64b:
                destImmBytes = $"{operand3?.B0:X2} {operand3?.B1:X2} {operand3?.B2:X2} {operand3?.B3:X2} {operand3?.B4:X2} {operand3?.B5:X2} {operand3?.B6:X2} {operand3?.B7:X2} ";
                destImm = $"@${operand3?.B7:X2}{operand3?.B6:X2}{operand3?.B5:X2}{operand3?.B4:X2}{operand3?.B3:X2}{operand3?.B2:X2}{operand3?.B1:X2}{operand3?.B0:X2}";
                break;
            }

            text = FormatOutput(address, opcode,
                $"{srcImm}{srcReg} {destImm}{destReg}",
                $"{operand1?.B0:X2} {operand2?.B0:X2} {srcImmBytes}{destImmBytes}");

            return DecodeResult.Complete;
        }


        DecodeResult UnaryReg(out string text) {
            text = null;

            if (operand1 == null) {
                return DecodeResult.ReadBytes1;
            }

            bool isAddress = ((opcode & MaizeInstruction.OpcodeFlag_SrcAddr) == MaizeInstruction.OpcodeFlag_SrcAddr);
            int destRegNameFlag = (int)(operand1?.B0 & MaizeInstruction.OpFlag_Reg);
            int destSubregNameFlag = (int)(operand1?.B0 & MaizeInstruction.OpFlag_SubReg);
            string destReg = $"{(isAddress ? "@" : "")}{RegName[destRegNameFlag]}{SubRegName[destSubregNameFlag]}";

            text = FormatOutput(address, opcode,
                destReg,
                $"{operand1?.B0:X2}");
            return DecodeResult.Complete;
        }


        DecodeResult UnaryImm(out string text) {
            text = null;

            if (operand1 == null) {
                return DecodeResult.ReadBytes1;
            }

            if (operand3 == null) {
                switch (operand1?.B0 & MaizeInstruction.OpFlag_ImmSize) {
                case MaizeInstruction.OpFlag_Imm08b:
                    return DecodeResult.ReadBytes1;

                case MaizeInstruction.OpFlag_Imm16b:
                    return DecodeResult.ReadBytes2;

                case MaizeInstruction.OpFlag_Imm32b:
                    return DecodeResult.ReadBytes4;

                case MaizeInstruction.OpFlag_Imm64b:
                    return DecodeResult.ReadBytes8;
                }
            }

            bool srcIsAddress = ((opcode & MaizeInstruction.OpcodeFlag_SrcAddr) == MaizeInstruction.OpcodeFlag_SrcAddr);
            string srcImm = "";
            string srcImmBytes = "";

            switch (operand1?.B0 & MaizeInstruction.OpFlag_ImmSize) {
            case MaizeInstruction.OpFlag_Imm08b:
                srcImmBytes = $"{operand3?.B0:X2} ";
                srcImm = $"{(srcIsAddress ? "@" : "")}${operand3?.B0:X2}";
                break;

            case MaizeInstruction.OpFlag_Imm16b:
                srcImmBytes = $"{operand3?.B0:X2} {operand3?.B1:X2} ";
                srcImm = $"{(srcIsAddress ? "@" : "")}${operand3?.B1:X2}{operand3?.B0:X2}";
                break;

            case MaizeInstruction.OpFlag_Imm32b:
                srcImmBytes = $"{operand3?.B0:X2} {operand3?.B1:X2} {operand3?.B2:X2} {operand3?.B3:X2} ";
                srcImm = $"{(srcIsAddress ? "@" : "")}${operand3?.B3:X2}{operand3?.B2:X2}{operand3?.B1:X2}{operand3?.B0:X2}";
                break;

            case MaizeInstruction.OpFlag_Imm64b:
                srcImmBytes = $"{operand3?.B0:X2} {operand3?.B1:X2} {operand3?.B2:X2} {operand3?.B3:X2} {operand3?.B4:X2} {operand3?.B5:X2} {operand3?.B6:X2} {operand3?.B7:X2} ";
                srcImm = $"{(srcIsAddress ? "@" : "")}${operand3?.B7:X2}{operand3?.B6:X2}{operand3?.B5:X2}{operand3?.B4:X2}{operand3?.B3:X2}{operand3?.B2:X2}{operand3?.B1:X2}{operand3?.B0:X2}";
                break;
            }


            text = FormatOutput(address, opcode,
                $"{srcImm}",
                $"{operand1?.B0:X2} {srcImmBytes}");
            return DecodeResult.Complete;
        }


        DecodeResult BinaryImm_AW0_64b(out string text) {
            text = null;

            if (operand1 == null) {
                return DecodeResult.Partial;
            }

            text = FormatOutput(address, opcode,
                $"${operand1?.B7:X2}{operand1?.B6:X2}{operand1?.B5:X2}{operand1?.B4:X2}{operand1?.B3:X2}{operand1?.B2:X2}{operand1?.B1:X2}{operand1?.B0:X2} A",
                $"{operand1?.B0:X2} {operand1?.B1:X2} {operand1?.B2:X2} {operand1?.B3:X2} {operand1?.B4:X2} {operand1?.B5:X2} {operand1?.B6:X2} {operand1?.B7:X2}");
            return DecodeResult.Complete;
        }

        DecodeResult BinaryReg_AQ0_16b(out string text) {
            text = null;

            if (operand1 == null) {
                return DecodeResult.Partial;
            }

            text = FormatOutput(address, opcode,
                $"A.Q0 @${operand1?.B1:X2}{operand1?.B0:X2}",
                $"{operand1?.B0:X2} {operand1?.B1:X2}");
            return DecodeResult.Complete;
        }

        string FormatOutput(RegValue address, byte opcode, string instruction, string paramBytes) {
            StringBuilder text = new StringBuilder ($"${address.H0:X8}: {$"{OpCodes[opcode].Item1} {instruction}",-42} ; {opcode:X2} {paramBytes}");
            return text.ToString();
        }
    }
}

