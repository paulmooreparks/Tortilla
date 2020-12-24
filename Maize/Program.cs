using System;
using Tortilla;
using Maize;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;

/* New in C# 8.0, and pretty nifty: no need for static Main! */

Program p = new(args);
return p.Run();

namespace Maize {
    public class Program {

        Program() {
            Motherboard = new Maize.Motherboard();
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

        Tortilla.IConsole tConsole = new TortillaCharacterConsole();
        IMotherboard<UInt64> Motherboard { get; init; }

        public int Run() {
            Motherboard.Debug += Hardware_Debug;
            tConsole.Show();
            PowerOn();
            tConsole.Close();
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
            }
        }
    }
}
