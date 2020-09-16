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

        private void ProcessTick() {
            ClockState state = ClockState.TickOn;

            foreach (var comp in compList) {
                comp.OnTick(state);
            }

            state = ClockState.TickEnable;

            foreach (var comp in compList) {
                if (comp.IsEnabled) {
                    comp.OnTick(state);
                }
            }

            state = ClockState.TickSet;

            foreach (var comp in compList) {
                if (comp.IsSet) {
                    comp.OnTick(state);
                }
            }

            state = ClockState.TickOff;

            foreach (var comp in compList) {
                comp.OnTick(state);
            }

            state = ClockState.TickDecode;

            foreach (var comp in compList) {
                comp.OnTick(state);
            }
        }

        public void Stop() {
            IsRunning = false;
        }

        protected System.Collections.Generic.List<IBusComponent> compList =
            new System.Collections.Generic.List<IBusComponent>();

        public void ConnectComponent(IBusComponent component) {
            if (!compList.Contains(component)) {
                compList.Add(component);
            }
        }

        public void Tick() {
            ProcessTick();
        }
    }

}
