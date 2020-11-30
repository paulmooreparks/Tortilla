using System;
using Tortilla;
using Maize;
using System.IO;
using System.Threading;

Program p = new(args);
return p.Run();

namespace Maize {
    public class Program {

        Program() {

        }

        public Program(string[] args) : base() {
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

        string BiosPath { get; set; }
        bool IsBiosLoaded { get; set; } = false;

        Tortilla.ITortillaConsole tConsole = new TortillaCharacterConsole();
        IMotherboard<UInt64> Motherboard { get; set; }

        public int Run() {
            Motherboard = new Maize.MaizeMotherboard();
            Motherboard.Debug += Hardware_Debug;

            tConsole.Show();
            PowerOn();

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
            tConsole.Clear();

            if (LoadBIOS()) {
                Motherboard.Cpu.SingleStep = false;
                Motherboard.EnableDebug(false);
                Motherboard.Cpu.DecodeInstruction += Cpu_DecodeInstruction;
                Motherboard.PowerOn();
                Motherboard.RaiseInterrupt(0);
            }
        }
    }
}
