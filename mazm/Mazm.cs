using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Dynamic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Tortilla;
using Maize;
using System.Diagnostics;

namespace mazm {
    using TokenTree = Expression<string>;

    public struct Expression<T> {
        public Expression(T token) {
            KVP = new KeyValuePair<T, List<Expression<T>>>(token, new List<Expression<T>>());
        }

        public T Key => KVP.Key;
        public List<Expression<T>> Value => KVP.Value;

        private KeyValuePair<T, List<Expression<T>>> KVP { get; set; }
        
        public Expression<T> this[int index] { 
            get => Value[index];
            set => Value[index] = value;
        }

        public bool IsEmpty => KVP.Value.Count == 0;
        public int Count => Value.Count;

        public Expression<T> Add(T token) {
            var expr = new Expression<T>(token);
            Value.Add(expr);
            return expr;
        }

        public bool Remove(T token) {
            return Value.Remove(new Expression<T>(token));
        }

        public void Clear() {
            Value.Clear();
        }

        public override string ToString() {
            return Key.ToString();
        }
    }

    public class Mazm {
        public Mazm() {
            Opcodes = new Dictionary<string, (KeywordParser, KeywordCompiler, byte)> {
                { "HALT", (Opcode_0Param_Tokenizer, NoOperand_Compiler, 0x00) },
                { "LD",   (Opcode_2Param_Tokenizer, RegImm_Reg_Compiler, 0x01) },
                { "ST",   (Opcode_2Param_Tokenizer, RegImm_ImmAddr_Compiler, 0x02) },
                { "ADD",  (Opcode_2Param_Tokenizer, RegImm_Reg_Compiler, 0x03) },
                { "SUB",  (Opcode_2Param_Tokenizer, RegImm_Reg_Compiler, 0x04) },
                { "MUL",  (Opcode_2Param_Tokenizer, RegImm_Reg_Compiler, 0x05) },
                { "DIV",  (Opcode_2Param_Tokenizer, RegImm_Reg_Compiler, 0x06) },
                { "MOD",  (Opcode_2Param_Tokenizer, RegImm_Reg_Compiler, 0x07) },
                { "AND",  (Opcode_2Param_Tokenizer, RegImm_Reg_Compiler, 0x08) },
                { "OR",   (Opcode_2Param_Tokenizer, RegImm_Reg_Compiler, 0x09) },
                { "NOR",  (Opcode_2Param_Tokenizer, RegImm_Reg_Compiler, 0x0A) },
                { "NAND", (Opcode_2Param_Tokenizer, RegImm_Reg_Compiler, 0x0B) },
                { "XOR",  (Opcode_2Param_Tokenizer, RegImm_Reg_Compiler, 0x0C) },
                { "SHL",  (Opcode_2Param_Tokenizer, RegImm_Reg_Compiler, 0x0D) },
                { "SHR",  (Opcode_2Param_Tokenizer, RegImm_Reg_Compiler, 0x0E) },
                { "CMP",  (Opcode_2Param_Tokenizer, RegImm_Reg_Compiler, 0x0F) },
                { "TEST", (Opcode_2Param_Tokenizer, RegImm_Reg_Compiler, 0x10) },
                { "INT",  (Opcode_1Param_Tokenizer, RegImm_Compiler, 0x12) },
                { "STIN", (Opcode_2Param_Tokenizer, RegImm_RegAddr_Compiler, 0x13) },
                { "OUTR", (Opcode_2Param_Tokenizer, RegImm_Reg_Compiler, 0x14) },
                { "JMP",  (Opcode_1Param_Tokenizer, RegImm_Compiler, 0x16) },
                { "JZ",   (Opcode_1Param_Tokenizer, RegImm_Compiler, 0x17) },
                { "JNZ",  (Opcode_1Param_Tokenizer, RegImm_Compiler, 0x18) },
                { "JLT",  (Opcode_1Param_Tokenizer, RegImm_Compiler, 0x19) },
                { "JLE",  (Opcode_1Param_Tokenizer, RegImm_Compiler, 0x1A) },
                { "JGT",  (Opcode_1Param_Tokenizer, RegImm_Compiler, 0x1B) },
                { "JGE",  (Opcode_1Param_Tokenizer, RegImm_Compiler, 0x1C) },
                { "CALL", (Opcode_1Param_Tokenizer, RegImm_Compiler, 0x1D) },
                { "OUT",  (Opcode_2Param_Tokenizer, RegImm_Imm_Compiler, 0x1E) },
                { "IN",   (Opcode_2Param_Tokenizer, RegImm_Reg_Compiler, 0x1F) },
                { "PUSH", (Opcode_1Param_Tokenizer, RegImm_Compiler, 0x20) },
                { "CLR",  (Opcode_1Param_Tokenizer, Reg_Compiler, 0x22) },
                { "INC",  (Opcode_1Param_Tokenizer, Reg_Compiler, 0x23) },
                { "DEC",  (Opcode_1Param_Tokenizer, Reg_Compiler, 0x24) },
                { "NOT",  (Opcode_1Param_Tokenizer, Reg_Compiler, 0x25) },
                { "POP",  (Opcode_1Param_Tokenizer, Reg_Compiler, 0x26) },
                { "RET",  (Opcode_0Param_Tokenizer, NoOperand_Compiler, 0x27) },
                { "NOP",  (Opcode_0Param_Tokenizer, NoOperand_Compiler, 0xAA) },
                { "BRK",  (Opcode_0Param_Tokenizer, NoOperand_Compiler, 0xFF) }
            };

            Keywords = new Dictionary<string, (KeywordParser, KeywordCompiler)> {
                { "ADDRESS", (ADDRESS_Tokenizer, ADDRESS_Compiler) },
                { "LABEL",   (LABEL_Tokenizer, LABEL_Compiler) },
                { "DATA",    (DATA_Tokenizer, DATA_Compiler) },
                { "STRING",  (STRING_Tokenizer, STRING_Compiler) },
                { "INCLUDE", (INCLUDE_Tokenizer, null) },
                { "CODE",    (null, CODE_Compiler) }
            };

            ReservedWords = Enumerable.Concat(Keywords.Keys, Opcodes.Keys).ToList();

            SpecialCharsList = Enum.GetValues(typeof(SpecialChars))
                        .Cast<int>()
                        .Select(d => ((char)d))
                        .ToList();

        }

        public enum ParserState {
            Newline,
            CodeBlock,
            Comment,
            Keyword,
            Opcode,
            LabelDeclaration,
            LabelInstance,
            Address,
            HexLiteral,
            DecLiteral,
            BinLiteral,
            Operand1,
            Operand2,
            Register,
            SubRegister,
            Whitespace
        }

        public enum LabelState {
            Start,
            Name,
            Value,
            End
        }

        public enum OpcodeState {
            Start,
            Operand1,
            Operand2,
            End
        }

        public enum LiteralState {
            Start,
            Type,
            Value,
            End
        }

        public enum SpecialChars : int {
            Hex = '$',
            Dec = '#',
            Bin = '%',
            LabelEnd = ':',
            CommentStart = ';',
            Address = '@',
            RegSep = '.'
        };

        static void Main(string[] args) {
            if (args.Length < 1) {
                Console.Error.WriteLine("Missing file name");
                return;
            }

            var mazm = new Mazm();
            var filename = args[0];

            mazm.Assemble(filename);
        }

        string BasePath { get; set; }

        public void Assemble(string filename) {
            TokenTree Tokens = new TokenTree(filename);
            BasePath = Path.GetDirectoryName(filename);

            using (StreamReader sr = new StreamReader(filename)) {
                Tokenize(sr, Tokens);
            }

            Compile(Tokens);

            var path = Path.GetFullPath(filename);
            var binfile = Path.ChangeExtension(filename, "bin");

            using (BinaryWriter binWriter = new BinaryWriter(File.Open(binfile, FileMode.Create))) {
                var lastBlock = MemoryMap.Last().Key << 8;
                var end = lastBlock + byte.MaxValue - 1;
                CurrentAddress = 0;

                while (CurrentAddress < end) {
                    binWriter.Write(ReadByte(CurrentAddress));
                    ++CurrentAddress;
                }
            }
        }

        protected string CurrentToken { get; set; } = "";

        public void Tokenize(StreamReader sr, TokenTree tree) {
            ParserState state = ParserState.Whitespace;
            while (sr.Peek() >= 0) {
                char c = (char)sr.Read();

                switch (state) {
                case ParserState.Comment:
                    switch (c) {
                    case '\r':
                    case '\n':
                        state = ParserState.Newline;
                        continue;
                    }
                    break;

                case ParserState.Keyword:
                    state = ReadKeyword(state, sr, tree, c);
                    continue;

                case ParserState.CodeBlock:
                    state = ParseCodeBlock(sr, tree, c);
                    continue;

                default:
                    state = ProcessCharStream(state, sr, tree, c);
                    continue;
                }
            }
        }

        private void Compile(TokenTree tokens) {
            int i = 0;

            while (i < tokens.Value.Count) {
                var tree = tokens.Value[i];
                var key = tree.Key;
                Keywords[key].Item2(tree, key);
                ++i;
            }

            foreach (var pair in Fixups) {
                WriteHalfWord(pair.Key, Labels[pair.Value]);
            }
        }

        protected UInt32 CurrentAddress { get; set; } = 0;

        protected Dictionary<string, UInt32> Labels { get; set; } = new Dictionary<string, uint>();
        protected Dictionary<UInt32, string> Fixups { get; set; } = new Dictionary<UInt32, string>();

        private ParserState INCLUDE_Tokenizer(StreamReader sr, TokenTree tree, char c) {
            TokenTree stringTree = new TokenTree("");
            STRING_Tokenizer(sr, stringTree, c);
            var filename = stringTree.Value[0].Key;
            filename = Path.Combine(BasePath, filename);

            using (StreamReader isr = new StreamReader(filename)) {
                Tokenize(isr, tree);
            }

            return ParserState.Whitespace;
        }


        protected void LABEL_Compiler(TokenTree tree, string opcodeStr) {
            var label = tree.Value[0].Key;
            var valueStr = tree.Value[1].Key;

            uint value = ConvertLabelString(valueStr);

            Labels.Add(label, value);
        }

        private uint ConvertLabelString(string valueStr) {
            UInt32 value = 0;
            char typeChar = valueStr[0];

            if (typeChar == (char)SpecialChars.Hex) {
                value = Convert.ToUInt32(valueStr.Substring(1), 16);
            }
            else if (typeChar == (char)SpecialChars.Dec) {
                value = Convert.ToUInt32(valueStr.Substring(1), 10);
            }
            else if (typeChar == (char)SpecialChars.Bin) {
                value = Convert.ToUInt32(valueStr.Substring(1), 1);
            }
            else {
                value = UInt32.MaxValue;
            }

            return value;
        }

        protected byte CompileLiteral(string literal, out RegValue value) {
            char typeChar = literal[0];
            byte typeByte = 0;
            value = 0;

            if (typeChar == (char)SpecialChars.Hex) {
                typeByte = CompileHexLiteral(literal.Substring(1), out value);
            }
            else if (typeChar == (char)SpecialChars.Dec) {
                throw new NotImplementedException("Decimal literals not implemented");
            }
            else if (typeChar == (char)SpecialChars.Bin) {
                throw new NotImplementedException("Binary literals not implemented");
            }

            return typeByte;
        }

        protected byte CompileLabel(string label, out RegValue value) {
            value = Labels[label];
            return MaizeInstruction.OpFlag_Imm32b;
        }

        protected byte CompileHexLiteral(string literal, out RegValue value) {
            var len = literal.Length;
            value = Convert.ToUInt64(literal, 16);
            byte typeByte = MaizeInstruction.OpFlag_Imm08b;

            if (len <= 2) {
                typeByte = MaizeInstruction.OpFlag_Imm08b;
            }
            else if (len <= 4) {
                typeByte = MaizeInstruction.OpFlag_Imm16b;
            }
            else if (len <= 8) {
                typeByte = MaizeInstruction.OpFlag_Imm32b;
            }
            else if (len <= 16) {
                typeByte = MaizeInstruction.OpFlag_Imm64b;
            }
            else {
                throw new Exception("Invalid literal format");
            }

            return typeByte;
        }

        protected byte CompileRegister(string regStr) {
            byte regByte = 0;
            var reg = regStr[0];

            switch (reg) {
            case 'A':
                regByte = MaizeInstruction.OpFlag_RegA;
                break;

            case 'B':
                regByte = MaizeInstruction.OpFlag_RegB;
                break;

            case 'C':
                regByte = MaizeInstruction.OpFlag_RegC;
                break;

            case 'D':
                regByte = MaizeInstruction.OpFlag_RegD;
                break;

            case 'E':
                regByte = MaizeInstruction.OpFlag_RegE;
                break;

            case 'G':
                regByte = MaizeInstruction.OpFlag_RegG;
                break;

            case 'H':
                regByte = MaizeInstruction.OpFlag_RegH;
                break;

            case 'J':
                regByte = MaizeInstruction.OpFlag_RegJ;
                break;

            case 'K':
                regByte = MaizeInstruction.OpFlag_RegK;
                break;

            case 'L':
                regByte = MaizeInstruction.OpFlag_RegL;
                break;

            case 'M':
                regByte = MaizeInstruction.OpFlag_RegM;
                break;

            case 'Z':
                regByte = MaizeInstruction.OpFlag_RegZ;
                break;

            case 'F':
                regByte = MaizeInstruction.OpFlag_RegF;
                break;

            case 'I':
                regByte = MaizeInstruction.OpFlag_RegI;
                break;

            case 'P':
                regByte = MaizeInstruction.OpFlag_RegP;
                break;

            case 'S':
                regByte = MaizeInstruction.OpFlag_RegS;
                break;

            default:
                throw new Exception("Invalid register");
            }

            if (regStr.Length > 1) {
                if (regStr[1] == (char)SpecialChars.RegSep) {
                    string subReg = regStr.Substring(2);

                    switch (subReg) {
                    case "B0":
                        regByte |= MaizeInstruction.OpFlag_RegB0;
                        break;

                    case "B1":
                        regByte |= MaizeInstruction.OpFlag_RegB1;
                        break;

                    case "B2":
                        regByte |= MaizeInstruction.OpFlag_RegB2;
                        break;

                    case "B3":
                        regByte |= MaizeInstruction.OpFlag_RegB3;
                        break;

                    case "B4":
                        regByte |= MaizeInstruction.OpFlag_RegB4;
                        break;

                    case "B5":
                        regByte |= MaizeInstruction.OpFlag_RegB5;
                        break;

                    case "B6":
                        regByte |= MaizeInstruction.OpFlag_RegB6;
                        break;

                    case "B7":
                        regByte |= MaizeInstruction.OpFlag_RegB7;
                        break;

                    case "Q0":
                        regByte |= MaizeInstruction.OpFlag_RegQ0;
                        break;

                    case "Q1":
                        regByte |= MaizeInstruction.OpFlag_RegQ1;
                        break;

                    case "Q2":
                        regByte |= MaizeInstruction.OpFlag_RegQ2;
                        break;

                    case "Q3":
                        regByte |= MaizeInstruction.OpFlag_RegQ3;
                        break;

                    case "H0":
                        regByte |= MaizeInstruction.OpFlag_RegH0;
                        break;

                    case "H1":
                        regByte |= MaizeInstruction.OpFlag_RegH1;
                        break;

                    case "W":
                    case "W0":
                        regByte |= MaizeInstruction.OpFlag_RegW0;
                        break;

                    }
                }
            }
            else {
                regByte |= MaizeInstruction.OpFlag_RegW0;
            }

            return regByte;
        }

        protected void CODE_Compiler(TokenTree tree, string opcodeStr) {
            var label = tree.Value[0].Key;
            var address = ConvertLabelString(label);

            if (address == UInt32.MaxValue) {
                if (Labels.ContainsKey(label)) {
                    address = Labels[label];
                }
                else {
                    address = UInt32.MaxValue;
                }

                if (address == UInt32.MaxValue) {
                    address = CurrentAddress;
                    Labels[label] = address;
                }
            }

            CurrentAddress = address;

            int i = 1;

            while (i < tree.Value.Count) {
                var subTree = tree.Value[i];
                var key = subTree.Key;

                if (Keywords.ContainsKey(key)) {
                    Keywords[key].Item2(subTree, key);
                }
                else if (Opcodes.ContainsKey(key)) {
                    Opcodes[key].Item2(subTree, key);
                }

                ++i;
            }
        }

        protected void ADDRESS_Compiler(TokenTree tree, string opcodeStr) {
            var dataStr = tree.Value[0].Key;
            uint data = 0;

            if (IsLabel(dataStr)) {
                RegValue value = Labels[dataStr];
                CurrentAddress = WriteLabel(CurrentAddress, dataStr, value);
            }
            else {
                data = ConvertLabelString(dataStr);
                CurrentAddress = WriteHalfWord(CurrentAddress, data);
            }
        }
        protected void STRING_Compiler(TokenTree tree, string opcodeStr) {
            int i = 0;

            while (i < tree.Value.Count) {
                var literal = tree.Value[i].Key;

                int j = 0;

                while (j < literal.Length) {
                    char c = literal[j];

                    if (c == '\\') {
                        ++j;
                        c = literal[j];

                        switch (c) {
                        case '\\':
                            CurrentAddress = WriteByte(CurrentAddress, (byte)c);
                            break;

                        case '\'':
                            CurrentAddress = WriteByte(CurrentAddress, (byte)c);
                            break;

                        case '\"':
                            CurrentAddress = WriteByte(CurrentAddress, (byte)c);
                            break;

                        case '0':
                            CurrentAddress = WriteByte(CurrentAddress, (byte)0);
                            break;

                        case 't':
                            CurrentAddress = WriteByte(CurrentAddress, (byte)'\t');
                            break;

                        case 'r':
                            CurrentAddress = WriteByte(CurrentAddress, (byte)'\r');
                            break;

                        case 'n':
                            CurrentAddress = WriteByte(CurrentAddress, (byte)'\n');
                            break;

                        case 'a':
                            CurrentAddress = WriteByte(CurrentAddress, (byte)'\a');
                            break;

                        case 'b':
                            CurrentAddress = WriteByte(CurrentAddress, (byte)'\b');
                            break;

                        case 'f':
                            CurrentAddress = WriteByte(CurrentAddress, (byte)'\f');
                            break;

                        case 'e':
                            CurrentAddress = WriteByte(CurrentAddress, (byte)0x1B);
                            break;

                        case 'v':
                            CurrentAddress = WriteByte(CurrentAddress, (byte)'\v');
                            break;

                        }
                    }
                    else {
                        CurrentAddress = WriteByte(CurrentAddress, (byte)c);
                    }

                    ++j;
                }

                ++i;
            }
        }

        protected void DATA_Compiler(TokenTree tree, string opcodeStr) {
            int i = 0;

            while (i < tree.Value.Count) {
                RegValue value;
                var literal = tree.Value[i].Key;
                var typeByte = CompileLiteral(literal, out value);

                if ((typeByte & MaizeInstruction.OpFlag_ImmSize) == MaizeInstruction.OpFlag_Imm16b) {
                    CurrentAddress = WriteQuarterWord(CurrentAddress, value.Q0);
                }
                else if ((typeByte & MaizeInstruction.OpFlag_ImmSize) == MaizeInstruction.OpFlag_Imm32b) {
                    CurrentAddress = WriteHalfWord(CurrentAddress, value.H0);
                }
                else if ((typeByte & MaizeInstruction.OpFlag_ImmSize) == MaizeInstruction.OpFlag_Imm64b) {
                    CurrentAddress = WriteWord(CurrentAddress, value.Value);
                }
                else {
                    CurrentAddress = WriteByte(CurrentAddress, value.B0);
                }

                ++i;
            }
        }

        protected void NoOperand_Compiler(TokenTree tree, string opcodeStr) {
            byte opcode = Opcodes[opcodeStr].Item3;
            CurrentAddress = WriteByte(CurrentAddress, opcode);
        }

        protected void RegImm_Reg_Compiler(TokenTree tree, string opcodeStr) {
            byte opcode = Opcodes[opcodeStr].Item3;
            var operand1 = tree.Value[0].Key;
            var operand2 = tree.Value[1].Key;
            byte operand1Byte = 0;
            bool isImmediate = false;
            bool isLabel = false;
            RegValue operand1Literal = 0;

            if (operand1[0] == (char)SpecialChars.Address) {
                opcode |= MaizeInstruction.OpcodeFlag_SrcAddr;
                operand1 = operand1.Substring(1);
                // operand1Byte = MaizeInstruction.OpFlag_Addr;
            }

            if (IsLiteral(operand1)) {
                isImmediate = true;
                opcode |= MaizeInstruction.OpcodeFlag_SrcImm;
                operand1Byte |= CompileLiteral(operand1, out operand1Literal);
            }
            else if (IsLabel(operand1)) {
                isLabel = true;
                isImmediate = true;
                opcode |= MaizeInstruction.OpcodeFlag_SrcImm;
                operand1Byte |= CompileLabel(operand1, out operand1Literal);
            }
            else {
                operand1Byte |= CompileRegister(operand1);
            }

            byte operand2Byte = CompileRegister(operand2);

            CurrentAddress = WriteByte(CurrentAddress, opcode);
            CurrentAddress = WriteByte(CurrentAddress, operand1Byte);
            CurrentAddress = WriteByte(CurrentAddress, operand2Byte);

            if (isImmediate) {
                if (isLabel && operand1Literal.H0 == UInt32.MaxValue) {
                    CurrentAddress = WriteLabel(CurrentAddress, operand1, operand1Literal);
                }
                else {
                    if ((operand1Byte & MaizeInstruction.OpFlag_ImmSize) == MaizeInstruction.OpFlag_Imm16b) {
                        CurrentAddress = WriteQuarterWord(CurrentAddress, operand1Literal.Q0);
                    }
                    else if ((operand1Byte & MaizeInstruction.OpFlag_ImmSize) == MaizeInstruction.OpFlag_Imm32b) {
                        CurrentAddress = WriteHalfWord(CurrentAddress, operand1Literal.H0);
                    }
                    else if ((operand1Byte & MaizeInstruction.OpFlag_ImmSize) == MaizeInstruction.OpFlag_Imm64b) {
                        CurrentAddress = WriteWord(CurrentAddress, operand1Literal.Value);
                    }
                    else {
                        CurrentAddress = WriteByte(CurrentAddress, operand1Literal.B0);
                    }
                }
            }
            else if (isLabel) {
                CurrentAddress = WriteLabel(CurrentAddress, operand1, operand1Literal);
            }
        }

        protected void RegImm_Compiler(TokenTree tree, string opcodeStr) {
            byte opcode = Opcodes[opcodeStr].Item3;
            var operand1 = tree.Value[0].Key;
            byte operand1Byte = 0;
            bool isImmediate = false;
            bool isLabel = false;
            RegValue operand1Literal = 0;

            if (operand1[0] == (char)SpecialChars.Address) {
                opcode |= MaizeInstruction.OpcodeFlag_SrcAddr;
                operand1 = operand1.Substring(1);
                // operand1Byte = MaizeInstruction.OpFlag_Addr;
            }

            if (IsLiteral(operand1)) {
                isImmediate = true;
                opcode |= MaizeInstruction.OpcodeFlag_SrcImm;
                operand1Byte |= CompileLiteral(operand1, out operand1Literal);
            }
            else if (IsLabel(operand1)) {
                isLabel = true;
                opcode |= MaizeInstruction.OpcodeFlag_SrcImm;
                operand1Byte |= CompileLabel(operand1, out operand1Literal);
            }
            else {
                operand1Byte |= CompileRegister(operand1);
            }

            CurrentAddress = WriteByte(CurrentAddress, opcode);
            CurrentAddress = WriteByte(CurrentAddress, operand1Byte);

            if (isImmediate) {
                if ((operand1Byte & MaizeInstruction.OpFlag_ImmSize) == MaizeInstruction.OpFlag_Imm16b) {
                    CurrentAddress = WriteQuarterWord(CurrentAddress, operand1Literal.Q0);
                }
                else if ((operand1Byte & MaizeInstruction.OpFlag_ImmSize) == MaizeInstruction.OpFlag_Imm32b) {
                    CurrentAddress = WriteHalfWord(CurrentAddress, operand1Literal.H0);
                }
                else if ((operand1Byte & MaizeInstruction.OpFlag_ImmSize) == MaizeInstruction.OpFlag_Imm64b) {
                    CurrentAddress = WriteWord(CurrentAddress, operand1Literal.Value);
                }
                else {
                    CurrentAddress = WriteByte(CurrentAddress, operand1Literal.B0);
                }
            }
            else if (isLabel) {
                CurrentAddress = WriteLabel(CurrentAddress, operand1, operand1Literal);
            }
        }

        protected void Reg_Compiler(TokenTree tree, string opcodeStr) {
            byte opcode = Opcodes[opcodeStr].Item3;
            var operand1 = tree.Value[0].Key;
            byte operand1Byte = CompileRegister(operand1);
            RegValue operand1Literal = 0;

            CurrentAddress = WriteByte(CurrentAddress, opcode);
            CurrentAddress = WriteByte(CurrentAddress, operand1Byte);
        }

        protected void RegImm_Imm_Compiler(TokenTree tree, string opcodeStr) {
            byte opcode = Opcodes[opcodeStr].Item3;
            var operand1 = tree.Value[0].Key;
            var operand2 = tree.Value[1].Key;
            byte operand1Byte = 0;
            byte operand2Byte = 0;
            bool isImmediate = false;
            bool isLabel = false;
            bool isLabel2 = false;
            RegValue operand1Literal = 0;
            RegValue operand2Literal = 0;

            if (operand2[0] == (char)SpecialChars.Address) {
                operand2 = operand2.Substring(1);
                // operand2Byte = MaizeInstruction.OpFlag_Addr;
            }

            if (IsLiteral(operand1)) {
                isImmediate = true;
                opcode |= MaizeInstruction.OpcodeFlag_SrcImm;
                operand1Byte |= CompileLiteral(operand1, out operand1Literal);
            }
            else if (IsLabel(operand1)) {
                isLabel = true;
                opcode |= MaizeInstruction.OpcodeFlag_SrcImm;
                operand1Byte |= CompileLabel(operand1, out operand1Literal);
            }
            else {
                operand1Byte |= CompileRegister(operand1);
            }

            if (IsLabel(operand2)) {
                isLabel2 = true;
                operand2Byte |= CompileLabel(operand2, out operand2Literal);
            }
            else {
                operand2Byte |= CompileLiteral(operand2, out operand2Literal);
            }

            CurrentAddress = WriteByte(CurrentAddress, opcode);
            CurrentAddress = WriteByte(CurrentAddress, operand1Byte);
            CurrentAddress = WriteByte(CurrentAddress, operand2Byte);

            if (isImmediate) {
                if ((operand1Byte & MaizeInstruction.OpFlag_ImmSize) == MaizeInstruction.OpFlag_Imm16b) {
                    CurrentAddress = WriteQuarterWord(CurrentAddress, operand1Literal.Q0);
                }
                else if ((operand1Byte & MaizeInstruction.OpFlag_ImmSize) == MaizeInstruction.OpFlag_Imm32b) {
                    CurrentAddress = WriteHalfWord(CurrentAddress, operand1Literal.H0);
                }
                else if ((operand1Byte & MaizeInstruction.OpFlag_ImmSize) == MaizeInstruction.OpFlag_Imm64b) {
                    CurrentAddress = WriteWord(CurrentAddress, operand1Literal.Value);
                }
                else {
                    CurrentAddress = WriteByte(CurrentAddress, operand1Literal.B0);
                }
            }
            else if (isLabel) {
                CurrentAddress = WriteLabel(CurrentAddress, operand1, operand1Literal);
            }

            if (isLabel2) {
                CurrentAddress = WriteLabel(CurrentAddress, operand2, operand2Literal);
            }
            else {
                if ((operand2Byte & MaizeInstruction.OpFlag_ImmSize) == MaizeInstruction.OpFlag_Imm16b) {
                    CurrentAddress = WriteQuarterWord(CurrentAddress, operand2Literal.Q0);
                }
                else if ((operand2Byte & MaizeInstruction.OpFlag_ImmSize) == MaizeInstruction.OpFlag_Imm32b) {
                    CurrentAddress = WriteHalfWord(CurrentAddress, operand2Literal.H0);
                }
                else if ((operand2Byte & MaizeInstruction.OpFlag_ImmSize) == MaizeInstruction.OpFlag_Imm64b) {
                    CurrentAddress = WriteWord(CurrentAddress, operand2Literal.Value);
                }
                else {
                    CurrentAddress = WriteByte(CurrentAddress, operand2Literal.B0);
                }
            }
        }

        protected void RegImm_ImmAddr_Compiler(TokenTree tree, string opcodeStr) {
            byte opcode = Opcodes[opcodeStr].Item3;
            var operand1 = tree.Value[0].Key;
            var operand2 = tree.Value[1].Key;
            byte operand1Byte = 0;
            byte operand2Byte = 0;
            bool isImmediate = false;
            bool isLabel = false;
            bool isLabel2 = false; // this used to be true, but I can't remember why!
            RegValue operand1Literal = 0;
            RegValue operand2Literal = 0;

            if (operand2[0] == (char)SpecialChars.Address) {
                operand2 = operand2.Substring(1);
                // operand2Byte = MaizeInstruction.OpFlag_Addr;
            }

            if (IsLiteral(operand1)) {
                isImmediate = true;
                opcode |= MaizeInstruction.OpcodeFlag_SrcImm;
                operand1Byte |= CompileLiteral(operand1, out operand1Literal);
            }
            else if (IsLabel(operand1)) {
                isLabel = true;
                opcode |= MaizeInstruction.OpcodeFlag_SrcImm;
                operand1Byte |= CompileLabel(operand1, out operand1Literal);
            }
            else {
                operand1Byte |= CompileRegister(operand1);
            }

            if (IsLabel(operand2)) {
                isLabel2 = true;
                operand2Byte |= CompileLabel(operand2, out operand2Literal);
            }
            else {
                operand2Byte |= CompileLiteral(operand2, out operand2Literal);
            }

            CurrentAddress = WriteByte(CurrentAddress, opcode);
            CurrentAddress = WriteByte(CurrentAddress, operand1Byte);
            CurrentAddress = WriteByte(CurrentAddress, operand2Byte);

            if (isImmediate) {
                if ((operand1Byte & MaizeInstruction.OpFlag_ImmSize) == MaizeInstruction.OpFlag_Imm16b) {
                    CurrentAddress = WriteQuarterWord(CurrentAddress, operand1Literal.Q0);
                }
                else if ((operand1Byte & MaizeInstruction.OpFlag_ImmSize) == MaizeInstruction.OpFlag_Imm32b) {
                    CurrentAddress = WriteHalfWord(CurrentAddress, operand1Literal.H0);
                }
                else if ((operand1Byte & MaizeInstruction.OpFlag_ImmSize) == MaizeInstruction.OpFlag_Imm64b) {
                    CurrentAddress = WriteWord(CurrentAddress, operand1Literal.Value);
                }
                else {
                    CurrentAddress = WriteByte(CurrentAddress, operand1Literal.B0);
                }
            }
            else if (isLabel) {
                CurrentAddress = WriteLabel(CurrentAddress, operand1, operand1Literal);
            }

            if (isLabel2) {
                CurrentAddress = WriteLabel(CurrentAddress, operand2, operand2Literal);
            }
            else {
                if ((operand2Byte & MaizeInstruction.OpFlag_ImmSize) == MaizeInstruction.OpFlag_Imm16b) {
                    CurrentAddress = WriteQuarterWord(CurrentAddress, operand2Literal.Q0);
                }
                else if ((operand2Byte & MaizeInstruction.OpFlag_ImmSize) == MaizeInstruction.OpFlag_Imm32b) {
                    CurrentAddress = WriteHalfWord(CurrentAddress, operand2Literal.H0);
                }
                else if ((operand2Byte & MaizeInstruction.OpFlag_ImmSize) == MaizeInstruction.OpFlag_Imm64b) {
                    CurrentAddress = WriteWord(CurrentAddress, operand2Literal.Value);
                }
                else {
                    CurrentAddress = WriteByte(CurrentAddress, operand2Literal.B0);
                }
            }
        }

        private UInt32 WriteLabel(UInt32 address, string label, RegValue value) {
            if (value.H0 == UInt32.MaxValue) {
                Fixups.Add(CurrentAddress, label);
            }

            return WriteHalfWord(address, value.H0);
        }

        protected void RegImm_RegAddr_Compiler(TokenTree tree, string opcodeStr) {
            byte opcode = Opcodes[opcodeStr].Item3;
            var operand1 = tree.Value[0].Key;
            var operand2 = tree.Value[1].Key;
            byte operand1Byte = 0;
            bool isImmediate = false;
            bool isLabel = false;
            RegValue operand1Literal = 0;

            if (IsLiteral(operand1)) {
                isImmediate = true;
                opcode |= MaizeInstruction.OpcodeFlag_SrcImm;
                operand1Byte |= CompileLiteral(operand1, out operand1Literal);
            }
            else if (IsLabel(operand1)) {
                isLabel = true;
                opcode |= MaizeInstruction.OpcodeFlag_SrcImm;
                operand1Byte |= CompileLabel(operand1, out operand1Literal);
            }
            else {
                operand1Byte |= CompileRegister(operand1);
            }

            byte operand2Byte = 0;

            if (operand2[0] == (char)SpecialChars.Address) {
                // operand2Byte |= MaizeInstruction.OpFlag_Addr;
                operand2 = operand2.Substring(1);
            }

            operand2Byte |= CompileRegister(operand2);

            CurrentAddress = WriteByte(CurrentAddress, opcode);
            CurrentAddress = WriteByte(CurrentAddress, operand1Byte);
            CurrentAddress = WriteByte(CurrentAddress, operand2Byte);

            if (isImmediate) {
                if ((operand1Byte & MaizeInstruction.OpFlag_ImmSize) == MaizeInstruction.OpFlag_Imm16b) {
                    CurrentAddress = WriteQuarterWord(CurrentAddress, operand1Literal.Q0);
                }
                else if ((operand1Byte & MaizeInstruction.OpFlag_ImmSize) == MaizeInstruction.OpFlag_Imm32b) {
                    CurrentAddress = WriteHalfWord(CurrentAddress, operand1Literal.H0);
                }
                else if ((operand1Byte & MaizeInstruction.OpFlag_ImmSize) == MaizeInstruction.OpFlag_Imm64b) {
                    CurrentAddress = WriteWord(CurrentAddress, operand1Literal.Value);
                }
                else {
                    CurrentAddress = WriteByte(CurrentAddress, operand1Literal.B0);
                }
            }
            else if (isLabel) {
                CurrentAddress = WriteLabel(CurrentAddress, operand1, operand1Literal);
            }
        }

        private static bool IsLiteral(string str) {
            return str[0] == (char)SpecialChars.Hex || str[0] == (char)SpecialChars.Bin || str[0] == (char)SpecialChars.Dec;
        }

        protected bool IsLabel(string str) {
            return Labels.ContainsKey(str);
        }

        protected List<char> SpecialCharsList { get; set; }

        protected List<byte> CodeOut { get; set; } = new List<byte>();

        protected Dictionary<string, (KeywordParser, KeywordCompiler, byte)> Opcodes { get; set; }

        protected Dictionary<string, (KeywordParser, KeywordCompiler)> Keywords { get; set; }
        // protected Dictionary<string, KeywordParser> Declarations { get; set; }

        protected List<string> ReservedWords { get; set; }

        public delegate ParserState KeywordParser(StreamReader sr, TokenTree tree, char c);
        public delegate void KeywordCompiler(TokenTree tree, string opcode);

        protected SortedDictionary<UInt64, byte[]> MemoryMap { get; set; } = new SortedDictionary<ulong, byte[]>();

        protected RegValue CacheAddress;

        protected byte[] Cache { get; set; }

        bool IsCached(UInt64 address) {
            return MemoryMap.ContainsKey(address >> 8);
        }

        void SetCacheAddress(UInt64 address) {
            RegValue tmp = address;

            if (Cache == null || tmp.Base != CacheAddress.Base) {
                if (!MemoryMap.ContainsKey(tmp.Base)) {
                    MemoryMap[tmp.Base] = new byte[0x100];
                }

                Cache = MemoryMap[tmp.Base];
            }

            CacheAddress.Value = address;
        }

        public UInt64 ReadWord(UInt32 address) {
            RegValue tmp = 0;
            tmp.B0 = ReadByte(address++);
            tmp.B1 = ReadByte(address++);
            tmp.B2 = ReadByte(address++);
            tmp.B3 = ReadByte(address++);
            tmp.B4 = ReadByte(address++);
            tmp.B5 = ReadByte(address++);
            tmp.B6 = ReadByte(address++);
            tmp.B7 = ReadByte(address++);
            return tmp.Value;
        }

        public UInt32 WriteWord(UInt32 address, UInt64 value) {
            RegValue tmp = value;
            WriteByte(address++, tmp.B0);
            WriteByte(address++, tmp.B1);
            WriteByte(address++, tmp.B2);
            WriteByte(address++, tmp.B3);
            WriteByte(address++, tmp.B4);
            WriteByte(address++, tmp.B5);
            WriteByte(address++, tmp.B6);
            WriteByte(address++, tmp.B7);
            return address;
        }

        public UInt32 ReadHalfWord(UInt32 address) {
            RegValue tmp = 0;
            tmp.B0 = ReadByte(address++);
            tmp.B1 = ReadByte(address++);
            tmp.B2 = ReadByte(address++);
            tmp.B3 = ReadByte(address++);
            return tmp.H0;
        }

        public UInt32 WriteHalfWord(UInt32 address, UInt32 value) {
            RegValue tmp = value;
            WriteByte(address++, tmp.B0);
            WriteByte(address++, tmp.B1);
            WriteByte(address++, tmp.B2);
            WriteByte(address++, tmp.B3);
            return address;
        }

        public UInt16 ReadQuarterWord(UInt32 address) {
            RegValue tmp = 0;
            tmp.B0 = ReadByte(address++);
            tmp.B1 = ReadByte(address++);
            return tmp.Q0;
        }

        public UInt32 WriteQuarterWord(UInt32 address, UInt16 value) {
            RegValue tmp = value;
            WriteByte(address++, tmp.B0);
            WriteByte(address++, tmp.B1);
            return address;
        }

        public byte ReadByte(UInt32 address) {
            if (IsCached(address)) {
                SetCacheAddress(address);
                return Cache[CacheAddress.Offset];
            }

            return 0;
        }

        public UInt32 WriteByte(UInt32 address, byte value) {
            SetCacheAddress(address++);
            Cache[CacheAddress.Offset] = value;
            return address;
        }


        private ParserState ProcessCharStream(ParserState state, StreamReader sr, TokenTree tree, char c) {
            if (char.IsWhiteSpace(c)) {
                return ParserState.Whitespace;
            }
            else if (char.IsLetterOrDigit(c) || c == ',' || c == '`') {
                state = ParserState.Keyword;
            }
            else {
                switch (c) {
                case '\r':
                case '\n':
                    return ParserState.Newline;

                case (char)SpecialChars.CommentStart:
                    return ParserState.Comment;

                case (char)SpecialChars.LabelEnd:
                    return ParserState.CodeBlock;
                }
            }

            CurrentToken += c;
            return state;
        }

        protected ParserState ReadKeyword(ParserState state, StreamReader sr, TokenTree tree, char c) {
            if (char.IsWhiteSpace(c)) {
                /* What a HORRIBLE hack, but I have better things to do right now.... */
                if (CurrentToken == "INCLUDE") {
                    var Keyword = CurrentToken;
                    CurrentToken = "";
                    return Keywords[Keyword].Item1(sr, tree, c);
                }
                else {
                    return ParseKeyword(sr, tree, c);
                }
            }

            return ProcessCharStream(state, sr, tree, c);
        }

        protected ParserState ParseKeyword(StreamReader sr, TokenTree tree, char c) {
            string Keyword = CurrentToken;
            CurrentToken = "";
            TokenTree subTree = tree.Add(Keyword);

            if (Keywords.Keys.Contains(Keyword)) {
                return Keywords[Keyword].Item1(sr, subTree, c);
            }
            else if (Opcodes.Keys.Contains(Keyword)) {
                return Opcodes[Keyword].Item1(sr, subTree, c);
            }

            return ParserState.Whitespace;
        }

        protected ParserState ParseCodeBlock(StreamReader sr, TokenTree tree, char c) {
            ParserState state = ParserState.Whitespace;

            TokenTree subTree = tree.Add("CODE");
            subTree.Add(CurrentToken);
            CurrentToken = "";

            do {
                switch (state) {
                case ParserState.Comment:
                    switch (c) {
                    case '\r':
                    case '\n':
                        state = ParserState.Newline;
                        break;
                    }

                    break;

                case ParserState.Keyword:
                    state = ReadKeyword(state, sr, subTree, c);

                    if (state == ParserState.CodeBlock) {
                        return state;
                    }

                    break;

                case ParserState.CodeBlock:
                    return state;

                default:
                    state = ProcessCharStream(state, sr, subTree, c);
                    break;
                }

                c = (char)sr.Read();

            } while (sr.Peek() >= 0);

            return state;
        }

        protected ParserState ParseOpcode(StreamReader sr, TokenTree tree, char c) {
            string Keyword = CurrentToken;
            CurrentToken = "";
            TokenTree subTree = tree.Add(Keyword);

            if (Opcodes.Keys.Contains(Keyword)) {
                return Opcodes[Keyword].Item1(sr, subTree, c);
            }

            return ParserState.Whitespace;
        }

        protected ParserState Opcode_0Param_Tokenizer(StreamReader sr, TokenTree tree, char c) {
            return ParserState.Whitespace;
        }

        protected ParserState Opcode_1Param_Tokenizer(StreamReader sr, TokenTree tree, char c) {
            OpcodeState ocState = OpcodeState.Start;

            while (sr.Peek() >= 0) {
                c = (char)sr.Read();

                switch (ocState) {
                case OpcodeState.Start:
                    if (Char.IsWhiteSpace(c)) {
                        continue;
                    }
                    else if (c == ',' || c == '`') {
                        ocState = OpcodeState.Operand1;
                    }
                    else {
                        ocState = OpcodeState.Operand1;
                        CurrentToken += c;
                    }

                    break;

                case OpcodeState.Operand1:
                    if (Char.IsWhiteSpace(c)) {
                        tree.Add(CurrentToken);
                        CurrentToken = "";
                        return ParserState.Whitespace;
                    }
                    else if (c == ',' || c == '`') {
                        break;
                    }
                    else {
                        CurrentToken += c;
                    }

                    break;
                }
            }

            return ParserState.Whitespace;
        }

        protected ParserState Opcode_2Param_Tokenizer(StreamReader sr, TokenTree tree, char c) {
            OpcodeState ocState = OpcodeState.Start;

            while (sr.Peek() >= 0) {
                c = (char)sr.Read();

                switch (ocState) {
                case OpcodeState.Start:
                    if (Char.IsWhiteSpace(c)) {
                        continue;
                    }
                    else if (c == ',' || c == '`') {
                        ocState = OpcodeState.Operand1;
                    }
                    else {
                        ocState = OpcodeState.Operand1;
                        CurrentToken += c;
                    }

                    break;

                case OpcodeState.Operand1:
                    if (Char.IsWhiteSpace(c)) {
                        tree.Add(CurrentToken);
                        CurrentToken = "";
                        ocState = OpcodeState.Operand2;
                    }
                    else if (c == ',' || c == '`') {
                        break;
                    }
                    else { 
                        CurrentToken += c;
                    }

                    break;

                case OpcodeState.Operand2:
                    if (Char.IsWhiteSpace(c)) {
                        tree.Add(CurrentToken);
                        CurrentToken = "";
                        return ParserState.Whitespace;
                    }
                    else if (c == ',' || c == '`') {
                        break;
                    }
                    else {
                        CurrentToken += c;
                    }

                    break;
                }
            }

            return ParserState.Whitespace;
        }

        protected ParserState ADDRESS_Tokenizer(StreamReader sr, TokenTree tree, char c) {
            while (ParseLiteral(sr, tree, c) != LiteralState.End) {
            }

            return ParserState.Whitespace;
        }

        protected ParserState LABEL_Tokenizer(StreamReader sr, TokenTree tree, char c) {
            LabelState lbState = LabelState.Start;

            while (sr.Peek() >= 0) {
                c = (char)sr.Read();

                switch (lbState) {
                case LabelState.Start:
                    if (char.IsLetter(c) || c == '_') {
                        lbState = LabelState.Name;
                        CurrentToken += c;
                    }

                    break;

                case LabelState.Name:
                    if (char.IsLetterOrDigit(c) || c == '_') {
                        lbState = LabelState.Name;
                        CurrentToken += c;
                    }
                    else if (Char.IsWhiteSpace(c)) {
                        tree.Add(CurrentToken);
                        CurrentToken = "";
                        lbState = LabelState.Value;
                    }

                    break;

                case LabelState.Value:
                    if (ParseLiteral(sr, tree, c) == LiteralState.End) {
                        lbState = LabelState.End;
                        return ParserState.Whitespace;
                    }

                    break;
                }
            }
            
            return ParserState.LabelDeclaration;
        }

        public enum StringState {
            Start,
            Read,
            Escape,
            End
        }

        private ParserState STRING_Tokenizer(StreamReader sr, TokenTree tree, char c) {
            var state = StringState.Start;

            while (sr.Peek() >= 0) {
                c = (char)sr.Read();

                if (c == '\r' || c == '\n') {
                    return ParserState.Newline;
                }

                switch (state) {
                case StringState.Start:
                    if (c == (char)SpecialChars.CommentStart) {
                        return ParserState.Comment;
                    }

                    if (c == '"') {
                        state = StringState.Read;
                    }

                    break;

                case StringState.Read:
                    if (c == '\\') {
                        CurrentToken += c;
                        state = StringState.Escape;
                        break;
                    }

                    if (c == '"') {
                        tree.Add(CurrentToken);
                        CurrentToken = "";
                        return ParserState.Whitespace;
                    }

                    CurrentToken += c;
                    break;

                case StringState.Escape:
                    CurrentToken += c;
                    state = StringState.Read;
                    break;
                }
            }

            return ParserState.Whitespace;
        }

        protected ParserState DATA_Tokenizer(StreamReader sr, TokenTree tree, char c) {
            while (sr.Peek() >= 0) {
                c = (char)sr.Read();

                if (c == ',' || c == '`') {
                    continue;
                }

                if (char.IsLetterOrDigit(c) || c == (char)SpecialChars.Bin || c == (char)SpecialChars.Dec || c == (char)SpecialChars.Hex) {
                    CurrentToken += c;
                }
                else if ((Char.IsWhiteSpace(c) || c == '\r' || c == '\n' || c == (char)SpecialChars.CommentStart) && !string.IsNullOrEmpty(CurrentToken)) {
                    tree.Add(CurrentToken);
                    CurrentToken = "";
                }

                if (c == '\r' || c == '\n') {
                    return ParserState.Newline;
                }
                if (c == (char)SpecialChars.CommentStart) {
                    return ParserState.Comment;
                }
            }

            return ParserState.Whitespace;
        }

        protected LiteralState ParseLiteral(StreamReader sr, TokenTree tree, char c) {
            LiteralState ltState = LiteralState.Start;

            while (c >= 0) {

                switch (ltState) {
                case LiteralState.Start:
                    if (Char.IsWhiteSpace(c)) {
                        ltState = LiteralState.Start;
                    }
                    else if (c == ',' || c == '`') {
                        ltState = LiteralState.Value;
                    }
                    else {
                        CurrentToken += c;
                        ltState = LiteralState.Value;
                    }

                    break;

                case LiteralState.Value:
                    if (c == ',' || c == '`') {
                        break;
                    }

                    if (Char.IsWhiteSpace(c)) {
                        tree.Add(CurrentToken);
                        CurrentToken = "";
                        return LiteralState.End;
                    }
                    else {
                        CurrentToken += c;
                        ltState = LiteralState.Value;
                    }

                    break;

                case LiteralState.End:
                    return ltState;
                }

                c = (char)sr.Read();
            }

            return LiteralState.End;
        }
    }
}
