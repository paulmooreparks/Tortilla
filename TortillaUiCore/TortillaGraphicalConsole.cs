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
using Tortilla;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;

namespace TortillaUI {
    public partial class TortillaGraphicalConsole : Form, Tortilla.IBusComponent, Tortilla.IConsole {
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

        public TortillaGraphicalConsole() {
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Load += TortillaConsole_Load;
            SizeChanged += TortillaConsole_SizeChanged;
            this.HandleCreated += TortillaConsole_HandleCreated;
            this.KeyDown += TortillaConsole_KeyDown;
            this.FormClosing += TortillaConsole_FormClosing;
        }

        private void TortillaConsole_FormClosing(object sender, FormClosingEventArgs e) {
            FreeConsole();
        }

        private void TortillaConsole_HandleCreated(object sender, EventArgs e) {
            cursorTimer.Elapsed += CursorTimer_Elapsed;
        }

        private void TortillaConsole_KeyDown(object sender, KeyEventArgs e) {
            KeyCode = (int)e.KeyCode;
            BusData.RegData.W0 = (ulong)KeyCode;
            MB?.RaiseInterrupt(InterruptID);
        }

        public System.Threading.Semaphore CharSemaphore { get; } = new(0, 255);
        public char NextChar { get; }

        public ConsoleKeyInfo NextKey { get; set; }
        public System.Threading.Semaphore KeySemaphore { get; } = new(0, 255);

        protected int KeyCode { get; set; }

        private void GetKeyCode(RegValue busValue) {
            BusData.RegData.W0 = (ulong)KeyCode;
            BusData.EnableToIOBus(SubRegister.W0);
        }

        private bool CursorOn { get; set; }

        System.Timers.Timer cursorTimer = new System.Timers.Timer(500);

        private void TortillaConsole_SizeChanged(object sender, EventArgs e) {
            SaveWindowPosition();
        }

        private void TortillaConsole_Load(object sender, EventArgs e) {
            RestoreWindowPosition();
            Move += TortillaConsole_Move;
            AllocConsole();
        }

        private void TortillaConsole_Move(object sender, EventArgs e) {
            SaveWindowPosition();
        }

        private void SaveWindowPosition() {
            Rectangle rect = (WindowState == FormWindowState.Normal) ?
                new Rectangle(DesktopBounds.Left, DesktopBounds.Top, DesktopBounds.Width, DesktopBounds.Height) :
                new Rectangle(RestoreBounds.Left, RestoreBounds.Top, this.Width, this.Height);

            var pos = $"{(int)this.WindowState},{rect.Left},{rect.Top},{rect.Width},{rect.Height}";
            us.ConsoleWindowPosition = pos;
            us.Save();
        }

        private void RestoreWindowPosition() {
            try {
                string pos = us.ConsoleWindowPosition;

                if (!string.IsNullOrEmpty(pos)) {
                    List<int> settings = pos.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(v => int.Parse(v)).ToList();

                    if (settings.Count == 5) {
                        this.SetDesktopBounds(settings[1], settings[2], settings[3], settings[4]);
                        this.WindowState = (FormWindowState)settings[0];
                    }
                }
            }
            catch { /* Just leave current position if error */ }
            
        }

        protected override CreateParams CreateParams {
            get {
                var value = base.CreateParams;
                value.ClassStyle |= 0x200; // no close button
                return value;
            }
        }

        private Graphics consoleGraphics = null;

        static byte[] ch_20 = { 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000 };

        /* Tip: If you stack the bytes like this, it's easier to see what the finished glyph will look like. 
        I collapsed the rest of them to save space.  If you decide to edit the glyph, lay it out this way 
        again to make your edits. */
        static byte[] ch_21 = {
            0b00000000,
            0b00000000,
            0b00000000,
            0b00011000,
            0b00111100,
            0b00111100,
            0b00111100,
            0b00011000,
            0b00011000,
            0b00011000,
            0b00000000,
            0b00011000,
            0b00011000,
            0b00000000,
            0b00000000,
            0b00000000
        };

        static byte[,] ch_21_test = new byte[,] {
            {0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0},
            {0,0,0,1,1,0,0,0},
            {0,0,1,1,1,1,0,0},
            {0,0,1,1,1,1,0,0},
            {0,0,1,1,1,1,0,0},
            {0,0,0,1,1,0,0,0},
            {0,0,0,1,1,0,0,0},
            {0,0,0,1,1,0,0,0},
            {0,0,0,0,0,0,0,0},
            {0,0,0,1,1,0,0,0},
            {0,0,0,1,1,0,0,0},
            {0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0}
        };

        static byte[] ch_22 = { 0b00000000, 0b00000000, 0b00000000, 0b01100110, 0b01100110, 0b01100110, 0b00100100, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_23 = { 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b01101100, 0b01101100, 0b11111110, 0b01101100, 0b01101100, 0b01101100, 0b11111110, 0b01101100, 0b01101100, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_24 = { 0b00000000, 0b00011000, 0b00011000, 0b01111100, 0b11000110, 0b11000010, 0b11000000, 0b01111100, 0b00000110, 0b00000110, 0b10000110, 0b11000110, 0b01111100, 0b00011000, 0b00011000, 0b00000000 };
        static byte[] ch_25 = { 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b11000010, 0b11000110, 0b00001100, 0b00011000, 0b00110000, 0b01100000, 0b11000110, 0b10000110, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_26 = { 0b00000000, 0b00000000, 0b00000000, 0b00111000, 0b01101100, 0b01101100, 0b00111000, 0b01110110, 0b11011100, 0b11001100, 0b11001100, 0b11001100, 0b01110110, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_27 = { 0b00000000, 0b00110000, 0b00110000, 0b00110000, 0b01100000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_28 = { 0b00000000, 0b00000000, 0b00000000, 0b00001100, 0b00011000, 0b00110000, 0b00110000, 0b00110000, 0b00110000, 0b00110000, 0b00110000, 0b00011000, 0b00001100, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_29 = { 0b00000000, 0b00000000, 0b00000000, 0b01100000, 0b00110000, 0b00011000, 0b00011000, 0b00011000, 0b00011000, 0b00011000, 0b00011000, 0b00110000, 0b01100000, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_2A = { 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b01100110, 0b00111100, 0b11111111, 0b00111100, 0b01100110, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_2B = { 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00011000, 0b00011000, 0b01111110, 0b00011000, 0b00011000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_2C = { 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00011000, 0b00011000, 0b00011000, 0b00110000, 0b00000000, 0b00000000 };
        static byte[] ch_2D = { 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b11111110, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_2E = { 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00011000, 0b00011000, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_2F = { 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000010, 0b00000110, 0b00001100, 0b00011000, 0b00110000, 0b01100000, 0b11000000, 0b10000000, 0b00000000, 0b00000000, 0b00000000 };

        static byte[] ch_30 = { 0b00000000, 0b00000000, 0b00000000, 0b00111000, 0b01101100, 0b11000110, 0b11000110, 0b11010110, 0b11010110, 0b11000110, 0b11000110, 0b01101100, 0b00111000, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_31 = { 0b00000000, 0b00000000, 0b00000000, 0b00011000, 0b00111000, 0b01111000, 0b00011000, 0b00011000, 0b00011000, 0b00011000, 0b00011000, 0b00011000, 0b01111110, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_32 = { 0b00000000, 0b00000000, 0b00000000, 0b01111100, 0b11000110, 0b00000110, 0b00001100, 0b00011000, 0b00110000, 0b01100000, 0b11000000, 0b11000110, 0b11111110, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_33 = { 0b00000000, 0b00000000, 0b00000000, 0b01111100, 0b11000110, 0b00000110, 0b00000110, 0b00111100, 0b00000110, 0b00000110, 0b00000110, 0b11000110, 0b01111100, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_34 = { 0b00000000, 0b00000000, 0b00000000, 0b00001100, 0b00011100, 0b00111100, 0b01101100, 0b11001100, 0b11111110, 0b00001100, 0b00001100, 0b00001100, 0b00011110, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_35 = { 0b00000000, 0b00000000, 0b00000000, 0b11111110, 0b11000000, 0b11000000, 0b11000000, 0b11111100, 0b00000110, 0b00000110, 0b00000110, 0b11000110, 0b01111100, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_36 = { 0b00000000, 0b00000000, 0b00000000, 0b00111000, 0b01100000, 0b11000000, 0b11000000, 0b11111100, 0b11000110, 0b11000110, 0b11000110, 0b11000110, 0b01111100, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_37 = { 0b00000000, 0b00000000, 0b00000000, 0b11111110, 0b11000110, 0b00000110, 0b00000110, 0b00001100, 0b00011000, 0b00110000, 0b00110000, 0b00110000, 0b00110000, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_38 = { 0b00000000, 0b00000000, 0b00000000, 0b01111100, 0b11000110, 0b11000110, 0b11000110, 0b01111100, 0b11000110, 0b11000110, 0b11000110, 0b11000110, 0b01111100, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_39 = { 0b00000000, 0b00000000, 0b00000000, 0b01111100, 0b11000110, 0b11000110, 0b11000110, 0b01111110, 0b00000110, 0b00000110, 0b00000110, 0b00001100, 0b01111000, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_3A = { 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00011000, 0b00011000, 0b00000000, 0b00000000, 0b00000000, 0b00011000, 0b00011000, 0b00000000, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_3B = { 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00011000, 0b00011000, 0b00000000, 0b00000000, 0b00000000, 0b00011000, 0b00011000, 0b00110000, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_3C = { 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000110, 0b00001100, 0b00011000, 0b00110000, 0b01100000, 0b00110000, 0b00011000, 0b00001100, 0b00000110, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_3D = { 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b01111110, 0b00000000, 0b00000000, 0b01111110, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_3E = { 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b01100000, 0b00110000, 0b00011000, 0b00001100, 0b00000110, 0b00001100, 0b00011000, 0b00110000, 0b01100000, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_3F = { 0b00000000, 0b00000000, 0b00000000, 0b01111100, 0b11000110, 0b11000110, 0b00001100, 0b00011000, 0b00011000, 0b00011000, 0b00000000, 0b00011000, 0b00011000, 0b00000000, 0b00000000, 0b00000000 };

        static byte[] ch_40 = { 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b01111100, 0b11000110, 0b11000110, 0b11011110, 0b11011110, 0b11011110, 0b11011100, 0b11000000, 0b01111100, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_41 = { 0b00000000, 0b00000000, 0b00000000, 0b00010000, 0b00111000, 0b01101100, 0b11000110, 0b11000110, 0b11111110, 0b11000110, 0b11000110, 0b11000110, 0b11000110, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_42 = { 0b00000000, 0b00000000, 0b00000000, 0b11111100, 0b01100110, 0b01100110, 0b01100110, 0b01111100, 0b01100110, 0b01100110, 0b01100110, 0b01100110, 0b11111100, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_43 = { 0b00000000, 0b00000000, 0b00000000, 0b00111100, 0b01100110, 0b11000010, 0b11000000, 0b11000000, 0b11000000, 0b11000000, 0b11000010, 0b01100110, 0b00111100, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_44 = { 0b00000000, 0b00000000, 0b00000000, 0b11111000, 0b01101100, 0b01100110, 0b01100110, 0b01100110, 0b01100110, 0b01100110, 0b01100110, 0b01101100, 0b11111000, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_45 = { 0b00000000, 0b00000000, 0b00000000, 0b11111110, 0b01100110, 0b01100010, 0b01101000, 0b01111000, 0b01101000, 0b01100000, 0b01100010, 0b01100110, 0b11111110, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_46 = { 0b00000000, 0b00000000, 0b00000000, 0b11111110, 0b01100110, 0b01100010, 0b01101000, 0b01111000, 0b01101000, 0b01100000, 0b01100000, 0b01100000, 0b11110000, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_47 = { 0b00000000, 0b00000000, 0b00000000, 0b00111100, 0b01100110, 0b11000010, 0b11000000, 0b11000000, 0b11011110, 0b11000110, 0b11000110, 0b01100110, 0b00111010, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_48 = { 0b00000000, 0b00000000, 0b00000000, 0b11000110, 0b11000110, 0b11000110, 0b11000110, 0b11111110, 0b11000110, 0b11000110, 0b11000110, 0b11000110, 0b11000110, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_49 = { 0b00000000, 0b00000000, 0b00000000, 0b00111100, 0b00011000, 0b00011000, 0b00011000, 0b00011000, 0b00011000, 0b00011000, 0b00011000, 0b00011000, 0b00111100, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_4A = { 0b00000000, 0b00000000, 0b00000000, 0b00011110, 0b00001100, 0b00001100, 0b00001100, 0b00001100, 0b00001100, 0b11001100, 0b11001100, 0b11001100, 0b01111000, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_4B = { 0b00000000, 0b00000000, 0b00000000, 0b11100110, 0b01100110, 0b01100110, 0b01101100, 0b01111000, 0b01111000, 0b01101100, 0b01100110, 0b01100110, 0b11100110, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_4C = { 0b00000000, 0b00000000, 0b00000000, 0b11110000, 0b01100000, 0b01100000, 0b01100000, 0b01100000, 0b01100000, 0b01100000, 0b01100010, 0b01100110, 0b11111110, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_4D = { 0b00000000, 0b00000000, 0b00000000, 0b11000110, 0b11101110, 0b11111110, 0b11111110, 0b11010110, 0b11000110, 0b11000110, 0b11000110, 0b11000110, 0b11000110, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_4E = { 0b00000000, 0b00000000, 0b00000000, 0b11000110, 0b11100110, 0b11110110, 0b11111110, 0b11011110, 0b11001110, 0b11000110, 0b11000110, 0b11000110, 0b11000110, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_4F = { 0b00000000, 0b00000000, 0b00000000, 0b01111100, 0b11000110, 0b11000110, 0b11000110, 0b11000110, 0b11000110, 0b11000110, 0b11000110, 0b11000110, 0b01111100, 0b00000000, 0b00000000, 0b00000000 };

        static byte[] ch_50 = { 0b00000000, 0b00000000, 0b00000000, 0b11111100, 0b01100110, 0b01100110, 0b01100110, 0b01111100, 0b01100000, 0b01100000, 0b01100000, 0b01100000, 0b11110000, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_51 = { 0b00000000, 0b00000000, 0b00000000, 0b01111100, 0b11000110, 0b11000110, 0b11000110, 0b11000110, 0b11000110, 0b11000110, 0b11010110, 0b11011110, 0b01111100, 0b00001100, 0b00001110, 0b00000000 };
        static byte[] ch_52 = { 0b00000000, 0b00000000, 0b00000000, 0b11111100, 0b01100110, 0b01100110, 0b01100110, 0b01111100, 0b01101100, 0b01100110, 0b01100110, 0b01100110, 0b11100110, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_53 = { 0b00000000, 0b00000000, 0b00000000, 0b01111100, 0b11000110, 0b11000110, 0b01100000, 0b00111000, 0b00001100, 0b00000110, 0b11000110, 0b11000110, 0b01111100, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_54 = { 0b00000000, 0b00000000, 0b00000000, 0b01111110, 0b01111110, 0b01011010, 0b00011000, 0b00011000, 0b00011000, 0b00011000, 0b00011000, 0b00011000, 0b00111100, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_55 = { 0b00000000, 0b00000000, 0b00000000, 0b11000110, 0b11000110, 0b11000110, 0b11000110, 0b11000110, 0b11000110, 0b11000110, 0b11000110, 0b11000110, 0b01111100, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_56 = { 0b00000000, 0b00000000, 0b00000000, 0b11000110, 0b11000110, 0b11000110, 0b11000110, 0b11000110, 0b11000110, 0b11000110, 0b01101100, 0b00111000, 0b00010000, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_57 = { 0b00000000, 0b00000000, 0b00000000, 0b11000110, 0b11000110, 0b11000110, 0b11000110, 0b11010110, 0b11010110, 0b11010110, 0b11111110, 0b11101110, 0b01101100, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_58 = { 0b00000000, 0b00000000, 0b00000000, 0b11000110, 0b11000110, 0b01101100, 0b01111100, 0b00111000, 0b00111000, 0b01111100, 0b01101100, 0b11000110, 0b11000110, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_59 = { 0b00000000, 0b00000000, 0b00000000, 0b01100110, 0b01100110, 0b01100110, 0b01100110, 0b00111100, 0b00011000, 0b00011000, 0b00011000, 0b00011000, 0b00111100, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_5A = { 0b00000000, 0b00000000, 0b00000000, 0b11111110, 0b11000110, 0b10000110, 0b00001100, 0b00011000, 0b00110000, 0b01100000, 0b11000010, 0b11000110, 0b11111110, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_5B = { 0b00000000, 0b00000000, 0b00000000, 0b01111000, 0b01100000, 0b01100000, 0b01100000, 0b01100000, 0b01100000, 0b01100000, 0b01100000, 0b01100000, 0b01111000, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_5C = { 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b10000000, 0b11000000, 0b11100000, 0b01110000, 0b00111000, 0b00011100, 0b00001110, 0b00000110, 0b00000010, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_5D = { 0b00000000, 0b00000000, 0b00000000, 0b00111100, 0b00001100, 0b00001100, 0b00001100, 0b00001100, 0b00001100, 0b00001100, 0b00001100, 0b00001100, 0b00111100, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_5E = { 0b00000000, 0b00010000, 0b00111000, 0b01101100, 0b11000110, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_5F = { 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b11111111, 0b00000000 };

        static byte[] ch_60 = { 0b00000000, 0b00110000, 0b00110000, 0b00011000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_61 = { 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b01111000, 0b00001100, 0b01111100, 0b11001100, 0b11001100, 0b11001100, 0b01110110, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_62 = { 0b00000000, 0b00000000, 0b00000000, 0b11100000, 0b01100000, 0b01100000, 0b01111000, 0b01101100, 0b01100110, 0b01100110, 0b01100110, 0b01100110, 0b01111100, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_63 = { 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b01111100, 0b11000110, 0b11000000, 0b11000000, 0b11000000, 0b11000110, 0b01111100, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_64 = { 0b00000000, 0b00000000, 0b00000000, 0b00011100, 0b00001100, 0b00001100, 0b00111100, 0b01101100, 0b11001100, 0b11001100, 0b11001100, 0b11001100, 0b01110110, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_65 = { 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b01111100, 0b11000110, 0b11111110, 0b11000000, 0b11000000, 0b11000110, 0b01111100, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_66 = { 0b00000000, 0b00000000, 0b00000000, 0b00111000, 0b01101100, 0b01100100, 0b01100000, 0b11110000, 0b01100000, 0b01100000, 0b01100000, 0b01100000, 0b11110000, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_67 = { 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b01110110, 0b11001100, 0b11001100, 0b11001100, 0b11001100, 0b01111100, 0b00001100, 0b11001100, 0b01111000, 0b00000000 };
        static byte[] ch_68 = { 0b00000000, 0b00000000, 0b00000000, 0b11100000, 0b01100000, 0b01100000, 0b01101100, 0b01110110, 0b01100110, 0b01100110, 0b01100110, 0b01100110, 0b11100110, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_69 = { 0b00000000, 0b00000000, 0b00000000, 0b00110000, 0b00110000, 0b00000000, 0b01110000, 0b00110000, 0b00110000, 0b00110000, 0b00110000, 0b00110000, 0b01111000, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_6A = { 0b00000000, 0b00000000, 0b00000000, 0b00000110, 0b00000110, 0b00000000, 0b00001110, 0b00000110, 0b00000110, 0b00000110, 0b00000110, 0b00000110, 0b00000110, 0b01100110, 0b01100110, 0b00111100 };
        static byte[] ch_6B = { 0b00000000, 0b00000000, 0b00000000, 0b11100000, 0b01100000, 0b01100000, 0b01100110, 0b01101100, 0b01111000, 0b01111000, 0b01101100, 0b01100110, 0b11100110, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_6C = { 0b00000000, 0b00000000, 0b00000000, 0b00111000, 0b00011000, 0b00011000, 0b00011000, 0b00011000, 0b00011000, 0b00011000, 0b00011000, 0b00011000, 0b00111100, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_6D = { 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b11101100, 0b11111110, 0b11010110, 0b11010110, 0b11010110, 0b11010110, 0b11000110, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_6E = { 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b11011100, 0b01100110, 0b01100110, 0b01100110, 0b01100110, 0b01100110, 0b01100110, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_6F = { 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b01111100, 0b11000110, 0b11000110, 0b11000110, 0b11000110, 0b11000110, 0b01111100, 0b00000000, 0b00000000, 0b00000000 };

        static byte[] ch_70 = { 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b11011100, 0b01100110, 0b01100110, 0b01100110, 0b01100110, 0b01100110, 0b01111100, 0b01100000, 0b01100000, 0b11110000 };
        static byte[] ch_71 = { 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b01110110, 0b11001100, 0b11001100, 0b11001100, 0b11001100, 0b11001100, 0b01111100, 0b00001100, 0b00001100, 0b00011110 };
        static byte[] ch_72 = { 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b11011100, 0b01110110, 0b01100110, 0b01100000, 0b01100000, 0b01100000, 0b11110000, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_73 = { 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b01111100, 0b11000110, 0b01100000, 0b00111000, 0b00001100, 0b11000110, 0b01111100, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_74 = { 0b00000000, 0b00000000, 0b00000000, 0b00010000, 0b00110000, 0b00110000, 0b11111100, 0b00110000, 0b00110000, 0b00110000, 0b00110000, 0b00110110, 0b00011100, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_75 = { 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b11001100, 0b11001100, 0b11001100, 0b11001100, 0b11001100, 0b11001100, 0b01110110, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_76 = { 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b01100110, 0b01100110, 0b01100110, 0b01100110, 0b01100110, 0b00111100, 0b00011000, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_77 = { 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b11000110, 0b11000110, 0b11010110, 0b11010110, 0b11010110, 0b11111110, 0b01101100, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_78 = { 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b11000110, 0b01101100, 0b00111000, 0b00111000, 0b00111000, 0b01101100, 0b11000110, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_79 = { 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b11000110, 0b11000110, 0b11000110, 0b11000110, 0b11000110, 0b11000110, 0b01111110, 0b00000110, 0b00001100, 0b11111000 };
        static byte[] ch_7A = { 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b11111110, 0b11001100, 0b00011000, 0b00110000, 0b01100000, 0b11000110, 0b11111110, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_7B = { 0b00000000, 0b00000000, 0b00000000, 0b00001110, 0b00011000, 0b00011000, 0b00011000, 0b01110000, 0b00011000, 0b00011000, 0b00011000, 0b00011000, 0b00001110, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_7C = { 0b00000000, 0b00000000, 0b00000000, 0b00011000, 0b00011000, 0b00011000, 0b00011000, 0b00000000, 0b00011000, 0b00011000, 0b00011000, 0b00011000, 0b00011000, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_7D = { 0b00000000, 0b00000000, 0b00000000, 0b01110000, 0b00011000, 0b00011000, 0b00011000, 0b00001110, 0b00011000, 0b00011000, 0b00011000, 0b00011000, 0b01110000, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_7E = { 0b00000000, 0b00000000, 0b00000000, 0b01110110, 0b11011100, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000 };
        static byte[] ch_7F = { 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00010000, 0b00111000, 0b01101100, 0b11000110, 0b11000110, 0b11000110, 0b11111110, 0b00000000, 0b00000000, 0b00000000 };

        static byte[] ch_80 = { 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000 };

        /* Laid out as ASCII */
        byte[][] chPatterns = {
            ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,
            ch_20,ch_21,ch_22,ch_23,ch_24,ch_25,ch_26,ch_27,ch_28,ch_29,ch_2A,ch_2B,ch_2C,ch_2D,ch_2E,ch_2F,ch_30,ch_31,ch_32,ch_33,ch_34,ch_35,ch_36,ch_37,ch_38,ch_39,ch_3A,ch_3B,ch_3C,ch_3D,ch_3E,ch_3F,
            ch_40,ch_41,ch_42,ch_43,ch_44,ch_45,ch_46,ch_47,ch_48,ch_49,ch_4A,ch_4B,ch_4C,ch_4D,ch_4E,ch_4F,ch_50,ch_51,ch_52,ch_53,ch_54,ch_55,ch_56,ch_57,ch_58,ch_59,ch_5A,ch_5B,ch_5C,ch_5D,ch_5E,ch_5F,
            ch_60,ch_61,ch_62,ch_63,ch_64,ch_65,ch_66,ch_67,ch_68,ch_69,ch_6A,ch_6B,ch_6C,ch_6D,ch_6E,ch_6F,ch_70,ch_71,ch_72,ch_73,ch_74,ch_75,ch_76,ch_77,ch_78,ch_79,ch_7A,ch_7B,ch_7C,ch_7D,ch_7E,ch_7F,
            ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,
            ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,
            ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,
            ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,ch_20,
        };


        static byte[] cursor = {
            0b00000000,
            0b00000000,
            0b00000000,
            0b00000000,
            0b00000000,
            0b00000000,
            0b00000000,
            0b00000000,
            0b00000000,
            0b00000000,
            0b00000000,
            0b00000000,
            0b00000000,
            0b11111111,
            0b11111111,
            0b11111111
        };


        static byte[,] test = new byte[,] {
            {0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0},
            {1,1,1,1,1,1,1,1},
            {1,1,1,1,1,1,1,1},
            {1,1,1,1,1,1,1,1}
        };


        Bitmap chBitmap = new Bitmap(8, 16);
        Rectangle rect = new Rectangle(0, 0, 8, 16);

        Bitmap[] bitmaps = new Bitmap[256];

        Color[] vgaColors = {
            Color.Black,    Color.DarkBlue, Color.DarkGreen, Color.DarkCyan, Color.DarkRed, Color.DarkMagenta, Color.Brown,  Color.LightGray,
            Color.DarkGray, Color.Blue,     Color.Green,     Color.Cyan,     Color.Red,     Color.Magenta,     Color.Yellow, Color.White 
        };

        private void DrawBitmapPattern(int left, int top, int fgColor, int bgColor, byte[] chPattern) {
            BitmapData data = chBitmap.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            Color fg = vgaColors[fgColor];
            Color bg = vgaColors[bgColor];
            Color c = bg;

            unsafe {
                byte* ptr = (byte*)data.Scan0;
                byte bits = 0;

                // for (int y = 0; y < 16; ++y)
                // Parallel.For(0, 16, y =>
                /* As you can see from the above comments, I've tried a couple of different approaches to 
                the following code. I've settled on unrolling the outer loop (walking down the list of bytes 
                that make up the rows of the bitmap) and the inner loop (walking through the bits of each 
                row). I've also placed the bitmaps into a native array for speed, and I've used LockBits to 
                allow me to create the bitmap all at once rather than calling SetPixel for each bit. */
                {
                    /* Start at the top row of bits (chPattern[0x00]). Walk through each bit in the row, one 
                    bit per line, and set the color based on whether the bit is on (foreground color) or off 
                    (background color). */
                    bits = chPattern[0x00];
                    c = (bits & 0x80) == 0x80 ? fg : bg; ptr[0x00] = c.B; ptr[0x01] = c.G; ptr[0x02] = c.R; 
                    c = (bits & 0x40) == 0x40 ? fg : bg; ptr[0x03] = c.B; ptr[0x04] = c.G; ptr[0x05] = c.R; 
                    c = (bits & 0x20) == 0x20 ? fg : bg; ptr[0x06] = c.B; ptr[0x07] = c.G; ptr[0x08] = c.R; 
                    c = (bits & 0x10) == 0x10 ? fg : bg; ptr[0x09] = c.B; ptr[0x0A] = c.G; ptr[0x0B] = c.R;
                    c = (bits & 0x08) == 0x08 ? fg : bg; ptr[0x0C] = c.B; ptr[0x0D] = c.G; ptr[0x0E] = c.R; 
                    c = (bits & 0x04) == 0x04 ? fg : bg; ptr[0x0F] = c.B; ptr[0x10] = c.G; ptr[0x11] = c.R; 
                    c = (bits & 0x02) == 0x02 ? fg : bg; ptr[0x12] = c.B; ptr[0x13] = c.G; ptr[0x14] = c.R; 
                    c = (bits & 0x01) == 0x01 ? fg : bg; ptr[0x15] = c.B; ptr[0x16] = c.G; ptr[0x17] = c.R; 
                    ptr += data.Stride;

                    /* Same as above, next row, and so on. */
                    bits = chPattern[0x01];
                    c = (bits & 0x80) == 0x80 ? fg : bg; ptr[0x00] = c.B; ptr[0x01] = c.G; ptr[0x02] = c.R;
                    c = (bits & 0x40) == 0x40 ? fg : bg; ptr[0x03] = c.B; ptr[0x04] = c.G; ptr[0x05] = c.R;
                    c = (bits & 0x20) == 0x20 ? fg : bg; ptr[0x06] = c.B; ptr[0x07] = c.G; ptr[0x08] = c.R;
                    c = (bits & 0x10) == 0x10 ? fg : bg; ptr[0x09] = c.B; ptr[0x0A] = c.G; ptr[0x0B] = c.R;
                    c = (bits & 0x08) == 0x08 ? fg : bg; ptr[0x0C] = c.B; ptr[0x0D] = c.G; ptr[0x0E] = c.R;
                    c = (bits & 0x04) == 0x04 ? fg : bg; ptr[0x0F] = c.B; ptr[0x10] = c.G; ptr[0x11] = c.R;
                    c = (bits & 0x02) == 0x02 ? fg : bg; ptr[0x12] = c.B; ptr[0x13] = c.G; ptr[0x14] = c.R;
                    c = (bits & 0x01) == 0x01 ? fg : bg; ptr[0x15] = c.B; ptr[0x16] = c.G; ptr[0x17] = c.R;
                    ptr += data.Stride;

                    bits = chPattern[0x02];
                    c = (bits & 0x80) == 0x80 ? fg : bg; ptr[0x00] = c.B; ptr[0x01] = c.G; ptr[0x02] = c.R;
                    c = (bits & 0x40) == 0x40 ? fg : bg; ptr[0x03] = c.B; ptr[0x04] = c.G; ptr[0x05] = c.R;
                    c = (bits & 0x20) == 0x20 ? fg : bg; ptr[0x06] = c.B; ptr[0x07] = c.G; ptr[0x08] = c.R;
                    c = (bits & 0x10) == 0x10 ? fg : bg; ptr[0x09] = c.B; ptr[0x0A] = c.G; ptr[0x0B] = c.R;
                    c = (bits & 0x08) == 0x08 ? fg : bg; ptr[0x0C] = c.B; ptr[0x0D] = c.G; ptr[0x0E] = c.R;
                    c = (bits & 0x04) == 0x04 ? fg : bg; ptr[0x0F] = c.B; ptr[0x10] = c.G; ptr[0x11] = c.R;
                    c = (bits & 0x02) == 0x02 ? fg : bg; ptr[0x12] = c.B; ptr[0x13] = c.G; ptr[0x14] = c.R;
                    c = (bits & 0x01) == 0x01 ? fg : bg; ptr[0x15] = c.B; ptr[0x16] = c.G; ptr[0x17] = c.R;
                    ptr += data.Stride;

                    bits = chPattern[0x03];
                    c = (bits & 0x80) == 0x80 ? fg : bg; ptr[0x00] = c.B; ptr[0x01] = c.G; ptr[0x02] = c.R;
                    c = (bits & 0x40) == 0x40 ? fg : bg; ptr[0x03] = c.B; ptr[0x04] = c.G; ptr[0x05] = c.R;
                    c = (bits & 0x20) == 0x20 ? fg : bg; ptr[0x06] = c.B; ptr[0x07] = c.G; ptr[0x08] = c.R;
                    c = (bits & 0x10) == 0x10 ? fg : bg; ptr[0x09] = c.B; ptr[0x0A] = c.G; ptr[0x0B] = c.R;
                    c = (bits & 0x08) == 0x08 ? fg : bg; ptr[0x0C] = c.B; ptr[0x0D] = c.G; ptr[0x0E] = c.R;
                    c = (bits & 0x04) == 0x04 ? fg : bg; ptr[0x0F] = c.B; ptr[0x10] = c.G; ptr[0x11] = c.R;
                    c = (bits & 0x02) == 0x02 ? fg : bg; ptr[0x12] = c.B; ptr[0x13] = c.G; ptr[0x14] = c.R;
                    c = (bits & 0x01) == 0x01 ? fg : bg; ptr[0x15] = c.B; ptr[0x16] = c.G; ptr[0x17] = c.R;
                    ptr += data.Stride;

                    bits = chPattern[0x04];
                    c = (bits & 0x80) == 0x80 ? fg : bg; ptr[0x00] = c.B; ptr[0x01] = c.G; ptr[0x02] = c.R;
                    c = (bits & 0x40) == 0x40 ? fg : bg; ptr[0x03] = c.B; ptr[0x04] = c.G; ptr[0x05] = c.R;
                    c = (bits & 0x20) == 0x20 ? fg : bg; ptr[0x06] = c.B; ptr[0x07] = c.G; ptr[0x08] = c.R;
                    c = (bits & 0x10) == 0x10 ? fg : bg; ptr[0x09] = c.B; ptr[0x0A] = c.G; ptr[0x0B] = c.R;
                    c = (bits & 0x08) == 0x08 ? fg : bg; ptr[0x0C] = c.B; ptr[0x0D] = c.G; ptr[0x0E] = c.R;
                    c = (bits & 0x04) == 0x04 ? fg : bg; ptr[0x0F] = c.B; ptr[0x10] = c.G; ptr[0x11] = c.R;
                    c = (bits & 0x02) == 0x02 ? fg : bg; ptr[0x12] = c.B; ptr[0x13] = c.G; ptr[0x14] = c.R;
                    c = (bits & 0x01) == 0x01 ? fg : bg; ptr[0x15] = c.B; ptr[0x16] = c.G; ptr[0x17] = c.R;
                    ptr += data.Stride;

                    bits = chPattern[0x05];
                    c = (bits & 0x80) == 0x80 ? fg : bg; ptr[0x00] = c.B; ptr[0x01] = c.G; ptr[0x02] = c.R;
                    c = (bits & 0x40) == 0x40 ? fg : bg; ptr[0x03] = c.B; ptr[0x04] = c.G; ptr[0x05] = c.R;
                    c = (bits & 0x20) == 0x20 ? fg : bg; ptr[0x06] = c.B; ptr[0x07] = c.G; ptr[0x08] = c.R;
                    c = (bits & 0x10) == 0x10 ? fg : bg; ptr[0x09] = c.B; ptr[0x0A] = c.G; ptr[0x0B] = c.R;
                    c = (bits & 0x08) == 0x08 ? fg : bg; ptr[0x0C] = c.B; ptr[0x0D] = c.G; ptr[0x0E] = c.R;
                    c = (bits & 0x04) == 0x04 ? fg : bg; ptr[0x0F] = c.B; ptr[0x10] = c.G; ptr[0x11] = c.R;
                    c = (bits & 0x02) == 0x02 ? fg : bg; ptr[0x12] = c.B; ptr[0x13] = c.G; ptr[0x14] = c.R;
                    c = (bits & 0x01) == 0x01 ? fg : bg; ptr[0x15] = c.B; ptr[0x16] = c.G; ptr[0x17] = c.R;
                    ptr += data.Stride;

                    bits = chPattern[0x06];
                    c = (bits & 0x80) == 0x80 ? fg : bg; ptr[0x00] = c.B; ptr[0x01] = c.G; ptr[0x02] = c.R;
                    c = (bits & 0x40) == 0x40 ? fg : bg; ptr[0x03] = c.B; ptr[0x04] = c.G; ptr[0x05] = c.R;
                    c = (bits & 0x20) == 0x20 ? fg : bg; ptr[0x06] = c.B; ptr[0x07] = c.G; ptr[0x08] = c.R;
                    c = (bits & 0x10) == 0x10 ? fg : bg; ptr[0x09] = c.B; ptr[0x0A] = c.G; ptr[0x0B] = c.R;
                    c = (bits & 0x08) == 0x08 ? fg : bg; ptr[0x0C] = c.B; ptr[0x0D] = c.G; ptr[0x0E] = c.R;
                    c = (bits & 0x04) == 0x04 ? fg : bg; ptr[0x0F] = c.B; ptr[0x10] = c.G; ptr[0x11] = c.R;
                    c = (bits & 0x02) == 0x02 ? fg : bg; ptr[0x12] = c.B; ptr[0x13] = c.G; ptr[0x14] = c.R;
                    c = (bits & 0x01) == 0x01 ? fg : bg; ptr[0x15] = c.B; ptr[0x16] = c.G; ptr[0x17] = c.R;
                    ptr += data.Stride;

                    bits = chPattern[0x07];
                    c = (bits & 0x80) == 0x80 ? fg : bg; ptr[0x00] = c.B; ptr[0x01] = c.G; ptr[0x02] = c.R;
                    c = (bits & 0x40) == 0x40 ? fg : bg; ptr[0x03] = c.B; ptr[0x04] = c.G; ptr[0x05] = c.R;
                    c = (bits & 0x20) == 0x20 ? fg : bg; ptr[0x06] = c.B; ptr[0x07] = c.G; ptr[0x08] = c.R;
                    c = (bits & 0x10) == 0x10 ? fg : bg; ptr[0x09] = c.B; ptr[0x0A] = c.G; ptr[0x0B] = c.R;
                    c = (bits & 0x08) == 0x08 ? fg : bg; ptr[0x0C] = c.B; ptr[0x0D] = c.G; ptr[0x0E] = c.R;
                    c = (bits & 0x04) == 0x04 ? fg : bg; ptr[0x0F] = c.B; ptr[0x10] = c.G; ptr[0x11] = c.R;
                    c = (bits & 0x02) == 0x02 ? fg : bg; ptr[0x12] = c.B; ptr[0x13] = c.G; ptr[0x14] = c.R;
                    c = (bits & 0x01) == 0x01 ? fg : bg; ptr[0x15] = c.B; ptr[0x16] = c.G; ptr[0x17] = c.R;
                    ptr += data.Stride;

                    bits = chPattern[0x08];
                    c = (bits & 0x80) == 0x80 ? fg : bg; ptr[0x00] = c.B; ptr[0x01] = c.G; ptr[0x02] = c.R;
                    c = (bits & 0x40) == 0x40 ? fg : bg; ptr[0x03] = c.B; ptr[0x04] = c.G; ptr[0x05] = c.R;
                    c = (bits & 0x20) == 0x20 ? fg : bg; ptr[0x06] = c.B; ptr[0x07] = c.G; ptr[0x08] = c.R;
                    c = (bits & 0x10) == 0x10 ? fg : bg; ptr[0x09] = c.B; ptr[0x0A] = c.G; ptr[0x0B] = c.R;
                    c = (bits & 0x08) == 0x08 ? fg : bg; ptr[0x0C] = c.B; ptr[0x0D] = c.G; ptr[0x0E] = c.R;
                    c = (bits & 0x04) == 0x04 ? fg : bg; ptr[0x0F] = c.B; ptr[0x10] = c.G; ptr[0x11] = c.R;
                    c = (bits & 0x02) == 0x02 ? fg : bg; ptr[0x12] = c.B; ptr[0x13] = c.G; ptr[0x14] = c.R;
                    c = (bits & 0x01) == 0x01 ? fg : bg; ptr[0x15] = c.B; ptr[0x16] = c.G; ptr[0x17] = c.R;
                    ptr += data.Stride;

                    bits = chPattern[0x09];
                    c = (bits & 0x80) == 0x80 ? fg : bg; ptr[0x00] = c.B; ptr[0x01] = c.G; ptr[0x02] = c.R;
                    c = (bits & 0x40) == 0x40 ? fg : bg; ptr[0x03] = c.B; ptr[0x04] = c.G; ptr[0x05] = c.R;
                    c = (bits & 0x20) == 0x20 ? fg : bg; ptr[0x06] = c.B; ptr[0x07] = c.G; ptr[0x08] = c.R;
                    c = (bits & 0x10) == 0x10 ? fg : bg; ptr[0x09] = c.B; ptr[0x0A] = c.G; ptr[0x0B] = c.R;
                    c = (bits & 0x08) == 0x08 ? fg : bg; ptr[0x0C] = c.B; ptr[0x0D] = c.G; ptr[0x0E] = c.R;
                    c = (bits & 0x04) == 0x04 ? fg : bg; ptr[0x0F] = c.B; ptr[0x10] = c.G; ptr[0x11] = c.R;
                    c = (bits & 0x02) == 0x02 ? fg : bg; ptr[0x12] = c.B; ptr[0x13] = c.G; ptr[0x14] = c.R;
                    c = (bits & 0x01) == 0x01 ? fg : bg; ptr[0x15] = c.B; ptr[0x16] = c.G; ptr[0x17] = c.R;
                    ptr += data.Stride;

                    bits = chPattern[0x0A];
                    c = (bits & 0x80) == 0x80 ? fg : bg; ptr[0x00] = c.B; ptr[0x01] = c.G; ptr[0x02] = c.R;
                    c = (bits & 0x40) == 0x40 ? fg : bg; ptr[0x03] = c.B; ptr[0x04] = c.G; ptr[0x05] = c.R;
                    c = (bits & 0x20) == 0x20 ? fg : bg; ptr[0x06] = c.B; ptr[0x07] = c.G; ptr[0x08] = c.R;
                    c = (bits & 0x10) == 0x10 ? fg : bg; ptr[0x09] = c.B; ptr[0x0A] = c.G; ptr[0x0B] = c.R;
                    c = (bits & 0x08) == 0x08 ? fg : bg; ptr[0x0C] = c.B; ptr[0x0D] = c.G; ptr[0x0E] = c.R;
                    c = (bits & 0x04) == 0x04 ? fg : bg; ptr[0x0F] = c.B; ptr[0x10] = c.G; ptr[0x11] = c.R;
                    c = (bits & 0x02) == 0x02 ? fg : bg; ptr[0x12] = c.B; ptr[0x13] = c.G; ptr[0x14] = c.R;
                    c = (bits & 0x01) == 0x01 ? fg : bg; ptr[0x15] = c.B; ptr[0x16] = c.G; ptr[0x17] = c.R;
                    ptr += data.Stride;

                    bits = chPattern[0x0B];
                    c = (bits & 0x80) == 0x80 ? fg : bg; ptr[0x00] = c.B; ptr[0x01] = c.G; ptr[0x02] = c.R;
                    c = (bits & 0x40) == 0x40 ? fg : bg; ptr[0x03] = c.B; ptr[0x04] = c.G; ptr[0x05] = c.R;
                    c = (bits & 0x20) == 0x20 ? fg : bg; ptr[0x06] = c.B; ptr[0x07] = c.G; ptr[0x08] = c.R;
                    c = (bits & 0x10) == 0x10 ? fg : bg; ptr[0x09] = c.B; ptr[0x0A] = c.G; ptr[0x0B] = c.R;
                    c = (bits & 0x08) == 0x08 ? fg : bg; ptr[0x0C] = c.B; ptr[0x0D] = c.G; ptr[0x0E] = c.R;
                    c = (bits & 0x04) == 0x04 ? fg : bg; ptr[0x0F] = c.B; ptr[0x10] = c.G; ptr[0x11] = c.R;
                    c = (bits & 0x02) == 0x02 ? fg : bg; ptr[0x12] = c.B; ptr[0x13] = c.G; ptr[0x14] = c.R;
                    c = (bits & 0x01) == 0x01 ? fg : bg; ptr[0x15] = c.B; ptr[0x16] = c.G; ptr[0x17] = c.R;
                    ptr += data.Stride;

                    bits = chPattern[0x0C];
                    c = (bits & 0x80) == 0x80 ? fg : bg; ptr[0x00] = c.B; ptr[0x01] = c.G; ptr[0x02] = c.R;
                    c = (bits & 0x40) == 0x40 ? fg : bg; ptr[0x03] = c.B; ptr[0x04] = c.G; ptr[0x05] = c.R;
                    c = (bits & 0x20) == 0x20 ? fg : bg; ptr[0x06] = c.B; ptr[0x07] = c.G; ptr[0x08] = c.R;
                    c = (bits & 0x10) == 0x10 ? fg : bg; ptr[0x09] = c.B; ptr[0x0A] = c.G; ptr[0x0B] = c.R;
                    c = (bits & 0x08) == 0x08 ? fg : bg; ptr[0x0C] = c.B; ptr[0x0D] = c.G; ptr[0x0E] = c.R;
                    c = (bits & 0x04) == 0x04 ? fg : bg; ptr[0x0F] = c.B; ptr[0x10] = c.G; ptr[0x11] = c.R;
                    c = (bits & 0x02) == 0x02 ? fg : bg; ptr[0x12] = c.B; ptr[0x13] = c.G; ptr[0x14] = c.R;
                    c = (bits & 0x01) == 0x01 ? fg : bg; ptr[0x15] = c.B; ptr[0x16] = c.G; ptr[0x17] = c.R;
                    ptr += data.Stride;

                    bits = chPattern[0x0D];
                    c = (bits & 0x80) == 0x80 ? fg : bg; ptr[0x00] = c.B; ptr[0x01] = c.G; ptr[0x02] = c.R;
                    c = (bits & 0x40) == 0x40 ? fg : bg; ptr[0x03] = c.B; ptr[0x04] = c.G; ptr[0x05] = c.R;
                    c = (bits & 0x20) == 0x20 ? fg : bg; ptr[0x06] = c.B; ptr[0x07] = c.G; ptr[0x08] = c.R;
                    c = (bits & 0x10) == 0x10 ? fg : bg; ptr[0x09] = c.B; ptr[0x0A] = c.G; ptr[0x0B] = c.R;
                    c = (bits & 0x08) == 0x08 ? fg : bg; ptr[0x0C] = c.B; ptr[0x0D] = c.G; ptr[0x0E] = c.R;
                    c = (bits & 0x04) == 0x04 ? fg : bg; ptr[0x0F] = c.B; ptr[0x10] = c.G; ptr[0x11] = c.R;
                    c = (bits & 0x02) == 0x02 ? fg : bg; ptr[0x12] = c.B; ptr[0x13] = c.G; ptr[0x14] = c.R;
                    c = (bits & 0x01) == 0x01 ? fg : bg; ptr[0x15] = c.B; ptr[0x16] = c.G; ptr[0x17] = c.R;
                    ptr += data.Stride;

                    bits = chPattern[0x0E];
                    c = (bits & 0x80) == 0x80 ? fg : bg; ptr[0x00] = c.B; ptr[0x01] = c.G; ptr[0x02] = c.R;
                    c = (bits & 0x40) == 0x40 ? fg : bg; ptr[0x03] = c.B; ptr[0x04] = c.G; ptr[0x05] = c.R;
                    c = (bits & 0x20) == 0x20 ? fg : bg; ptr[0x06] = c.B; ptr[0x07] = c.G; ptr[0x08] = c.R;
                    c = (bits & 0x10) == 0x10 ? fg : bg; ptr[0x09] = c.B; ptr[0x0A] = c.G; ptr[0x0B] = c.R;
                    c = (bits & 0x08) == 0x08 ? fg : bg; ptr[0x0C] = c.B; ptr[0x0D] = c.G; ptr[0x0E] = c.R;
                    c = (bits & 0x04) == 0x04 ? fg : bg; ptr[0x0F] = c.B; ptr[0x10] = c.G; ptr[0x11] = c.R;
                    c = (bits & 0x02) == 0x02 ? fg : bg; ptr[0x12] = c.B; ptr[0x13] = c.G; ptr[0x14] = c.R;
                    c = (bits & 0x01) == 0x01 ? fg : bg; ptr[0x15] = c.B; ptr[0x16] = c.G; ptr[0x17] = c.R;
                    ptr += data.Stride;

                    bits = chPattern[0x0F];
                    c = (bits & 0x80) == 0x80 ? fg : bg; ptr[0x00] = c.B; ptr[0x01] = c.G; ptr[0x02] = c.R;
                    c = (bits & 0x40) == 0x40 ? fg : bg; ptr[0x03] = c.B; ptr[0x04] = c.G; ptr[0x05] = c.R;
                    c = (bits & 0x20) == 0x20 ? fg : bg; ptr[0x06] = c.B; ptr[0x07] = c.G; ptr[0x08] = c.R;
                    c = (bits & 0x10) == 0x10 ? fg : bg; ptr[0x09] = c.B; ptr[0x0A] = c.G; ptr[0x0B] = c.R;
                    c = (bits & 0x08) == 0x08 ? fg : bg; ptr[0x0C] = c.B; ptr[0x0D] = c.G; ptr[0x0E] = c.R;
                    c = (bits & 0x04) == 0x04 ? fg : bg; ptr[0x0F] = c.B; ptr[0x10] = c.G; ptr[0x11] = c.R;
                    c = (bits & 0x02) == 0x02 ? fg : bg; ptr[0x12] = c.B; ptr[0x13] = c.G; ptr[0x14] = c.R;
                    c = (bits & 0x01) == 0x01 ? fg : bg; ptr[0x15] = c.B; ptr[0x16] = c.G; ptr[0x17] = c.R;
                    ptr += data.Stride;
                }
                // );
            }

            chBitmap.UnlockBits(data);

            int pxLeft = left * 8;
            int pxTop = top * 16;
            consoleGraphics.DrawImage(chBitmap, pxLeft, pxTop);
        }

        public void DrawCharacter(char ch, int left, int top, int fgColor, int bgColor) {
            // BeginInvoke((Action)(() => {
                byte[] chPattern = chPatterns[ch];
                DrawBitmapPattern(left, top, fgColor, bgColor, chPattern);
            // }));
        }

        private void DrawCursor(int left, int top, int fgColor, int bgColor) {
            BeginInvoke((Action)(() => {
                DrawBitmapPattern(left, top, fgColor, bgColor, cursor);
            }));
        }

        public void Clear() {
            CursorLocation = 0;
            consoleGraphics.Clear(Color.Black);
            Console.Clear();
        }

        private void TortillaConsole_Shown(object sender, EventArgs e) {
            consoleGraphics = pictureBox.CreateGraphics();
            pictureBox.BackColor = Color.Black;
            Clear();
            DataBusSet = true;
            cursorTimer.Start();
        }

        byte[] Memory = new byte[0xf02];

        public event Action<IBusComponent> RequestTickExecute;
        public event Action<IBusComponent> RequestTickUpdate;
        public event Action<IBusComponent> RequestTickEnableToAddressBus;
        public event Action<IBusComponent> RequestTickEnableToDataBus;
        public event Action<IBusComponent> RequestTickEnableToIOBus;
        public event Action<IBusComponent> RequestTickSetFromAddressBus;
        public event Action<IBusComponent> RequestTickSetFromDataBus;
        public event Action<IBusComponent> RequestTickSetFromIOBus;
        public event Action<IBusComponent> RequestTickStore;
        public event Action<IBusComponent> OnRegisterTickLoad;

        public void RegisterTickExecute() {
            RequestTickExecute?.Invoke(this);
        }

        public void RegisterTickUpdate() {
            RequestTickUpdate?.Invoke(this);
        }

        public void RegisterTickEnableToAddressBus() {
            RequestTickEnableToAddressBus?.Invoke(this);
        }

        public void RegisterTickEnableToDataBus() {
            RequestTickEnableToDataBus?.Invoke(this);
        }

        public void RegisterTickEnableToIOBus() {
            RequestTickEnableToIOBus?.Invoke(this);
        }

        public void RegisterTickSetFromAddressBus() {
            RequestTickSetFromAddressBus?.Invoke(this);
        }

        public void RegisterTickSetFromDataBus() {
            RequestTickSetFromDataBus?.Invoke(this);
        }

        public void RegisterTickSetFromIOBus() {
            RequestTickSetFromIOBus?.Invoke(this);
        }
        public void RegisterTickStore() {
            RequestTickStore?.Invoke(this);
        }

        public void RegisterTickLoad() {
            OnRegisterTickLoad?.Invoke(this);
        }


        UInt32 AddressValue { get; set; }
        UInt64 DataValue { get; set; }

        public bool DataBusEnabled { get; protected set; }
        public bool DataBusSet { get; protected set; } = true;

        public bool AddressBusEnabled { get; protected set; }
        public bool AddressBusSet { get; protected set; }
        public bool IOBusEnabled { get; protected set; }
        public bool IOBusSet { get; protected set; }

        public bool IsEnabled { get { return IOBusEnabled; } }
        public bool IsSet { get { return IOBusSet; } }

        public void EnableToAddressBus(SubRegister subReg) {
        }

        public void EnableToDataBus(SubRegister subReg) {
        }

        public void EnableToIOBus(SubRegister subReg) {
            BusData.EnableToIOBus(subReg);
            IOBusEnabled = true;
            RegisterTickEnableToIOBus();
        }

        public void SetFromAddressBus(SubRegister subReg) {
        }

        public void SetFromDataBus(SubRegister subReg) {
        }

        public void SetFromIOBus(SubRegister subReg) {
            IOBusSet = true;
            RegisterTickSetFromIOBus();
        }


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

        IMotherboard<UInt64> MB { get; set; }
        UInt64 InterruptID { get; set; }

        public IDataBus<UInt64> DataBus { get; set; }
        public IDataBus<UInt64> AddressBus { get; set; }
        public IDataBus<UInt64> IOBus { get; set; }

        public IBusComponent PrivilegeFlags { get; set; }

        public void OnTickUpdate(IBusComponent cpuFlags) {
        }

        public void OnTickEnable(IBusComponent cpuFlags) {
        }

        public void OnTickEnableToAddressBus(IBusComponent cpuFlags) {
        }

        public void OnTickEnableToDataBus(IBusComponent cpuFlags) {
        }

        public void OnTickEnableToIOBus(IBusComponent cpuFlags) {
        }

        public void OnTickSet(IBusComponent cpuFlags) {
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
        }

        public void OnTickSetFromAddressBus(IBusComponent cpuFlags) {
        }

        public void OnTickSetFromDataBus(IBusComponent cpuFlags) {
        }

        public void OnTickSetFromIOBus(IBusComponent cpuFlags) {
        }

        public virtual void OnTickLoad(IBusComponent cpuFlags) {
        }

        public void OnTickStore(IBusComponent cpuFlags) {
        }

        public void OnTickExecute(IBusComponent cpuFlags) {
        }

        Maize.Register BusData { get; set; } = new Maize.Register();

        private void SetCursorLocation(RegValue busValue) {
            CursorLocation = busValue.H1;
        }

        private void WriteCharacter(char ch, int left, int top, int fgColor, int bgColor, int count) {
            ConWriteCharacter(ch, left, top, fgColor, bgColor, count);

            CursorLocation = (uint)(left + (top * 80));
            RegValue tmp = 0;
            tmp.B0 = (byte)ch;
            tmp.B1 = (byte)((bgColor << 4) & ((fgColor & 0x0F) & 0xFF));
            cursorTimer.Stop();

            do {
                DrawCharacter(ch, left, top, fgColor, bgColor);
                // var address = CursorLocation * 2;
                // WriteDoubleByte(address, tmp.Q0);
                ++CursorLocation;
                --count;

                if (count > 0) {
                    int offset = (int)CursorLocation;
                    left = offset % 80;
                    top = offset / 80;
                }
            } while (count > 0);

            cursorTimer.Start();
        }

        private void ConWriteCharacter(char ch, int left, int top, int fgColor, int bgColor, int count) {
            do {
                Console.Write(ch);
                --count;
            } while (count > 0);
        }


        // WHOLEWRD
        // HLF1HLF0
        // Q3Q2Q1Q0
        // 76543210

        // B0 = opcode
        // B1 = repeat count
        // B2 = character
        // B3 = color
        // B4 = x position
        // B5 = y position

        private void WriteCharacterAtCursorPosition(RegValue busValue) {
            int offset = (int)CursorLocation;
            int left = offset % 80;
            int top = offset / 80;
            char ch = (char)busValue.B2;

            WriteCharacter(ch, left, top, DefaultFgColor, DefaultBgColor, busValue.B1);
        }

        private void WriteCharacterAt(RegValue busValue) {
            int left = busValue.B4;
            int top = busValue.B5;
            char ch = (char)busValue.B2;

            WriteCharacter(ch, left, top, DefaultFgColor, DefaultBgColor, busValue.B1);
        }

        private void WriteCharacterAndColor(RegValue busValue) {
            int offset = (int)CursorLocation;
            int left = offset % 80;
            int top = offset / 80;
            char ch = (char)busValue.B2;
            char co = (char)busValue.B3;
            int fgColor = (co & 0x0f);
            int bgColor = (co & 0xf0) >> 4;

            WriteCharacter(ch, left, top, fgColor, bgColor, busValue.B1);
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

        private void CursorTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
            CursorOn = !CursorOn;
            int fgColor = DefaultBgColor;

            if (CursorOn) {
                fgColor = DefaultFgColor;
            }

            int offset = (int)CursorLocation;
            int top = offset / 80;
            int left = offset % 80;
            DrawCursor(left, top, fgColor, DefaultBgColor);
        }

        int DefaultFgColor { get; set; } = 0x07;
        int DefaultBgColor { get; set; } = 0x00;
        uint CursorLocation { get; set; } = 0x00000000;

        public bool AccessCheck(IBusComponent flags) => true;

        private void SetForegroundColor(RegValue busValue) {
            char co = (char)busValue.B3;
            int fgColor = (co & 0x0f);
            DefaultFgColor = fgColor;
        }

        private void SetBackgroundColor(RegValue busValue) {
            char co = (char)busValue.B3;
            int bgColor = (co & 0xf0) >> 4;
            DefaultBgColor = bgColor;
        }

        public byte ReadByte(UInt32 address) {
            return Memory[address];
        }

        public void WriteByte(UInt32 address, byte value) {
            Memory[address] = value;
        }

        public void WriteDoubleByte(UInt32 address, UInt16 value) {
            Maize.RegValue tmp = value;
            WriteByte(address, tmp.B0);
            WriteByte(address + 1, tmp.B1);
        }
    }

}
