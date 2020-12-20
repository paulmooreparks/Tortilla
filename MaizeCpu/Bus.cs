using System;

namespace Maize {
    public class Bus : Tortilla.IDataBus<UInt64> {
        public UInt64 Value { get; set; }
        public override string ToString() {
            return $"0x{Value:X16}";
        }
    }

}
