/* 
IntervalTree implementation by Ido Ran 
https://archive.codeplex.com/?p=intervaltree
http://dotdotnet.blogspot.com/2011/06/interval-tree-c-implementation.html
BSD License
*/

namespace Tortilla {

    public class BusComponent : Tortilla.IBusComponent {
        public bool DataBusEnabled { get; protected set; }
        public bool DataBusSet { get; protected set; }
        public bool AddressBusEnabled { get; protected set; }
        public bool AddressBusSet { get; protected set; }
        public bool IOBusEnabled { get; protected set; }
        public bool IOBusSet { get; protected set; }

        public bool IsEnabled { get { return DataBusEnabled || AddressBusEnabled || IOBusEnabled; } }
        public bool IsSet { get { return DataBusSet || AddressBusSet || IOBusSet; } }

        public enum BusDataSizes {
            Word,
            HalfWord,
            QuarterWord,
            Byte
        }

        public BusDataSizes BusDataSize { get; protected set; } = BusDataSizes.Word;

        public virtual void Enable(BusTypes type) {
            BusDataSize = BusDataSizes.Word;

            switch (type) {
            case BusTypes.AddressBus:
                AddressBusEnabled = true;
                break;

            case BusTypes.DataBus:
                DataBusEnabled = true;
                break;

            case BusTypes.IOBus:
                IOBusEnabled = true;
                break;
            }
        }

        public virtual void Set(BusTypes type) {
            BusDataSize = BusDataSizes.Word;

            switch (type) {
            case BusTypes.AddressBus:
                AddressBusSet = true;
                break;

            case BusTypes.DataBus:
                DataBusSet = true;
                break;

            case BusTypes.IOBus:
                IOBusSet = true;
                break;
            }
        }

        public virtual void OnTick(ClockState state) {
            
        }
    }

    public class Register<DataType> : BusComponent, Tortilla.IRegister<DataType> {
        public Register() {

        }

        public Register(DataType _valueInit) {
            Value = _valueInit;
        }

        public virtual DataType Value { get; set; }
        public IDataBus<DataType> DataBus { get; set; }
        public IDataBus<DataType> AddressBus { get; set; }
        public IDataBus<DataType> IOBus { get; set; }

        public override void OnTick(ClockState state) {
            switch (state) {
            case ClockState.TickEnable:
                if (DataBusEnabled) {
                    DataBus.Value = Value;
                    DataBusEnabled = false;
                }

                if (AddressBusEnabled) {
                    AddressBus.Value = Value;
                    AddressBusEnabled = false;
                }

                if (IOBusEnabled) {
                    IOBus.Value = Value;
                    IOBusEnabled = false;
                }

                break;

            case ClockState.TickSet:
                if (DataBusSet) {
                    Value = DataBus.Value;
                    DataBusSet = false;
                }

                if (AddressBusSet) {
                    Value = AddressBus.Value;
                    AddressBusSet = false;
                }

                if (IOBusSet) {
                    Value = IOBus.Value;
                    IOBusSet = false;
                }

                break;
            }
        }

        public static implicit operator Register<DataType>(DataType v) => new Register<DataType>(v);

        public override string ToString() {
            return $"0x{Value:X16}";
        }
    }
}
