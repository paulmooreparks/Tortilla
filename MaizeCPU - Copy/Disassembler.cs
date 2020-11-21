using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maize {
    public class Disassembler : Tortilla.IDissasember<UInt64, UInt64> {
        public Disassembler() {
            ConnectOpCodesToMethods();
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
                // flags = (byte)(opcode & 0b_1110_0000);
                handler = OpCodeMap[opcode];

                if (handler != null) {
                    LastResult = handler(out retval);
                }

                text = retval;
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
            { MaizeInstruction.OpFlag_RegF, "F" },
            { MaizeInstruction.OpFlag_RegI, "I" },
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

        protected Dictionary<int, string> OpCodes = new Dictionary<int, string> {
            { 0x00, "HALT" },
            { 0x01, "LD" },     
            { 0x41, "LD" },
            { 0x81, "LD" },
            { 0xC1, "LD" },
            { 0x02, "ST" },
            { 0x42, "ST" },
            { 0x03, "ADD" },
            { 0x43, "ADD" },
            { 0x83, "ADD" },
            { 0xC3, "ADD" },
            { 0x04, "SUB" },
            { 0x44, "SUB" },
            { 0x84, "SUB" },
            { 0xC4, "SUB" },
            { 0x05, "MUL" },
            { 0x45, "MUL" },
            { 0x85, "MUL" },
            { 0xC5, "MUL" },
            { 0x06, "DIV" },
            { 0x46, "DIV" },
            { 0x86, "DIV" },
            { 0xC6, "DIV" },
            { 0x07, "MOD" },
            { 0x47, "MOD" },
            { 0x87, "MOD" },
            { 0xC7, "MOD" },
            { 0x08, "AND" },
            { 0x48, "AND" },
            { 0x88, "AND" },
            { 0xC8, "AND" },
            { 0x09, "OR" },
            { 0x49, "OR" },
            { 0x89, "OR" },
            { 0xC9, "OR" },
            { 0x0A, "NOR" },
            { 0x4A, "NOR" },
            { 0x8A, "NOR" },
            { 0xCA, "NOR" },
            { 0x0B, "NAND" },
            { 0x4B, "NAND" },
            { 0x8B, "NAND" },
            { 0xCB, "NAND" },
            { 0x0C, "XOR" },
            { 0x4C, "XOR" },
            { 0x8C, "XOR" },
            { 0xCC, "XOR" },
            { 0x0D, "SHL" },
            { 0x4D, "SHL" },
            { 0x8D, "SHL" },
            { 0xCD, "SHL" },
            { 0x0E, "SHR" },
            { 0x4E, "SHR" },
            { 0x8E, "SHR" },
            { 0xCE, "SHR" },
            { 0x0F, "CMP" },
            { 0x4F, "CMP" },
            { 0x8F, "CMP" },
            { 0xCF, "CMP" },
            { 0x10, "TEST" },
            { 0x50, "TEST" },
            { 0x90, "TEST" },
            { 0xD0, "TEST" },
            { 0x11, "PUSH" },
            { 0x51, "PUSH" },
            { 0x12, "INT" },
            { 0x52, "INT" },
            { 0x13, "STI" },
            { 0x14, "res" },
            { 0x15, "res" },
            { 0x16, "JMP" },
            { 0x56, "JMP" },
            { 0x96, "JMP" },
            { 0xD6, "JMP" },
            { 0x17, "JZ" },
            { 0x57, "JZ" },
            { 0x97, "JZ" },
            { 0xD7, "JZ" },
            { 0x18, "JNZ" },
            { 0x58, "JNZ" },
            { 0x98, "JNZ" },
            { 0xD8, "JNZ" },
            { 0x19, "JLT" },
            { 0x59, "JLT" },
            { 0x99, "JLT" },
            { 0xD9, "JLT" },
            { 0x1A, "JLE" },
            { 0x5A, "JLE" },
            { 0x9A, "JLE" },
            { 0xDA, "JLE" },
            { 0x1B, "JGT" },
            { 0x5B, "JGT" },
            { 0x9B, "JGT" },
            { 0xDB, "JGT" },
            { 0x1C, "JGE" },
            { 0x5C, "JGE" },
            { 0x9C, "JGE" },
            { 0xDC, "JGE" },
            { 0x1D, "CALL" },
            { 0x5D, "CALL" },
            { 0x9D, "CALL" },
            { 0xDD, "CALL" },
            { 0x1E, "OUT" },
            { 0x5E, "OUT" },
            { 0x9E, "OUT" },
            { 0xDE, "OUT" },
            { 0x1F, "IN" },
            { 0x5F, "IN" },
            { 0x9F, "IN" },
            { 0xDF, "IN" },
            { 0x20, "PUSH" },
            { 0x21, "PUSH" },
            { 0x22, "CLR" },
            { 0x23, "INC" },
            { 0x24, "DEC" },
            { 0x25, "NOT" },
            { 0x26, "POP" },
            { 0x27, "RET" },
            { 0xAA, "NOP" },
            { 0xFF, "BRK" }
        };

        [OpCode(0x00, 0x27, 0xAA, 0xFF)]
        DecodeResult NoParams(out string text) {
            text = FormatOutput(address, opcode, $"", "");
            return DecodeResult.Complete;
        }

        [OpCode(0x01, 0x41, 0x81, 0xC1, 0x03, 0x43, 0x83, 0xC3, 0x13, 0x1E, 0x1F, 0x5F)]
        DecodeResult SrcImmReg_DestReg(out string text) {
            text = null;

            if (operand1 == null) {
                return DecodeResult.Partial;
            }

            if (operand2 == null) {
                return DecodeResult.Partial;
            }

            bool srcIsAddress = ((opcode & MaizeInstruction.OpcodeFlag_SrcAddr) == MaizeInstruction.OpcodeFlag_SrcAddr);
            bool destIsAddress = ((operand2?.B0 & MaizeInstruction.OpFlag_Addr) == MaizeInstruction.OpFlag_Addr);
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
            destReg = $"{(destIsAddress ? "@" : "")}{RegName[destRegNameFlag]}{SubRegName[destSubregNameFlag]}";

            text = FormatOutput(address, opcode, 
                $"{srcImm}{srcReg} {destImm}{destReg}", 
                $"{operand1?.B0:X2} {operand2?.B0:X2} {srcImmBytes}{destImmBytes}");

            return DecodeResult.Complete;
        }


        [OpCode(0x02, 0x42, 0x82, 0xC2)]
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


        [OpCode(0x16, 0x1D, 0x20, 0x22, 0x23, 0x24, 0x26)]
        DecodeResult UnaryReg(out string text) {
            text = null;

            if (operand1 == null) {
                return DecodeResult.ReadBytes1;
            }

            int destRegNameFlag = (int)(operand1?.B0 & MaizeInstruction.OpFlag_Reg);
            int destSubregNameFlag = (int)(operand1?.B0 & MaizeInstruction.OpFlag_SubReg);
            string destReg = $"{RegName[destRegNameFlag]}{SubRegName[destSubregNameFlag]}";

            text = FormatOutput(address, opcode,
                destReg,
                $"{operand1?.B0:X2}");
            return DecodeResult.Complete;
        }


        [OpCode(0x21, 0x56, 0x5D)]
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

            // text = $"{address.H0,-10:X8}{$"{OpCodes[opcode]} A.Q0, [${operand1?.B1:X2}{operand1?.B0:X2}]",-42} ; {opcode:X2} {operand1?.B0:X2} {operand1?.B1:X2}";
            text = FormatOutput(address, opcode,
                $"A.Q0 @${operand1?.B1:X2}{operand1?.B0:X2}",
                $"{operand1?.B0:X2} {operand1?.B1:X2}");
            return DecodeResult.Complete;
        }

        string FormatOutput(RegValue address, byte opcode, string instruction, string paramBytes) {
            StringBuilder text = new StringBuilder ($"${address.H0:X8}: {$"{OpCodes[opcode]} {instruction}",-52} ; {opcode:X2} {paramBytes}");
            return text.ToString();
        }
    }
}

