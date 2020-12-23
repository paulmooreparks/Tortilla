using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
// using System.Runtime.Remoting.Messaging;
using System.Threading;
using Tortilla;

/* There are MANY opportunities for consolidating duplicate code here, but I was re-writing and 
refactoring the code so much that it ended up being easier to just copy and paste initially. At 
this point, I don't know when I'll revisit the microcode, so I'll leave it as-is for now and 
consider consolidating later. It might have to wait until I rewrite it in C++ or Rust, though. */

/* One thing that you'll notice upon reading the code, especially if you have any familiarity with C# and 
with .NET in general, is that I don't use a lot of properties, but rather fields. I did this for speed and 
simplicity. Inside a CPU, and in assembly code in general, there's not a lot of information hiding, though 
there is encapsulation to some degree. My primary concern in this code is not to write enterprise-grade 
interfaces, but rather to get raw speed wherever I can. I'm sacrificing some sacred OO principles in pursuit 
of performance, and inside the ugly innards of the CPU implementation, it's probably worth it. */

namespace Maize {
    /* This is where I plan to implement microcode related to CPU exception (bad opcode, divide by 
    zero, etc.). For now I'm just throwing exceptions directly, but that's probably not how it will 
    remain. I plan to implement the actual software interrupts for CPU-level exceptions so that 
    programs can replace the handlers themselves. */

    namespace Exceptions {
        public class BadOpcode : InstructionBase<BadOpcode> {
            public override void BuildMicrocode() {
                Code = new Action[] {
                    () => {
                        throw new Exception("Unknown opcode");
                        // MB.RaiseInterrupt(0x06);
                    }
                };
            }
        }

    }

    /* The "instructions" in the Core namespace are virtual instructions inside the CPU rather 
    than actual instructions exposed to end-user code. */

    namespace Core {
        public class ReadOpcodeAndDispatch : InstructionBase<ReadOpcodeAndDispatch> {
            public override void BuildMicrocode() {
                Code = new Action[] {
                    () => {
                        P.EnableToAddressBus(SubRegister.H0);
                        MemoryModule.AddressRegister.SetFromAddressBus(SubRegister.H0);
                    },
                    () => {
                        MemoryModule.DataRegister.EnableToDataBus(SubRegister.W0);
                        Decoder.SetFromDataBus(SubRegister.W0);
                    },
                    () => {
                        P.Increment(1);
                        var opcode = Decoder.RegData.B0;
                        var instruction = Decoder.InstructionArray[opcode];
                        Decoder.JumpTo(instruction);
                        // InstructionRead?.Invoke(this, Tuple.Create(this.Value, MB.Cpu.PC.Value - 1));
                    }
                };
            }
        }

        public class ReadImmediate1Byte : InstructionBase<ReadImmediate1Byte> {
            public override void BuildMicrocode() {
                Code = new Action[] {
                    () => {
                        P.EnableToAddressBus(SubRegister.H0);
                        MemoryModule.AddressRegister.SetFromAddressBus(SubRegister.H0);
                    },
                    () => {
                        P.Increment(1);
                        MemoryModule.DataRegister.EnableToDataBus(SubRegister.B0);
                        Decoder.PopMicrocodeStack();
                    }
                };
            }
        }

        public class ReadImmediate2Byte : InstructionBase<ReadImmediate2Byte> {
            public override void BuildMicrocode() {
                Code = new Action[] {
                    () => {
                        P.EnableToAddressBus(SubRegister.H0);
                        MemoryModule.AddressRegister.SetFromAddressBus(SubRegister.H0);
                    },
                    () => {
                        P.Increment(2);
                        MemoryModule.DataRegister.EnableToDataBus(SubRegister.Q0);
                        Decoder.PopMicrocodeStack();
                    }
                };
            }
        }

        public class ReadImmediate4Byte : InstructionBase<ReadImmediate4Byte> {
            public override void BuildMicrocode() {
                Code = new Action[] {
                    () => {
                        P.EnableToAddressBus(SubRegister.H0);
                        MemoryModule.AddressRegister.SetFromAddressBus(SubRegister.H0);
                    },
                    () => {
                        P.Increment(4);
                        MemoryModule.DataRegister.EnableToDataBus(SubRegister.H0);
                        Decoder.PopMicrocodeStack();
                    }
                };
            }
        }

        public class ReadImmediate8Byte : InstructionBase<ReadImmediate8Byte> {
            public override void BuildMicrocode() {
                Code = new Action[] {
                    () => {
                        P.EnableToAddressBus(SubRegister.H0);
                        MemoryModule.AddressRegister.SetFromAddressBus(SubRegister.H0);
                    },
                    () => {
                        P.Increment(8);
                        MemoryModule.DataRegister.EnableToDataBus(SubRegister.W0);
                        Decoder.PopMicrocodeStack();
                    }
                };
            }
        }

    }

    namespace Instructions {

        public class HALT : InstructionBase<HALT> {
            public override string Mnemonic => "HALT";

            public override void BuildMicrocode() {
                Code = new Action[] {
                    StopClock
                };
            }

            public virtual void StopClock() {
                if (!Cpu.PrivilegeFlag) {
                    throw new Exception("Insufficient privilege");
                }

                MB.Clock.Stop();
            }
        }

        public class SETINT : InstructionBase<SETINT> {
            public override string Mnemonic => "SETINT";

            public override void BuildMicrocode() {
                Code = new Action[] {
                    () => {
                        if (!Cpu.PrivilegeFlag) {
                            throw new Exception("Insufficient privilege");
                        }

                        Cpu.InterruptEnabledFlag = true;
                    }
                };
            }
        }

        public class CLRINT : InstructionBase<CLRINT> {
            public override string Mnemonic => "CLRINT";

            public override void BuildMicrocode() {
                Code = new Action[] {
                    () => {
                        if (!Cpu.PrivilegeFlag) {
                            throw new Exception("Insufficient privilege");
                        }

                        Cpu.InterruptEnabledFlag = false;
                    }
                };
            }
        }

        public class SETCRY : InstructionBase<SETCRY> {
            public override string Mnemonic => "SETCRY";

            public override void BuildMicrocode() {
                Code = new Action[] {
                    () => {
                        Cpu.CarryFlag = true;
                    }
                };
            }
        }

        public class CLRCRY : InstructionBase<CLRCRY> {
            public override string Mnemonic => "CLRCRY";

            public override void BuildMicrocode() {
                Code = new Action[] {
                    () => {
                        Cpu.CarryFlag = false;
                    }
                };
            }
        }

        public class NOP : InstructionBase<NOP> {
            public override string Mnemonic => "NOP";

            public override void BuildMicrocode() {
                Code = new Action[] {
                    () => {}
                };
            }
        }

        public class CLR : InstructionBase<CLR> {
            public override string Mnemonic => "CLR";

            public override void BuildMicrocode() {
                Code = new Action[] {
                    () => {
                        P.Increment(1);
                        OperandRegister1.RegData.W0 = 0;
                        OperandRegister1.EnableToDataBus(SubRegister.W0);
                        SrcReg = Decoder.RegisterMap[SrcRegisterFlag >> 4];
                        SrcReg.SetFromDataBus((SubRegister)SrcSubRegisterFlag);
                    }
                };
            }
        }

        public class LD_RegVal_Reg : InstructionBase<LD_RegVal_Reg> {
            public override string Mnemonic => "LD";

            public override void BuildMicrocode() {
                Code = new Action[] {
                    () => {
                        P.Increment(2);
                        SrcReg = Decoder.RegisterMap[SrcRegisterFlag >> 4];
                        SrcReg.EnableToDataBus((SubRegister)SrcSubRegisterFlag);
                        DestReg = Decoder.RegisterMap[DestRegisterFlag >> 4];
                        DestReg.SetFromDataBus((SubRegister)DestSubRegisterFlag);
                    }
                };
            }
        }

        // TODO: This is an experiment in on-the-fly disassembly.
        public class ImmVal_Reg : Instruction {
            public override string ToString() {
                StringBuilder text = new StringBuilder($"${P.RegData.H0:X8}: {$"{Mnemonic}",-42} ; {Opcode:X2} {Decoder.RegData.B1:X2} {Decoder.RegData.B2:X2}");
                return text.ToString();
            }
        }

        public class LD_ImmVal_Reg_Impl : ImmVal_Reg {
            public override string Mnemonic => "LD";

            public override void BuildMicrocode() {
                Code = new Action[] {
                    () => {
                        P.Increment(2);
                        Decoder.ReadAndEnableImmediate(SrcImmSize);
                    },
                    () => {
                        DestReg = Decoder.RegisterMap[DestRegisterFlag >> 4];
                        DestReg.SetFromDataBus((SubRegister)DestSubRegisterFlag);
                    }
                };
            }
        }

        public class LD_ImmVal_Reg : InstructionBase<LD_ImmVal_Reg_Impl> { }

        public class LD_RegAddr_Reg : InstructionBase<LD_RegAddr_Reg> {
            public override string Mnemonic => "LD";

            public override void BuildMicrocode() {
                Code = new Action[] {
                    () => {
                        P.Increment(2);
                        SrcReg = Decoder.RegisterMap[SrcRegisterFlag >> 4];
                        SrcReg.EnableToAddressBus((SubRegister)SrcSubRegisterFlag);
                        MemoryModule.AddressRegister.SetFromAddressBus(SubRegister.W0);
                    },
                    () => {
                        MemoryModule.DataRegister.EnableToDataBus(SubRegister.W0);
                        DestReg = Decoder.RegisterMap[DestRegisterFlag >> 4];
                        DestReg.SetFromDataBus((SubRegister)DestSubRegisterFlag);
                    }
                };
            }
        }

        public class LD_ImmAddr_Reg : InstructionBase<LD_ImmAddr_Reg> {
            public override string Mnemonic => "LD";

            public override void BuildMicrocode() {
                Code = new Action[] {
                    () => {
                        P.Increment(2);
                        Decoder.ReadAndEnableImmediate(SrcImmSize);
                    },
                    () => {
                        MemoryModule.AddressRegister.SetFromDataBus(SubRegister.H0);
                    },
                    () => {
                        MemoryModule.DataRegister.EnableToDataBus(SubRegister.W0);
                        DestReg = Decoder.RegisterMap[DestRegisterFlag >> 4];
                        DestReg.SetFromDataBus((SubRegister)DestSubRegisterFlag);
                    }
                };
            }
        }

        public class ST_RegVal_RegAddr : InstructionBase<ST_RegVal_RegAddr> {
            public override string Mnemonic => "ST";

            public override void BuildMicrocode() {
                Code = new Action[] {
                    () => {
                        P.Increment(2);
                        SrcReg = Decoder.RegisterMap[SrcRegisterFlag >> 4];
                        DestReg = Decoder.RegisterMap[DestRegisterFlag >> 4];
                        DestReg.EnableToAddressBus((SubRegister)DestSubRegisterFlag);
                        MemoryModule.AddressRegister.SetFromAddressBus(SubRegister.W0);
                    },
                    () => {
                        SrcReg.EnableToDataBus((SubRegister)SrcSubRegisterFlag);
                        MemoryModule.DataRegister.SetFromDataBus((SubRegister)SrcSubRegisterFlag);
                    }
                };
            }
        }

        public class MathOperation : Instruction {
            protected virtual byte Operation => Alu.OpCode_ADD;
        }

        public class BinaryMathOperation : MathOperation {
        }

        public class UnaryMathOperation : MathOperation {
            protected override byte Operation => Alu.OpCode_INC;
        }

        public class UnaryMathOperation_Reg : UnaryMathOperation {
            public override void BuildMicrocode() {
                Code = new Action[] {
                    () => {
                        P.Increment(1);
                        SrcReg = Decoder.RegisterMap[SrcRegisterFlag >> 4];
                        SrcReg.EnableToDataBus((SubRegister)SrcSubRegisterFlag);
                        Alu.DestReg.SetFromDataBus(SubRegister.W0);
                    },
                    () => {
                        OperandRegister1.RegData.W0 = (byte)(Operation | AluOpSizeMap[SrcSubRegisterFlag]);
                        OperandRegister1.EnableToDataBus(SubRegister.W0);
                        Alu.SetFromDataBus(SubRegister.W0);
                    },
                    () => {
                        Alu.DestReg.EnableToDataBus(SubRegister.W0);
                        SrcReg.SetFromDataBus((SubRegister)SrcSubRegisterFlag);
                    }
                };
            }
        }


        public class BinaryMathOperation_RegVal_Reg : BinaryMathOperation {
            public override void BuildMicrocode() {
                Code = new Action[] {
                    () => {
                        P.Increment(2);
                        SrcReg = Decoder.RegisterMap[SrcRegisterFlag >> 4];
                        SrcReg.EnableToDataBus((SubRegister)SrcSubRegisterFlag);
                        Alu.SrcReg.SetFromDataBus(SubRegister.W0);
                    },
                    () => {
                        DestReg = Decoder.RegisterMap[DestRegisterFlag >> 4];
                        DestReg.EnableToDataBus((SubRegister)DestSubRegisterFlag);
                        Alu.DestReg.SetFromDataBus(SubRegister.W0);
                    },
                    () => {
                        OperandRegister1.RegData.W0 = (byte)(Operation | AluOpSizeMap[DestSubRegisterFlag]);
                        OperandRegister1.EnableToDataBus(SubRegister.W0);
                        Alu.SetFromDataBus(SubRegister.W0);
                    },
                    () => {
                        Alu.DestReg.EnableToDataBus(SubRegister.W0);
                        DestReg.SetFromDataBus((SubRegister)DestSubRegisterFlag);
                    }
                };
            }
        }

        public class BinaryMathOperation_RegVal_RegAddr : BinaryMathOperation {
            public override void BuildMicrocode() {
                Code = new Action[] {
                    () => {
                        P.Increment(2);
                        SrcReg = Decoder.RegisterMap[SrcRegisterFlag >> 4];
                        SrcReg.EnableToDataBus((SubRegister)SrcSubRegisterFlag);
                        Alu.SrcReg.SetFromDataBus(SubRegister.W0);
                    },
                    () => {
                        DestReg = Decoder.RegisterMap[DestRegisterFlag >> 4];
                        DestReg.EnableToAddressBus((SubRegister)DestSubRegisterFlag);
                        MemoryModule.AddressRegister.SetFromAddressBus(SubRegister.W0);
                    },
                    () => {
                        MemoryModule.DataRegister.EnableToDataBus(SubRegister.W0);
                        Alu.DestReg.SetFromDataBus((SubRegister)DestSubRegisterFlag);
                    },
                    () => {
                        OperandRegister1.RegData.W0 = (byte)(Operation | AluOpSizeMap[(int)ImmSizeSubRegMap[SrcImmSizeFlag]]);
                        OperandRegister1.EnableToDataBus(SubRegister.W0);
                        Alu.SetFromDataBus(SubRegister.W0);
                    }
                };
            }
        }

        public class BinaryMathOperation_ImmVal_Reg : BinaryMathOperation {
            public override void BuildMicrocode() {
                Code = new Action[] {
                    () => {
                        P.Increment(2);
                        Decoder.ReadAndEnableImmediate(SrcImmSize);
                    },
                    () => {
                        Alu.SrcReg.SetFromDataBus(SubRegister.W0);
                    },
                    () => {
                        DestReg = Decoder.RegisterMap[DestRegisterFlag >> 4];
                        DestReg.EnableToDataBus((SubRegister)DestSubRegisterFlag);
                        Alu.DestReg.SetFromDataBus(SubRegister.W0);
                    },
                    () => {
                        OperandRegister1.RegData.W0 = (byte)(Operation | AluOpSizeMap[DestSubRegisterFlag]);
                        OperandRegister1.EnableToDataBus(SubRegister.W0);
                        Alu.SetFromDataBus(SubRegister.W0);
                    },
                    () => {
                        Alu.DestReg.EnableToDataBus(SubRegister.W0);
                        DestReg.SetFromDataBus((SubRegister)DestSubRegisterFlag);
                    }
                };
            }
        }

        public class BinaryMathOperation_ImmVal_RegAddr : BinaryMathOperation {
            public override void BuildMicrocode() {
                Code = new Action[] {
                    () => {
                        P.Increment(2);
                        Decoder.ReadAndEnableImmediate(SrcImmSize);
                    },
                    () => {
                        Alu.SrcReg.SetFromDataBus(SubRegister.W0);
                    },
                    () => {
                        DestReg = Decoder.RegisterMap[DestRegisterFlag >> 4];
                        DestReg.EnableToAddressBus((SubRegister)DestSubRegisterFlag);
                        MemoryModule.AddressRegister.SetFromAddressBus(SubRegister.W0);
                    },
                    () => {
                        MemoryModule.DataRegister.EnableToDataBus(SubRegister.W0);
                        Alu.DestReg.SetFromDataBus((SubRegister)DestSubRegisterFlag);
                    },
                    () => {
                        OperandRegister1.RegData.W0 = (byte)(Operation | AluOpSizeMap[(int)ImmSizeSubRegMap[SrcImmSizeFlag]]);
                        OperandRegister1.EnableToDataBus(SubRegister.W0);
                        Alu.SetFromDataBus(SubRegister.W0);
                    }
                };
            }
        }


        public class BinaryMathOperation_RegAddr_Reg : BinaryMathOperation {
            public override void BuildMicrocode() {
                Code = new Action[] {
                    () => {
                        P.Increment(2);
                        SrcReg = Decoder.RegisterMap[SrcRegisterFlag >> 4];
                        SrcReg.EnableToAddressBus((SubRegister)SrcSubRegisterFlag);
                        MemoryModule.AddressRegister.SetFromAddressBus(SubRegister.W0);
                    },
                    () => {
                        MemoryModule.DataRegister.EnableToDataBus(SubRegister.W0);
                        Alu.SrcReg.SetFromDataBus((SubRegister)SrcSubRegisterFlag);
                    },
                    () => {
                        DestReg = Decoder.RegisterMap[DestRegisterFlag >> 4];
                        DestReg.EnableToDataBus((SubRegister)DestSubRegisterFlag);
                        Alu.DestReg.SetFromDataBus(SubRegister.W0);
                    },
                    () => {
                        OperandRegister1.RegData.W0 = (byte)(Operation | AluOpSizeMap[DestSubRegisterFlag]);
                        OperandRegister1.EnableToDataBus(SubRegister.W0);
                        Alu.SetFromDataBus(SubRegister.W0);
                    },
                    () => {
                        Alu.DestReg.EnableToDataBus(SubRegister.W0);
                        DestReg.SetFromDataBus((SubRegister)DestSubRegisterFlag);
                    }
                };
            }
        }

        public class BinaryMathOperation_ImmAddr_Reg : BinaryMathOperation {
            public override void BuildMicrocode() {
                Code = new Action[] {
                    () => {
                        P.Increment(2);
                        Decoder.ReadAndEnableImmediate(SrcImmSize);
                    },
                    () => {
                        MemoryModule.AddressRegister.SetFromDataBus(SubRegister.W0);
                    },
                    () => {
                        MemoryModule.DataRegister.EnableToAddressBus(ImmSizeSubRegMap[SrcImmSizeFlag]);
                        MemoryModule.AddressRegister.SetFromAddressBus(SubRegister.W0);
                    },
                    () => {
                        MemoryModule.DataRegister.EnableToDataBus(ImmSizeSubRegMap[SrcImmSizeFlag]);
                        Alu.SrcReg.SetFromDataBus(SubRegister.W0);
                    },
                    () => {
                        DestReg = Decoder.RegisterMap[DestRegisterFlag >> 4];
                        DestReg.EnableToDataBus((SubRegister)DestSubRegisterFlag);
                        Alu.DestReg.SetFromDataBus(SubRegister.W0);
                    },
                    () => {
                        OperandRegister1.RegData.W0 = (byte)(Operation | AluOpSizeMap[DestSubRegisterFlag]);
                        OperandRegister1.EnableToDataBus(SubRegister.W0);
                        Alu.SetFromDataBus(SubRegister.W0);
                    },
                    () => {
                        Alu.DestReg.EnableToDataBus(SubRegister.W0);
                        DestReg.SetFromDataBus((SubRegister)DestSubRegisterFlag);
                    }
                };
            }
        }


        public class INC_Operation : UnaryMathOperation_Reg {
            public override string Mnemonic => "INC";
            protected override byte Operation => Alu.OpCode_INC;
        }

        public class INC : InstructionBase<INC_Operation> { 
        }

        public class DEC_Operation : UnaryMathOperation_Reg {
            public override string Mnemonic => "DEC";
            protected override byte Operation => Alu.OpCode_DEC;
        }

        public class DEC : InstructionBase<DEC_Operation> {
        }

        public class ADD_ImmAddr_Reg_Operation : BinaryMathOperation_ImmAddr_Reg {
            public override string Mnemonic => "ADD";
            protected override byte Operation => Alu.OpCode_ADD;
        }

        public class ADD_ImmAddr_Reg : InstructionBase<ADD_ImmAddr_Reg_Operation> {
        }

        public class ADD_ImmVal_Reg_Operation : BinaryMathOperation_ImmVal_Reg {
            public override string Mnemonic => "ADD";
            protected override byte Operation => Alu.OpCode_ADD;
        }

        public class ADD_ImmVal_Reg : InstructionBase<ADD_ImmVal_Reg_Operation> {
        }

        public class ADD_RegAddr_Reg_Operation : BinaryMathOperation_RegAddr_Reg {
            public override string Mnemonic => "ADD";
            protected override byte Operation => Alu.OpCode_ADD;
        }

        public class ADD_RegAddr_Reg : InstructionBase<ADD_RegAddr_Reg_Operation> {
        }

        public class ADD_RegVal_Reg_Operation : BinaryMathOperation_RegVal_Reg {
            public override string Mnemonic => "ADD";
            protected override byte Operation => Alu.OpCode_ADD;
        }

        public class ADD_RegVal_Reg : InstructionBase<ADD_RegVal_Reg_Operation> {
        }


        public class SUB_ImmAddr_Reg_Operation : BinaryMathOperation_ImmAddr_Reg {
            public override string Mnemonic => "SUB";
            protected override byte Operation => Alu.OpCode_SUB;
        }

        public class SUB_ImmAddr_Reg : InstructionBase<SUB_ImmAddr_Reg_Operation> {
        }

        public class SUB_ImmVal_Reg_Operation : BinaryMathOperation_ImmVal_Reg {
            public override string Mnemonic => "SUB";
            protected override byte Operation => Alu.OpCode_SUB;
        }

        public class SUB_ImmVal_Reg : InstructionBase<SUB_ImmVal_Reg_Operation> {
        }

        public class SUB_RegAddr_Reg_Operation : BinaryMathOperation_RegAddr_Reg {
            public override string Mnemonic => "SUB";
            protected override byte Operation => Alu.OpCode_SUB;
        }

        public class SUB_RegAddr_Reg : InstructionBase<SUB_RegAddr_Reg_Operation> {
        }

        public class SUB_RegVal_Reg_Operation : BinaryMathOperation_RegVal_Reg {
            public override string Mnemonic => "SUB";
            protected override byte Operation => Alu.OpCode_SUB;
        }

        public class SUB_RegVal_Reg : InstructionBase<SUB_RegVal_Reg_Operation> {
        }

        public class MUL_ImmAddr_Reg_Operation : BinaryMathOperation_ImmAddr_Reg {
            public override string Mnemonic => "MUL";
            protected override byte Operation => Alu.OpCode_MUL;
        }

        public class MUL_ImmAddr_Reg : InstructionBase<MUL_ImmAddr_Reg_Operation> {
        }

        public class MUL_ImmVal_Reg_Operation : BinaryMathOperation_ImmVal_Reg {
            public override string Mnemonic => "MUL";
            protected override byte Operation => Alu.OpCode_MUL;
        }

        public class MUL_ImmVal_Reg : InstructionBase<MUL_ImmVal_Reg_Operation> {
        }

        public class MUL_RegAddr_Reg_Operation : BinaryMathOperation_RegAddr_Reg {
            public override string Mnemonic => "MUL";
            protected override byte Operation => Alu.OpCode_MUL;
        }

        public class MUL_RegAddr_Reg : InstructionBase<MUL_RegAddr_Reg_Operation> {
        }

        public class MUL_RegVal_Reg_Operation : BinaryMathOperation_RegVal_Reg {
            public override string Mnemonic => "MUL";
            protected override byte Operation => Alu.OpCode_MUL;
        }

        public class MUL_RegVal_Reg : InstructionBase<MUL_RegVal_Reg_Operation> {
        }


        public class DIV_ImmAddr_Reg_Operation : BinaryMathOperation_ImmAddr_Reg {
            public override string Mnemonic => "DIV";
            protected override byte Operation => Alu.OpCode_DIV;
        }

        public class DIV_ImmAddr_Reg : InstructionBase<DIV_ImmAddr_Reg_Operation> {
        }

        public class DIV_ImmVal_Reg_Operation : BinaryMathOperation_ImmVal_Reg {
            public override string Mnemonic => "DIV";
            protected override byte Operation => Alu.OpCode_DIV;
        }

        public class DIV_ImmVal_Reg : InstructionBase<DIV_ImmVal_Reg_Operation> {
        }

        public class DIV_RegAddr_Reg_Operation : BinaryMathOperation_RegAddr_Reg {
            public override string Mnemonic => "DIV";
            protected override byte Operation => Alu.OpCode_DIV;
        }

        public class DIV_RegAddr_Reg : InstructionBase<DIV_RegAddr_Reg_Operation> {
        }

        public class DIV_RegVal_Reg_Operation : BinaryMathOperation_RegVal_Reg {
            public override string Mnemonic => "DIV";
            protected override byte Operation => Alu.OpCode_DIV;
        }

        public class DIV_RegVal_Reg : InstructionBase<DIV_RegVal_Reg_Operation> {
        }


        public class MOD_ImmAddr_Reg_Operation : BinaryMathOperation_ImmAddr_Reg {
            public override string Mnemonic => "MOD";
            protected override byte Operation => Alu.OpCode_MOD;
        }

        public class MOD_ImmAddr_Reg : InstructionBase<MOD_ImmAddr_Reg_Operation> {
        }

        public class MOD_ImmVal_Reg_Operation : BinaryMathOperation_ImmVal_Reg {
            public override string Mnemonic => "MOD";
            protected override byte Operation => Alu.OpCode_MOD;
        }

        public class MOD_ImmVal_Reg : InstructionBase<MOD_ImmVal_Reg_Operation> {
        }

        public class MOD_RegAddr_Reg_Operation : BinaryMathOperation_RegAddr_Reg {
            public override string Mnemonic => "MOD";
            protected override byte Operation => Alu.OpCode_MOD;
        }

        public class MOD_RegAddr_Reg : InstructionBase<MOD_RegAddr_Reg_Operation> {
        }

        public class MOD_RegVal_Reg_Operation : BinaryMathOperation_RegVal_Reg {
            public override string Mnemonic => "MOD";
            protected override byte Operation => Alu.OpCode_MOD;
        }

        public class MOD_RegVal_Reg : InstructionBase<MOD_RegVal_Reg_Operation> {
        }




        public class AND_ImmAddr_Reg_Operation : BinaryMathOperation_ImmAddr_Reg {
            public override string Mnemonic => "AND";
            protected override byte Operation => Alu.OpCode_AND;
        }

        public class AND_ImmAddr_Reg : InstructionBase<AND_ImmAddr_Reg_Operation> {
        }

        public class AND_ImmVal_Reg_Operation : BinaryMathOperation_ImmVal_Reg {
            public override string Mnemonic => "AND";
            protected override byte Operation => Alu.OpCode_AND;
        }

        public class AND_ImmVal_Reg : InstructionBase<AND_ImmVal_Reg_Operation> {
        }

        public class AND_RegAddr_Reg_Operation : BinaryMathOperation_RegAddr_Reg {
            public override string Mnemonic => "AND";
            protected override byte Operation => Alu.OpCode_AND;
        }

        public class AND_RegAddr_Reg : InstructionBase<AND_RegAddr_Reg_Operation> {
        }

        public class AND_RegVal_Reg_Operation : BinaryMathOperation_RegVal_Reg {
            public override string Mnemonic => "AND";
            protected override byte Operation => Alu.OpCode_AND;
        }

        public class AND_RegVal_Reg : InstructionBase<AND_RegVal_Reg_Operation> {
        }




        public class OR_ImmAddr_Reg_Operation : BinaryMathOperation_ImmAddr_Reg {
            public override string Mnemonic => "OR";
            protected override byte Operation => Alu.OpCode_OR;
        }

        public class OR_ImmAddr_Reg : InstructionBase<OR_ImmAddr_Reg_Operation> {
        }

        public class OR_ImmVal_Reg_Operation : BinaryMathOperation_ImmVal_Reg {
            public override string Mnemonic => "OR";
            protected override byte Operation => Alu.OpCode_OR;
        }

        public class OR_ImmVal_Reg : InstructionBase<OR_ImmVal_Reg_Operation> {
        }

        public class OR_RegAddr_Reg_Operation : BinaryMathOperation_RegAddr_Reg {
            public override string Mnemonic => "OR";
            protected override byte Operation => Alu.OpCode_OR;
        }

        public class OR_RegAddr_Reg : InstructionBase<OR_RegAddr_Reg_Operation> {
        }

        public class OR_RegVal_Reg_Operation : BinaryMathOperation_RegVal_Reg {
            public override string Mnemonic => "OR";
            protected override byte Operation => Alu.OpCode_OR;
        }

        public class OR_RegVal_Reg : InstructionBase<OR_RegVal_Reg_Operation> {
        }




        public class NOR_ImmAddr_Reg_Operation : BinaryMathOperation_ImmAddr_Reg {
            public override string Mnemonic => "NOR";
            protected override byte Operation => Alu.OpCode_NOR;
        }

        public class NOR_ImmAddr_Reg : InstructionBase<NOR_ImmAddr_Reg_Operation> {
        }

        public class NOR_ImmVal_Reg_Operation : BinaryMathOperation_ImmVal_Reg {
            public override string Mnemonic => "NOR";
            protected override byte Operation => Alu.OpCode_NOR;
        }

        public class NOR_ImmVal_Reg : InstructionBase<NOR_ImmVal_Reg_Operation> {
        }

        public class NOR_RegAddr_Reg_Operation : BinaryMathOperation_RegAddr_Reg {
            public override string Mnemonic => "NOR";
            protected override byte Operation => Alu.OpCode_NOR;
        }

        public class NOR_RegAddr_Reg : InstructionBase<NOR_RegAddr_Reg_Operation> {
        }

        public class NOR_RegVal_Reg_Operation : BinaryMathOperation_RegVal_Reg {
            public override string Mnemonic => "NOR";
            protected override byte Operation => Alu.OpCode_NOR;
        }

        public class NOR_RegVal_Reg : InstructionBase<NOR_RegVal_Reg_Operation> {
        }




        public class NAND_ImmAddr_Reg_Operation : BinaryMathOperation_ImmAddr_Reg {
            public override string Mnemonic => "NAND";
            protected override byte Operation => Alu.OpCode_NAND;
        }

        public class NAND_ImmAddr_Reg : InstructionBase<NAND_ImmAddr_Reg_Operation> {
        }

        public class NAND_ImmVal_Reg_Operation : BinaryMathOperation_ImmVal_Reg {
            public override string Mnemonic => "NAND";
            protected override byte Operation => Alu.OpCode_NAND;
        }

        public class NAND_ImmVal_Reg : InstructionBase<NAND_ImmVal_Reg_Operation> {
        }

        public class NAND_RegAddr_Reg_Operation : BinaryMathOperation_RegAddr_Reg {
            public override string Mnemonic => "NAND";
            protected override byte Operation => Alu.OpCode_NAND;
        }

        public class NAND_RegAddr_Reg : InstructionBase<NAND_RegAddr_Reg_Operation> {
        }

        public class NAND_RegVal_Reg_Operation : BinaryMathOperation_RegVal_Reg {
            public override string Mnemonic => "NAND";
            protected override byte Operation => Alu.OpCode_NAND;
        }

        public class NAND_RegVal_Reg : InstructionBase<NAND_RegVal_Reg_Operation> {
        }




        public class XOR_ImmAddr_Reg_Operation : BinaryMathOperation_ImmAddr_Reg {
            public override string Mnemonic => "XOR";
            protected override byte Operation => Alu.OpCode_XOR;
        }

        public class XOR_ImmAddr_Reg : InstructionBase<XOR_ImmAddr_Reg_Operation> {
        }

        public class XOR_ImmVal_Reg_Operation : BinaryMathOperation_ImmVal_Reg {
            public override string Mnemonic => "XOR";
            protected override byte Operation => Alu.OpCode_XOR;
        }

        public class XOR_ImmVal_Reg : InstructionBase<XOR_ImmVal_Reg_Operation> {
        }

        public class XOR_RegAddr_Reg_Operation : BinaryMathOperation_RegAddr_Reg {
            public override string Mnemonic => "XOR";
            protected override byte Operation => Alu.OpCode_XOR;
        }

        public class XOR_RegAddr_Reg : InstructionBase<XOR_RegAddr_Reg_Operation> {
        }

        public class XOR_RegVal_Reg_Operation : BinaryMathOperation_RegVal_Reg {
            public override string Mnemonic => "";
            protected override byte Operation => Alu.OpCode_XOR;
        }

        public class XOR_RegVal_Reg : InstructionBase<XOR_RegVal_Reg_Operation> {
        }




        public class SHL_ImmAddr_Reg_Operation : BinaryMathOperation_ImmAddr_Reg {
            public override string Mnemonic => "SHL";
            protected override byte Operation => Alu.OpCode_SHL;
        }

        public class SHL_ImmAddr_Reg : InstructionBase<SHL_ImmAddr_Reg_Operation> {
        }

        public class SHL_ImmVal_Reg_Operation : BinaryMathOperation_ImmVal_Reg {
            public override string Mnemonic => "SHL";
            protected override byte Operation => Alu.OpCode_SHL;
        }

        public class SHL_ImmVal_Reg : InstructionBase<SHL_ImmVal_Reg_Operation> {
        }

        public class SHL_RegAddr_Reg_Operation : BinaryMathOperation_RegAddr_Reg {
            public override string Mnemonic => "SHL";
            protected override byte Operation => Alu.OpCode_SHL;
        }

        public class SHL_RegAddr_Reg : InstructionBase<SHL_RegAddr_Reg_Operation> {
        }

        public class SHL_RegVal_Reg_Operation : BinaryMathOperation_RegVal_Reg {
            public override string Mnemonic => "SHL";
            protected override byte Operation => Alu.OpCode_SHL;
        }

        public class SHL_RegVal_Reg : InstructionBase<SHL_RegVal_Reg_Operation> {
        }




        public class SHR_ImmAddr_Reg_Operation : BinaryMathOperation_ImmAddr_Reg {
            public override string Mnemonic => "SHR";
            protected override byte Operation => Alu.OpCode_SHR;
        }

        public class SHR_ImmAddr_Reg : InstructionBase<SHR_ImmAddr_Reg_Operation> {
        }

        public class SHR_ImmVal_Reg_Operation : BinaryMathOperation_ImmVal_Reg {
            public override string Mnemonic => "SHR";
            protected override byte Operation => Alu.OpCode_SHR;
        }

        public class SHR_ImmVal_Reg : InstructionBase<SHR_ImmVal_Reg_Operation> {
        }

        public class SHR_RegAddr_Reg_Operation : BinaryMathOperation_RegAddr_Reg {
            public override string Mnemonic => "SHR";
            protected override byte Operation => Alu.OpCode_SHR;
        }

        public class SHR_RegAddr_Reg : InstructionBase<SHR_RegAddr_Reg_Operation> {
        }

        public class SHR_RegVal_Reg_Operation : BinaryMathOperation_RegVal_Reg {
            public override string Mnemonic => "SHR";
            protected override byte Operation => Alu.OpCode_SHR;
        }

        public class SHR_RegVal_Reg : InstructionBase<SHR_RegVal_Reg_Operation> {
        }




        public class CMP_ImmAddr_Reg_Operation : BinaryMathOperation_ImmAddr_Reg {
            public override string Mnemonic => "CMP";
            protected override byte Operation => Alu.OpCode_CMP;
        }

        public class CMP_ImmAddr_Reg : InstructionBase<CMP_ImmAddr_Reg_Operation> {
        }

        public class CMP_ImmVal_Reg_Operation : BinaryMathOperation_ImmVal_Reg {
            public override string Mnemonic => "CMP";
            protected override byte Operation => Alu.OpCode_CMP;
        }

        public class CMP_ImmVal_Reg : InstructionBase<CMP_ImmVal_Reg_Operation> {
        }

        public class CMP_RegAddr_Reg_Operation : BinaryMathOperation_RegAddr_Reg {
            public override string Mnemonic => "CMP";
            protected override byte Operation => Alu.OpCode_CMP;
        }

        public class CMP_RegAddr_Reg : InstructionBase<CMP_RegAddr_Reg_Operation> {
        }

        public class CMP_RegVal_Reg_Operation : BinaryMathOperation_RegVal_Reg {
            public override string Mnemonic => "CMP";
            protected override byte Operation => Alu.OpCode_CMP;
        }

        public class CMP_RegVal_Reg : InstructionBase<CMP_RegVal_Reg_Operation> {
        }




        public class TEST_ImmAddr_Reg_Operation : BinaryMathOperation_ImmAddr_Reg {
            public override string Mnemonic => "TEST";
            protected override byte Operation => Alu.OpCode_TEST;
        }

        public class TEST_ImmAddr_Reg : InstructionBase<TEST_ImmAddr_Reg_Operation> {
        }

        public class TEST_ImmVal_Reg_Operation : BinaryMathOperation_ImmVal_Reg {
            public override string Mnemonic => "TEST";
            protected override byte Operation => Alu.OpCode_TEST;
        }

        public class TST_ImmVal_Reg : InstructionBase<TEST_ImmVal_Reg_Operation> {
        }

        public class TEST_RegAddr_Reg_Operation : BinaryMathOperation_RegAddr_Reg {
            public override string Mnemonic => "TEST";
            protected override byte Operation => Alu.OpCode_TEST;
        }

        public class TEST_RegAddr_Reg : InstructionBase<TEST_RegAddr_Reg_Operation> {
        }

        public class TEST_RegVal_Reg_Operation : BinaryMathOperation_RegVal_Reg {
            public override string Mnemonic => "TEST";
            protected override byte Operation => Alu.OpCode_TEST;
        }

        public class TST_RegVal_Reg : InstructionBase<TEST_RegVal_Reg_Operation> {
        }

        public class TSTIND_RegVal_RegAddr_Operation : BinaryMathOperation_RegVal_RegAddr {
            public override string Mnemonic => "TSTIND";
            protected override byte Operation => Alu.OpCode_TEST;
        }

        public class TSTIND_RegVal_RegAddr : InstructionBase<TSTIND_RegVal_RegAddr_Operation> {
        }

        public class TSTIND_ImmVal_RegAddr_Operation : BinaryMathOperation_ImmVal_RegAddr {
            public override string Mnemonic => "TSTIND";
            protected override byte Operation => Alu.OpCode_TEST;
        }

        public class TSTIND_ImmVal_RegAddr : InstructionBase<TSTIND_ImmVal_RegAddr_Operation> {
        }

        public class CMPIND_RegVal_RegAddr_Operation : BinaryMathOperation_RegVal_RegAddr {
            public override string Mnemonic => "CMPIND";
            protected override byte Operation => Alu.OpCode_CMP;
        }

        public class CMPIND_RegVal_RegAddr : InstructionBase<CMPIND_RegVal_RegAddr_Operation> {
        }

        public class CMPIND_ImmVal_RegAddr_Operation : BinaryMathOperation_ImmVal_RegAddr {
            public override string Mnemonic => "CMPIND";
            protected override byte Operation => Alu.OpCode_CMP;
        }

        public class CMPIND_ImmVal_RegAddr : InstructionBase<CMPIND_ImmVal_RegAddr_Operation> {
        }


        public class PUSH_RegVal : InstructionBase<PUSH_RegVal> {
            public override string Mnemonic => "PUSH";

            public override void BuildMicrocode() {
                Code = new Action[] {
                    () => {
                        P.Increment(1);
                        S.Decrement(SizeMap[SrcSubRegisterFlag]);
                        SrcReg = Decoder.RegisterMap[SrcRegisterFlag >> 4];
                    },
                    () => {
                        S.EnableToAddressBus(SubRegister.H0);
                        MemoryModule.AddressRegister.SetFromAddressBus(SubRegister.W0);
                    },
                    () => {
                        SrcReg.EnableToDataBus((SubRegister)SrcSubRegisterFlag);
                        MemoryModule.DataRegister.SetFromDataBus((SubRegister)SrcSubRegisterFlag);
                    }
                };
            }
        }

        public class PUSH_ImmVal : InstructionBase<PUSH_ImmVal> {
            public override string Mnemonic => "PUSH";

            public override void BuildMicrocode() {
                Code = new Action[] {
                    () => {
                        P.Increment(1);
                        S.Decrement(SrcImmSize);
                        Decoder.ReadAndEnableImmediate(SrcImmSize);
                    },
                    () => {
                        OperandRegister1.SetFromDataBus(SubRegister.W0);
                    },
                    () => {
                        S.EnableToAddressBus(SubRegister.H0);
                        MemoryModule.AddressRegister.SetFromAddressBus(SubRegister.W0);
                    },
                    () => {
                        OperandRegister1.EnableToDataBus(ImmSizeSubRegMap[SrcImmSizeFlag]);
                        MemoryModule.DataRegister.SetFromDataBus(ImmSizeSubRegMap[SrcImmSizeFlag]);
                    }
                };
            }
        }

        public class POP_RegVal : InstructionBase<POP_RegVal> {
            public override string Mnemonic => "POP";

            public override void BuildMicrocode() {
                Code = new Action[] {
                    () => {
                        SrcReg = Decoder.RegisterMap[SrcRegisterFlag >> 4];
                        P.Increment(1);
                        S.EnableToAddressBus(SubRegister.H0);
                        MemoryModule.AddressRegister.SetFromAddressBus(SubRegister.W0);
                    },
                    () => {
                        S.Increment(SizeMap[SrcSubRegisterFlag]);
                        MemoryModule.DataRegister.EnableToDataBus(SubRegister.W0);
                        SrcReg.SetFromDataBus((SubRegister)SrcSubRegisterFlag);
                    }
                };
            }
        }

        public class JMP_RegVal : InstructionBase<JMP_RegVal> {
            public override string Mnemonic => "JMP";

            public override void BuildMicrocode() {
                Code = new Action[] {
                    () => {
                        SrcReg = Decoder.RegisterMap[SrcRegisterFlag >> 4];
                        SrcReg.EnableToAddressBus((SubRegister)SrcSubRegisterFlag);
                        P.SetFromAddressBus(SubRegister.H0);
                    }
                };
            }
        }


        public class JMP_ImmVal : InstructionBase<JMP_ImmVal> {
            public override string Mnemonic => "JMP";

            public override void BuildMicrocode() {
                Code = new Action[] {
                    () => {
                        P.Increment(1);
                        Decoder.ReadAndEnableImmediate(SrcImmSize);
                    },
                    () => {
                        P.SetFromDataBus(ImmSizeSubRegMap[SrcImmSizeFlag]);
                    }
                };
            }
        }

        public class JMP_RegAddr : InstructionBase<JMP_RegAddr> {
            public override string Mnemonic => "JMP";

            public override void BuildMicrocode() {
                Code = new Action[] {
                    () => {
                        SrcReg = Decoder.RegisterMap[SrcRegisterFlag >> 4];
                        SrcReg.EnableToAddressBus((SubRegister)SrcSubRegisterFlag);
                        MemoryModule.AddressRegister.SetFromAddressBus(SubRegister.W0);
                    },
                    () => {
                        MemoryModule.DataRegister.EnableToDataBus(SubRegister.W0);
                        P.SetFromAddressBus(SubRegister.H0);
                    }
                };
            }
        }

        public class JMP_ImmAddr : InstructionBase<JMP_ImmAddr> {
            public override string Mnemonic => "JMP";

            public override void BuildMicrocode() {
                Code = new Action[] {
                    () => {
                        P.Increment(1);
                        Decoder.ReadAndEnableImmediate(SrcImmSize);
                    },
                    () => {
                        MemoryModule.AddressRegister.SetFromDataBus(SubRegister.W0);
                    },
                    () => {
                        MemoryModule.DataRegister.EnableToDataBus(SubRegister.W0);
                        P.SetFromDataBus(SubRegister.H0);
                    }
                };
            }
        }


        public class JZ_RegVal : InstructionBase<JZ_RegVal> {
            public override string Mnemonic => "JZ";

            public override void BuildMicrocode() {
                Code = new Action[] {
                    () => {
                        if (Cpu.ZeroFlag) {
                            SrcReg = Decoder.RegisterMap[SrcRegisterFlag >> 4];
                            SrcReg.EnableToAddressBus((SubRegister)SrcSubRegisterFlag);
                            P.SetFromAddressBus(SubRegister.H0);
                        }
                    }
                };
            }
        }


        public class JZ_ImmVal : InstructionBase<JZ_ImmVal> {
            public override string Mnemonic => "JZ";

            public override void BuildMicrocode() {
                Code = new Action[] {
                    () => {
                        P.Increment(1);
                        Decoder.ReadAndEnableImmediate(SrcImmSize);
                    },
                    () => {
                        if (Cpu.ZeroFlag) {
                            P.SetFromDataBus(ImmSizeSubRegMap[SrcImmSizeFlag]);
                        }
                    }
                };
            }
        }

        public class JZ_RegAddr : InstructionBase<JZ_RegAddr> {
            public override string Mnemonic => "JZ";

            public override void BuildMicrocode() {
                Code = new Action[] {
                    () => {
                        if (Cpu.ZeroFlag) {
                            SrcReg = Decoder.RegisterMap[SrcRegisterFlag >> 4];
                            SrcReg.EnableToAddressBus((SubRegister)SrcSubRegisterFlag);
                            MemoryModule.AddressRegister.SetFromAddressBus(SubRegister.W0);
                        }
                    },
                    () => {
                        if (Cpu.ZeroFlag) {
                            MemoryModule.DataRegister.EnableToDataBus(SubRegister.W0);
                            P.SetFromAddressBus(SubRegister.H0);
                        }
                    }
                };
            }
        }

        public class JZ_ImmAddr : InstructionBase<JZ_ImmAddr> {
            public override string Mnemonic => "JZ";

            public override void BuildMicrocode() {
                Code = new Action[] {
                    () => {
                        P.Increment(1);
                        Decoder.ReadAndEnableImmediate(SrcImmSize);
                    },
                    () => {
                        if (Cpu.ZeroFlag) {
                            MemoryModule.AddressRegister.SetFromDataBus(SubRegister.W0);
                        }
                    },
                    () => {
                        if (Cpu.ZeroFlag) {
                            MemoryModule.DataRegister.EnableToDataBus(SubRegister.W0);
                            P.SetFromDataBus(SubRegister.H0);
                        }
                    }
                };
            }
        }


        public class JNZ_RegVal : InstructionBase<JNZ_RegVal> {
            public override string Mnemonic => "JNZ";

            public override void BuildMicrocode() {
                Code = new Action[] {
                    () => {
                        if (!Cpu.ZeroFlag) {
                            SrcReg = Decoder.RegisterMap[SrcRegisterFlag >> 4];
                            SrcReg.EnableToAddressBus((SubRegister)SrcSubRegisterFlag);
                            P.SetFromAddressBus(SubRegister.H0);
                        }
                    }
                };
            }
        }


        public class JNZ_ImmVal : InstructionBase<JNZ_ImmVal> {
            public override string Mnemonic => "JNZ";

            public override void BuildMicrocode() {
                Code = new Action[] {
                    () => {
                        P.Increment(1);
                        Decoder.ReadAndEnableImmediate(SrcImmSize);
                    },
                    () => {
                        if (!Cpu.ZeroFlag) {
                            P.SetFromDataBus(ImmSizeSubRegMap[SrcImmSizeFlag]);
                        }
                    }
                };
            }
        }

        public class JNZ_RegAddr : InstructionBase<JNZ_RegAddr> {
            public override string Mnemonic => "JNZ";

            public override void BuildMicrocode() {
                Code = new Action[] {
                    () => {
                        if (!Cpu.ZeroFlag) {
                            SrcReg = Decoder.RegisterMap[SrcRegisterFlag >> 4];
                            SrcReg.EnableToAddressBus((SubRegister)SrcSubRegisterFlag);
                            MemoryModule.AddressRegister.SetFromAddressBus(SubRegister.W0);
                        }
                    },
                    () => {
                        if (!Cpu.ZeroFlag) {
                            MemoryModule.DataRegister.EnableToDataBus(SubRegister.W0);
                            P.SetFromAddressBus(SubRegister.H0);
                        }
                    }
                };
            }
        }

        public class JNZ_ImmAddr : InstructionBase<JNZ_ImmAddr> {
            public override string Mnemonic => "JNZ";

            public override void BuildMicrocode() {
                Code = new Action[] {
                    () => {
                        P.Increment(1);
                        Decoder.ReadAndEnableImmediate(SrcImmSize);
                    },
                    () => {
                        if (!Cpu.ZeroFlag) {
                            MemoryModule.AddressRegister.SetFromDataBus(SubRegister.W0);
                        }
                    },
                    () => {
                        if (!Cpu.ZeroFlag) {
                            MemoryModule.DataRegister.EnableToDataBus(SubRegister.W0);
                            P.SetFromDataBus(SubRegister.H0);
                        }
                    }
                };
            }
        }

        public class JLT_RegVal : InstructionBase<JLT_RegVal> {
            public override string Mnemonic => "JLT";

            public override void BuildMicrocode() {
                Code = new Action[] {
                    () => {
                        if (Cpu.NegativeFlag != Cpu.OverflowFlag) {
                            SrcReg = Decoder.RegisterMap[SrcRegisterFlag >> 4];
                            SrcReg.EnableToAddressBus((SubRegister)SrcSubRegisterFlag);
                            P.SetFromAddressBus(SubRegister.H0);
                        }
                    }
                };
            }
        }


        public class JLT_ImmVal : InstructionBase<JLT_ImmVal> {
            public override string Mnemonic => "JLT";

            public override void BuildMicrocode() {
                Code = new Action[] {
                    () => {
                        P.Increment(1);
                        Decoder.ReadAndEnableImmediate(SrcImmSize);
                    },
                    () => {
                        if (Cpu.NegativeFlag != Cpu.OverflowFlag) {
                            P.SetFromDataBus(ImmSizeSubRegMap[SrcImmSizeFlag]);
                        }
                    }
                };
            }
        }

        public class JLT_RegAddr : InstructionBase<JLT_RegAddr> {
            public override string Mnemonic => "JLT";

            public override void BuildMicrocode() {
                Code = new Action[] {
                    () => {
                        if (Cpu.NegativeFlag != Cpu.OverflowFlag) {
                            SrcReg = Decoder.RegisterMap[SrcRegisterFlag >> 4];
                            SrcReg.EnableToAddressBus((SubRegister)SrcSubRegisterFlag);
                            MemoryModule.AddressRegister.SetFromAddressBus(SubRegister.W0);
                        }
                    },
                    () => {
                        if (Cpu.NegativeFlag != Cpu.OverflowFlag) {
                            MemoryModule.DataRegister.EnableToDataBus(SubRegister.W0);
                            P.SetFromAddressBus(SubRegister.H0);
                        }
                    }
                };
            }
        }

        public class JLT_ImmAddr : InstructionBase<JLT_ImmAddr> {
            public override string Mnemonic => "JLT";

            public override void BuildMicrocode() {
                Code = new Action[] {
                    () => {
                        P.Increment(1);
                        Decoder.ReadAndEnableImmediate(SrcImmSize);
                    },
                    () => {
                        if (Cpu.NegativeFlag != Cpu.OverflowFlag) {
                            MemoryModule.AddressRegister.SetFromDataBus(SubRegister.W0);
                        }
                    },
                    () => {
                        if (Cpu.NegativeFlag != Cpu.OverflowFlag) {
                            MemoryModule.DataRegister.EnableToDataBus(SubRegister.W0);
                            P.SetFromDataBus(SubRegister.H0);
                        }
                    }
                };
            }
        }


        public class JGT_RegVal : InstructionBase<JGT_RegVal> {
            public override string Mnemonic => "JGT";

            public override void BuildMicrocode() {
                Code = new Action[] {
                    () => {
                        if (!Cpu.ZeroFlag && Cpu.NegativeFlag == Cpu.OverflowFlag) {
                            SrcReg = Decoder.RegisterMap[SrcRegisterFlag >> 4];
                            SrcReg.EnableToAddressBus((SubRegister)SrcSubRegisterFlag);
                            P.SetFromAddressBus(SubRegister.H0);
                        }
                    }
                };
            }
        }


        public class JGT_ImmVal : InstructionBase<JGT_ImmVal> {
            public override string Mnemonic => "JGT";

            public override void BuildMicrocode() {
                Code = new Action[] {
                    () => {
                        P.Increment(1);
                        Decoder.ReadAndEnableImmediate(SrcImmSize);
                    },
                    () => {
                        if (!Cpu.ZeroFlag && Cpu.NegativeFlag == Cpu.OverflowFlag) {
                            P.SetFromDataBus(ImmSizeSubRegMap[SrcImmSizeFlag]);
                        }
                    }
                };
            }
        }

        public class JGT_RegAddr : InstructionBase<JGT_RegAddr> {
            public override string Mnemonic => "JGT";

            public override void BuildMicrocode() {
                Code = new Action[] {
                    () => {
                        if (!Cpu.ZeroFlag && Cpu.NegativeFlag == Cpu.OverflowFlag) {
                            SrcReg = Decoder.RegisterMap[SrcRegisterFlag >> 4];
                            SrcReg.EnableToAddressBus((SubRegister)SrcSubRegisterFlag);
                            MemoryModule.AddressRegister.SetFromAddressBus(SubRegister.W0);
                        }
                    },
                    () => {
                        if (!Cpu.ZeroFlag && Cpu.NegativeFlag == Cpu.OverflowFlag) {
                            MemoryModule.DataRegister.EnableToDataBus(SubRegister.W0);
                            P.SetFromAddressBus(SubRegister.H0);
                        }
                    }
                };
            }
        }

        public class JGT_ImmAddr : InstructionBase<JGT_ImmAddr> {
            public override string Mnemonic => "JGT";

            public override void BuildMicrocode() {
                Code = new Action[] {
                    () => {
                        P.Increment(1);
                        Decoder.ReadAndEnableImmediate(SrcImmSize);
                    },
                    () => {
                        if (!Cpu.ZeroFlag && Cpu.NegativeFlag == Cpu.OverflowFlag) {
                            MemoryModule.AddressRegister.SetFromDataBus(SubRegister.W0);
                        }
                    },
                    () => {
                        if (!Cpu.ZeroFlag && Cpu.NegativeFlag == Cpu.OverflowFlag) {
                            MemoryModule.DataRegister.EnableToDataBus(SubRegister.W0);
                            P.SetFromDataBus(SubRegister.H0);
                        }
                    }
                };
            }
        }


        public class JB_RegVal : InstructionBase<JB_RegVal> {
            public override string Mnemonic => "JB";

            public override void BuildMicrocode() {
                Code = new Action[] {
                    () => {
                        if (Cpu.CarryFlag) {
                            SrcReg = Decoder.RegisterMap[SrcRegisterFlag >> 4];
                            SrcReg.EnableToAddressBus((SubRegister)SrcSubRegisterFlag);
                            P.SetFromAddressBus(SubRegister.H0);
                        }
                    }
                };
            }
        }


        public class JB_ImmVal : InstructionBase<JB_ImmVal> {
            public override string Mnemonic => "JB";

            public override void BuildMicrocode() {
                Code = new Action[] {
                    () => {
                        P.Increment(1);
                        Decoder.ReadAndEnableImmediate(SrcImmSize);
                    },
                    () => {
                        if (Cpu.CarryFlag) {
                            P.SetFromDataBus(ImmSizeSubRegMap[SrcImmSizeFlag]);
                        }
                    }
                };
            }
        }

        public class JB_RegAddr : InstructionBase<JB_RegAddr> {
            public override string Mnemonic => "JB";

            public override void BuildMicrocode() {
                Code = new Action[] {
                    () => {
                        if (Cpu.CarryFlag) {
                            SrcReg = Decoder.RegisterMap[SrcRegisterFlag >> 4];
                            SrcReg.EnableToAddressBus((SubRegister)SrcSubRegisterFlag);
                            MemoryModule.AddressRegister.SetFromAddressBus(SubRegister.W0);
                        }
                    },
                    () => {
                        if (Cpu.CarryFlag) {
                            MemoryModule.DataRegister.EnableToDataBus(SubRegister.W0);
                            P.SetFromAddressBus(SubRegister.H0);
                        }
                    }
                };
            }
        }

        public class JB_ImmAddr : InstructionBase<JB_ImmAddr> {
            public override string Mnemonic => "JB";

            public override void BuildMicrocode() {
                Code = new Action[] {
                    () => {
                        P.Increment(1);
                        Decoder.ReadAndEnableImmediate(SrcImmSize);
                    },
                    () => {
                        if (Cpu.CarryFlag) {
                            MemoryModule.AddressRegister.SetFromDataBus(SubRegister.W0);
                        }
                    },
                    () => {
                        if (Cpu.CarryFlag) {
                            MemoryModule.DataRegister.EnableToDataBus(SubRegister.W0);
                            P.SetFromDataBus(SubRegister.H0);
                        }
                    }
                };
            }
        }


        public class JA_RegVal : InstructionBase<JA_RegVal> {
            public override string Mnemonic => "JA";

            public override void BuildMicrocode() {
                Code = new Action[] {
                    () => {
                        if (!Cpu.CarryFlag && !Cpu.ZeroFlag) {
                            SrcReg = Decoder.RegisterMap[SrcRegisterFlag >> 4];
                            SrcReg.EnableToAddressBus((SubRegister)SrcSubRegisterFlag);
                            P.SetFromAddressBus(SubRegister.H0);
                        }
                    }
                };
            }
        }


        public class JA_ImmVal : InstructionBase<JA_ImmVal> {
            public override string Mnemonic => "JA";

            public override void BuildMicrocode() {
                Code = new Action[] {
                    () => {
                        P.Increment(1);
                        Decoder.ReadAndEnableImmediate(SrcImmSize);
                    },
                    () => {
                        if (!Cpu.CarryFlag && !Cpu.ZeroFlag) {
                            P.SetFromDataBus(ImmSizeSubRegMap[SrcImmSizeFlag]);
                        }
                    }
                };
            }
        }

        public class JA_RegAddr : InstructionBase<JA_RegAddr> {
            public override string Mnemonic => "JA";

            public override void BuildMicrocode() {
                Code = new Action[] {
                    () => {
                        if (!Cpu.CarryFlag && !Cpu.ZeroFlag) {
                            SrcReg = Decoder.RegisterMap[SrcRegisterFlag >> 4];
                            SrcReg.EnableToAddressBus((SubRegister)SrcSubRegisterFlag);
                            MemoryModule.AddressRegister.SetFromAddressBus(SubRegister.W0);
                        }
                    },
                    () => {
                        if (!Cpu.CarryFlag && !Cpu.ZeroFlag) {
                            MemoryModule.DataRegister.EnableToDataBus(SubRegister.W0);
                            P.SetFromAddressBus(SubRegister.H0);
                        }
                    }
                };
            }
        }

        public class JA_ImmAddr : InstructionBase<JA_ImmAddr> {
            public override string Mnemonic => "JA";

            public override void BuildMicrocode() {
                Code = new Action[] {
                    () => {
                        P.Increment(1);
                        Decoder.ReadAndEnableImmediate(SrcImmSize);
                    },
                    () => {
                        if (!Cpu.CarryFlag && !Cpu.ZeroFlag) {
                            MemoryModule.AddressRegister.SetFromDataBus(SubRegister.W0);
                        }
                    },
                    () => {
                        if (!Cpu.CarryFlag && !Cpu.ZeroFlag) {
                            MemoryModule.DataRegister.EnableToDataBus(SubRegister.W0);
                            P.SetFromDataBus(SubRegister.H0);
                        }
                    }
                };
            }
        }


        public class CALL_RegVal : InstructionBase<CALL_RegVal> {
            public override string Mnemonic => "CALL";

            public override void BuildMicrocode() {
                Code = new Action[] {
                    () => {
                        P.Increment(1);
                        S.Decrement(4);
                        S.EnableToAddressBus(SubRegister.H0);
                        MemoryModule.AddressRegister.SetFromAddressBus(SubRegister.W0);
                    },
                    () => {
                        // Push program counter
                        P.EnableToDataBus(SubRegister.H0);
                        MemoryModule.DataRegister.SetFromDataBus(SubRegister.W0);
                    },
                    () => {
                        SrcReg = Decoder.RegisterMap[SrcRegisterFlag >> 4];
                        SrcReg.EnableToAddressBus((SubRegister)SrcSubRegisterFlag);
                        P.SetFromAddressBus(SubRegister.H0);
                    }
                };
            }
        }

        public class CALL_ImmVal : InstructionBase<CALL_ImmVal> {
            public override string Mnemonic => "CALL";

            public override void BuildMicrocode() {
                Code = new Action[] {
                    () => {
                        P.Increment(1);
                        Decoder.ReadAndEnableImmediate(SrcImmSize);
                    },
                    () => {
                        OperandRegister1.SetFromDataBus(SubRegister.W0);
                    },
                    () => {
                        S.Decrement(4);
                        S.EnableToAddressBus(SubRegister.H0);
                        MemoryModule.AddressRegister.SetFromAddressBus(SubRegister.W0);
                    },
                    () => {
                        // Push program counter
                        P.EnableToDataBus(SubRegister.H0);
                        MemoryModule.DataRegister.SetFromDataBus(SubRegister.H0);
                    },
                    () => {
                        OperandRegister1.EnableToAddressBus(ImmSizeSubRegMap[SrcImmSizeFlag]);
                        P.SetFromAddressBus(SubRegister.H0);
                    }
                };
            }
        }

        public class CALL_RegAddr : InstructionBase<CALL_RegAddr> {
            public override string Mnemonic => "CALL";

            public override void BuildMicrocode() {
                Code = new Action[] {
                    () => {
                        P.Increment(1);
                        S.Decrement(4);
                        S.EnableToAddressBus(SubRegister.H0);
                        MemoryModule.AddressRegister.SetFromAddressBus(SubRegister.W0);
                    },
                    () => {
                        // Push program counter
                        P.EnableToDataBus(SubRegister.H0);
                        MemoryModule.DataRegister.SetFromDataBus(SubRegister.H0);
                    },
                    () => {
                        SrcReg = Decoder.RegisterMap[SrcRegisterFlag >> 4];
                        SrcReg.EnableToAddressBus((SubRegister)SrcSubRegisterFlag);
                        MemoryModule.AddressRegister.SetFromAddressBus(SubRegister.W0);
                    },
                    () => {
                        MemoryModule.DataRegister.EnableToDataBus(SubRegister.W0);
                        P.SetFromDataBus(SubRegister.H0);
                    }
                };
            }
        }

        public class RET : InstructionBase<RET> {
            public override string Mnemonic => "RET";

            public override void BuildMicrocode() {
                Code = new Action[] {
                    () => {
                        S.EnableToAddressBus(SubRegister.H0);
                        MemoryModule.AddressRegister.SetFromAddressBus(SubRegister.W0);
                    },
                    () => {
                        S.Increment(4);
                        MemoryModule.DataRegister.EnableToDataBus(SubRegister.W0);
                        P.SetFromDataBus(SubRegister.H0);
                    }
                };
            }
        }

        public class INT_RegVal : InstructionBase<INT_RegVal> {
            public override string Mnemonic => "INT";

            public override void BuildMicrocode() {
                Code = new Action[] {
                    () => {
                        P.Increment(1);
                        Cpu.PrivilegeFlag = true;
                        S.Decrement(8);
                        S.EnableToAddressBus(SubRegister.H0);
                        MemoryModule.AddressRegister.SetFromAddressBus(SubRegister.W0);
                    },
                    () => {
                        F.EnableToDataBus(SubRegister.W0);
                        MemoryModule.DataRegister.SetFromDataBus(SubRegister.W0);
                    },
                    () => {
                        Cpu.PrivilegeFlag = true;
                        S.Decrement(8);
                        S.EnableToAddressBus(SubRegister.H0);
                        MemoryModule.AddressRegister.SetFromAddressBus(SubRegister.W0);
                    },
                    () => {
                        // push CS and PC, which is entire P register
                        P.EnableToDataBus(SubRegister.W0);
                        MemoryModule.DataRegister.SetFromDataBus(SubRegister.W0);
                    },
                    () => {
                        SrcReg = Decoder.RegisterMap[SrcRegisterFlag >> 4];
                        SrcReg.EnableToAddressBus((SubRegister)SrcSubRegisterFlag);
                        OperandRegister1.SetFromDataBus(SubRegister.W0);
                    },
                    () => {
                        OperandRegister1.RegData.H0 = (ushort)(OperandRegister1.RegData.B0 * 4);
                        OperandRegister1.EnableToAddressBus(SubRegister.H0);
                        MemoryModule.AddressRegister.SetFromAddressBus(SubRegister.W0);
                    },
                    () => {
                        MemoryModule.DataRegister.EnableToDataBus(SubRegister.H0);
                        P.SetFromDataBus(SubRegister.W0);
                    }
                };
            }
        }

        public class INT_ImmVal : InstructionBase<INT_ImmVal> {
            public override string Mnemonic => "INT";

            public override void BuildMicrocode() {
                Code = new Action[] {
                    () => {
                        P.Increment(1);
                        Cpu.PrivilegeFlag = true;
                        Decoder.ReadAndEnableImmediate(SrcImmSize);
                    },
                    () => {
                        OperandRegister1.SetFromDataBus(SubRegister.W0);
                    },
                    () => {
                        S.Decrement(8);
                        S.EnableToAddressBus(SubRegister.H0);
                        MemoryModule.AddressRegister.SetFromAddressBus(SubRegister.W0);
                    },
                    () => {
                        F.EnableToDataBus(SubRegister.W0);
                        MemoryModule.DataRegister.SetFromDataBus(SubRegister.W0);
                    },
                    () => {
                        Cpu.PrivilegeFlag = true;
                        S.Decrement(8);
                        S.EnableToAddressBus(SubRegister.H0);
                        MemoryModule.AddressRegister.SetFromAddressBus(SubRegister.W0);
                    },
                    () => {
                        P.EnableToDataBus(SubRegister.W0);
                        MemoryModule.DataRegister.SetFromDataBus(SubRegister.W0);
                    },
                    () => {
                        OperandRegister1.RegData.H0 = (ushort)(OperandRegister1.RegData.B0 * 4);
                        OperandRegister1.EnableToAddressBus(SubRegister.H0);
                        MemoryModule.AddressRegister.SetFromAddressBus(SubRegister.W0);
                    },
                    () => {
                        MemoryModule.DataRegister.EnableToDataBus(SubRegister.H0);
                        P.SetFromDataBus(SubRegister.W0);
                    }
                };
            }
        }

        public class IRET : InstructionBase<IRET> {
            public override string Mnemonic => "IRET";

            public override void BuildMicrocode() {
                Code = new Action[] {
                    () => {
                        if (!Cpu.PrivilegeFlag) {
                            throw new Exception("Insufficient privilege");
                        }

                        S.EnableToAddressBus(SubRegister.H0);
                        MemoryModule.AddressRegister.SetFromAddressBus(SubRegister.W0);
                    },
                    () => {
                        S.Increment(8);
                        MemoryModule.DataRegister.EnableToDataBus(SubRegister.W0);
                        P.SetFromDataBus(SubRegister.W0);
                    },
                    () => {
                        S.EnableToAddressBus(SubRegister.H0);
                        MemoryModule.AddressRegister.SetFromAddressBus(SubRegister.W0);
                    },
                    () => {
                        S.Increment(8);
                        MemoryModule.DataRegister.EnableToDataBus(SubRegister.W0);
                        F.SetFromDataBus(SubRegister.W0);
                    }
                };
            }
        }

        public class OUT_RegVal_Imm : InstructionBase<OUT_RegVal_Imm> {
            public override string Mnemonic => "OUT";

            public override void BuildMicrocode() {
                Code = new Action[] {
                    () => {
                        P.Increment(2);

                        if (!Cpu.PrivilegeFlag) {
                            throw new Exception("Insufficient privilege");
                        }

                        SrcReg = Decoder.RegisterMap[SrcRegisterFlag >> 4];
                        Decoder.ReadAndEnableImmediate(DestImmSize);
                    },
                    () => {
                        OperandRegister1.SetFromDataBus(SubRegister.W0);
                    },
                    () => {
                        OperandRegister1.EnableToAddressBus(SubRegister.W0);
                        MB.SetPortAddress(OperandRegister1.RegData.W0);
                    },
                    () => {
                        SrcReg.EnableToIOBus(SubRegister.W0);
                        MB.SetPortIO(OperandRegister1.RegData.W0);
                    }
                };
            }
        }

        public class OUTR_RegVal_Reg : InstructionBase<OUTR_RegVal_Reg> {
            public override string Mnemonic => "OUTR";

            public override void BuildMicrocode() {
                Code = new Action[] {
                    () => {
                        P.Increment(2);

                        if (!Cpu.PrivilegeFlag) {
                            throw new Exception("Insufficient privilege");
                        }

                        SrcReg = Decoder.RegisterMap[SrcRegisterFlag >> 4];
                        DestReg = Decoder.RegisterMap[DestRegisterFlag >> 4];
                    },
                    () => {
                        DestReg.EnableToAddressBus(SubRegister.W0);
                        MB.SetPortAddress(DestReg.RegData.W0);
                    },
                    () => {
                        SrcReg.EnableToIOBus(SubRegister.W0);
                        MB.SetPortIO(DestReg.RegData.W0);
                    }
                };
            }
        }

        public class OUTR_ImmVal_Reg : InstructionBase<OUTR_ImmVal_Reg> {
            public override string Mnemonic => "OUTR";

            public override void BuildMicrocode() {
                Code = new Action[] {
                    () => {
                        if (!Cpu.PrivilegeFlag) {
                            throw new Exception("Insufficient privilege");
                        }

                        DestReg = Decoder.RegisterMap[DestRegisterFlag >> 4];
                        P.Increment(2);
                        Decoder.ReadAndEnableImmediate(SrcImmSize);
                    },
                    () => {
                        OperandRegister1.SetFromDataBus(SubRegister.W0);
                    },
                    () => {
                        DestReg.EnableToAddressBus(SubRegister.W0);
                        MB.SetPortAddress(DestReg.RegData.W0);
                    },
                    () => {
                        OperandRegister1.EnableToIOBus(ImmSizeSubRegMap[SrcImmSizeFlag]);
                        MB.SetPortIO(DestReg.RegData.W0);
                    }
                };
            }
        }

        public class IN_RegVal_Reg : InstructionBase<IN_RegVal_Reg> {
            public override string Mnemonic => "IN";

            public override void BuildMicrocode() {
                Code = new Action[] {
                    () => {
                        P.Increment(2);

                        if (!Cpu.PrivilegeFlag) {
                            throw new Exception("Insufficient privilege");
                        }

                        SrcReg = Decoder.RegisterMap[SrcRegisterFlag >> 4];
                        DestReg = Decoder.RegisterMap[DestRegisterFlag >> 4];
                    },
                    () => {
                        DestReg.EnableToAddressBus(SubRegister.W0);
                        MB.SetPortAddress(DestReg.RegData.W0);
                    },
                    () => {
                        MB.EnablePortIO(DestReg.RegData.W0);
                        SrcReg.SetFromIOBus(SubRegister.W0);
                    }
                };
            }
        }

        public class IN_ImmVal_Reg : InstructionBase<IN_ImmVal_Reg> {
            public override string Mnemonic => "IN";

            public override void BuildMicrocode() {
                Code = new Action[] {
                    () => {
                        P.Increment(2);

                        if (!Cpu.PrivilegeFlag) {
                            throw new Exception("Insufficient privilege");
                        }

                        DestReg = Decoder.RegisterMap[DestRegisterFlag >> 4];
                        Decoder.ReadAndEnableImmediate(SrcImmSize);
                    },
                    () => {
                        OperandRegister1.SetFromDataBus(SubRegister.W0);
                    },
                    () => {
                        OperandRegister1.EnableToAddressBus(SubRegister.W0);
                        MB.SetPortAddress(OperandRegister1.RegData.W0);
                    },
                    () => {
                        MB.EnablePortIO(OperandRegister1.RegData.W0);
                        DestReg.SetFromIOBus((SubRegister)DestSubRegisterFlag);
                    }
                };
            }
        }




        /*

        public class NAME : InstructionBase<NAME> {
            public override string Mnemonic => "";

            public override void BuildMicrocode() {
                Code = new Action[] {

                };
            }
        }
        */
    }
}
