﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tortilla {
    public enum ClockState {
        TickOn,
        TickEnable,
        TickSet,
        TickOff,
        TickDecode
    }

    public interface IClock {
        bool IsRunning { get; }
        void Start();
        void Stop();
        void ConnectComponent(IBusComponent component);
        void Tick();
    }

    public interface IBus {

    }

    public interface IDataBus<DataType> : IBus {
        DataType Value { get; set; }
    }

    public enum BusTypes {
        AddressBus,
        DataBus,
        IOBus
    }

    public interface IBusComponent {
        void OnTick(ClockState state);
        void Enable(BusTypes type);
        void Set(BusTypes type);
        bool AddressBusEnabled { get; }
        bool AddressBusSet { get; }
        bool DataBusEnabled { get; }
        bool DataBusSet { get; }
        bool IOBusEnabled { get; }
        bool IOBusSet { get; }

        bool IsEnabled { get; }
        bool IsSet { get; }
    }

    public interface IRegister<DataType> : IBusComponent {
        IDataBus<DataType> DataBus { get; set; }
        IDataBus<DataType> AddressBus { get; set; }
        IDataBus<DataType> IOBus { get; set; }
        DataType Value { get; set; }
    }

    public interface IMotherboard<DataType> {
        IClock Clock { get; }
        
        void OnDebug(string disasm);
        void OnDebug();
        void OnPowerOff();
        void OnRaiseException(byte id);
        void RaiseInterrupt(int id);

        event EventHandler<string> Debug;
        event EventHandler PoweredOff;
        event EventHandler<byte> RaiseException;

        IDataBus<DataType> DataBus { get; }
        IDataBus<DataType> AddressBus { get; }
        IDataBus<DataType> IOBus { get; }
        ICpu<DataType> Cpu { get; }
        UInt32 MemorySize { get; }
        void Reset();
        void PowerOn();
        void PowerOff();
        byte ReadByte(DataType address);
        void WriteByte(DataType address, byte value);
        void EnablePort(byte address);
        void SetPort(byte address);

        void ConnectComponent(IBusComponent component);
        void ConnectDevice(IBusComponent component, int address);
        int ConnectInterrupt(IBusComponent component, int address);
    }

    public interface ICpu<DataType> : IBusComponent {
        void PowerOn();
        void PowerOff();
        void Reset();
        void RaiseInterrupt(int id);
        void Break();
        bool SingleStep { get; set; }
        void Continue();
        string RegisterDump { get; }
        bool IsPowerOn { get; }

        event EventHandler<Tuple<UInt64, UInt64>> DecodeInstruction;
    }

    public interface IPic<DataType> : IRegister<DataType> {
        UInt32 CommandPort { get; set; }
        UInt32 DataPort { get; set; }
    }

    public interface IDissasember<DataType, AddressType> {
        int Decode(DataType value, AddressType address, out string text);
    }
}
