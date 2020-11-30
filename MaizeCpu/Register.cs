namespace Tortilla {

    public abstract class BusComponent : Tortilla.IBusComponent {
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

        public abstract void OnTick(ClockState state, IBusComponent cpuFlags);
    }

    public abstract class Register<DataType> : BusComponent, Tortilla.IRegister<DataType> {
        public Register() {

        }

        public Register(DataType _valueInit) {
            Value = _valueInit;
        }

        public virtual DataType Value { get; set; }
        public virtual DataType PrivilegeFlags { get; set; }
        public virtual DataType PrivilegeMask { get; set; }
        public IDataBus<DataType> DataBus { get; set; }
        public IDataBus<DataType> AddressBus { get; set; }
        public IDataBus<DataType> IOBus { get; set; }
    }
}
