using System;
using System.Collections.Generic;
using Tortilla;

namespace Maize {
    public class MemoryModule : Register {

        public MemoryModule(IMotherboard<UInt64> motherboard) {
            AddressRegister.AddressBus = motherboard.AddressBus;
            AddressRegister.DataBus = motherboard.DataBus;
            AddressRegister.IOBus = motherboard.IOBus;
            motherboard.ConnectComponent(AddressRegister);

            DataRegister.AddressBus = motherboard.AddressBus;
            DataRegister.DataBus = motherboard.DataBus;
            DataRegister.IOBus = motherboard.IOBus;
            motherboard.ConnectComponent(DataRegister);

            motherboard.ConnectComponent(this);

            CacheAddress.W0 = 0xFFFFFFFF_FFFFFFFF;
            CacheBase = CacheAddress.W0 >> 8;

            AddressRegister.RequestTickSetFromAddressBus += AddressRegisterSet;
            AddressRegister.RequestTickSetFromDataBus += AddressRegisterSet;
            AddressRegister.RequestTickSetFromIOBus += AddressRegisterSet;
            DataRegister.RequestTickSetFromAddressBus += DataRegisterSet;
            DataRegister.RequestTickSetFromDataBus += DataRegisterSet;
            DataRegister.RequestTickSetFromIOBus += DataRegisterSet;
        }

        private void AddressRegisterSet(IBusComponent comp) {
            RegisterTickLoad();
        }

        private void DataRegisterSet(IBusComponent comp) {
            RegisterTickStore();
        }

        public Register AddressRegister = new Register();
        public Register DataRegister = new Register();

        public override void OnTickStore(IBusComponent cpuFlags) {
            switch (DataRegister.SetSubRegisterMask) {
            case SubRegisterMask.B0:
                WriteByte(AddressRegister.RegData.W0, DataRegister.RegData.B0);
                break;

            case SubRegisterMask.B1:
                WriteByte(AddressRegister.RegData.W0, DataRegister.RegData.B1);
                break;

            case SubRegisterMask.B2:
                WriteByte(AddressRegister.RegData.W0, DataRegister.RegData.B2);
                break;

            case SubRegisterMask.B3:
                WriteByte(AddressRegister.RegData.W0, DataRegister.RegData.B3);
                break;

            case SubRegisterMask.B4:
                WriteByte(AddressRegister.RegData.W0, DataRegister.RegData.B4);
                break;

            case SubRegisterMask.B5:
                WriteByte(AddressRegister.RegData.W0, DataRegister.RegData.B5);
                break;

            case SubRegisterMask.B6:
                WriteByte(AddressRegister.RegData.W0, DataRegister.RegData.B6);
                break;

            case SubRegisterMask.B7:
                WriteByte(AddressRegister.RegData.W0, DataRegister.RegData.B7);
                break;

            case SubRegisterMask.Q0:
                WriteQuarterWord(AddressRegister.RegData.W0, DataRegister.RegData.Q0);
                break;

            case SubRegisterMask.Q1:
                WriteQuarterWord(AddressRegister.RegData.W0, DataRegister.RegData.Q1);
                break;

            case SubRegisterMask.Q2:
                WriteQuarterWord(AddressRegister.RegData.W0, DataRegister.RegData.Q2);
                break;

            case SubRegisterMask.Q3:
                WriteQuarterWord(AddressRegister.RegData.W0, DataRegister.RegData.Q3);
                break;

            case SubRegisterMask.H0:
                WriteHalfWord(AddressRegister.RegData.W0, DataRegister.RegData.H0);
                break;

            case SubRegisterMask.H1:
                WriteHalfWord(AddressRegister.RegData.W0, DataRegister.RegData.H1);
                break;

            case SubRegisterMask.W0:
                WriteWord(AddressRegister.RegData.W0, DataRegister.RegData.W0);
                break;
            }
        }

        public override void OnTickLoad(IBusComponent cpuFlags) {
            DataRegister.RegData.W0 = ReadWord(AddressRegister.RegData.W0);
        }

        RegValue tmp = 0;

        public UInt64 ReadWord(UInt64 address) {
            tmp.B0 = ReadByte(address);
            tmp.B1 = ReadByte(++address);
            tmp.B2 = ReadByte(++address);
            tmp.B3 = ReadByte(++address);
            tmp.B4 = ReadByte(++address);
            tmp.B5 = ReadByte(++address);
            tmp.B6 = ReadByte(++address);
            tmp.B7 = ReadByte(++address);
            return tmp.W0;
        }

        public void WriteWord(UInt64 address, UInt64 value) {
            tmp.W0 = value;
            WriteByte(address, tmp.B0);
            WriteByte(++address, tmp.B1);
            WriteByte(++address, tmp.B2);
            WriteByte(++address, tmp.B3);
            WriteByte(++address, tmp.B4);
            WriteByte(++address, tmp.B5);
            WriteByte(++address, tmp.B6);
            WriteByte(++address, tmp.B7);
        }

        public UInt32 ReadHalfWord(UInt64 address) {
            tmp.B0 = ReadByte(address);
            tmp.B1 = ReadByte(++address);
            tmp.B2 = ReadByte(++address);
            tmp.B3 = ReadByte(++address);
            return tmp.H0;
        }

        public void WriteHalfWord(UInt64 address, UInt32 value) {
            tmp.H0 = value;
            WriteByte(address, tmp.B0);
            WriteByte(++address, tmp.B1);
            WriteByte(++address, tmp.B2);
            WriteByte(++address, tmp.B3);
        }

        public UInt16 ReadQuarterWord(UInt64 address) {
            tmp.B0 = ReadByte(address);
            tmp.B1 = ReadByte(++address);
            return tmp.Q0;
        }

        public void WriteQuarterWord(UInt64 address, UInt16 value) {
            tmp.Q0 = value;
            WriteByte(address, tmp.B0);
            WriteByte(++address, tmp.B1);
        }

        public byte ReadByte(UInt64 address) {
            SetCacheAddress(address);
            return Cache[CacheAddress.B0];
        }

        public void WriteByte(UInt64 address, byte value) {
            SetCacheAddress(address);
            Cache[CacheAddress.B0] = value;
        }

        void SetCacheAddress(UInt64 address) {
            var addressBase = address >> 8;

            if (addressBase != CacheBase) {
                if (!MemoryMap.ContainsKey(addressBase)) {
                    MemoryMap[addressBase] = new byte[0x100];
                }

                Cache = MemoryMap[addressBase];
                CacheBase = addressBase;
            }

            CacheAddress.W0 = address;
        }

        protected byte[] Cache = null;
        protected Dictionary<UInt64, byte[]> MemoryMap = new();

        public UInt32 MemorySize => (UInt32)MemoryMap.Count * 0x100;

        protected RegValue CacheAddress;
        protected UInt64 CacheBase;
    }

}
