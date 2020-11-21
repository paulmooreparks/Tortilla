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

namespace TortillaUI {
    public partial class TortillaConsole : Form, Tortilla.IBusComponent {
        UserSettings us = new UserSettings();

        public TortillaConsole() {
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Load += TortillaConsole_Load;
            SizeChanged += TortillaConsole_SizeChanged;
            InitBitmaps();
            clockTimer.Elapsed += ClockTimer_Elapsed;
            clockTimer.Start();
            this.KeyDown += TortillaConsole_KeyDown;
        }

        private bool CursorOn { get; set; }

        System.Timers.Timer clockTimer = new System.Timers.Timer(500);

        private void TortillaConsole_SizeChanged(object sender, EventArgs e) {
            SaveWindowPosition();
        }

        private void TortillaConsole_Load(object sender, EventArgs e) {
            RestoreWindowPosition();
            Move += TortillaConsole_Move;
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



        Bitmap chBitmap = new Bitmap(8, 16);

        Bitmap[] bitmaps = new Bitmap[256];

        Color[] vgaColors = {
            Color.Black, Color.DarkBlue, Color.DarkGreen, Color.DarkCyan, Color.DarkRed, Color.DarkMagenta, Color.Brown, Color.LightGray,
            Color.DarkGray, Color.Blue, Color.Green, Color.Cyan, Color.Red, Color.Magenta, Color.Yellow, Color.White };

        /* This is really, really slow, and it doesn't appear to match VGA character size and resolution, 
        but it works well enough for now. I'll revisit all of this later. */
        public void DrawCharacter(char ch, int left, int top, int fgColor, int bgColor) {
            BeginInvoke((Action)(() => { 
                int pxLeft = left * 8;
                int pxTop = top * 16;
                byte[] chPattern = chPatterns[ch];
                Color cbgColor = vgaColors[bgColor];
                Color cfgColor = vgaColors[fgColor];

                int y = 0;

                for (int i = 0; i < 16; ++i) {
                    byte bits = chPattern[i];
                    byte mask = 0b_1000_0000;

                    for (int x = 0; x < 8; ++x) {
                        if ((bits & mask) == mask) {
                            chBitmap.SetPixel(x, y, cfgColor);
                        }
                        else {
                            chBitmap.SetPixel(x, y, cbgColor);
                        }

                        mask >>= 1;
                    }

                    ++y;
                }

                consoleGraphics.DrawImage(chBitmap, pxLeft, pxTop);
            }));
        }

        void InitBitmaps() {
            for (int i = 0; i < 256; ++i) {

            }
        }


        public void Clear() {
            CursorLocation = 0;
            consoleGraphics.Clear(Color.Black);
        }

        private void TortillaConsole_Shown(object sender, EventArgs e) {
            consoleGraphics = pictureBox.CreateGraphics();
            pictureBox.BackColor = Color.Black;
            Clear();
            DataBusSet = true;
        }

        byte[] Memory = new byte[0xf02];

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
        int InterruptID { get; set; }

        public IDataBus<UInt64> DataBus { get; set; }
        public IDataBus<UInt64> AddressBus { get; set; }
        public IDataBus<UInt64> IOBus { get; set; }

        public void OnTick(ClockState state) {
            switch (state) {
            case ClockState.TickSet:
                Maize.RegValue busValue = IOBus.Value;
                var opcode = busValue.B0;

                switch (opcode) {
                case 0x00:
                    Clear();
                    break;

                case 0x01:
                    WriteCharacter(busValue);
                    break;

                case 0x02:
                    WriteCharacterAndColor(busValue);
                    break;

                case 0x03:
                    WriteCharacterAt(busValue);
                    break;

                case 0x04:
                    WriteCharacterAndColorAt(busValue);
                    break;

                case 0x05:
                    SetForegroundColor(busValue);
                    break;

                case 0x06:
                    SetBackgroundColor(busValue);
                    break;

                case 0x07:
                    SetCursorLocation(busValue);
                    break;

                case 0x08:
                    GetKeyCode(busValue);
                    break;
                }

                IOBusSet = false;

                break;
            }
        }

        MaizeRegister BusData { get; set; } = new MaizeRegister();

        private void TortillaConsole_KeyDown(object sender, KeyEventArgs e) {
            KeyCode = e.KeyCode;
            BusData.Value = (ulong)KeyCode;
            MB?.RaiseInterrupt(InterruptID);
        }

        protected Keys KeyCode { get; set; }

        private void GetKeyCode(RegValue busValue) {
            BusData.Value = (ulong)KeyCode;
            BusData.Enable(BusTypes.IOBus);
        }

        private void SetCursorLocation(RegValue busValue) {
            CursorLocation = busValue.H1;
        }

        private void WriteCharacter(RegValue busValue) {
            // CursorLocation = busValue.H1;
            var address = CursorLocation * 2;
            int offset = (int)CursorLocation;
            int top = offset / 80;
            int left = offset % 80;
            char ch = (char)busValue.B2;
            int fgColor = DefaultFgColor;
            int bgColor = DefaultBgColor;
            var chcoPair = busValue.Q1;

            WriteDoubleByte(address, chcoPair);
            DrawCharacter(ch, left, top, fgColor, bgColor);
            ++CursorLocation;
        }

        // WHOLEWRD
        // HLF1HLF0
        // Q3Q2Q1Q0
        // 76543210

        // B0 = opcode
        // B1 = empty
        // B2 = character
        // B3 = empty
        // B4 = x position
        // B5 = y position
        private void WriteCharacterAt(RegValue busValue) {
            char ch = (char)busValue.B2;
            int left = busValue.B4;
            int top = busValue.B5;
            CursorLocation = (uint)(left + (top * 80));
            var address = CursorLocation * 2;
            int fgColor = DefaultFgColor;
            int bgColor = DefaultBgColor;
            var chcoPair = busValue.Q1;

            WriteDoubleByte(address, chcoPair);
            DrawCharacter(ch, left, top, fgColor, bgColor);
            ++CursorLocation;
        }

        private void WriteCharacterAndColor(RegValue busValue) {
            // CursorLocation = busValue.H1;
            var address = CursorLocation * 2;
            int offset = (int)CursorLocation;
            int top = offset / 80;
            int left = offset % 80;
            char ch = (char)busValue.B2;
            char co = (char)busValue.B3;
            int fgColor = (co & 0x0f);
            int bgColor = (co & 0xf0) >> 4;
            var chcoPair = busValue.Q1;

            WriteDoubleByte(address, chcoPair);
            DrawCharacter(ch, left, top, fgColor, bgColor);
            ++CursorLocation;
        }

        private void WriteCharacterAndColorAt(RegValue busValue) {
            char ch = (char)busValue.B2;
            int left = busValue.B4;
            int top = busValue.B5;
            CursorLocation = (uint)(left + (top * 80));
            var address = CursorLocation * 2;
            char co = (char)busValue.B3;
            int fgColor = (co & 0x0f);
            int bgColor = (co & 0xf0) >> 4;
            var chcoPair = busValue.Q1;

            WriteDoubleByte(address, chcoPair);
            DrawCharacter(ch, left, top, fgColor, bgColor);
            ++CursorLocation;
        }

        private void ClockTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
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

        private void DrawCursor(int left, int top, int fgColor, int bgColor) {
            if (this.IsHandleCreated) {
                BeginInvoke((Action)(() => {
                    int pxLeft = left * 8;
                    int pxTop = top * 16;
                    byte[] chPattern = cursor;
                    Color cbgColor = vgaColors[bgColor];
                    Color cfgColor = vgaColors[fgColor];

                    int y = 0;

                    for (int i = 0; i < 16; ++i) {
                        byte bits = chPattern[i];
                        byte mask = 0b_1000_0000;

                        for (int x = 0; x < 8; ++x) {
                            if ((bits & mask) == mask) {
                                chBitmap.SetPixel(x, y, cfgColor);
                            }
                            else {
                                chBitmap.SetPixel(x, y, cbgColor);
                            }

                            mask >>= 1;
                        }

                        ++y;
                    }

                    consoleGraphics.DrawImage(chBitmap, pxLeft, pxTop);
                }));
            }
        }


        int DefaultFgColor { get; set; } = 0x07;
        int DefaultBgColor { get; set; } = 0x00;
        uint CursorLocation { get; set; } = 0x00000000;

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
