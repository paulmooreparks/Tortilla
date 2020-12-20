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
    public class TortillaCharacterConsole : Maize.Register, IConsole {

        public TortillaCharacterConsole() {
        }

        ConsoleColor[] vgaColors = {
            ConsoleColor.Black,    ConsoleColor.DarkBlue, ConsoleColor.DarkGreen, ConsoleColor.DarkCyan, ConsoleColor.DarkRed, ConsoleColor.DarkMagenta, ConsoleColor.DarkYellow,  ConsoleColor.Gray,
            ConsoleColor.DarkGray, ConsoleColor.Blue,     ConsoleColor.Green,     ConsoleColor.Cyan,     ConsoleColor.Red,     ConsoleColor.Magenta,     ConsoleColor.Yellow, ConsoleColor.White
        };

        Maize.Register DataRegister { get; } = new();

        IMotherboard<UInt64> MB { get; set; }
        UInt64 KbdInterruptID { get; set; }


        UInt64 AddressValue { get; set; }
        UInt64 IOValue { get; set; }


        // Maize system implementation

        protected int KeyChar { get; set; }
        protected ConsoleKeyInfo KeyInfo { get; set; }

        private void GetKeyCode(RegValue busValue) {
            DataRegister.RegData.Q0 = NextKey.KeyChar;
            IOBus.Value = DataRegister.RegData.W0;
        }

        private void GetCursorLocation(RegValue busValue) {
            DataRegister.RegData.B6 = (byte)Console.CursorLeft;
            DataRegister.RegData.B7 = (byte)Console.CursorTop;
            IOBus.Value = DataRegister.RegData.W0;
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


        // IConsole interface
        public void Connect(IMotherboard<UInt64> _motherboard) {
            MB = _motherboard;
            AddressBus = MB.AddressBus;
            DataBus = MB.DataBus;
            IOBus = MB.IOBus;
            
            MB.ConnectDevice(this, 0x60); // Keycode
            MB.ConnectDevice(this, 0x64); // Status
            MB.ConnectDevice(this, 0x7F); // Video

            KbdInterruptID = MB.ConnectInterrupt(this, 0x21);

            DataRegister.IOBus = MB.IOBus;
            DataRegister.AddressBus = MB.AddressBus;
            MB.ConnectComponent(DataRegister);
        }

        protected CancellationTokenSource CTS { get; } = new();

        protected Queue<ConsoleKeyInfo> KeyQueue { get; } = new();
        public ConsoleKeyInfo NextKey {
            get {
                return KeyQueue.Dequeue();
            }
            set {
                KeyQueue.Enqueue(value);
                MB?.RaiseInterrupt(KbdInterruptID);
            }
        }

        void KeyReader() {
            WaitHandle[] handles = new WaitHandle[] { ConsoleClose };

            while (true) {
                int index = WaitHandle.WaitAny(handles, 0x20);

                if (index == 0) {
                    return;
                }
                else {
                    while (Console.KeyAvailable) {
                        NextKey = Console.ReadKey(true);
                    }
                }
            }
        }

        public void Show() {
            Console.Title = "Maize Console";
            // CTS.Token.ThrowIfCancellationRequested();
            Task.Run(KeyReader);
        }

        ManualResetEvent ConsoleClose = new(false);

        public void Close() {
            ConsoleClose.Set();
        }

        public void Clear() {
            Console.Clear();
        }


        // IBusComponent interface

        public override void OnTickEnableToIOBus(IBusComponent cpuFlags) {
            //IOBusEnabled = false;

            switch (AddressValue) {
            case 0x60:
                GetKeyCode(IOBus.Value);
                break;

            case 0x64:
                break;

            default:
                break;
            }
        }

        public override void OnTickSetFromAddressBus(IBusComponent cpuFlags) {
            AddressValue = AddressBus.Value;
        }

        public override void OnTickSetFromIOBus(IBusComponent cpuFlags) {
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

        public override void OnTickStore(IBusComponent cpuFlags) {
        }

        public override void OnTickExecute(IBusComponent cpuFlags) {
        }
    }
}
