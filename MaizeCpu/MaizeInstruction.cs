using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
// using System.Runtime.Remoting.Messaging;
using System.Threading;
using Tortilla;

namespace Maize {
    namespace Exceptions {
        public class BadOpcode : InstructionBase<BadOpcode> {
            public override void BuildMicrocode() {
                Code = new Action[] {
                    () => {
                        MB.RaiseInterrupt(0x06);
                    }
                };
            }
        }

    }

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
                        OperandRegister1.SetFromDataBus(SubRegister.W0);
                    },
                    () => {
                        P.Increment(1);
                        var opcode = OperandRegister1.RegData.B0;
                        var instruction = Decoder.InstructionArray[opcode];
                        Decoder.JumpTo(instruction);
                        // InstructionRead?.Invoke(this, Tuple.Create(this.Value, MB.Cpu.PC.Value - 1));
                    }
                };
            }
        }

        public class DecoderValueAssign : InstructionBase<DecoderValueAssign> {
            public override void BuildMicrocode() {
                Code = new Action[] {
                    () => {
                        // Nothing to do here yet
                    }
                };
            }
        }

        public class DecoderValueAssignDebug : InstructionBase<DecoderValueAssign> {
            public override void BuildMicrocode() {
                Code = new Action[] {
                    () => {
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

        public class STI : InstructionBase<STI> {
            public override string Mnemonic => "STI";

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

        public class CLI : InstructionBase<CLI> {
            public override string Mnemonic => "CLI";

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

        public class STC : InstructionBase<STC> {
            public override string Mnemonic => "STC";

            public override void BuildMicrocode() {
                Code = new Action[] {
                    () => {
                        Cpu.CarryFlag = true;
                    }
                };
            }
        }

        public class CLC : InstructionBase<CLC> {
            public override string Mnemonic => "CLC";

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
                        OperandRegister2.RegData.W0 = 0;
                        OperandRegister2.EnableToDataBus(SubRegister.W0);
                        SrcReg = Decoder.RegisterMap[SrcRegisterFlag >> 4];
                        SrcReg.SetFromDataBus((SubRegister)SrcSubRegisterFlag);
                        Cpu.ZeroFlag = true;
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
        public class ImmVal_Reg : MaizeInstruction {
            public override string ToString() {
                StringBuilder text = new StringBuilder($"${P.RegData.H0:X8}: {$"{Mnemonic}",-42} ; {Opcode:X2} {OperandRegister1.RegData.B1:X2} {OperandRegister1.RegData.B2:X2}");
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

        public class STIM : InstructionBase<STIM> {
            public override string Mnemonic => "STIM";

            public override void BuildMicrocode() {
                Code = new Action[] {
                    () => {
                        P.Increment(2);
                        SrcReg = Decoder.RegisterMap[SrcRegisterFlag >> 4];
                        SrcReg.EnableToDataBus((SubRegister)SrcSubRegisterFlag);
                        OperandRegister2.SetFromDataBus(SubRegister.W0);
                    },
                    () => {
                        Decoder.ReadAndEnableImmediate(DestImmSize);
                    },
                    () => {
                        OperandRegister3.RegData.W0 = 0;
                        OperandRegister3.SetFromDataBus(ImmSizeSubRegMap[DestImmSizeFlag]);
                    },
                    () => {
                        OperandRegister2.EnableToDataBus(ImmSizeSubRegMap[DestImmSizeFlag]);
                        OperandRegister3.EnableToAddressBus(SubRegister.W0);
                        MemoryModule.AddressRegister.SetFromAddressBus(SubRegister.W0);
                        MemoryModule.DataRegister.SetFromDataBus(ImmSizeSubRegMap[DestImmSizeFlag]);
                    }
                };
            }
        }

        public class ST : InstructionBase<ST> {
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

        public class MathOperation : MaizeInstruction {
            protected virtual byte Operation => MaizeAlu.OpCode_ADD;
        }

        public class BinaryMathOperation : MathOperation {
        }

        public class UnaryMathOperation : MathOperation {
            protected override byte Operation => MaizeAlu.OpCode_INC;
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
                        OperandRegister2.RegData.W0 = (byte)(Operation | AluOpSizeMap[SrcSubRegisterFlag]);
                        OperandRegister2.EnableToDataBus(SubRegister.W0);
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
                        OperandRegister2.RegData.W0 = (byte)(Operation | AluOpSizeMap[DestSubRegisterFlag]);
                        OperandRegister2.EnableToDataBus(SubRegister.W0);
                        Alu.SetFromDataBus(SubRegister.W0);
                    },
                    () => {
                        Alu.DestReg.EnableToDataBus(SubRegister.W0);
                        DestReg.SetFromDataBus((SubRegister)DestSubRegisterFlag);
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
                        OperandRegister2.RegData.W0 = (byte)(Operation | AluOpSizeMap[DestSubRegisterFlag]);
                        OperandRegister2.EnableToDataBus(SubRegister.W0);
                        Alu.SetFromDataBus(SubRegister.W0);
                    },
                    () => {
                        Alu.DestReg.EnableToDataBus(SubRegister.W0);
                        DestReg.SetFromDataBus((SubRegister)DestSubRegisterFlag);
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
                        OperandRegister2.RegData.W0 = (byte)(Operation | AluOpSizeMap[DestSubRegisterFlag]);
                        OperandRegister2.EnableToDataBus(SubRegister.W0);
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
                        OperandRegister2.RegData.W0 = (byte)(Operation | AluOpSizeMap[DestSubRegisterFlag]);
                        OperandRegister2.EnableToDataBus(SubRegister.W0);
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
            protected override byte Operation => MaizeAlu.OpCode_INC;
        }

        public class INC : InstructionBase<INC_Operation> { 
        }

        public class DEC_Operation : UnaryMathOperation_Reg {
            public override string Mnemonic => "DEC";
            protected override byte Operation => MaizeAlu.OpCode_DEC;
        }

        public class DEC : InstructionBase<DEC_Operation> {
        }

        public class ADD_ImmAddr_Reg_Operation : BinaryMathOperation_ImmAddr_Reg {
            public override string Mnemonic => "ADD";
            protected override byte Operation => MaizeAlu.OpCode_ADD;
        }

        public class ADD_ImmAddr_Reg : InstructionBase<ADD_ImmAddr_Reg_Operation> {
        }

        public class ADD_ImmVal_Reg_Operation : BinaryMathOperation_ImmVal_Reg {
            public override string Mnemonic => "ADD";
            protected override byte Operation => MaizeAlu.OpCode_ADD;
        }

        public class ADD_ImmVal_Reg : InstructionBase<ADD_ImmVal_Reg_Operation> {
        }

        public class ADD_RegAddr_Reg_Operation : BinaryMathOperation_RegAddr_Reg {
            public override string Mnemonic => "ADD";
            protected override byte Operation => MaizeAlu.OpCode_ADD;
        }

        public class ADD_RegAddr_Reg : InstructionBase<ADD_RegAddr_Reg_Operation> {
        }

        public class ADD_RegVal_Reg_Operation : BinaryMathOperation_RegVal_Reg {
            public override string Mnemonic => "ADD";
            protected override byte Operation => MaizeAlu.OpCode_ADD;
        }

        public class ADD_RegVal_Reg : InstructionBase<ADD_RegVal_Reg_Operation> {
        }


        public class SUB_ImmAddr_Reg_Operation : BinaryMathOperation_ImmAddr_Reg {
            public override string Mnemonic => "SUB";
            protected override byte Operation => MaizeAlu.OpCode_SUB;
        }

        public class SUB_ImmAddr_Reg : InstructionBase<SUB_ImmAddr_Reg_Operation> {
        }

        public class SUB_ImmVal_Reg_Operation : BinaryMathOperation_ImmVal_Reg {
            public override string Mnemonic => "SUB";
            protected override byte Operation => MaizeAlu.OpCode_SUB;
        }

        public class SUB_ImmVal_Reg : InstructionBase<SUB_ImmVal_Reg_Operation> {
        }

        public class SUB_RegAddr_Reg_Operation : BinaryMathOperation_RegAddr_Reg {
            public override string Mnemonic => "SUB";
            protected override byte Operation => MaizeAlu.OpCode_SUB;
        }

        public class SUB_RegAddr_Reg : InstructionBase<SUB_RegAddr_Reg_Operation> {
        }

        public class SUB_RegVal_Reg_Operation : BinaryMathOperation_RegVal_Reg {
            public override string Mnemonic => "SUB";
            protected override byte Operation => MaizeAlu.OpCode_SUB;
        }

        public class SUB_RegVal_Reg : InstructionBase<SUB_RegVal_Reg_Operation> {
        }

        public class MUL_ImmAddr_Reg_Operation : BinaryMathOperation_ImmAddr_Reg {
            public override string Mnemonic => "MUL";
            protected override byte Operation => MaizeAlu.OpCode_MUL;
        }

        public class MUL_ImmAddr_Reg : InstructionBase<MUL_ImmAddr_Reg_Operation> {
        }

        public class MUL_ImmVal_Reg_Operation : BinaryMathOperation_ImmVal_Reg {
            public override string Mnemonic => "MUL";
            protected override byte Operation => MaizeAlu.OpCode_MUL;
        }

        public class MUL_ImmVal_Reg : InstructionBase<MUL_ImmVal_Reg_Operation> {
        }

        public class MUL_RegAddr_Reg_Operation : BinaryMathOperation_RegAddr_Reg {
            public override string Mnemonic => "MUL";
            protected override byte Operation => MaizeAlu.OpCode_MUL;
        }

        public class MUL_RegAddr_Reg : InstructionBase<MUL_RegAddr_Reg_Operation> {
        }

        public class MUL_RegVal_Reg_Operation : BinaryMathOperation_RegVal_Reg {
            public override string Mnemonic => "MUL";
            protected override byte Operation => MaizeAlu.OpCode_MUL;
        }

        public class MUL_RegVal_Reg : InstructionBase<MUL_RegVal_Reg_Operation> {
        }


        public class DIV_ImmAddr_Reg_Operation : BinaryMathOperation_ImmAddr_Reg {
            public override string Mnemonic => "DIV";
            protected override byte Operation => MaizeAlu.OpCode_DIV;
        }

        public class DIV_ImmAddr_Reg : InstructionBase<DIV_ImmAddr_Reg_Operation> {
        }

        public class DIV_ImmVal_Reg_Operation : BinaryMathOperation_ImmVal_Reg {
            public override string Mnemonic => "DIV";
            protected override byte Operation => MaizeAlu.OpCode_DIV;
        }

        public class DIV_ImmVal_Reg : InstructionBase<DIV_ImmVal_Reg_Operation> {
        }

        public class DIV_RegAddr_Reg_Operation : BinaryMathOperation_RegAddr_Reg {
            public override string Mnemonic => "DIV";
            protected override byte Operation => MaizeAlu.OpCode_DIV;
        }

        public class DIV_RegAddr_Reg : InstructionBase<DIV_RegAddr_Reg_Operation> {
        }

        public class DIV_RegVal_Reg_Operation : BinaryMathOperation_RegVal_Reg {
            public override string Mnemonic => "DIV";
            protected override byte Operation => MaizeAlu.OpCode_DIV;
        }

        public class DIV_RegVal_Reg : InstructionBase<DIV_RegVal_Reg_Operation> {
        }


        public class MOD_ImmAddr_Reg_Operation : BinaryMathOperation_ImmAddr_Reg {
            public override string Mnemonic => "MOD";
            protected override byte Operation => MaizeAlu.OpCode_MOD;
        }

        public class MOD_ImmAddr_Reg : InstructionBase<MOD_ImmAddr_Reg_Operation> {
        }

        public class MOD_ImmVal_Reg_Operation : BinaryMathOperation_ImmVal_Reg {
            public override string Mnemonic => "MOD";
            protected override byte Operation => MaizeAlu.OpCode_MOD;
        }

        public class MOD_ImmVal_Reg : InstructionBase<MOD_ImmVal_Reg_Operation> {
        }

        public class MOD_RegAddr_Reg_Operation : BinaryMathOperation_RegAddr_Reg {
            public override string Mnemonic => "MOD";
            protected override byte Operation => MaizeAlu.OpCode_MOD;
        }

        public class MOD_RegAddr_Reg : InstructionBase<MOD_RegAddr_Reg_Operation> {
        }

        public class MOD_RegVal_Reg_Operation : BinaryMathOperation_RegVal_Reg {
            public override string Mnemonic => "MOD";
            protected override byte Operation => MaizeAlu.OpCode_MOD;
        }

        public class MOD_RegVal_Reg : InstructionBase<MOD_RegVal_Reg_Operation> {
        }




        public class AND_ImmAddr_Reg_Operation : BinaryMathOperation_ImmAddr_Reg {
            public override string Mnemonic => "AND";
            protected override byte Operation => MaizeAlu.OpCode_AND;
        }

        public class AND_ImmAddr_Reg : InstructionBase<AND_ImmAddr_Reg_Operation> {
        }

        public class AND_ImmVal_Reg_Operation : BinaryMathOperation_ImmVal_Reg {
            public override string Mnemonic => "AND";
            protected override byte Operation => MaizeAlu.OpCode_AND;
        }

        public class AND_ImmVal_Reg : InstructionBase<AND_ImmVal_Reg_Operation> {
        }

        public class AND_RegAddr_Reg_Operation : BinaryMathOperation_RegAddr_Reg {
            public override string Mnemonic => "AND";
            protected override byte Operation => MaizeAlu.OpCode_AND;
        }

        public class AND_RegAddr_Reg : InstructionBase<AND_RegAddr_Reg_Operation> {
        }

        public class AND_RegVal_Reg_Operation : BinaryMathOperation_RegVal_Reg {
            public override string Mnemonic => "AND";
            protected override byte Operation => MaizeAlu.OpCode_AND;
        }

        public class AND_RegVal_Reg : InstructionBase<AND_RegVal_Reg_Operation> {
        }




        public class OR_ImmAddr_Reg_Operation : BinaryMathOperation_ImmAddr_Reg {
            public override string Mnemonic => "OR";
            protected override byte Operation => MaizeAlu.OpCode_OR;
        }

        public class OR_ImmAddr_Reg : InstructionBase<OR_ImmAddr_Reg_Operation> {
        }

        public class OR_ImmVal_Reg_Operation : BinaryMathOperation_ImmVal_Reg {
            public override string Mnemonic => "OR";
            protected override byte Operation => MaizeAlu.OpCode_OR;
        }

        public class OR_ImmVal_Reg : InstructionBase<OR_ImmVal_Reg_Operation> {
        }

        public class OR_RegAddr_Reg_Operation : BinaryMathOperation_RegAddr_Reg {
            public override string Mnemonic => "OR";
            protected override byte Operation => MaizeAlu.OpCode_OR;
        }

        public class OR_RegAddr_Reg : InstructionBase<OR_RegAddr_Reg_Operation> {
        }

        public class OR_RegVal_Reg_Operation : BinaryMathOperation_RegVal_Reg {
            public override string Mnemonic => "OR";
            protected override byte Operation => MaizeAlu.OpCode_OR;
        }

        public class OR_RegVal_Reg : InstructionBase<OR_RegVal_Reg_Operation> {
        }




        public class NOR_ImmAddr_Reg_Operation : BinaryMathOperation_ImmAddr_Reg {
            public override string Mnemonic => "NOR";
            protected override byte Operation => MaizeAlu.OpCode_NOR;
        }

        public class NOR_ImmAddr_Reg : InstructionBase<NOR_ImmAddr_Reg_Operation> {
        }

        public class NOR_ImmVal_Reg_Operation : BinaryMathOperation_ImmVal_Reg {
            public override string Mnemonic => "NOR";
            protected override byte Operation => MaizeAlu.OpCode_NOR;
        }

        public class NOR_ImmVal_Reg : InstructionBase<NOR_ImmVal_Reg_Operation> {
        }

        public class NOR_RegAddr_Reg_Operation : BinaryMathOperation_RegAddr_Reg {
            public override string Mnemonic => "NOR";
            protected override byte Operation => MaizeAlu.OpCode_NOR;
        }

        public class NOR_RegAddr_Reg : InstructionBase<NOR_RegAddr_Reg_Operation> {
        }

        public class NOR_RegVal_Reg_Operation : BinaryMathOperation_RegVal_Reg {
            public override string Mnemonic => "NOR";
            protected override byte Operation => MaizeAlu.OpCode_NOR;
        }

        public class NOR_RegVal_Reg : InstructionBase<NOR_RegVal_Reg_Operation> {
        }




        public class NAND_ImmAddr_Reg_Operation : BinaryMathOperation_ImmAddr_Reg {
            public override string Mnemonic => "NAND";
            protected override byte Operation => MaizeAlu.OpCode_NAND;
        }

        public class NAND_ImmAddr_Reg : InstructionBase<NAND_ImmAddr_Reg_Operation> {
        }

        public class NAND_ImmVal_Reg_Operation : BinaryMathOperation_ImmVal_Reg {
            public override string Mnemonic => "NAND";
            protected override byte Operation => MaizeAlu.OpCode_NAND;
        }

        public class NAND_ImmVal_Reg : InstructionBase<NAND_ImmVal_Reg_Operation> {
        }

        public class NAND_RegAddr_Reg_Operation : BinaryMathOperation_RegAddr_Reg {
            public override string Mnemonic => "NAND";
            protected override byte Operation => MaizeAlu.OpCode_NAND;
        }

        public class NAND_RegAddr_Reg : InstructionBase<NAND_RegAddr_Reg_Operation> {
        }

        public class NAND_RegVal_Reg_Operation : BinaryMathOperation_RegVal_Reg {
            public override string Mnemonic => "NAND";
            protected override byte Operation => MaizeAlu.OpCode_NAND;
        }

        public class NAND_RegVal_Reg : InstructionBase<NAND_RegVal_Reg_Operation> {
        }




        public class XOR_ImmAddr_Reg_Operation : BinaryMathOperation_ImmAddr_Reg {
            public override string Mnemonic => "XOR";
            protected override byte Operation => MaizeAlu.OpCode_XOR;
        }

        public class XOR_ImmAddr_Reg : InstructionBase<XOR_ImmAddr_Reg_Operation> {
        }

        public class XOR_ImmVal_Reg_Operation : BinaryMathOperation_ImmVal_Reg {
            public override string Mnemonic => "XOR";
            protected override byte Operation => MaizeAlu.OpCode_XOR;
        }

        public class XOR_ImmVal_Reg : InstructionBase<XOR_ImmVal_Reg_Operation> {
        }

        public class XOR_RegAddr_Reg_Operation : BinaryMathOperation_RegAddr_Reg {
            public override string Mnemonic => "XOR";
            protected override byte Operation => MaizeAlu.OpCode_XOR;
        }

        public class XOR_RegAddr_Reg : InstructionBase<XOR_RegAddr_Reg_Operation> {
        }

        public class XOR_RegVal_Reg_Operation : BinaryMathOperation_RegVal_Reg {
            public override string Mnemonic => "";
            protected override byte Operation => MaizeAlu.OpCode_XOR;
        }

        public class XOR_RegVal_Reg : InstructionBase<XOR_RegVal_Reg_Operation> {
        }




        public class SHL_ImmAddr_Reg_Operation : BinaryMathOperation_ImmAddr_Reg {
            public override string Mnemonic => "SHL";
            protected override byte Operation => MaizeAlu.OpCode_SHL;
        }

        public class SHL_ImmAddr_Reg : InstructionBase<SHL_ImmAddr_Reg_Operation> {
        }

        public class SHL_ImmVal_Reg_Operation : BinaryMathOperation_ImmVal_Reg {
            public override string Mnemonic => "SHL";
            protected override byte Operation => MaizeAlu.OpCode_SHL;
        }

        public class SHL_ImmVal_Reg : InstructionBase<SHL_ImmVal_Reg_Operation> {
        }

        public class SHL_RegAddr_Reg_Operation : BinaryMathOperation_RegAddr_Reg {
            public override string Mnemonic => "SHL";
            protected override byte Operation => MaizeAlu.OpCode_SHL;
        }

        public class SHL_RegAddr_Reg : InstructionBase<SHL_RegAddr_Reg_Operation> {
        }

        public class SHL_RegVal_Reg_Operation : BinaryMathOperation_RegVal_Reg {
            public override string Mnemonic => "SHL";
            protected override byte Operation => MaizeAlu.OpCode_SHL;
        }

        public class SHL_RegVal_Reg : InstructionBase<SHL_RegVal_Reg_Operation> {
        }




        public class SHR_ImmAddr_Reg_Operation : BinaryMathOperation_ImmAddr_Reg {
            public override string Mnemonic => "SHR";
            protected override byte Operation => MaizeAlu.OpCode_SHR;
        }

        public class SHR_ImmAddr_Reg : InstructionBase<SHR_ImmAddr_Reg_Operation> {
        }

        public class SHR_ImmVal_Reg_Operation : BinaryMathOperation_ImmVal_Reg {
            public override string Mnemonic => "SHR";
            protected override byte Operation => MaizeAlu.OpCode_SHR;
        }

        public class SHR_ImmVal_Reg : InstructionBase<SHR_ImmVal_Reg_Operation> {
        }

        public class SHR_RegAddr_Reg_Operation : BinaryMathOperation_RegAddr_Reg {
            public override string Mnemonic => "SHR";
            protected override byte Operation => MaizeAlu.OpCode_SHR;
        }

        public class SHR_RegAddr_Reg : InstructionBase<SHR_RegAddr_Reg_Operation> {
        }

        public class SHR_RegVal_Reg_Operation : BinaryMathOperation_RegVal_Reg {
            public override string Mnemonic => "SHR";
            protected override byte Operation => MaizeAlu.OpCode_SHR;
        }

        public class SHR_RegVal_Reg : InstructionBase<SHR_RegVal_Reg_Operation> {
        }




        public class CMP_ImmAddr_Reg_Operation : BinaryMathOperation_ImmAddr_Reg {
            public override string Mnemonic => "CMP";
            protected override byte Operation => MaizeAlu.OpCode_CMP;
        }

        public class CMP_ImmAddr_Reg : InstructionBase<CMP_ImmAddr_Reg_Operation> {
        }

        public class CMP_ImmVal_Reg_Operation : BinaryMathOperation_ImmVal_Reg {
            public override string Mnemonic => "CMP";
            protected override byte Operation => MaizeAlu.OpCode_CMP;
        }

        public class CMP_ImmVal_Reg : InstructionBase<CMP_ImmVal_Reg_Operation> {
        }

        public class CMP_RegAddr_Reg_Operation : BinaryMathOperation_RegAddr_Reg {
            public override string Mnemonic => "CMP";
            protected override byte Operation => MaizeAlu.OpCode_CMP;
        }

        public class CMP_RegAddr_Reg : InstructionBase<CMP_RegAddr_Reg_Operation> {
        }

        public class CMP_RegVal_Reg_Operation : BinaryMathOperation_RegVal_Reg {
            public override string Mnemonic => "CMP";
            protected override byte Operation => MaizeAlu.OpCode_CMP;
        }

        public class CMP_RegVal_Reg : InstructionBase<CMP_RegVal_Reg_Operation> {
        }




        public class TEST_ImmAddr_Reg_Operation : BinaryMathOperation_ImmAddr_Reg {
            public override string Mnemonic => "TEST";
            protected override byte Operation => MaizeAlu.OpCode_TEST;
        }

        public class TEST_ImmAddr_Reg : InstructionBase<TEST_ImmAddr_Reg_Operation> {
        }

        public class TEST_ImmVal_Reg_Operation : BinaryMathOperation_ImmVal_Reg {
            public override string Mnemonic => "TEST";
            protected override byte Operation => MaizeAlu.OpCode_TEST;
        }

        public class TEST_ImmVal_Reg : InstructionBase<TEST_ImmVal_Reg_Operation> {
        }

        public class TEST_RegAddr_Reg_Operation : BinaryMathOperation_RegAddr_Reg {
            public override string Mnemonic => "TEST";
            protected override byte Operation => MaizeAlu.OpCode_TEST;
        }

        public class TEST_RegAddr_Reg : InstructionBase<TEST_RegAddr_Reg_Operation> {
        }

        public class TEST_RegVal_Reg_Operation : BinaryMathOperation_RegVal_Reg {
            public override string Mnemonic => "TEST";
            protected override byte Operation => MaizeAlu.OpCode_TEST;
        }

        public class TEST_RegVal_Reg : InstructionBase<TEST_RegVal_Reg_Operation> {
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
                        OperandRegister2.SetFromDataBus(SubRegister.W0);
                    },
                    () => {
                        S.EnableToAddressBus(SubRegister.H0);
                        MemoryModule.AddressRegister.SetFromAddressBus(SubRegister.W0);
                    },
                    () => {
                        OperandRegister2.EnableToDataBus(ImmSizeSubRegMap[SrcImmSizeFlag]);
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
                        OperandRegister2.SetFromDataBus(SubRegister.W0);
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
                        OperandRegister2.EnableToAddressBus(ImmSizeSubRegMap[SrcImmSizeFlag]);
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
                        OperandRegister2.SetFromDataBus(SubRegister.W0);
                    },
                    () => {
                        OperandRegister2.RegData.H0 = (ushort)(OperandRegister2.RegData.B0 * 4);
                        OperandRegister2.EnableToAddressBus(SubRegister.H0);
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
                        OperandRegister2.SetFromDataBus(SubRegister.W0);
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
                        OperandRegister2.RegData.H0 = (ushort)(OperandRegister2.RegData.B0 * 4);
                        OperandRegister2.EnableToAddressBus(SubRegister.H0);
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
                        SrcReg = Decoder.RegisterMap[SrcRegisterFlag >> 4];
                        Decoder.ReadAndEnableImmediate(DestImmSize);
                    },
                    () => {
                        OperandRegister2.SetFromDataBus(SubRegister.W0);
                    },
                    () => {
                        OperandRegister2.EnableToAddressBus(SubRegister.W0);
                        MB.SetPortAddress(OperandRegister2.RegData.W0);
                    },
                    () => {
                        SrcReg.EnableToIOBus(SubRegister.W0);
                        MB.SetPortIO(OperandRegister2.RegData.W0);
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
                        DestReg = Decoder.RegisterMap[DestRegisterFlag >> 4];
                        P.Increment(2);
                        Decoder.ReadAndEnableImmediate(SrcImmSize);
                    },
                    () => {
                        OperandRegister2.SetFromDataBus(SubRegister.W0);
                    },
                    () => {
                        DestReg.EnableToAddressBus(SubRegister.W0);
                        MB.SetPortAddress(DestReg.RegData.W0);
                    },
                    () => {
                        OperandRegister2.EnableToIOBus(ImmSizeSubRegMap[SrcImmSizeFlag]);
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
                        DestReg = Decoder.RegisterMap[DestRegisterFlag >> 4];
                        P.Increment(2);
                        Decoder.ReadAndEnableImmediate(SrcImmSize);
                    },
                    () => {
                        OperandRegister2.SetFromDataBus(SubRegister.W0);
                    },
                    () => {
                        OperandRegister2.EnableToAddressBus(SubRegister.W0);
                        MB.SetPortAddress(OperandRegister2.RegData.W0);
                    },
                    () => {
                        MB.EnablePortIO(OperandRegister2.RegData.W0);
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
