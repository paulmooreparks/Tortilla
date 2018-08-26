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
                                    throw new Exception(string.Format("Duplicate port out handler for address {0:X2}", address));
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
                                    throw new Exception(string.Format("Duplicate port in handler for address {0:X2}", address));
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

        public void Debug(string disasm, object o) {
            Tortilla.IA32 cpu = (Tortilla.IA32)o;
            var regText = string.Format("EAX = {0:X8} EBX = {1:X8} ECX = {2:X8} EDX = {3:X8} ESI = {4:X8} EDI = {5:X8} EIP = {6:X8} ESP = {7:X8} EBP = {8:X8} EFLAGS = {9:X4}\r\n\r\nCS = {10:X4} DS = {11:X4} ES = {12:X4} SS = {13:X4} FS = {14:X4} GS = {15:X4}\r\n\r\nCF = {16} PF = {17} AF = {18} ZF = {19} SF = {20} DF = {21} OF = {22}, TF = {23}",
                       cpu.EAX, cpu.EBX, cpu.ECX, cpu.EDX, cpu.ESI, cpu.EDI, cpu.EIP, cpu.ESP, cpu.EBP, cpu.EFLAGS, cpu.CS, cpu.DS, cpu.ES, cpu.SS, cpu.FS, cpu.GS, cpu.CF, cpu.PF, cpu.AF, cpu.ZF, cpu.SF, cpu.DF, cpu.OF,cpu.TF);

            BeginInvoke((Action)(() => {
                debug.AppendText(disasm + "\r\n");
                debug.ScrollToCaret();
                registers.Text = regText;
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

            hardwareWaitHandles = new WaitHandle[] { videoMemoryChanged };
            hardwareTimer = new System.Threading.Timer(HardwareEvents, this, 4, 4);
        }

        public void Wait() {
        }

        public void PowerOff() {
            cpu.PowerOff();
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

        }

        private void breakButton_Click(object sender, EventArgs e) {

        }

        private void stepButton_Click(object sender, EventArgs e) {

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
