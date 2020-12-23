using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Tortilla;

/* One thing that you'll notice upon reading the code, especially if you have any familiarity with C# and 
with .NET in general, is that I don't use a lot of properties, but rather fields. I did this for speed and 
simplicity. Inside a CPU, and in assembly code in general, there's not a lot of information hiding, though 
there is encapsulation to some degree. My primary concern in this code is not to write enterprise-grade 
interfaces, but rather to get raw speed wherever I can. I'm sacrificing some sacred OO principles in pursuit 
of performance, and inside the ugly innards of the CPU implementation, it's probably worth it. */

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

            /* Can you spot the huge no-no being committed between here and the RegisterTick* functions 
            above? 

            DING! DING! DING! That's right! I'm modifying the data structure WHILE I'M IN THE LOOP ITERATING 
            OVER IT! Kids, don't try this at home unless you ask a parent for help. It's dangerous.

            The rationale is that I want to build up the list of functions to call on each tick as I'm executing 
            previous functions, and the first function is always the TickExecute called on the Decoder. That starts 
            the whole Tick cascade. As long as I don't write on top of existing functions, lose track of the 
            TickEventsCount, or write past the end of the array, I should be fine. */

            /* Having said all of that, I can already envision some opportunities for silliness. I'm going to 
            revisit this soon. */

            for (var i = 0; i < TickEventsCount; ++i) {
                TickEvents[i](cpuFlags);
            }

            /* I went back and forth over this. For debugging sanity, I think it's worth it to erase the contents 
            of the array so that I can always see what's going to executed in each tick. If it becomes a performance 
            issue later, then please wipe it out, but there are probably much better opportunities for performance 
            improvement to be found elsewhere. */

            for (var i = 0; i < TickEvents.Length; ++i) {
                TickEvents[i] = null;
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
