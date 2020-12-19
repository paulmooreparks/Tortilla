using System;
using Tortilla;
using Maize;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;

Program p = new(args);
return p.Run();

namespace Maize {
    public class Program {

        Program() {
            Motherboard = new Maize.MaizeMotherboard();
        }

        public Program(string[] args) : this() {
            for (var i = 0; i < args.Length; ++i) {
                var arg = args[i];

                switch (arg.ToUpper()) {
                case "-IMG" or "/IMG":
                    ++i;
                    BiosPath = args[i];
                    break;

                default:
                    break;
                }
            }
        }

        string BiosPath { get; set; } = String.Empty;
        bool IsBiosLoaded { get; set; } = false;

        Tortilla.ITortillaConsole tConsole = new TortillaCharacterConsole();
        IMotherboard<UInt64> Motherboard { get; init; }

        ManualResetEvent cpuStop = new(false);

        void Lifetime() {
            WaitHandle[] handles = new WaitHandle[] { cpuStop };

            while (true) {
                int index = WaitHandle.WaitAny(handles);

                if (index == 0) {
                    tConsole.Close();
                    break;
                }
            }

            // Environment.Exit(0);
        }

        public int Run() {
            Motherboard.Debug += Hardware_Debug;

            tConsole.Show();
            Task.Run(PowerOn);

            Lifetime();

            Console.WriteLine();
            return 0;
        }

        private void Hardware_Debug(object sender, string e) {
        }

        private void Cpu_DecodeInstruction(object sender, Tuple<ulong, ulong> e) {
        }

        bool LoadBIOS() {
            if (string.IsNullOrEmpty(BiosPath)) {
                IsBiosLoaded = false;
            }
            else {
                byte[] tempMemory = File.ReadAllBytes(BiosPath);

                for (uint i = 0; i < tempMemory.Length; ++i) {
                    Motherboard.WriteByte(i, tempMemory[i]);
                }

                IsBiosLoaded = true;
            }

            return IsBiosLoaded;
        }

        public void PowerOn() {
            Motherboard.Reset();
            tConsole.Connect(Motherboard);

            if (LoadBIOS()) {
                Motherboard.Cpu.SingleStep = false;
                Motherboard.EnableDebug(false);
                Motherboard.Cpu.DecodeInstruction += Cpu_DecodeInstruction;
                Motherboard.PowerOn();
                cpuStop.Set();
            }
        }
    }
}
