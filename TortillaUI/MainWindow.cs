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

        [DllImport("kernel32.dll",
            EntryPoint = "GetStdHandle",
            SetLastError = true,
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll",
            EntryPoint = "AllocConsole",
            SetLastError = true,
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        private static extern int AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr CreateFile(
            string lpFileName,
            uint dwDesiredAccess,
            uint dwShareMode,
            uint lpSecurityAttributes,
            uint dwCreationDisposition,
            uint dwFlagsAndAttributes,
            uint hTemplateFile);

        private const int MY_CODE_PAGE = 437;
        private const uint GENERIC_WRITE = 0x40000000;
        private const uint FILE_SHARE_WRITE = 0x2;
        private const uint OPEN_EXISTING = 0x3;

        private const int STD_OUTPUT_HANDLE = -11;

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);

            using (BinaryReader reader = new BinaryReader(File.Open("../../../assembly/bios.rom", FileMode.Open))) {
                reader.Read(memory, 0, memory.Length);
            }

            Run();
        }

        protected override void OnClosing(CancelEventArgs e) {
            base.OnClosing(e);
            PowerOff();
        }

        public byte[] memory = new byte[0x1000000]; // 16MB should be enough for anybody

        AutoResetEvent autoEvent = new AutoResetEvent(false);

        public void RaiseInterrupt(byte id) {
            Console.WriteLine("Interrupt 0x{0:X}", id);
        }

        public void Debug(string disasm, object o) {
            Tortilla.IA32 cpu = (Tortilla.IA32)o;
            var regText = string.Format("EAX = {0:X8} EBX = {1:X8} ECX = {2:X8} EDX = {3:X8} ESI = {4:X8} EDI = {5:X8} EIP = {6:X8} ESP = {7:X8} EBP = {8:X8} EFLAGS = {9:X4}\r\n\r\nCS = {10:X4} DS = {11:X4} ES = {12:X4} SS = {13:X4} FS = {14:X4} GS = {15:X4}\r\n\r\nCF = {16} PF = {17} AF = {18} ZF = {19} SF = {20} DF = {21} OF = {22}",
                       cpu.EAX, cpu.EBX, cpu.ECX, cpu.EDX, cpu.ESI, cpu.EDI, cpu.EIP, cpu.ESP, cpu.EBP, cpu.EFLAGS, cpu.CS, cpu.DS, cpu.ES, cpu.SS, cpu.FS, cpu.GS, cpu.CF, cpu.PF, cpu.AF, cpu.ZF, cpu.SF, cpu.DF, cpu.OF);

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
            Console.WriteLine("EXCEPTION {0:x}", id);
        }

        public Tortilla.ICpu cpu;

        WaitHandle[] hardwareWaitHandles;
        AutoResetEvent videoMemoryChanged = new AutoResetEvent(false);
        Queue<Tuple<UInt32, byte>> videoQueue = new Queue<Tuple<uint, byte>>();

        void QueueVideoUpdate(UInt32 address, byte value) {
            // videoQueue.Enqueue(new Tuple<UInt32, byte>(address, value));
            videoMemoryChanged.Set();
        }

        void UpdateConsole() {
            Console.CursorVisible = false;
            Console.CursorLeft = 0;
            Console.CursorTop = 0;

            int left = 0;
            int top = 0;

            for (var address = 0xb8000; address < 0xb8f00; address += 2) {
                var ch = (char)memory[address];
                var co = (int)memory[address + 1];
                int fgColor = (co & 0x000f);
                int bgColor = (co & 0x00f0) >> 4;

                Console.SetCursorPosition(left, top);
                Console.ForegroundColor = (ConsoleColor)fgColor;
                Console.BackgroundColor = (ConsoleColor)bgColor;

                Console.Write(ch);
                ++left;

                if (left == 80) {
                    left = 0;
                    ++top;
                }
            }
        }

        void HardwareEvents(object o) {
            int index = WaitHandle.WaitTimeout;

            do {
                index = WaitHandle.WaitAny(hardwareWaitHandles, 0);

                switch (index) {
                    case 0:
                        UpdateConsole();
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

            if (address >= 0xb8000 && address <= 0xB8F00) { /* 0xB8FA0 */
                QueueVideoUpdate(address, value);
            }

            // Console.WriteLine("{0:X8} = {1:X8}", address, value);
            // MemoryPage[Address >> 13][Address & 0x1FFF] = Value;
        }

        System.Threading.Timer hardwareTimer;

        private void RunBackground(Action fn) {
            new Thread(new ThreadStart(fn)).Start();
        }

        public void Run() {
            AllocConsole();
            IntPtr stdHandle = GetStdHandle(STD_OUTPUT_HANDLE);
            // IntPtr stdHandle = CreateFile("CONOUT$", GENERIC_WRITE, FILE_SHARE_WRITE, 0, OPEN_EXISTING, 0, 0);

            SafeFileHandle safeFileHandle = new SafeFileHandle(stdHandle, true);
            FileStream fileStream = new FileStream(safeFileHandle, FileAccess.Write);
            Encoding encoding = System.Text.Encoding.GetEncoding(MY_CODE_PAGE);
            StreamWriter standardOutput = new StreamWriter(fileStream, encoding);
            standardOutput.AutoFlush = true;

            Console.SetWindowPosition(0, 0);
            Console.SetOut(standardOutput);

            Console.WindowWidth = 80;
            Console.WindowHeight = 25;
            Console.BufferWidth = 80;
            Console.BufferHeight = 25;
            Console.SetCursorPosition(0, 0);
            Console.TreatControlCAsInput = true;
            Console.CursorVisible = true;

            // Console.WriteLine("Grey text.");
            // Console.ForegroundColor = ConsoleColor.Red;
            // Console.WriteLine("Red text");

            cpu = new Tortilla.IA32();

            RunBackground(() => {
                cpu.Run(this);
                haltEvent.Set();
            });

            UpdateConsole();
            hardwareWaitHandles = new WaitHandle[] { videoMemoryChanged };
            hardwareTimer = new System.Threading.Timer(HardwareEvents, this, 4, 4);
        }

        public void Wait() {
        }

        public void PowerOff() {
            cpu.PowerOff();
        }

    }

}
