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

namespace TortillaUI {

    public partial class MainWindow : Form, Tortilla.IHardware {
        public MainWindow() {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
            InitEnvironment();
            startAddress.Text = $"{startAddressView:X8}";
            endAddress.Text = $"{endAddressView:X8}";

            startAddress.TextChanged += new System.EventHandler(startAddress_TextChanged);
            endAddress.TextChanged += new System.EventHandler(endAddress_TextChanged);
            UpdateMemoryWindow();
        }

        protected override void OnClosing(CancelEventArgs e) {
            base.OnClosing(e);
            PowerOff();
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


        public byte[] memory = new byte[0x1000000]; // 16MB should be enough for anybody
        public UInt16[] ports = new UInt16[0x10000];

        AutoResetEvent autoEvent = new AutoResetEvent(false);

        public void RaiseInterrupt(byte id) {
            // Console.WriteLine("Interrupt 0x{0:X}", id);
        }

        UInt32 startAddressView = 0xb8000;
        UInt32 endAddressView = 0xb8060;

        private void startAddress_TextChanged(object sender, EventArgs e) {
            var temp = startAddressView;

            try {
                startAddressView = Convert.ToUInt32(startAddress.Text, 16);
            }
            catch (Exception) {
                startAddressView = temp;
            }

            UpdateMemoryWindow();
        }

        private void endAddress_TextChanged(object sender, EventArgs e) {
            var temp = endAddressView;

            try {
                endAddressView = Convert.ToUInt32(endAddress.Text, 16);
            }
            catch (Exception) {
                endAddressView = temp;
            }

            UpdateMemoryWindow();
        }

        public void Debug(string disasm, object o) {
            if (traceCheckBox.Checked) {
                Tortilla.IA32 cpu = (Tortilla.IA32)o;
                var regText = $"EAX = {cpu.EAX:X8} EBX = {cpu.EBX:X8} ECX = {cpu.ECX:X8} EDX = {cpu.EDX:X8} ESI = {cpu.ESI:X8} EDI = {cpu.EDI:X8} EIP = {cpu.EIP:X8} ESP = {cpu.ESP:X8} EBP = {cpu.EBP:X8} EFLAGS = {cpu.EFLAGS:X4}\r\n\r\nCS = {cpu.CS:X4} DS = {cpu.DS:X4} ES = {cpu.ES:X4} SS = {cpu.SS:X4} FS = {cpu.FS:X4} GS = {cpu.GS:X4}\r\n\r\nCF = {cpu.CF} PF = {cpu.PF} AF = {cpu.AF} ZF = {cpu.ZF} SF = {cpu.SF} DF = {cpu.DF} OF = {cpu.OF}, TF = {cpu.TF}";

                BeginInvoke((Action)(() => {
                    debug.AppendText(disasm + "\r\n");
                    debug.ScrollToCaret();
                    registers.Text = regText;
                }));
            }
        }

        private void UpdateMemoryWindow() {
            if (endAddressView <= startAddressView || (endAddressView - startAddressView) > 0x1008) {
                BeginInvoke((Action)(() => {
                    addressRangeError.Visible = true;
                    addressRangeError.Text = "Address range error";
                    memoryOutput.Clear();
                }));

                return;
            }

            UInt32 address = startAddressView;
            StringBuilder memText = new StringBuilder();

            while (address <= endAddressView) {
                addressRangeError.Visible = false;
                addressRangeError.Text = string.Empty;
                memText.Append($"{address:X8}: {Read8(address++):X2} {Read8(address++):X2} {Read8(address++):X2} {Read8(address++):X2} {Read8(address++):X2} {Read8(address++):X2} {Read8(address++):X2} {Read8(address++):X2} {Read8(address++):X2} {Read8(address++):X2} {Read8(address++):X2} {Read8(address++):X2} {Read8(address++):X2} {Read8(address++):X2} {Read8(address++):X2} {Read8(address++):X2}\r\n");
            }

            BeginInvoke((Action)(() => {
                memoryOutput.Text = memText.ToString();
            }));
        }

        AutoResetEvent exceptionEvent = new AutoResetEvent(false);
        AutoResetEvent haltEvent = new AutoResetEvent(false);

        public void RaiseException(byte id) {
            exceptionEvent.Set();
            // Console.WriteLine("EXCEPTION {0:x}", id);
        }

        public Tortilla.ICpu cpu;

        WaitHandle[] hardwareWaitHandles;
        AutoResetEvent videoMemoryChanged = new AutoResetEvent(false);
        AutoResetEvent powerOffEvent = new AutoResetEvent(false);
        // Queue<Tuple<UInt32, byte>> videoQueue = new Queue<Tuple<uint, byte>>();

        void QueueVideoUpdate(UInt32 address, byte value) {
            // videoQueue.Enqueue(new Tuple<UInt32, byte>(address, value));
            videoMemoryChanged.Set();
        }

        TortillaConsole tConsole = new TortillaConsole();

        void InitConsole() {
            int left = 0;
            int top = 0;

            for (var address = 0xb8000; address < 0xb8f00; address += 2) {
                var ch = (char)memory[address];
                var co = (int)memory[address + 1];
                int fgColor = (co & 0x000f);
                int bgColor = (co & 0x00f0) >> 4;

                // Console.SetCursorPosition(left, top);
                // Console.ForegroundColor = (ConsoleColor)fgColor;
                // Console.BackgroundColor = (ConsoleColor)bgColor;

                // Console.Write(ch);
                ++left;

                if (left == 80) {
                    left = 0;
                    ++top;
                }
            }
        }

        void UpdateConsole(UInt32 address) {
            address = (UInt32)(address & ~0x01);
            int offset = (int)(address - 0xb8000) / 2;
            int top = offset / 80;
            int left = offset % 80;

            var ch = (char)memory[address];
            var co = (int)memory[address + 1];
            int fgColor = (co & 0x000f);
            int bgColor = (co & 0x00f0) >> 4;

            tConsole.DrawCharacter(ch, left, top, fgColor, bgColor);
        }

        void HardwareEvents(object o) {
            int index = WaitHandle.WaitTimeout;

            do {
                index = WaitHandle.WaitAny(hardwareWaitHandles, 0);

                switch (index) {
                    case 0:
                        // UpdateConsole();
                        break;

                        /* Power off */
                    case 1:
                        return;
                }
            } while (index != WaitHandle.WaitTimeout);
        }

        public byte Read8(uint address) {
            return memory[address];
            // return (MemoryPage[Address >> 13][Address & 0x1FFF]);
        }

        public void Write8(UInt32 address, byte value) {
            memory[address] = value;

            if (address >= 0xb8000 && address <= 0xb8F00) {
                // QueueVideoUpdate(address, value);
                UpdateConsole(address);
            }

            if (traceCheckBox.Checked && address >= startAddressView && address <= endAddressView) {
                UpdateMemoryWindow();
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

        System.Threading.Timer hardwareTimer;

        private void RunBackground(Action fn) {
            new Thread(new ThreadStart(fn)).Start();
        }

        public void InitEnvironment() {
            cpu = new Tortilla.IA32();

            for (var i = 0; i < memory.Length; ++i) {
                memory[i] = 0;
            }

            using (BinaryReader reader = new BinaryReader(File.Open("../../../assembly/bios.rom", FileMode.Open))) {
                reader.Read(memory, 0, memory.Length);
            }

            tConsole.Show();

            hardwareWaitHandles = new WaitHandle[] { videoMemoryChanged, powerOffEvent };
            hardwareTimer = new System.Threading.Timer(HardwareEvents, this, 4, 4);
        }

        public void Wait() {
        }

        public void PowerOff() {
            cpu.PowerOff();
            powerOffEvent.Set();
        }

        public void ResetCPU() {
            tConsole.Clear();
            debug.Clear();
            registers.Clear();

            RunBackground(() => {
                cpu.Run(this);
                haltEvent.Set();
            });
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
            cpu.Continue();
        }

        private void breakButton_Click(object sender, EventArgs e) {
            cpu.Break();
        }

        private void stepButton_Click(object sender, EventArgs e) {
            cpu.Step();
        }

        private void stopButton_Click(object sender, EventArgs e) {

        }

        private void resetButton_Click(object sender, EventArgs e) {
            ResetCPU();
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

}
