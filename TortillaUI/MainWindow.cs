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

/* 
IntervalTree implementation by Ido Ran 
https://archive.codeplex.com/?p=intervaltree
http://dotdotnet.blogspot.com/2011/06/interval-tree-c-implementation.html
BSD License
*/
using IntervalTreeLib;
using Tortilla;

namespace TortillaUI {

    public partial class MainWindow : Form {
        public MainWindow() {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
            InitEnvironment();

            ConnectAddressRangesToMethods();
            ConnectPortAddressesToMethods();

            RangeDelegates.AddInterval(startAddressView, endAddressView, UpdateMemoryWindow);

            startAddress.Text = $"{startAddressView:X8}";
            endAddress.Text = $"{endAddressView:X8}";

            startAddress.TextChanged += new System.EventHandler(startAddress_TextChanged);
            endAddress.TextChanged += new System.EventHandler(endAddress_TextChanged);

            // UpdateMemoryWindow(startAddressView);
        }

        protected override void OnClosing(CancelEventArgs e) {
            base.OnClosing(e);
            PowerOff();
        }

        protected delegate void AddressRangeDelegate(UInt64 address);
        protected IntervalTree<AddressRangeDelegate, UInt64> RangeDelegates { get; } = new IntervalTree<AddressRangeDelegate, UInt64>();

        protected virtual void ConnectAddressRangesToMethods() {
            System.Reflection.MethodInfo[] methods = this.GetType().GetMethods(
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.IgnoreCase |
                System.Reflection.BindingFlags.InvokeMethod |
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic);

            foreach (System.Reflection.MethodInfo method in methods) {
                object[] outattrs = method.GetCustomAttributes(typeof(AddressRangeHandlerAttribute), true);

                if (outattrs != null) {
                    foreach (System.Attribute attr in outattrs) {
                        AddressRangeHandlerAttribute methodAttr = (AddressRangeHandlerAttribute)attr;

                        if (methodAttr.AddressRangeHandlers != null) {
                            foreach (Tuple<UInt32, UInt32> addressRange in methodAttr.AddressRangeHandlers) {
                                RangeDelegates.AddInterval(addressRange.Item1, addressRange.Item2, (AddressRangeDelegate)AddressRangeDelegate.CreateDelegate(typeof(AddressRangeDelegate), this, method));
                            }
                        }
                    }
                }
            }
        }

        protected delegate void PortOutHandlerDelegate(UInt16 address, UInt16 value);
        protected Dictionary<UInt16, PortOutHandlerDelegate> PortOutHandlerMap { get; } = new Dictionary<ushort, PortOutHandlerDelegate>();

        protected delegate UInt16 PortInHandlerDelegate(UInt16 address);
        protected Dictionary<UInt16, PortInHandlerDelegate> PortInHandlerMap { get; } = new Dictionary<ushort, PortInHandlerDelegate>();

        protected virtual void ConnectPortAddressesToMethods() {
            System.Reflection.MethodInfo[] methods = this.GetType().GetMethods(
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.IgnoreCase |
                System.Reflection.BindingFlags.InvokeMethod |
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic);

            foreach (System.Reflection.MethodInfo method in methods) {
                object[] outattrs = method.GetCustomAttributes(typeof(PortOutHandlerAttribute), true);

                if (outattrs != null) {
                    foreach (System.Attribute attr in outattrs) {
                        PortOutHandlerAttribute methodAttr = (PortOutHandlerAttribute)attr;

                        if (methodAttr.PortOutHandlers != null) {
                            foreach (byte address in methodAttr.PortOutHandlers) {
                                if (PortOutHandlerMap.ContainsKey(address)) {
                                    throw new Exception($"Duplicate port out handler for address {address:X2}");
                                }

                                PortOutHandlerMap[address] = (PortOutHandlerDelegate)PortOutHandlerDelegate.CreateDelegate(typeof(PortOutHandlerDelegate), this, method);
                            }
                        }
                    }
                }

                object[] inattrs = method.GetCustomAttributes(typeof(PortInHandlerAttribute), true);

                if (inattrs != null) {
                    foreach (System.Attribute attr in inattrs) {
                        PortInHandlerAttribute methodAttr = (PortInHandlerAttribute)attr;

                        if (methodAttr.PortInHandlers != null) {
                            foreach (byte address in methodAttr.PortInHandlers) {
                                if (PortInHandlerMap.ContainsKey(address)) {
                                    throw new Exception($"Duplicate port in handler for address {address:X2}");
                                }

                                PortInHandlerMap[address] = (PortInHandlerDelegate)PortInHandlerDelegate.CreateDelegate(typeof(PortInHandlerDelegate), this, method);
                            }
                        }
                    }
                }
            }
        }


        public UInt16[] ports = new UInt16[0x10000];

        UInt32 startAddressView = 0x80; // 0xb8000;
        UInt32 endAddressView = 0xFF; // 0xb8060;
        UInt32 maxAddressRange = 0x1008;

        bool StartEndRangeValid() {
            if (endAddressView <= startAddressView || (endAddressView - startAddressView) > maxAddressRange) {
                addressRangeError.Visible = true;
                addressRangeError.Text = "Address range error";
                memoryOutput.Clear();
                return false;
            }

            return true;
        }

        private void startAddress_TextChanged(object sender, EventArgs e) {
            RangeDelegates.RemoveInterval(startAddressView, endAddressView);
            var temp = startAddressView;

            try {
                startAddressView = Convert.ToUInt32(startAddress.Text, 16);

                if (!StartEndRangeValid()) {
                    return;
                }

                if (traceCheckBox.Checked) {
                    RangeDelegates.AddInterval(startAddressView, endAddressView, UpdateMemoryWindow);
                }
            }
            catch (Exception) {
                startAddressView = temp;
            }

            UpdateMemoryWindow(startAddressView);
        }

        private void endAddress_TextChanged(object sender, EventArgs e) {
            RangeDelegates.RemoveInterval(startAddressView, endAddressView);
            var temp = endAddressView;

            try {
                endAddressView = Convert.ToUInt32(endAddress.Text, 16);

                if (!StartEndRangeValid()) {
                    return;
                }

                if (traceCheckBox.Checked) {
                    RangeDelegates.AddInterval(startAddressView, endAddressView, UpdateMemoryWindow);
                }
            }
            catch (Exception) {
                endAddressView = temp;
            }

            UpdateMemoryWindow(endAddressView);
        }

        private void UpdateMemoryWindow(UInt64 modifiedAddress) {
            BeginInvoke((Action)(() => {
                UInt32 address = startAddressView;
                StringBuilder memText = new StringBuilder();

                while (address <= endAddressView) {
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
        
        TortillaConsole tConsole = new TortillaConsole();

        private void RunBackground(Action fn) {
            new Thread(new ThreadStart(fn)).Start();
        }

        private string BiosPath { get; set; }

        private void openBIOSROMToolStripMenuItem_Click(object sender, EventArgs e) {
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
                Properties.Settings.Default.DefaultBIOS = BiosPath;
                Properties.Settings.Default.Save();

                LoadBIOS();
            }

            return IsBIOSLoaded;
        }

        private bool LoadBIOS() {
            if (string.IsNullOrEmpty(BiosPath)) {
                IsBIOSLoaded = false;
            }
            else {
                byte[] tempMemory = new byte[Motherboard.MemorySize];

                using (BinaryReader reader = new BinaryReader(File.Open(BiosPath, FileMode.Open))) {
                    int bytesRead = reader.Read(tempMemory, 0, tempMemory.Length);

                    for (uint i = 0; i < bytesRead; ++i) {
                        Motherboard.WriteByte(i, tempMemory[i]);
                    }
                }

                IsBIOSLoaded = true;
            }

            return IsBIOSLoaded;
        }

        IMotherboard<UInt64> Motherboard { get; set; }

        public void InitEnvironment() {
            Motherboard = new Maize.MaizeMotherboard();
            Motherboard.Debug += Hardware_Debug;

            // tConsole.Connect(Motherboard);
            tConsole.Show();

            BiosPath = Properties.Settings.Default.DefaultBIOS;
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
            Properties.Settings.Default.WindowPosition = pos;
            Properties.Settings.Default.Save();
        }

        private void RestoreWindowPosition() {
            try {
                string pos = Properties.Settings.Default.WindowPosition;

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

        private void MainWindow_Move(object sender, EventArgs e) {
            SaveWindowPosition();
        }

        private void MainWindow_SizeChanged(object sender, EventArgs e) {
            SaveWindowPosition();
        }

        private void MainWindow_Load(object sender, EventArgs e) {
            RestoreWindowPosition();
            Move += new EventHandler(MainWindow_Move);
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
                RangeDelegates.AddInterval(startAddressView, endAddressView, UpdateMemoryWindow);
            }
            else {
                RangeDelegates.RemoveInterval(startAddressView, endAddressView);
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

        public void Write8(UInt64 address, byte value) {
            Motherboard.WriteByte(address, value);

            var delegateList = RangeDelegates.Get(address, StubMode.ContainsStartThenEnd);

            if (delegateList != null) {
                foreach (var handler in delegateList) {
                    handler(address);
                }
            }
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

}
