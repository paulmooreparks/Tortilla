using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Tortilla;


namespace Maize {
    public class Clock : Tortilla.IClock {
        public Clock() {
        }

        public bool IsRunning { get; protected set; } = false;

        public void Start() {
            IsRunning = true;
        }

        public void Initialize() {
            IBusComponent[] tmp = new IBusComponent[compNextIndex];
            TickEvents = new Action<IBusComponent>[compNextIndex];

            for (var i = 0; i < compNextIndex; ++i) {
                var component = compList[i];
                tmp[i] = compList[i];
            }

            compList = tmp;
        }

        public void RegisterTickExecute(IBusComponent comp) {
            TickEvents[TickEventsCount] = comp.OnTickExecute;
            ++TickEventsCount;
        }

        private void RegisterTickUpdate(IBusComponent comp) {
            TickEvents[TickEventsCount] = comp.OnTickUpdate;
            ++TickEventsCount;
        }

        private void RegisterTickEnableToAddressBus(IBusComponent comp) {
            TickEvents[TickEventsCount] = comp.OnTickEnableToAddressBus;
            ++TickEventsCount;
        }

        private void RegisterTickEnableToDataBus(IBusComponent comp) {
            TickEvents[TickEventsCount] = comp.OnTickEnableToDataBus;
            ++TickEventsCount;
        }

        private void RegisterTickEnableToIOBus(IBusComponent comp) {
            TickEvents[TickEventsCount] = comp.OnTickEnableToIOBus;
            ++TickEventsCount;
        }

        private void RegisterTickSetFromAddressBus(IBusComponent comp) {
            TickEvents[TickEventsCount] = comp.OnTickSetFromAddressBus;
            ++TickEventsCount;
        }

        private void RegisterTickSetFromDataBus(IBusComponent comp) {
            TickEvents[TickEventsCount] = comp.OnTickSetFromDataBus;
            ++TickEventsCount;
        }

        private void RegisterTickSetFromIOBus(IBusComponent comp) {
            TickEvents[TickEventsCount] = comp.OnTickSetFromIOBus;
            ++TickEventsCount;
        }

        private void RegisterTickStore(IBusComponent comp) {
            TickEvents[TickEventsCount] = comp.OnTickStore;
            ++TickEventsCount;
        }

        private void RegisterTickLoad(IBusComponent comp) {
            TickEvents[TickEventsCount] = comp.OnTickLoad;
            ++TickEventsCount;
        }

        protected Action<IBusComponent>[] TickEvents = null;
        protected int TickEventsCount = 0;
        protected int TickExecuteCount = 0;

        public void Tick(IBusComponent cpuFlags) {
            for (var i = 0; i < TickEventsCount; ++i) {
                TickEvents[i](cpuFlags);
            }

            TickEventsCount = 0;
        }

        public void Stop() {
            IsRunning = false;
        }

        protected IBusComponent[] compList = new IBusComponent[32];
        protected int compNextIndex = 0;

        public void ConnectComponent(IBusComponent component) {
            foreach (var c in compList) {
                if (c == component) {
                    return;
                }
            }

            if (compNextIndex == compList.Length) {
                IBusComponent[] tmp = new IBusComponent[compNextIndex * 2];
                compList.CopyTo(tmp, 0);
                compList = tmp;
            }

            component.RequestTickExecute += RegisterTickExecute;
            component.RequestTickUpdate += RegisterTickUpdate;
            component.RequestTickEnableToAddressBus += RegisterTickEnableToAddressBus;
            component.RequestTickEnableToDataBus += RegisterTickEnableToDataBus;
            component.RequestTickEnableToIOBus += RegisterTickEnableToIOBus;
            component.RequestTickSetFromAddressBus += RegisterTickSetFromAddressBus;
            component.RequestTickSetFromDataBus += RegisterTickSetFromDataBus;
            component.RequestTickSetFromIOBus += RegisterTickSetFromIOBus;
            component.OnRegisterTickLoad += RegisterTickLoad;
            component.OnRegisterTickStore += RegisterTickStore;

            compList[compNextIndex] = component;
            ++compNextIndex;
        }
    }
}
