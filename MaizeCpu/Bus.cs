using System;

namespace Tortilla {
    public class Bus<DataType> : Tortilla.IDataBus<DataType> {
        public DataType Value { get; set; }
        public override string ToString() {
            return $"0x{Value:X16}";
        }
    }
}
