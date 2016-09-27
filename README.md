# Tortilla CPU Emulator

The Tortilla project is intended to provide a way to implement various processors in .NET as pluggable assemblies. The project will also provide hardware emulation, allowing the user to "plug" an emulated CPU into a "socket" in an emulated machine.

This is still very much a work in progress. So far I have the basic opcode interpreter working for x86 real mode and enough opcodes implemented to execute rudimentary assembly-language applications. The hardware emulator implements video memory for color text-mode output to a console window.

Here's the near-term road map for coming releases:

* Implement the most-frequently occurring opcodes
* Complete real-mode implementation
* Implement interrupts
* Write a minimal BIOS that provides the interrupts required for a simple console OS
* Write a minimal OS that will execute flat real-mode binaries
* Implement protected mode

Longer term, I want the emulator to use a third-party BIOS such as SeaBIOS, which implies a much more robust hardware emulation. I also want the x86 CPU emulator to run DOS and command-line Unix/Linux, and later Windows and Unix/Linux graphical shells.

Longer-longer term, I'll implement an ARM chip emulator, but before that I may implement some fun stuff like a 6502.
