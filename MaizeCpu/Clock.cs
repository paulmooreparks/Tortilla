using System;
using System.Runtime.InteropServices;



namespace Tortilla {

    public class Clock : IClock {
        public Clock() {
        }

        public bool IsRunning { get; protected set; } = false;

        public void Start() {
            IsRunning = true;
        }

        public void Initialize() {
            IBusComponent[] tmp = new IBusComponent[compNextIndex];

            for (var i = 0; i < compNextIndex; ++i) {
                tmp[i] = compList[i];
            }

            compList = tmp;
        }

        public void Tick(IBusComponent CpuFlags) {
            ClockState state = ClockState.TickOn;

            foreach (var comp in compList) {
                comp.OnTick(state, CpuFlags);
            }

            state = ClockState.TickEnable;

            foreach (var comp in compList) {
                if (comp.IsEnabled) {
                    comp.OnTick(state, CpuFlags);
                }
            }

            state = ClockState.TickSet;

            foreach (var comp in compList) {
                if (comp.IsSet) {
                    comp.OnTick(state, CpuFlags);
                }
            }

            state = ClockState.TickOff;

            foreach (var comp in compList) {
                comp.OnTick(state, CpuFlags);
            }

            state = ClockState.TickDecode;

            foreach (var comp in compList) {
                comp.OnTick(state, CpuFlags);
            }
        }

        public void Stop() {
            IsRunning = false;
        }

        // protected System.Collections.Generic.List<IBusComponent> compList = new System.Collections.Generic.List<IBusComponent>();
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

            compList[compNextIndex] = component;
            ++compNextIndex;
        }
    }
}
