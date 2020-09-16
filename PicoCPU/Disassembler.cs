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
        byte flags;
        RegValue? operand1;
        RegValue? operand2;
        RegValue? operand3;
        RegValue? operand4;

        protected const byte InsFlag_Imm08b = 0b_10000000;
        protected const byte InsFlag_Imm16b = 0b_10100000;
        protected const byte InsFlag_Imm32b = 0b_11000000;
        protected const byte InsFlag_Imm64b = 0b_11100000;

        protected const byte OpFlag_RegA = 0b_00000000;
        protected const byte OpFlag_RegB = 0b_00000010;
        protected const byte OpFlag_RegC = 0b_00000100;
        protected const byte OpFlag_RegD = 0b_00000110;
        protected const byte OpFlag_RegE = 0b_00001000;
        protected const byte OpFlag_RegI = 0b_00001010;
        protected const byte OpFlag_RegS = 0b_00001100;
        protected const byte OpFlag_RegF = 0b_00001110;

        protected const byte OpFlag_RegW = 0b_0001_0000;
        protected const byte OpFlag_RegH0 = 0b_0010_0000;
        protected const byte OpFlag_RegH1 = 0b_0011_0000;
        protected const byte OpFlag_RegQ0 = 0b_0100_0000;
        protected const byte OpFlag_RegQ1 = 0b_0101_0000;
        protected const byte OpFlag_RegQ2 = 0b_0110_0000;
        protected const byte OpFlag_RegQ3 = 0b_0111_0000;
        protected const byte OpFlag_RegB0 = 0b_1000_0000;
        protected const byte OpFlag_RegB1 = 0b_1001_0000;
        protected const byte OpFlag_RegB2 = 0b_1010_0000;
        protected const byte OpFlag_RegB3 = 0b_1011_0000;
        protected const byte OpFlag_RegB4 = 0b_1100_0000;
        protected const byte OpFlag_RegB5 = 0b_1101_0000;
        protected const byte OpFlag_RegB6 = 0b_1110_0000;
        protected const byte OpFlag_RegB7 = 0b_1111_0000;

        protected const byte OpFlag_Imm08b = 0b_0000_0000;
        protected const byte OpFlag_Imm16b = 0b_0000_0010;
        protected const byte OpFlag_Imm32b = 0b_0000_0100;
        protected const byte OpFlag_Imm64b = 0b_0000_0110;

        protected const byte OpFlag_Reserved01 = 0b_0000_1000;
        protected const byte OpFlag_Reserved02 = 0b_0000_1010;
        protected const byte OpFlag_Reserved03 = 0b_0000_1100;
        protected const byte OpFlag_Reserved04 = 0b_0000_1110;


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

        protected byte InsFlagImmediate08b = 0b_0000_0100;
        protected byte InsFlagImmediate16b = 0b_0000_0101;
        protected byte InsFlagImmediate32b = 0b_0000_0110;
        protected byte InsFlagImmediate64b = 0b_0000_0111;


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
                operand1 = null;
                operand2 = null;
                operand3 = null;
                operand4 = null;
                address = addressInit;
                opcode = input.B0;
                flags = (byte)(opcode & 0b_1110_0000);
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
            { OpFlag_RegA, "A" },
            { OpFlag_RegB, "B" },
            { OpFlag_RegC, "C" },
            { OpFlag_RegD, "D" },
            { OpFlag_RegE, "E" },
            { OpFlag_RegF, "F" },
            { OpFlag_RegI, "I" },
            { OpFlag_RegS, "S" }
        };

        protected Dictionary<int, string> SubRegName = new Dictionary<int, string> {
            { OpFlag_RegB0, ".B0" },
            { OpFlag_RegB1, ".B1" },
            { OpFlag_RegB2, ".B2" },
            { OpFlag_RegB3, ".B3" },
            { OpFlag_RegB4, ".B4" },
            { OpFlag_RegB5, ".B5" },
            { OpFlag_RegB6, ".B6" },
            { OpFlag_RegB7, ".B7" },
            { OpFlag_RegQ0, ".Q0" },
            { OpFlag_RegQ1, ".Q1" },
            { OpFlag_RegQ2, ".Q2" },
            { OpFlag_RegQ3, ".Q3" },
            { OpFlag_RegH0, ".H0" },
            { OpFlag_RegH1, ".H1" },
            { OpFlag_RegW, "" }
        };

        protected Dictionary<int, string> OpCodes = new Dictionary<int, string> {
            { 0x00, "HALT" },  { 0x80, "INT 0" }, { 0xA0, "INT 1" }, { 0xC0, "INT 2" }, { 0xE0, "INT 3" },
            { 0x01, "LD" },    { 0x81, "LD" },    { 0xA1, "LD" },    { 0xC1, "LD" },    { 0xE1, "LD" }, 
            { 0x02, "ST" },    { 0x82, "ST" },    { 0xA2, "ST" },    { 0xC2, "ST" },    { 0xE2, "ST" },
            { 0x03, "ADD" },   { 0x83, "ADD" },   { 0xA3, "ADD" },   { 0xC3, "ADD" },   { 0xE3, "ADD" },
            { 0x04, "SUB" },   { 0x84, "SUB" },   { 0xA4, "SUB" },   { 0xC4, "SUB" },   { 0xE4, "SUB" },
            { 0x05, "MUL" },   { 0x85, "MUL" },   { 0xA5, "MUL" },   { 0xC5, "MUL" },   { 0xE5, "MUL" },
            { 0x06, "DIV" },   { 0x86, "DIV" },   { 0xA6, "DIV" },   { 0xC6, "DIV" },   { 0xE6, "DIV" },
            { 0x07, "MOD" },   { 0x87, "MOD" },   { 0xA7, "MOD" },   { 0xC7, "MOD" },   { 0xE7, "MOD" },
            { 0x08, "INC" },   { 0x88, "INT" },   { 0xA8, "res" },   { 0xC8, "res" },   { 0xE8, "res" },
            { 0x09, "DEC" },   { 0x89, "res" },   { 0xA9, "res" },   { 0xC9, "res" },   { 0xE9, "res" },
            { 0x0A, "AND" },   { 0x8A, "AND" },   { 0xAA, "AND" },   { 0xCA, "AND" },   { 0xEA, "AND" },
            { 0x0B, "OR" },    { 0x8B, "OR" },    { 0xAB, "OR" },    { 0xCB, "OR" },    { 0xEB, "OR" },
            { 0x0C, "NOR" },   { 0x8C, "NOR" },   { 0xAC, "NOR" },   { 0xCC, "NOR" },   { 0xEC, "NOR" },
            { 0x0D, "NOT" },   { 0x8D, "NOT" },   { 0xAD, "NOT" },   { 0xCD, "NOT" },   { 0xED, "NOT" },
            { 0x0E, "NAND" },  { 0x8E, "NAND" },  { 0xAE, "NAND" },  { 0xCE, "NAND" },  { 0xEE, "NAND" },
            { 0x0F, "XOR" },   { 0x8F, "XOR" },   { 0xAF, "XOR" },   { 0xCF, "XOR" },   { 0xEF, "XOR" },
            { 0x10, "SHL" },   { 0x90, "SHL" },   { 0xB0, "SHL" },   { 0xD0, "SHL" },   { 0xF0, "SHL" },
            { 0x11, "SHR" },   { 0x91, "SHR" },   { 0xB1, "SHR" },   { 0xD1, "SHR" },   { 0xF1, "SHR" },
            { 0x12, "CMP" },   { 0x92, "CMP" },   { 0xB2, "CMP" },   { 0xD2, "CMP" },   { 0xF2, "CMP" },
            { 0x13, "JMP" },   { 0x93, "JMP" },   { 0xB3, "JMP" },   { 0xD3, "JMP" },   { 0xF3, "JMP" },
            { 0x14, "JLT" },   { 0x94, "JLT" },   { 0xB4, "JLT" },   { 0xD4, "JLT" },   { 0xF4, "JLT" },
            { 0x15, "JLE" },   { 0x95, "JLE" },   { 0xB5, "JLE" },   { 0xD5, "JLE" },   { 0xF5, "JLE" },
            { 0x16, "JGT" },   { 0x96, "JGT" },   { 0xB6, "JGT" },   { 0xD6, "JGT" },   { 0xF6, "JGT" },
            { 0x17, "JGE" },   { 0x97, "JGE" },   { 0xB7, "JGE" },   { 0xD7, "JGE" },   { 0xF7, "JGE" },
            { 0x18, "JZ" },    { 0x98, "JZ" },    { 0xB8, "JZ" },    { 0xD8, "JZ" },    { 0xF8, "JZ" },
            { 0x19, "JNZ" },   { 0x99, "NOP" },   { 0xB9, "res" },   { 0xD9, "JNZ" },   { 0xF9, "JNZ" },
            { 0x1A, "OUT" },   { 0x9A, "res" },   { 0xBA, "res" },   { 0xDA, "OUT" },   { 0xFA, "OUT" },
            { 0x1B, "IN" },    { 0x9B, "res" },   { 0xBB, "res" },   { 0xDB, "IN" },    { 0xFB, "IN" },
            { 0x1C, "PUSH" },  { 0x9C, "PUSH" },  { 0xBC, "PUSH" },  { 0xDC, "PUSH" },  { 0xFC, "PUSH" },
            { 0x1D, "POP" },   { 0x9D, "POP" },   { 0xBD, "POP" },   { 0xDD, "POP" },   { 0xFD, "POP" },
            { 0x1E, "LEA" },   { 0x9E, "res" },   { 0xBE, "res" },   { 0xDE, "res" },   { 0xFE, "res" },
            { 0x1F, "CALL" },  { 0x9F, "CALL" },  { 0xBF, "CALL" },  { 0xDF, "CALL" },  { 0xFF, "CALL" }
        };

        [OpCode(0x00, 0x80, 0xA0, 0xC0, 0xE0)]
        DecodeResult HLT(out string text) {
            // text = $"{address.H0,-10:X8}{OpCodes[opcode]} ; {opcode:X2} {operand1?.B0:X2}";
            text = FormatOutput(address, opcode, $"", "");
            return DecodeResult.Complete;
        }

        [OpCode(0x01, 0x02, 0x03)]
        DecodeResult ST(out string text) {
            text = null;

            if (operand1 == null) {
                return DecodeResult.Partial;
            }

            if ((operand1?.B0 & 0b_1111_0000) == 0b_0000_0000) {
                // Source operand is immediate
                if (operand2 == null) {
                    return DecodeResult.Partial;
                }
            }

            if ((operand1?.B1 & 0b_1111_0000) == 0b_0000_0000) {
                // Destination operand is immediate
                if (operand2 == null) {
                    return DecodeResult.Partial;
                }
            }

            bool srcIsAddress = ((operand1?.B0 & 0b_0000_0001) == 0b_0000_0001);
            bool destIsAddress = ((operand1?.B1 & 0b_0000_0001) == 0b_0000_0001);

            string srcImmBytes = "";
            string destImmBytes = "";

            string srcReg = "";
            string destReg = "";

            string srcImm = "";
            string destImm = "";

            if ((operand1?.B0 & 0b_1111_0000) == 0b_0000_0000) {
                // Source operand is immediate

                switch (operand1?.B0 & 0b_0000_1110) {
                case OpFlag_Imm08b:
                    srcImmBytes = $"{operand2?.B0:X2} ";
                    srcImm = $"{(srcIsAddress ? "[" : "")}${operand2?.B0:X2}{(srcIsAddress ? "]" : "")}";
                    break;

                case OpFlag_Imm16b:
                    srcImmBytes = $"{operand2?.B0:X2} {operand2?.B1:X2} ";
                    srcImm = $"{(srcIsAddress ? "[" : "")}${operand2?.B1:X2}{operand2?.B0:X2}{(srcIsAddress ? "]" : "")}";
                    break;

                case OpFlag_Imm32b:
                    srcImmBytes = $"{operand2?.B0:X2} {operand2?.B1:X2} {operand2?.B2:X2} {operand2?.B3:X2} ";
                    srcImm = $"{(srcIsAddress ? "[" : "")}${operand2?.B3:X2}{operand2?.B2:X2}{operand2?.B1:X2}{operand2?.B0:X2}{(srcIsAddress ? "]" : "")}";
                    break;

                case OpFlag_Imm64b:
                    srcImmBytes = $"{operand2?.B0:X2} {operand2?.B1:X2} {operand2?.B2:X2} {operand2?.B3:X2} {operand2?.B4:X2} {operand2?.B5:X2} {operand2?.B6:X2} {operand2?.B7:X2} ";
                    srcImm = $"{(srcIsAddress ? "[" : "")}${operand2?.B7:X2}{operand2?.B6:X2}{operand2?.B5:X2}{operand2?.B4:X2}{operand2?.B3:X2}{operand2?.B2:X2}{operand2?.B1:X2}{operand2?.B0:X2}{(srcIsAddress ? "]" : "")}";
                    break;
                }

            }
            else {
                // Source operand is register

                int srcRegNameFlag = (int)(operand1?.B0 & 0b_0000_1110);
                int srcSubregNameFlag = (int)(operand1?.B0 & 0b_1111_0000);

                srcReg = $"{(srcIsAddress ? "[" : "")}{RegName[srcRegNameFlag]}{SubRegName[srcSubregNameFlag]}{(srcIsAddress ? "]" : "")}";
            }

            if ((operand1?.B1 & 0b_1111_0000) == 0b_0000_0000) {
                // Destination operand is immediate

                switch (operand1?.B1 & 0b_0000_1110) {
                case OpFlag_Imm08b:
                    destImmBytes = $"{operand2?.B0:X2} ";
                    destImm = $"{(destIsAddress ? "[" : "")}${operand2?.B0:X2}{(destIsAddress ? "]" : "")}";
                    break;

                case OpFlag_Imm16b:
                    destImmBytes = $"{operand2?.B0:X2} {operand2?.B1:X2} ";
                    destImm = $"{(destIsAddress ? "[" : "")}${operand2?.B1:X2}{operand2?.B0:X2}{(destIsAddress ? "]" : "")}";
                    break;

                case OpFlag_Imm32b:
                    destImmBytes = $"{operand2?.B0:X2} {operand2?.B1:X2} {operand2?.B2:X2} {operand2?.B3:X2} ";
                    destImm = $"{(destIsAddress ? "[" : "")}${operand2?.B3:X2}{operand2?.B2:X2}{operand2?.B1:X2}{operand2?.B0:X2}{(destIsAddress ? "]" : "")}";
                    break;

                case OpFlag_Imm64b:
                    destImmBytes = $"{operand2?.B0:X2} {operand2?.B1:X2} {operand2?.B2:X2} {operand2?.B3:X2} {operand2?.B4:X2} {operand2?.B5:X2} {operand2?.B6:X2} {operand2?.B7:X2} ";
                    destImm = $"{(destIsAddress ? "[" : "")}${operand2?.B7:X2}{operand2?.B6:X2}{operand2?.B5:X2}{operand2?.B4:X2}{operand2?.B3:X2}{operand2?.B2:X2}{operand2?.B1:X2}{operand2?.B0:X2}{(destIsAddress ? "]" : "")}";
                    break;
                }
            }
            else {
                // Destination operand is register

                int destRegNameFlag = (int)(operand1?.B1 & 0b_0000_1110);
                int destSubregNameFlag = (int)(operand1?.B1 & 0b_1111_0000);
                destReg = $"{(destIsAddress ? "[" : "")}{RegName[destRegNameFlag]}{SubRegName[destSubregNameFlag]}{(destIsAddress ? "]" : "")}";
            }

            StringBuilder output = new StringBuilder(
                $"{address.H0,-10:X8}{$"{OpCodes[opcode]} {srcImm}{srcReg}, {destImm}{destReg}",-42} ; {opcode:X2} {operand1?.B0:X2} {operand1?.B1:X2} {srcImmBytes}{destImmBytes}"
            );

            // text = output.ToString();

            text = FormatOutput(address, opcode, 
                $"{srcImm}{srcReg}, {destImm}{destReg}", 
                $"{operand1?.B0:X2} {operand1?.B1:X2} {srcImmBytes}{destImmBytes}");

            return DecodeResult.Complete;
        }


        [OpCode(0x81, 0x83)]
        DecodeResult ADD_AB0_08b(out string text) {
            text = null;

            if (operand1 == null) {
                return DecodeResult.Partial;
            }

            // text = $"{address.H0,-10:X8}{$"{OpCodes[opcode]} ${operand1?.B0:X2}, A.B0",-42} ; {opcode:X2} {operand1?.B0:X2}";
            text = FormatOutput(address, opcode,
                $"${ operand1?.B0:X2}, A.B0",
                $"{operand1?.B0:X2}");
            return DecodeResult.Complete;
        }

        [OpCode(0xA1, 0xA3)]
        DecodeResult ADD_AQ0_16b(out string text) {
            text = null;

            if (operand1 == null) {
                return DecodeResult.Partial;
            }

            // text = $"{address.H0,-10:X8}{$"{OpCodes[opcode]} ${operand1?.B1:X2}{operand1?.B0:X2}, A.Q0",-42} ; {opcode:X2} {operand1?.B0:X2} {operand1?.B1:X2}";
            text = FormatOutput(address, opcode,
                $"${operand1?.B1:X2}{operand1?.B0:X2}, A.Q0",
                $"{operand1?.B0:X2} {operand1?.B1:X2}");
            return DecodeResult.Complete;
        }

        [OpCode(0xC1, 0xC3)]
        DecodeResult ADD_AH0_32b(out string text) {
            text = null;

            if (operand1 == null) {
                return DecodeResult.Partial;
            }

            // text = $"{address.H0,-10:X8}{$"{OpCodes[opcode]} ${operand1?.B3:X2}{operand1?.B2:X2}{operand1?.B1:X2}{operand1?.B0:X2}, A.H0",-42} ; {opcode:X2} {operand1?.B0:X2} {operand1?.B1:X2} {operand1?.B2:X2} {operand1?.B3:X2}";
            text = FormatOutput(address, opcode,
                $"${operand1?.B3:X2}{operand1?.B2:X2}{operand1?.B1:X2}{operand1?.B0:X2}, A.H0",
                $"{operand1?.B0:X2} {operand1?.B1:X2} {operand1?.B2:X2} {operand1?.B3:X2}");
            return DecodeResult.Complete;
        }

        [OpCode(0xE1, 0xE3)]
        DecodeResult ADD_AW0_64b(out string text) {
            text = null;

            if (operand1 == null) {
                return DecodeResult.Partial;
            }

            // text = $"{address.H0,-10:X8}{$"{OpCodes[opcode]} ${operand1?.B7:X2}{operand1?.B6:X2}{operand1?.B5:X2}{operand1?.B4:X2}{operand1?.B3:X2}{operand1?.B2:X2}{operand1?.B1:X2}{operand1?.B0:X2}, A",-42}; {opcode:X2} {operand1?.B0:X2} {operand1?.B1:X2} {operand1?.B2:X2} {operand1?.B3:X2} {operand1?.B4:X2} {operand1?.B5:X2} {operand1?.B6:X2} {operand1?.B7:X2}";
            text = FormatOutput(address, opcode,
                $"${operand1?.B7:X2}{operand1?.B6:X2}{operand1?.B5:X2}{operand1?.B4:X2}{operand1?.B3:X2}{operand1?.B2:X2}{operand1?.B1:X2}{operand1?.B0:X2}, A",
                $"{operand1?.B0:X2} {operand1?.B1:X2} {operand1?.B2:X2} {operand1?.B3:X2} {operand1?.B4:X2} {operand1?.B5:X2} {operand1?.B6:X2} {operand1?.B7:X2}");
            return DecodeResult.Complete;
        }

        [OpCode(0x82)]
        DecodeResult ST_AB0_08b(out string text) {
            text = null;

            if (operand1 == null) {
                return DecodeResult.Partial;
            }

            // text = $"{address.H0,-10:X8}{$"{OpCodes[opcode]} A.B0, [${operand1?.B0:X2}]",-42}; {opcode:X2} {operand1?.B0:X2}";
            text = FormatOutput(address, opcode,
                $"A.B0, [${operand1?.B0:X2}]",
                $"{operand1?.B0:X2}");
            return DecodeResult.Complete;
        }

        [OpCode(0xA2)]
        DecodeResult ST_AQ0_16b(out string text) {
            text = null;

            if (operand1 == null) {
                return DecodeResult.Partial;
            }

            // text = $"{address.H0,-10:X8}{$"{OpCodes[opcode]} A.Q0, [${operand1?.B1:X2}{operand1?.B0:X2}]",-42} ; {opcode:X2} {operand1?.B0:X2} {operand1?.B1:X2}";
            text = FormatOutput(address, opcode,
                $"A.Q0, [${operand1?.B1:X2}{operand1?.B0:X2}]",
                $"{operand1?.B0:X2} {operand1?.B1:X2}");
            return DecodeResult.Complete;
        }

        [OpCode(0xC2)]
        DecodeResult ST_AH0_32b(out string text) {
            text = null;

            if (operand1 == null) {
                return DecodeResult.Partial;
            }

            // text = $"{address.H0,-10:X8}{$"{OpCodes[opcode]} A.H0, [${operand1?.B3:X2}{operand1?.B2:X2}{operand1?.B1:X2}{operand1?.B0:X2}]",-42} ; {opcode:X2} {operand1?.B0:X2} {operand1?.B1:X2} {operand1?.B2:X2} {operand1?.B3:X2}";
            text = FormatOutput(address, opcode,
                $"A.H0, [${operand1?.B3:X2}{operand1?.B2:X2}{operand1?.B1:X2}{operand1?.B0:X2}]",
                $"{operand1?.B0:X2} {operand1?.B1:X2} {operand1?.B2:X2} {operand1?.B3:X2}");
            return DecodeResult.Complete;
        }

        [OpCode(0xE2)]
        DecodeResult ST_AW0_64b(out string text) {
            text = null;

            if (operand1 == null) {
                return DecodeResult.Partial;
            }

            // text = $"{address.H0,-10:X8}{$"{OpCodes[opcode]} A, [${operand1?.B7:X2}{operand1?.B6:X2}{operand1?.B5:X2}{operand1?.B4:X2}{operand1?.B3:X2}{operand1?.B2:X2}{operand1?.B1:X2}{operand1?.B0:X2}]",-42} ; {opcode:X2} {operand1?.B0:X2} {operand1?.B1:X2} {operand1?.B2:X2} {operand1?.B3:X2} {operand1?.B4:X2} {operand1?.B5:X2} {operand1?.B6:X2} {operand1?.B7:X2}";
            text = FormatOutput(address, opcode,
                $"A, [${operand1?.B7:X2}{operand1?.B6:X2}{operand1?.B5:X2}{operand1?.B4:X2}{operand1?.B3:X2}{operand1?.B2:X2}{operand1?.B1:X2}{operand1?.B0:X2}]",
                $"{operand1?.B0:X2} {operand1?.B1:X2} {operand1?.B2:X2} {operand1?.B3:X2} {operand1?.B4:X2} {operand1?.B5:X2} {operand1?.B6:X2} {operand1?.B7:X2}");
            return DecodeResult.Complete;
        }

        string FormatOutput(RegValue address, byte opcode, string instruction, string paramBytes) {
            StringBuilder text = new StringBuilder ($"{address.H0:X8}: {$"{OpCodes[opcode]} {instruction}",-52} ; {opcode:X2} {paramBytes}");
            return text.ToString();
        }
    }

}

