using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

using Tortilla;
using System.Configuration;

namespace TortillaUI {

    public partial class MainWindow : Form {
        public MainWindow() {
            us = new UserSettings();
            us.SettingsLoaded += Us_SettingsLoaded;
            InitializeComponent();
        }

        private void Us_SettingsLoaded(object sender, SettingsLoadedEventArgs e) {
            BiosPath = us.BiosPath;
        }

        protected UserSettings us;

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
            InitEnvironment();

            // RangeDelegates.AddInterval(startAddressView, viewSizeView, UpdateMemoryWindow);

            startAddress.Text = $"{startAddressView:X8}";
            viewSize.Text = $"{viewSizeView:X8}";

            startAddress.TextChanged += new System.EventHandler(startAddress_TextChanged);
            viewSize.TextChanged += new System.EventHandler(viewSize_TextChanged);

            // UpdateMemoryWindow(startAddressView);
        }

        protected override void OnClosing(CancelEventArgs e) {
            base.OnClosing(e);
            tConsole.Close();
            PowerOff();
        }

        protected delegate void AddressRangeDelegate(UInt64 address);

        protected delegate void PortOutHandlerDelegate(UInt16 address, UInt16 value);
        protected Dictionary<UInt16, PortOutHandlerDelegate> PortOutHandlerMap { get; } = new Dictionary<ushort, PortOutHandlerDelegate>();

        protected delegate UInt16 PortInHandlerDelegate(UInt16 address);
        protected Dictionary<UInt16, PortInHandlerDelegate> PortInHandlerMap { get; } = new Dictionary<ushort, PortInHandlerDelegate>();

        public UInt16[] ports = new UInt16[0x10000];

        UInt32 startAddressView = 0x80; // 0xb8000;
        UInt32 viewSizeView = 0x120; // 0xb8060;
        UInt32 maxAddressRange = 0x4008;

        bool StartEndRangeValid() {
            if (viewSizeView <= 0 || (viewSizeView > maxAddressRange)) {
                addressRangeError.Visible = true;
                addressRangeError.Text = "Address range error";
                memoryOutput.Clear();
                return false;
            }

            return true;
        }

        private void startAddress_TextChanged(object sender, EventArgs e) {
            // RangeDelegates.RemoveInterval(startAddressView, viewSizeView);
            var temp = startAddressView;

            try {
                startAddressView = Convert.ToUInt32(startAddress.Text, 16);

                if (!StartEndRangeValid()) {
                    return;
                }

                if (traceCheckBox.Checked) {
                    // RangeDelegates.AddInterval(startAddressView, viewSizeView, UpdateMemoryWindow);
                }
            }
            catch (Exception) {
                startAddressView = temp;
            }

            UpdateMemoryWindow(startAddressView);
        }

        private void viewSize_TextChanged(object sender, EventArgs e) {
            // RangeDelegates.RemoveInterval(startAddressView, viewSizeView);
            var temp = viewSizeView;

            try {
                viewSizeView = Convert.ToUInt32(viewSize.Text, 16);

                if (!StartEndRangeValid()) {
                    return;
                }

                if (traceCheckBox.Checked) {
                    // RangeDelegates.AddInterval(startAddressView, startAddressView + viewSizeView, UpdateMemoryWindow);
                }
            }
            catch (Exception) {
                viewSizeView = temp;
            }

            UpdateMemoryWindow(startAddressView);
        }

        private void UpdateMemoryWindow(UInt64 modifiedAddress) {
            BeginInvoke((Action)(() => {
                UInt32 address = startAddressView;
                StringBuilder memText = new StringBuilder();

                while (address <= startAddressView + viewSizeView) {
                    addressRangeError.Visible = false;
                    addressRangeError.Text = string.Empty;
                    memText.Append($"{address:X8}: {Read8(address++):X2} {Read8(address++):X2} {Read8(address++):X2} {Read8(address++):X2} {Read8(address++):X2} {Read8(address++):X2} {Read8(address++):X2} {Read8(address++):X2} {Read8(address++):X2} {Read8(address++):X2} {Read8(address++):X2} {Read8(address++):X2} {Read8(address++):X2} {Read8(address++):X2} {Read8(address++):X2} {Read8(address++):X2}\r\n");
                }

                memoryOutput.Text = memText.ToString();
            }));
        }

        AutoResetEvent exceptionEvent = new AutoResetEvent(false);
        AutoResetEvent haltEvent = new AutoResetEvent(false);

        AutoResetEvent powerOffEvent = new AutoResetEvent(false);

        // TortillaGraphicalConsole tConsole = new TortillaGraphicalConsole();
        TortillaCharacterConsole tConsole = new TortillaCharacterConsole();

        private void RunBackground(Action fn) {
            new Thread(new ThreadStart(fn)).Start();
        }

        private string BiosPath { get; set; }

        private void openBIOSROMToolStripMenuItem_Click(object sender, EventArgs e) {
            tConsole.Clear();
            debug.Clear();
            registers.Clear();

            Motherboard.Reset();
            tConsole.Connect(Motherboard);

            OpenBIOS();
        }

        public bool IsBIOSLoaded { get; set; }

        private bool OpenBIOS() {
            var openDlg = new OpenFileDialog();
            openDlg.Filter = "BIOS binary files (*.BIN;*.ROM)|*.BIN;*.ROM";
            openDlg.Title = "Select a BIOS binary";

            if (string.IsNullOrEmpty(BiosPath)) {
                openDlg.InitialDirectory = Environment.CurrentDirectory;
            }
            else {
                openDlg.InitialDirectory = Path.GetDirectoryName(BiosPath);
            }

            if (openDlg.ShowDialog() == DialogResult.OK) {
                BiosPath = openDlg.FileName;
                us.BiosPath = BiosPath;
                us.Save();
                LoadBIOS();
            }

            return IsBIOSLoaded;
        }

        private bool LoadBIOS() {
            if (string.IsNullOrEmpty(BiosPath)) {
                IsBIOSLoaded = false;
            }
            else {
                byte[] tempMemory = File.ReadAllBytes(BiosPath);

                for (uint i = 0; i < tempMemory.Length; ++i) {
                    Motherboard.WriteByte(i, tempMemory[i]);
                }

                IsBIOSLoaded = true;
            }

            return IsBIOSLoaded;
        }

        IMotherboard<UInt64> Motherboard { get; set; }

        public void InitEnvironment() {
            Motherboard = new Maize.MaizeMotherboard();
            Motherboard.Debug += Hardware_Debug;

            tConsole.Show();
            BiosPath = us.BiosPath;
        }

        private void Cpu_DecodeInstruction(object sender, Tuple<UInt64, UInt64> e) {
            if (traceCheckBox.Checked) {
                Decode(e.Item1, e.Item2);
            }
        }

        private void Hardware_Debug(object sender, string e) {
            if (traceCheckBox.Checked) {
                Debug(e, sender);
            }
        }

        public void Wait() {
        }

        public void ResetCPU() {
            tConsole.Clear();
            debug.Clear();
            registers.Clear();

            Motherboard.Reset();
            tConsole.Connect(Motherboard);

            if (!LoadBIOS()) {
                OpenBIOS();
            }

            Motherboard.Cpu.SingleStep = stepCheckBox.Checked;
            Motherboard.Cpu.DecodeInstruction += Cpu_DecodeInstruction;
            Motherboard.PowerOn();
            Motherboard.RaiseInterrupt(0);

            haltEvent.Set();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e) {
            this.Close();
            Application.Exit();
        }

        private void runToolStripMenuItem_Click(object sender, EventArgs e) {
            ResetCPU();
        }

        private void SaveWindowPosition() {
            Rectangle rect = (WindowState == FormWindowState.Normal) ?
                new Rectangle(DesktopBounds.Left, DesktopBounds.Top, DesktopBounds.Width, DesktopBounds.Height) :
                new Rectangle(RestoreBounds.Left, RestoreBounds.Top, this.Width, this.Height);

            var pos = $"{(int)this.WindowState},{rect.Left},{rect.Top},{rect.Width},{rect.Height}";
            us.MainWindowPosition = pos;
            us.Save();
        }

        private void RestoreWindowPosition() {
            try {
                string pos = us.MainWindowPosition;

                if (!string.IsNullOrEmpty(pos)) {
                    List<int> settings = pos.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(v => int.Parse(v)).ToList();

                    if (settings.Count == 5) {
                        this.SetDesktopLocation(settings[1], settings[2]);
                        this.Width = settings[3];
                        this.Height = settings[4];
                        this.WindowState = (FormWindowState)settings[0];
                    }
                }
            }
            catch { /* Just leave current position if error */ }
        }

        private void MainWindow_Move(object sender, EventArgs e) {
            SaveWindowPosition();
        }

        private void MainWindow_SizeChanged(object sender, EventArgs e) {
            SaveWindowPosition();
        }

        private void MainWindow_Load(object sender, EventArgs e) {
            this.debug.Height = this.ClientSize.Height - this.debug.Top - debug.Margin.Bottom;
            RestoreWindowPosition();
        }

        private void MainWindow_Shown(object sender, EventArgs e) {
            // RestoreWindowPosition();
            // Move += MainWindow_Move;
            // SizeChanged += MainWindow_SizeChanged;
            FormClosing += MainWindow_FormClosing;
        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e) {
            SaveWindowPosition();
        }

        private void MainWindow_ClientSizeChanged(object sender, EventArgs e) {
            // throw new NotImplementedException();
        }

        private void runButton_Click(object sender, EventArgs e) {
            Motherboard.Cpu.Continue();
        }

        private void breakButton_Click(object sender, EventArgs e) {
            Motherboard.Cpu.Break();
        }

        private void stopButton_Click(object sender, EventArgs e) {

        }

        private void resetButton_Click(object sender, EventArgs e) {
            ResetCPU();
        }

        private void traceCheckBox_CheckedChanged(object sender, EventArgs e) {
            if (traceCheckBox.Checked) {
                // RangeDelegates.AddInterval(startAddressView, viewSizeView, UpdateMemoryWindow);
            }
            else {
                // RangeDelegates.RemoveInterval(startAddressView, viewSizeView);
            }
        }

        private void stepCheckBox_CheckedChanged(object sender, EventArgs e) {
            if (Motherboard != null && Motherboard.Cpu != null) {
                Motherboard.Cpu.SingleStep = stepCheckBox.Checked;
            }

            if (stepCheckBox.Checked && !traceCheckBox.Checked) {
                traceCheckBox.Checked = true;
            }
        }

        // IHardware

        public void PowerOff() {
            Motherboard.PowerOff();
            powerOffEvent.Set();
        }

        Maize.Disassembler Disassembler = new Maize.Disassembler();

        public void Decode(ulong value1, ulong value2) {
            BeginInvoke((Action)(() => {
                string text;
                int bytesToRead = Disassembler.Decode(value1, value2, out text);

                if (!string.IsNullOrEmpty(text)) {
                    debug.AppendText(text + "\r\n");
                    debug.ScrollToCaret();
                }
            }));
        }

        public void Debug(string disasm, object o) {
            var regText = this.Motherboard.Cpu.RegisterDump;

            BeginInvoke((Action)(() => {
                if (!string.IsNullOrEmpty(disasm)) {
                    debug.AppendText(disasm + "\r\n");
                    debug.ScrollToCaret();
                }
                registers.Text = regText;
                UpdateMemoryWindow(0);
            }));
        }

        public void RaiseException(byte id) {
            exceptionEvent.Set();
            // Console.WriteLine("EXCEPTION {0:x}", id);
        }

        public void RaiseInterrupt(byte id) {
            // Console.WriteLine("Interrupt 0x{0:X}", id);
        }

        public byte Read8(UInt64 address) {
            return Motherboard.ReadByte(address);
        }

        public byte ReadPort8(ushort address) {
            return (byte)(ReadPort16(address) & 0x00FF);
        }

        public ushort ReadPort16(ushort address) {
            PortInHandlerDelegate handler = PortInHandlerMap[address];
            UInt16 value = 0;

            if (handler != null) {
                value = handler(address);
            }
            else {
                // DbgIns(string.Format("No port-in handler for address {0:X}", address));
            }
            return value;
        }

        public void WritePort8(ushort address, byte value) {
            WritePort16(address, (UInt16)(value & 0x00FF));
        }

        public void WritePort16(ushort address, UInt16 value) {
            PortOutHandlerDelegate handler = PortOutHandlerMap[address];

            if (handler != null) {
                handler(address, value);
            }
            else {
                // DbgIns(string.Format("No port-out handler for address {0:X}", address));
            }
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    sealed public class PortOutHandlerAttribute : System.Attribute {
        public byte[] PortOutHandlers { get; set; }

        public PortOutHandlerAttribute(byte address) {
            PortOutHandlers = (byte[])Array.CreateInstance(typeof(byte), 1);
            PortOutHandlers[0] = address;
        }

        public PortOutHandlerAttribute(params byte[] addresses) {
            PortOutHandlers = (byte[])Array.CreateInstance(typeof(byte), addresses.Length);
            Array.Copy(addresses, PortOutHandlers, addresses.Length);
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    sealed public class PortInHandlerAttribute : System.Attribute {
        public byte[] PortInHandlers { get; set; }

        public PortInHandlerAttribute(byte address) {
            PortInHandlers = (byte[])Array.CreateInstance(typeof(byte), 1);
            PortInHandlers[0] = address;
        }

        public PortInHandlerAttribute(params byte[] addresses) {
            PortInHandlers = (byte[])Array.CreateInstance(typeof(byte), addresses.Length);
            Array.Copy(addresses, PortInHandlers, addresses.Length);
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    sealed public class AddressRangeHandlerAttribute : System.Attribute {
        public Tuple<UInt32, UInt32>[] AddressRangeHandlers { get; set; }

        public AddressRangeHandlerAttribute(UInt32 startAddress, UInt32 endAddress) {
            AddressRangeHandlers = (Tuple<UInt32, UInt32>[])Array.CreateInstance(typeof(Tuple<UInt32, UInt32>), 1);
            AddressRangeHandlers[0] = new Tuple<uint, uint>(startAddress, endAddress);
        }

        public AddressRangeHandlerAttribute(params Tuple<UInt32, UInt32>[] addresses) {
            AddressRangeHandlers = (Tuple<UInt32, UInt32>[])Array.CreateInstance(typeof(Tuple<UInt32, UInt32>), addresses.Length);
            Array.Copy(addresses, AddressRangeHandlers, addresses.Length);
        }
    }

    public class UserSettings : ApplicationSettingsBase {
        [UserScopedSetting()]
        public string MainWindowPosition {
            get => (string)this["MainWindowPosition"];
            set => this["MainWindowPosition"] = (string)value;
        }

        [UserScopedSetting]
        public string ConsoleWindowPosition {
            get => (string)this["ConsoleWindowPosition"];
            set => this["ConsoleWindowPosition"] = (string)value;
        }

        [UserScopedSetting]
        public string BiosPath {
            get => (string)this["BiosPath"];
            set => this["BiosPath"] = (string)value;
        }
    }

}
