using System;
using System.Collections.Generic;
using Tortilla;

/* One thing that you'll notice upon reading the code, especially if you have any familiarity with C# and 
with .NET in general, is that I don't use a lot of properties, but rather fields. I did this for speed and 
simplicity. Inside a CPU, and in assembly code in general, there's not a lot of information hiding, though 
there is encapsulation to some degree. My primary concern in this code is not to write enterprise-grade 
interfaces, but rather to get raw speed wherever I can. I'm sacrificing some sacred OO principles in pursuit 
of performance, and inside the ugly innards of the CPU implementation, it's probably worth it. */

namespace Maize {
    public class Motherboard : IMotherboard<UInt64> {
        public Motherboard() {
            // Reset();
        }

        public void Reset() {
            Clock = new Clock();
            DataBus = new Bus();
            AddressBus = new Bus();
            IOBus = new Bus();
            Cpu = new Cpu(this);
            ConnectComponent(Cpu);
            MemoryModule = new MemoryModule(this);
            PowerModule = new PowerModule(this);

            Instruction.WireUp(this);
        }

        public void PowerOff() {
            Clock?.Stop();
            Cpu?.PowerOff();
        }

        public void PowerOn() {
            Clock.Initialize();
            Cpu.PowerOn();
        }

        public void EnableDebug(bool isEnabled) {
            if (Cpu is null) {
                return;
            }

            if (isEnabled) {
            }
            else {
            }
        }

        public IDataBus<UInt64> DataBus { get; protected set; }
        public IDataBus<UInt64> AddressBus { get; protected set; }
        public IDataBus<UInt64> IOBus { get; protected set; }

        public Clock Clock { get; protected set; }

        public Cpu Cpu;
        public MemoryModule MemoryModule;
        public PowerModule PowerModule;

        public UInt32 MemorySize { get { return MemoryModule.MemorySize; } }

        ICpu<ulong> IMotherboard<ulong>.Cpu => Cpu;

        public event EventHandler<string> Debug;
        public event EventHandler PoweredOff;
        public event EventHandler<byte> RaiseException;

        public void OnDebug() {
            Debug?.Invoke(this, null);
        }

        public void OnPowerOff() {
            PoweredOff?.Invoke(this, null);
        }

        public void OnRaiseException(byte id) {
            RaiseException?.Invoke(this, id);
        }

        public void RaiseInterrupt(UInt64 id) {
            Cpu.RaiseInterrupt(id);
        }

        public byte ReadByte(UInt64 address) {
            return MemoryModule.ReadByte(address);
        }

        public void WriteByte(UInt64 address, byte value) {
            MemoryModule.WriteByte(address, value);
        }

        public void EnablePortIO(UInt64 address) {
            IBusComponent component = deviceTable[address];
            component?.EnableToIOBus(SubRegister.W0);
        }

        public void SetPortAddress(UInt64 address) {
            IBusComponent component = deviceTable[address];
            component?.SetFromAddressBus(SubRegister.W0);
        }

        public void SetPortIO(UInt64 address) {
            IBusComponent component = deviceTable[address];
            component?.SetFromIOBus(SubRegister.W0);
        }

        protected Dictionary<UInt64, IBusComponent> deviceTable = new();

        public void ConnectComponent(IBusComponent component) {
            Clock.ConnectComponent(component);
        }

        public void ConnectDevice(IBusComponent component, UInt64 address) {
            Clock.ConnectComponent(component);
            deviceTable[address] = component;
        }

        public UInt64 ConnectInterrupt(IBusComponent component, UInt64 address) {
            return address;
        }
    }

}
