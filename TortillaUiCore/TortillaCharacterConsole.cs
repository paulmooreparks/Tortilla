using Maize;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Runtime.InteropServices;
using Tortilla;
using System.Threading;

namespace TortillaUI {
    class TortillaCharacterConsole : Tortilla.IBusComponent, TortillaUI.ITortillaConsole {
        UserSettings us = new UserSettings();

        private const UInt32 StdOutputHandle = 0xFFFFFFF5;
        [DllImport("kernel32.dll")]
        private static extern IntPtr GetStdHandle(UInt32 nStdHandle);
        [DllImport("kernel32.dll")]
        private static extern void SetStdHandle(UInt32 nStdHandle, IntPtr handle);
        [DllImport("kernel32")]
        static extern bool AllocConsole();
        [DllImport("kernel32")]
        static extern bool FreeConsole();

        public TortillaCharacterConsole() {
        }

        ConsoleColor[] vgaColors = {
            ConsoleColor.Black,    ConsoleColor.DarkBlue, ConsoleColor.DarkGreen, ConsoleColor.DarkCyan, ConsoleColor.DarkRed, ConsoleColor.DarkMagenta, ConsoleColor.DarkYellow,  ConsoleColor.Gray,
            ConsoleColor.DarkGray, ConsoleColor.Blue,     ConsoleColor.Green,     ConsoleColor.Cyan,     ConsoleColor.Red,     ConsoleColor.Magenta,     ConsoleColor.Yellow, ConsoleColor.White
        };

        MaizeRegister BusData { get; set; } = new MaizeRegister();

        IMotherboard<UInt64> MB { get; set; }
        int InterruptID { get; set; }

        public IDataBus<UInt64> DataBus { get; set; }
        public IDataBus<UInt64> AddressBus { get; set; }
        public IDataBus<UInt64> IOBus { get; set; }

        public IBusComponent PrivilegeFlags { get; set; }


        UInt32 AddressValue { get; set; }
        UInt64 DataValue { get; set; }


        // Maize system implementation

        protected int KeyCode { get; set; }
        ConsoleColor DefaultFgColor { get; set; } = ConsoleColor.Gray; // 0x07
        ConsoleColor DefaultBgColor { get; set; } = ConsoleColor.Black; // 0x00

        public static void InputProc(object o) {
            var tConsole = o as TortillaCharacterConsole;
            ConsoleKeyInfo keyInfo;

            while (true) {
                keyInfo = Console.ReadKey(true);
                tConsole.KeyCode = keyInfo.KeyChar;
                tConsole.BusData.Value = (ulong)tConsole.KeyCode;
                tConsole.MB?.RaiseInterrupt(tConsole.InterruptID);
            }
        }



        private void GetKeyCode(RegValue busValue) {
            BusData.Value = (ulong)KeyCode;
            BusData.Enable(BusTypes.IOBus);
        }

        private void SetCursorLocation(RegValue busValue) {
            Console.SetCursorPosition(0, 0);
        }

        private void SetBackgroundColor(RegValue busValue) {
            char co = (char)busValue.B3;
            int bgColor = (co & 0xf0) >> 4;
            DefaultBgColor = (ConsoleColor)bgColor;
            Console.BackgroundColor = DefaultBgColor;
        }

        private void SetForegroundColor(RegValue busValue) {
            char co = (char)busValue.B3;
            int fgColor = (co & 0x0f);
            DefaultFgColor = (ConsoleColor)fgColor;
            Console.ForegroundColor = DefaultFgColor;
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

        private void WriteCharacterAndColorAt(RegValue busValue) {
            int left = busValue.B4;
            int top = busValue.B5;
            char ch = (char)busValue.B2;
            char co = (char)busValue.B3;
            int fgColor = (co & 0x0f);
            int bgColor = (co & 0xf0) >> 4;

            WriteCharacter(ch, left, top, fgColor, bgColor, busValue.B1);
        }

        private void WriteCharacterAt(RegValue busValue) {
            int left = busValue.B4;
            int top = busValue.B5;
            char ch = (char)busValue.B2;

            WriteCharacter(ch, left, top, (int)DefaultFgColor, (int)DefaultBgColor, busValue.B1);
        }

        private void WriteCharacterAndColor(RegValue busValue) {
            int left = Console.CursorLeft;
            int top = Console.CursorTop;
            char ch = (char)busValue.B2;
            char co = (char)busValue.B3;
            int fgColor = (co & 0x0f);
            int bgColor = (co & 0xf0) >> 4;

            WriteCharacter(ch, left, top, fgColor, bgColor, busValue.B1);
        }

        private void WriteCharacterAtCursorPosition(RegValue busValue) {
            int left = Console.CursorLeft;
            int top = Console.CursorTop;
            char ch = (char)busValue.B2;

            WriteCharacter(ch, left, top, (int)DefaultFgColor, (int)DefaultBgColor, busValue.B1);
        }


        // ITortillaConsole interface
        public void Connect(IMotherboard<UInt64> _motherboard) {
            MB = _motherboard;
            AddressBus = MB.AddressBus;
            DataBus = MB.DataBus;
            IOBus = MB.IOBus;
            MB.ConnectDevice(this, 0x01);
            InterruptID = MB.ConnectInterrupt(this, 0x02);

            BusData.IOBus = MB.IOBus;
            MB.ConnectComponent(BusData);
        }

        private Thread inputThread { get; set; }

        public void Show() {
            AllocConsole();
            inputThread = new Thread(InputProc);
            inputThread.Start(this);
        }

        public void Close() {
            FreeConsole();
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
        public bool IsSet => IOBusSet;

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
            }
        }
        public void OnTick(ClockState state, IBusComponent _flags) {
            switch (state) {
            case ClockState.TickSet:
                var opcode = IOBus.Value & 0xFF;

                switch (opcode) {
                case 0x00:
                    Clear();
                    break;

                case 0x01:
                    WriteCharacterAtCursorPosition(IOBus.Value);
                    break;

                case 0x02:
                    WriteCharacterAndColor(IOBus.Value);
                    break;

                case 0x03:
                    WriteCharacterAt(IOBus.Value);
                    break;

                case 0x04:
                    WriteCharacterAndColorAt(IOBus.Value);
                    break;

                case 0x05:
                    SetForegroundColor(IOBus.Value);
                    break;

                case 0x06:
                    SetBackgroundColor(IOBus.Value);
                    break;

                case 0x07:
                    SetCursorLocation(IOBus.Value);
                    break;

                case 0x08:
                    GetKeyCode(IOBus.Value);
                    break;
                }

                IOBusSet = false;

                break;
            }
        }
    }
}
