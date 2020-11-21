using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using Tortilla;

namespace Maize {
    using Microcode = List<Action>;

    namespace Instruction {

        public class HALT : InstructionBase<HALT> {
            public override void BuildMicrocode() {
                Code = new Microcode {
                    StopClock
                };
            }

            public virtual void StopClock() {
                Clock.Stop();
            }
        }

        public class NOP : InstructionBase<NOP> {
            public override void BuildMicrocode() {
                Code = new Microcode {
                    () => {}
                };
            }
        }

        public class CLR : InstructionBase<CLR> {
            public override void BuildMicrocode() {
                Code = new Microcode {
                    () => {
                        PC.Increment(1);
                        OperandRegister2.Value = 0;
                        OperandRegister2.Enable(BusTypes.DataBus);
                        SrcReg = Decoder.RegisterMap[SrcRegisterFlag];
                        SrcReg.Set(BusTypes.DataBus, SubRegisterMap[SrcSubRegisterFlag]);
                        Cpu.Zero = true;
                    }
                };
            }
        }

        public class LD_RegVal_Reg : InstructionBase<LD_RegVal_Reg> {
            public override void BuildMicrocode() {
                Code = new Microcode {
                    () => {
                        PC.Increment(2);
                        SrcReg = Decoder.RegisterMap[SrcRegisterFlag];
                        SrcReg.Enable(BusTypes.DataBus, SubRegisterMap[SrcSubRegisterFlag]);
                        DestReg = Decoder.RegisterMap[DestRegisterFlag];
                        DestReg.Set(BusTypes.DataBus, SubRegisterMap[DestSubRegisterFlag]);
                    }
                };
            }
        }

        public class LD_ImmVal_Reg : InstructionBase<LD_ImmVal_Reg> {
            public override void BuildMicrocode() {
                Code = new Microcode {
                    () => {
                        PC.Increment(2);
                        Decoder.LoadImmediate(SrcImmSize);
                    },
                    () => {
                        DestReg = Decoder.RegisterMap[DestRegisterFlag];
                        DestReg.Set(BusTypes.DataBus, SubRegisterMap[DestSubRegisterFlag]);
                    }
                };
            }
        }

        public class LD_RegAddr_Reg : InstructionBase<LD_RegAddr_Reg> {
            public override void BuildMicrocode() {
                Code = new Microcode {
                    () => {
                        PC.Increment(2);
                        SrcReg = Decoder.RegisterMap[SrcRegisterFlag];
                        SrcReg.Enable(BusTypes.AddressBus, SubRegisterMap[SrcSubRegisterFlag]);
                        MemoryModule.AddressRegister.Set(BusTypes.AddressBus);
                    },
                    () => {
                        MemoryModule.DataRegister.Enable(BusTypes.DataBus);
                        DestReg = Decoder.RegisterMap[DestRegisterFlag];
                        DestReg.Set(BusTypes.DataBus, SubRegisterMap[DestSubRegisterFlag]);
                    }
                };
            }
        }

        public class LD_ImmAddr_Reg : InstructionBase<LD_ImmAddr_Reg> {
            public override void BuildMicrocode() {
                Code = new Microcode {
                    () => {
                        PC.Increment(2);
                        Decoder.LoadImmediate(SrcImmSize);
                    },
                    () => {
                        MemoryModule.AddressRegister.Set(BusTypes.AddressBus);
                    },
                    () => {
                        MemoryModule.DataRegister.Enable(BusTypes.DataBus);
                        DestReg = Decoder.RegisterMap[DestRegisterFlag];
                        DestReg.Set(BusTypes.DataBus, SubRegisterMap[DestSubRegisterFlag]);
                    }
                };
            }
        }

        public class ST : InstructionBase<ST> {
            public override void BuildMicrocode() {
                Code = new Microcode {
                    () => {
                        PC.Increment(2);
                        SrcReg = Decoder.RegisterMap[SrcRegisterFlag];
                        SrcReg.Enable(BusTypes.DataBus, SubRegisterMap[SrcSubRegisterFlag]);
                        OperandRegister2.Set(BusTypes.DataBus);
                    },
                    () => {
                        Decoder.LoadImmediate(DestImmSize);
                    },
                    () => {
                        OperandRegister3.Value = 0;
                        OperandRegister3.Set(BusTypes.DataBus, ImmSizeSubRegMap[DestImmSizeFlag]);
                    },
                    () => {
                        OperandRegister2.Enable(BusTypes.DataBus, ImmSizeSubRegMap[DestImmSizeFlag]);
                        OperandRegister3.Enable(BusTypes.AddressBus);
                        MemoryModule.AddressRegister.Set(BusTypes.AddressBus);
                        MemoryModule.DataRegister.Set(BusTypes.DataBus, ImmSizeSubRegMap[DestImmSizeFlag]);
                    }
                };
            }
        }

        public class STI : InstructionBase<STI> {
            public override void BuildMicrocode() {
                Code = new Microcode {
                    () => {
                        PC.Increment(2);
                        SrcReg = Decoder.RegisterMap[SrcRegisterFlag];
                        DestReg = Decoder.RegisterMap[DestRegisterFlag];
                        DestReg.Enable(BusTypes.AddressBus, SubRegisterMap[DestSubRegisterFlag]);
                        MemoryModule.AddressRegister.Set(BusTypes.AddressBus);
                    },
                    () => {
                        SrcReg.Enable(BusTypes.DataBus, SubRegisterMap[SrcSubRegisterFlag]);
                        MemoryModule.DataRegister.Set(BusTypes.DataBus, SubRegisterMap[SrcSubRegisterFlag]);
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
                Code = new Microcode {
                    () => {
                        PC.Increment(1);
                        SrcReg = Decoder.RegisterMap[SrcRegisterFlag];
                        SrcReg.Enable(BusTypes.DataBus, SubRegisterMap[SrcSubRegisterFlag]);
                        Alu.DestReg.Set(BusTypes.DataBus);
                    },
                    () => {
                        OperandRegister2.W0 = (byte)(Operation | AluOpSizeMap[SrcSubRegisterFlag]);
                        OperandRegister2.Enable(BusTypes.DataBus);
                        Alu.Set(BusTypes.DataBus);
                    },
                    () => {
                        Alu.DestReg.Enable(BusTypes.DataBus);
                        SrcReg.Set(BusTypes.DataBus, SubRegisterMap[SrcSubRegisterFlag]);
                    }
                };
            }
        }


        public class BinaryMathOperation_RegVal_Reg : BinaryMathOperation {
            public override void BuildMicrocode() {
                Code = new Microcode {
                    () => {
                        PC.Increment(2);
                        SrcReg = Decoder.RegisterMap[SrcRegisterFlag];
                        SrcReg.Enable(BusTypes.DataBus, SubRegisterMap[SrcSubRegisterFlag]);
                        Alu.SrcReg.Set(BusTypes.DataBus, SubRegisterMap[DestSubRegisterFlag]);
                    },
                    () => {
                        DestReg = Decoder.RegisterMap[DestRegisterFlag];
                        DestReg.Enable(BusTypes.DataBus, SubRegisterMap[DestSubRegisterFlag]);
                        Alu.DestReg.Set(BusTypes.DataBus);
                    },
                    () => {
                        OperandRegister2.W0 = (byte)(Operation | AluOpSizeMap[DestSubRegisterFlag]);
                        OperandRegister2.Enable(BusTypes.DataBus);
                        Alu.Set(BusTypes.DataBus);
                    },
                    () => {
                        Alu.DestReg.Enable(BusTypes.DataBus);
                        DestReg.Set(BusTypes.DataBus, SubRegisterMap[DestSubRegisterFlag]);
                    }
                };
            }
        }

        public class BinaryMathOperation_ImmVal_Reg : BinaryMathOperation {
            public override void BuildMicrocode() {
                Code = new Microcode {
                    () => {
                        PC.Increment(2);
                        Decoder.LoadImmediate(SrcImmSize);
                    },
                    () => {
                        Alu.SrcReg.Set(BusTypes.DataBus);
                    },
                    () => {
                        DestReg = Decoder.RegisterMap[DestRegisterFlag];
                        DestReg.Enable(BusTypes.DataBus, SubRegisterMap[DestSubRegisterFlag]);
                        Alu.DestReg.Set(BusTypes.DataBus);
                    },
                    () => {
                        OperandRegister2.W0 = (byte)(Operation | AluOpSizeMap[DestSubRegisterFlag]);
                        OperandRegister2.Enable(BusTypes.DataBus);
                        Alu.Set(BusTypes.DataBus);
                    },
                    () => {
                        Alu.DestReg.Enable(BusTypes.DataBus);
                        DestReg.Set(BusTypes.DataBus, SubRegisterMap[DestSubRegisterFlag]);
                    }
                };
            }
        }

        public class BinaryMathOperation_RegAddr_Reg : BinaryMathOperation {
            public override void BuildMicrocode() {
                Code = new Microcode {
                    () => {
                        SrcReg = Decoder.RegisterMap[SrcRegisterFlag];
                        SrcReg.Enable(BusTypes.AddressBus, SubRegisterMap[SrcSubRegisterFlag]);
                        MemoryModule.AddressRegister.Set(BusTypes.AddressBus);
                    },
                    () => {
                        MemoryModule.DataRegister.Enable(BusTypes.DataBus);
                        Alu.SrcReg.Set(BusTypes.DataBus, SubRegisterMap[DestSubRegisterFlag]);
                    },
                    () => {
                        DestReg = Decoder.RegisterMap[DestRegisterFlag];
                        DestReg.Enable(BusTypes.DataBus, SubRegisterMap[DestSubRegisterFlag]);
                        Alu.DestReg.Set(BusTypes.DataBus);
                    },
                    () => {
                        OperandRegister2.W0 = (byte)(Operation | AluOpSizeMap[DestRegisterFlag]);
                        OperandRegister2.Enable(BusTypes.DataBus);
                        Alu.Set(BusTypes.DataBus);
                    },
                    () => {
                        Alu.DestReg.Enable(BusTypes.DataBus);
                        DestReg.Set(BusTypes.DataBus, SubRegisterMap[DestSubRegisterFlag]);
                    }
                };
            }
        }

        public class BinaryMathOperation_ImmAddr_Reg : BinaryMathOperation {
            public override void BuildMicrocode() {
                Code = new Microcode {
                    () => {
                        Decoder.LoadImmediate(SrcImmSize);
                    },
                    () => {
                        MemoryModule.AddressRegister.Set(BusTypes.AddressBus);
                    },
                    () => {
                        MemoryModule.DataRegister.Enable(BusTypes.DataBus, ImmSizeSubRegMap[SrcImmSizeFlag]);
                        Alu.SrcReg.Set(BusTypes.DataBus);
                    },
                    () => {
                        DestReg = Decoder.RegisterMap[DestRegisterFlag];
                        DestReg.Enable(BusTypes.DataBus, SubRegisterMap[DestSubRegisterFlag]);
                        Alu.DestReg.Set(BusTypes.DataBus);
                    },
                    () => {
                        OperandRegister2.W0 = (byte)(Operation | AluOpSizeMap[DestRegisterFlag]);
                        OperandRegister2.Enable(BusTypes.DataBus);
                        Alu.Set(BusTypes.DataBus);
                    },
                    () => {
                        Alu.DestReg.Enable(BusTypes.DataBus);
                        DestReg.Set(BusTypes.DataBus, SubRegisterMap[DestSubRegisterFlag]);
                    }
                };
            }
        }


        public class INC_Operation : UnaryMathOperation_Reg {
            protected override byte Operation => MaizeAlu.OpCode_INC;
        }

        public class INC : InstructionBase<INC_Operation> { }

        public class DEC_Operation : UnaryMathOperation_Reg {
            protected override byte Operation => MaizeAlu.OpCode_DEC;
        }

        public class DEC : InstructionBase<DEC_Operation> { }

        public class ADD_ImmAddr_Reg_Operation : BinaryMathOperation_ImmAddr_Reg {
            protected override byte Operation => MaizeAlu.OpCode_ADD;
        }

        public class ADD_ImmAddr_Reg : InstructionBase<ADD_ImmAddr_Reg_Operation> { }

        public class ADD_ImmVal_Reg_Operation : BinaryMathOperation_ImmVal_Reg {
            protected override byte Operation => MaizeAlu.OpCode_ADD;
        }

        public class ADD_ImmVal_Reg : InstructionBase<ADD_ImmVal_Reg_Operation> { }

        public class ADD_RegAddr_Reg_Operation : BinaryMathOperation_RegAddr_Reg {
            protected override byte Operation => MaizeAlu.OpCode_ADD;
        }

        public class ADD_RegAddr_Reg : InstructionBase<ADD_RegAddr_Reg_Operation> { }

        public class ADD_RegVal_Reg_Operation : BinaryMathOperation_RegVal_Reg {
            protected override byte Operation => MaizeAlu.OpCode_ADD;
        }

        public class ADD_RegVal_Reg : InstructionBase<ADD_RegVal_Reg_Operation> { }


        public class SUB_ImmAddr_Reg_Operation : BinaryMathOperation_ImmAddr_Reg {
            protected override byte Operation => MaizeAlu.OpCode_SUB;
        }

        public class SUB_ImmAddr_Reg : InstructionBase<SUB_ImmAddr_Reg_Operation> { }

        public class SUB_ImmVal_Reg_Operation : BinaryMathOperation_ImmVal_Reg {
            protected override byte Operation => MaizeAlu.OpCode_SUB;
        }

        public class SUB_ImmVal_Reg : InstructionBase<SUB_ImmVal_Reg_Operation> { }

        public class SUB_RegAddr_Reg_Operation : BinaryMathOperation_RegAddr_Reg {
            protected override byte Operation => MaizeAlu.OpCode_SUB;
        }

        public class SUB_RegAddr_Reg : InstructionBase<SUB_RegAddr_Reg_Operation> { }

        public class SUB_RegVal_Reg_Operation : BinaryMathOperation_RegVal_Reg {
            protected override byte Operation => MaizeAlu.OpCode_SUB;
        }

        public class SUB_RegVal_Reg : InstructionBase<SUB_RegVal_Reg_Operation> { }

        public class MUL_ImmAddr_Reg_Operation : BinaryMathOperation_ImmAddr_Reg {
            protected override byte Operation => MaizeAlu.OpCode_MUL;
        }

        public class MUL_ImmAddr_Reg : InstructionBase<MUL_ImmAddr_Reg_Operation> { }

        public class MUL_ImmVal_Reg_Operation : BinaryMathOperation_ImmVal_Reg {
            protected override byte Operation => MaizeAlu.OpCode_MUL;
        }

        public class MUL_ImmVal_Reg : InstructionBase<MUL_ImmVal_Reg_Operation> { }

        public class MUL_RegAddr_Reg_Operation : BinaryMathOperation_RegAddr_Reg {
            protected override byte Operation => MaizeAlu.OpCode_MUL;
        }

        public class MUL_RegAddr_Reg : InstructionBase<MUL_RegAddr_Reg_Operation> { }

        public class MUL_RegVal_Reg_Operation : BinaryMathOperation_RegVal_Reg {
            protected override byte Operation => MaizeAlu.OpCode_MUL;
        }

        public class MUL_RegVal_Reg : InstructionBase<MUL_RegVal_Reg_Operation> { }


        public class DIV_ImmAddr_Reg_Operation : BinaryMathOperation_ImmAddr_Reg {
            protected override byte Operation => MaizeAlu.OpCode_DIV;
        }

        public class DIV_ImmAddr_Reg : InstructionBase<DIV_ImmAddr_Reg_Operation> { }

        public class DIV_ImmVal_Reg_Operation : BinaryMathOperation_ImmVal_Reg {
            protected override byte Operation => MaizeAlu.OpCode_DIV;
        }

        public class DIV_ImmVal_Reg : InstructionBase<DIV_ImmVal_Reg_Operation> { }

        public class DIV_RegAddr_Reg_Operation : BinaryMathOperation_RegAddr_Reg {
            protected override byte Operation => MaizeAlu.OpCode_DIV;
        }

        public class DIV_RegAddr_Reg : InstructionBase<DIV_RegAddr_Reg_Operation> { }

        public class DIV_RegVal_Reg_Operation : BinaryMathOperation_RegVal_Reg {
            protected override byte Operation => MaizeAlu.OpCode_DIV;
        }

        public class DIV_RegVal_Reg : InstructionBase<DIV_RegVal_Reg_Operation> { }


        public class MOD_ImmAddr_Reg_Operation : BinaryMathOperation_ImmAddr_Reg {
            protected override byte Operation => MaizeAlu.OpCode_MOD;
        }

        public class MOD_ImmAddr_Reg : InstructionBase<MOD_ImmAddr_Reg_Operation> { }

        public class MOD_ImmVal_Reg_Operation : BinaryMathOperation_ImmVal_Reg {
            protected override byte Operation => MaizeAlu.OpCode_MOD;
        }

        public class MOD_ImmVal_Reg : InstructionBase<MOD_ImmVal_Reg_Operation> { }

        public class MOD_RegAddr_Reg_Operation : BinaryMathOperation_RegAddr_Reg {
            protected override byte Operation => MaizeAlu.OpCode_MOD;
        }

        public class MOD_RegAddr_Reg : InstructionBase<MOD_RegAddr_Reg_Operation> { }

        public class MOD_RegVal_Reg_Operation : BinaryMathOperation_RegVal_Reg {
            protected override byte Operation => MaizeAlu.OpCode_MOD;
        }

        public class MOD_RegVal_Reg : InstructionBase<MOD_RegVal_Reg_Operation> { }


        public class PUSH_RegVal : InstructionBase<PUSH_RegVal> {
            public override void BuildMicrocode() {
                Code = new Microcode {
                    () => {
                        PC.Increment(1);
                        SP.Decrement(SizeMap[SrcSubRegisterFlag]);
                        SrcReg = Decoder.RegisterMap[SrcRegisterFlag];
                    },
                    () => {
                        SP.Enable(BusTypes.AddressBus, SubRegisters.H0);
                        MemoryModule.AddressRegister.Set(BusTypes.AddressBus);
                    },
                    () => {
                        SrcReg.Enable(BusTypes.DataBus, SubRegisterMap[SrcSubRegisterFlag]);
                        MemoryModule.DataRegister.Set(BusTypes.DataBus, SubRegisterMap[SrcSubRegisterFlag]);
                    }
                };
            }
        }

        public class PUSH_ImmVal : InstructionBase<PUSH_ImmVal> {
            public override void BuildMicrocode() {
                Code = new Microcode {
                    () => {
                        PC.Increment(1);
                        SP.Decrement(SrcImmSize);
                        Decoder.LoadImmediate(SrcImmSize);
                    },
                    () => {
                        OperandRegister2.Set(BusTypes.DataBus);
                    },
                    () => {
                        SP.Enable(BusTypes.AddressBus, SubRegisters.H0);
                        MemoryModule.AddressRegister.Set(BusTypes.AddressBus);
                    },
                    () => {
                        OperandRegister2.Enable(BusTypes.DataBus, ImmSizeSubRegMap[SrcImmSizeFlag]);
                        MemoryModule.DataRegister.Set(BusTypes.DataBus, ImmSizeSubRegMap[SrcImmSizeFlag]);
                    }
                };
            }
        }

        public class POP_RegVal : InstructionBase<POP_RegVal> {
            public override void BuildMicrocode() {
                Code = new Microcode {
                    () => {
                        SrcReg = Decoder.RegisterMap[SrcRegisterFlag];
                        PC.Increment(1);
                        SP.Enable(BusTypes.AddressBus, SubRegisters.H0);
                        MemoryModule.AddressRegister.Set(BusTypes.AddressBus);
                    },
                    () => {
                        SP.Increment(SizeMap[SrcSubRegisterFlag]);
                        MemoryModule.DataRegister.Enable(BusTypes.DataBus);
                        SrcReg.Set(BusTypes.DataBus, SubRegisterMap[SrcSubRegisterFlag]);
                    }
                };
            }
        }

        public class JMP_RegVal : InstructionBase<JMP_RegVal> {
            public override void BuildMicrocode() {
                Code = new Microcode {
                    () => {
                        SrcReg = Decoder.RegisterMap[SrcRegisterFlag];
                        SrcReg.Enable(BusTypes.AddressBus, SubRegisterMap[SrcSubRegisterFlag]);
                        PC.Set(BusTypes.AddressBus);
                    }
                };
            }
        }


        public class JMP_ImmVal : InstructionBase<JMP_ImmVal> {
            public override void BuildMicrocode() {
                Code = new Microcode {
                    () => {
                        PC.Increment(1);
                        Decoder.LoadImmediate(SrcImmSize);
                    },
                    () => {
                        PC.Set(BusTypes.DataBus, ImmSizeSubRegMap[SrcImmSizeFlag]);
                    }
                };
            }
        }

        public class JMP_RegAddr : InstructionBase<JMP_RegAddr> {
            public override void BuildMicrocode() {
                Code = new Microcode {
                    () => {
                        SrcReg = Decoder.RegisterMap[SrcRegisterFlag];
                        SrcReg.Enable(BusTypes.AddressBus, SubRegisterMap[SrcSubRegisterFlag]);
                        MemoryModule.AddressRegister.Set(BusTypes.AddressBus);
                    },
                    () => {
                        MemoryModule.DataRegister.Enable(BusTypes.DataBus);
                        PC.Set(BusTypes.AddressBus);
                    }
                };
            }
        }

        public class JMP_ImmAddr : InstructionBase<JMP_ImmAddr> {
            public override void BuildMicrocode() {
                Code = new Microcode {
                    () => {
                        PC.Increment(1);
                        Decoder.LoadImmediate(SrcImmSize);
                    },
                    () => {
                        MemoryModule.AddressRegister.Set(BusTypes.AddressBus);
                    },
                    () => {
                        MemoryModule.DataRegister.Enable(BusTypes.DataBus);
                        PC.Set(BusTypes.DataBus, SubRegisters.H0);
                    }
                };
            }
        }

        public class CALL_RegVal : InstructionBase<CALL_RegVal> {
            public override void BuildMicrocode() {
                Code = new Microcode {
                    () => {
                        PC.Increment(1);
                        SP.Decrement(8);
                        SP.Enable(BusTypes.AddressBus, SubRegisters.H0);
                        MemoryModule.AddressRegister.Set(BusTypes.AddressBus);
                    },
                    () => {
                        // push CS and PC, which is entire E register
                        Cpu.ERegister.Enable(BusTypes.DataBus, SubRegisterMap[SrcSubRegisterFlag]);
                        MemoryModule.DataRegister.Set(BusTypes.DataBus, SubRegisterMap[SrcSubRegisterFlag]);
                    },
                    () => {
                        SrcReg = Decoder.RegisterMap[SrcRegisterFlag];
                        SrcReg.Enable(BusTypes.AddressBus, SubRegisterMap[SrcSubRegisterFlag]);
                        PC.Set(BusTypes.AddressBus, SubRegisters.H0);
                    }
                };
            }
        }

        public class CALL_ImmVal : InstructionBase<CALL_ImmVal> {
            public override void BuildMicrocode() {
                Code = new Microcode {
                    () => {
                        PC.Increment(1);
                        Decoder.LoadImmediate(SrcImmSize);
                    },
                    () => {
                        SP.Decrement(8);
                        OperandRegister2.Set(BusTypes.DataBus);
                    },
                    () => {
                        SP.Enable(BusTypes.AddressBus, SubRegisters.H0);
                        MemoryModule.AddressRegister.Set(BusTypes.AddressBus);
                    },
                    () => {
                        Cpu.ERegister.Enable(BusTypes.DataBus, SubRegisters.W0);
                        MemoryModule.DataRegister.Set(BusTypes.DataBus, SubRegisters.W0);
                    },
                    () => {
                        OperandRegister2.Enable(BusTypes.AddressBus, ImmSizeSubRegMap[SrcImmSizeFlag]);
                        PC.Set(BusTypes.AddressBus, SubRegisters.H0);
                    }
                };
            }
        }

        public class RET : InstructionBase<RET> {
            public override void BuildMicrocode() {
                Code = new Microcode {
                    () => {
                        SrcReg = Decoder.RegisterMap[SrcRegisterFlag];
                        PC.Increment(1);
                        SP.Enable(BusTypes.AddressBus, SubRegisters.H0);
                        MemoryModule.AddressRegister.Set(BusTypes.AddressBus);
                    },
                    () => {
                        SP.Increment(8);
                        MemoryModule.DataRegister.Enable(BusTypes.DataBus);
                        Cpu.ERegister.Set(BusTypes.DataBus, SubRegisters.W0);
                    }
                };
            }
        }

        public class OUT_RegVal_Reg : InstructionBase<OUT_RegVal_Reg> {
            public override void BuildMicrocode() {
                Code = new Microcode {
                    () => {
                        PC.Increment(2);
                        SrcReg = Decoder.RegisterMap[SrcRegisterFlag];
                        DestReg = Decoder.RegisterMap[DestRegisterFlag];
                        SrcReg.Enable(BusTypes.IOBus);
                        MB.SetPort(DestReg.B0);
                    }
                };
            }
        }

        public class OUT_ImmVal_Reg : InstructionBase<OUT_ImmVal_Reg> {
            public override void BuildMicrocode() {
                Code = new Microcode {
                    () => {
                        DestReg = Decoder.RegisterMap[DestRegisterFlag];
                        PC.Increment(2);
                        Decoder.LoadImmediate(SrcImmSize);
                    },
                    () => {
                        OperandRegister2.Set(BusTypes.DataBus);
                    },
                    () => {
                        OperandRegister2.Enable(BusTypes.IOBus, ImmSizeSubRegMap[SrcImmSizeFlag]);
                        MB.SetPort(DestReg.B0);
                    }
                };
            }
        }

        public class IN_RegVal_Reg : InstructionBase<IN_RegVal_Reg> {
            public override void BuildMicrocode() {
                Code = new Microcode {
                    () => {
                        PC.Increment(2);
                        SrcReg = Decoder.RegisterMap[SrcRegisterFlag];
                        DestReg = Decoder.RegisterMap[DestRegisterFlag];
                        SrcReg.Set(BusTypes.IOBus);
                        MB.EnablePort(DestReg.B0);
                    }
                };
            }
        }

        public class IN_ImmVal_Reg : InstructionBase<IN_ImmVal_Reg> {
            public override void BuildMicrocode() {
                Code = new Microcode {
                    () => {
                        DestReg = Decoder.RegisterMap[DestRegisterFlag];
                        PC.Increment(2);
                        Decoder.LoadImmediate(SrcImmSize);
                    },
                    () => {
                        OperandRegister2.Set(BusTypes.DataBus);
                    },
                    () => {
                        MB.EnablePort(OperandRegister2.B0);
                        DestReg.Set(BusTypes.IOBus, SubRegisterMap[DestSubRegisterFlag]);
                    }
                };
            }
        }




        /*

        public class NAME : InstructionBase<NAME> {
            public override void BuildMicrocode() {
                Code = new Microcode {

                };
            }
        }
        */
    }
}
