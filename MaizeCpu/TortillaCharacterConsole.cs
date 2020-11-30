using Maize;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Configuration;
using System.Runtime.InteropServices;
using Tortilla;

namespace Tortilla {
    public class TortillaCharacterConsole : IBusComponent, ITortillaConsole {

        public TortillaCharacterConsole() {
        }

        ConsoleColor[] vgaColors = {
            ConsoleColor.Black,    ConsoleColor.DarkBlue, ConsoleColor.DarkGreen, ConsoleColor.DarkCyan, ConsoleColor.DarkRed, ConsoleColor.DarkMagenta, ConsoleColor.DarkYellow,  ConsoleColor.Gray,
            ConsoleColor.DarkGray, ConsoleColor.Blue,     ConsoleColor.Green,     ConsoleColor.Cyan,     ConsoleColor.Red,     ConsoleColor.Magenta,     ConsoleColor.Yellow, ConsoleColor.White
        };

        MaizeRegister BusData { get; set; } = new MaizeRegister();

        IMotherboard<UInt64> MB { get; set; }
        UInt64 KbdInterruptID { get; set; }

        public IDataBus<UInt64> DataBus { get; set; }
        public IDataBus<UInt64> AddressBus { get; set; }
        public IDataBus<UInt64> IOBus { get; set; }

        public IBusComponent PrivilegeFlags { get; set; }


        UInt64 AddressValue { get; set; }
        UInt64 IOValue { get; set; }


        // Maize system implementation

        protected int KeyChar => KeyInfo.KeyChar;
        protected ConsoleKeyInfo KeyInfo { get; set; }

        class Reader {
            private static Thread inputThread;
            private static AutoResetEvent getInput, gotInput;
            private static ConsoleKeyInfo KeyInfo { get; set; }

            static Reader() {
                getInput = new AutoResetEvent(false);
                gotInput = new AutoResetEvent(false);
                inputThread = new Thread(ReadProc);
                inputThread.IsBackground = true;
                inputThread.Start();
            }

            private static void ReadProc() {
                while (true) {
                    getInput.WaitOne();
                    KeyInfo = Console.ReadKey(true);
                    gotInput.Set();
                }
            }

            public static bool TryReadLine(out ConsoleKeyInfo keyInfo, int timeOutMillisecs = Timeout.Infinite) {
                getInput.Set();
                bool success = gotInput.WaitOne(timeOutMillisecs);

                if (success) {
                    keyInfo = KeyInfo;
                }
                else {
                    keyInfo = new();
                }

                return success;
            }

            public static ConsoleKeyInfo ReadKey(int timeOutMillisecs = Timeout.Infinite) {
                getInput.Set();
                bool success = gotInput.WaitOne(timeOutMillisecs);

                if (success) {
                    return KeyInfo;
                }
                else {
                    throw new TimeoutException("User did not provide input within the timelimit.");
                }
            }
        }

        public static void InputProc(object o) {
            var tConsole = o as TortillaCharacterConsole;
            var token = tConsole.CTS.Token;

            while (!token.IsCancellationRequested) {
                tConsole.KeyInfo = Reader.ReadKey();
                tConsole.MB?.RaiseInterrupt(tConsole.KbdInterruptID);
            }
        }

        private void GetKeyCode(RegValue busValue) {
            BusData.Q0 = KeyInfo.KeyChar;
            BusData.Enable(BusTypes.IOBus);
        }

        private void GetCursorLocation(RegValue busValue) {
            BusData.B6 = (byte)Console.CursorLeft;
            BusData.B7 = (byte)Console.CursorTop;
            BusData.Enable(BusTypes.IOBus);
        }

        private void SetCursorLocation(RegValue busValue) {
            Console.SetCursorPosition(busValue.B6, busValue.B7);
        }

        private void SetBackgroundColor(RegValue busValue) {
            char co = (char)busValue.B2;
            int bgColor = co; // (co & 0xf0) >> 4;
            Console.BackgroundColor = (ConsoleColor)bgColor;
        }

        private void SetForegroundColor(RegValue busValue) {
            char co = (char)busValue.B2;
            int fgColor = co; // (co & 0x0f);
            Console.ForegroundColor = (ConsoleColor)fgColor;
        }

        private void WriteCharacter(char ch, int left, int top, int fgColor, int bgColor, int count) {
            Console.ForegroundColor = (ConsoleColor)fgColor;
            Console.BackgroundColor = (ConsoleColor)bgColor;
            Console.SetCursorPosition(left, top);

            do {
                Console.Write(ch);
                --count;
            } while (count > 0);
        }

        private void WriteCharacterAndColor(RegValue busValue) {
            int left = busValue.B6;
            int top = busValue.B7;
            char ch = (char)busValue.B0;
            char co = (char)busValue.B2;
            int fgColor = (co & 0x0f);
            int bgColor = (co & 0xf0) >> 4;

            WriteCharacter(ch, left, top, fgColor, bgColor, busValue.B5);
        }

        private void WriteCharacter(RegValue busValue) {
            int left = busValue.B6;
            int top = busValue.B7;
            char ch = (char)busValue.B0;

            WriteCharacter(ch, left, top, (int)Console.ForegroundColor, (int)Console.BackgroundColor, busValue.B5);
        }

        private void WriteCharacterAndColorAtCursorPosition(RegValue busValue) {
            int left = Console.CursorLeft;
            int top = Console.CursorTop;
            char ch = (char)busValue.B0;
            char co = (char)busValue.B2;
            int fgColor = (co & 0x0f);
            int bgColor = (co & 0xf0) >> 4;

            WriteCharacter(ch, left, top, fgColor, bgColor, busValue.B5);
        }

        private void WriteCharacterAtCursorPosition(RegValue busValue) {
            int left = Console.CursorLeft;
            int top = Console.CursorTop;
            char ch = (char)busValue.B0;

            WriteCharacter(ch, left, top, (int)Console.ForegroundColor, (int)Console.BackgroundColor, busValue.B5);
        }


        // ITortillaConsole interface
        public void Connect(IMotherboard<UInt64> _motherboard) {
            MB = _motherboard;
            AddressBus = MB.AddressBus;
            DataBus = MB.DataBus;
            IOBus = MB.IOBus;
            
            MB.ConnectDevice(this, 0x60); // Keycode
            MB.ConnectDevice(this, 0x64); // Status
            MB.ConnectDevice(this, 0x7F); // Video

            KbdInterruptID = MB.ConnectInterrupt(this, 0x21);

            BusData.IOBus = MB.IOBus;
            BusData.AddressBus = MB.AddressBus;
            MB.ConnectComponent(BusData);
        }

        private Thread inputThread { get; set; }
        protected CancellationTokenSource CTS { get; } = new();

        public void Show() {
            Console.Title = "Maize Console";
            // inputThread = new Thread(InputProc);
            Task.Run(() => InputProc(this), CTS.Token);
        }

        public void Close() {
            CTS.Cancel();
        }

        public void Clear() {
            Console.Clear();
        }


        // IBusComponent interface

        public bool AddressBusEnabled { get; protected set; }
        public bool AddressBusSet { get; protected set; }
        public bool DataBusEnabled { get; protected set; }
        public bool DataBusSet { get; protected set; }
        public bool IOBusEnabled { get; protected set; }
        public bool IOBusSet { get; protected set; }
        public bool IsEnabled => IOBusEnabled;
        public bool IsSet => IOBusSet | AddressBusSet;

        public void Enable(BusTypes type) {
            switch (type) {
            case BusTypes.IOBus:
                BusData.Enable(BusTypes.IOBus);
                IOBusEnabled = true;
                break;
            }
        }

        public void Set(BusTypes type) {
            switch (type) {
            case BusTypes.IOBus:
                IOBusSet = true;
                break;
            case BusTypes.AddressBus:
                AddressBusSet = true;
                break;
            }
        }
        public void OnTick(ClockState state, IBusComponent cpuFlags) {
            switch (state) {
            case ClockState.TickEnable:
                IOBusEnabled = false;

                switch (AddressValue) {
                case 0x60:
                    GetKeyCode(IOBus.Value);
                    break;

                case 0x64:
                    break;

                default:
                    break;
                }

                break;

            case ClockState.TickSet:
                if (AddressBusSet) {
                    AddressBusSet = false;
                    AddressValue = AddressBus.Value;
                }

                if (IOBusSet) {
                    IOBusSet = false;
                    RegValue value = IOBus.Value;

                    switch (AddressValue) {
                    case 0x7F:
                        var opcode = value.B1;

                        switch (opcode) {
                        case 0x00:
                            // Set video mode
                            // See http://www.ctyme.com/intr/rb-0069.htm for more on this
                            break;

                        case 0x01:
                            // set cursor shape
                            break;

                        case 0x02:
                            SetCursorLocation(IOBus.Value);
                            break;

                        case 0x03:
                            // GetCursorLocation(IOBus.Value);
                            break;

                        case 0x04:
                            Clear();
                            break;

                        case 0x05:
                            // select active display page
                            break;

                        case 0x06:
                            // scroll window up
                            break;

                        case 0x07:
                            // scroll window down
                            break;

                        case 0x08:
                            // read character and attribute
                            break;

                        case 0x09:
                            WriteCharacterAndColorAtCursorPosition(IOBus.Value);
                            break;

                        case 0x0A:
                            WriteCharacterAtCursorPosition(IOBus.Value);
                            break;

                        case 0x0B:
                            RegValue r = IOBus.Value;

                            if (r.B3 == 0) { 
                                SetBackgroundColor(IOBus.Value);
                            }
                            else if (r.B3 == 1) {
                                SetForegroundColor(IOBus.Value);
                            }

                            break;

                        case 0x0C:
                            break;

                        case 0x0D:
                            WriteCharacterAndColor(IOBus.Value);
                            break;

                        case 0x0E:
                            WriteCharacter(IOBus.Value);
                            break;
                        }

                        break;

                    default:
                        break;
                    }



                }

                break;
            }
        }
    }
}
