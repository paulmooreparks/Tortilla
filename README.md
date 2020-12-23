# The Maize Virtual CPU 
## Implemented on the Tortilla Emulation Platform

This project implements a virtual CPU (called "Maize") on a library that enables the creation of virtual CPUs (called "Tortilla"). 
See the file [Maize.txt](https://github.com/paulmooreparks/Tortilla/blob/master/Maize.txt) for more details on the CPU assembly 
language and the system that runs on it.

The goal is to create a "BIOS" layer above the virtual devices, a simple OS, a subset of Unix/Linux system calls (interrupt $80), 
and finally an implementation of a C/C++ compiler (likely Clang or GCC) that will compile and link to Maize machine code.

## How To Use Maize

Maize is currently implemented in .NET 5, which means it will run on Windows, Linux, and macOS. It also means it's slower than I'd 
like it to be, despite my attempts at squeezing as much performance out of .NET as I could.

To compile a Maize assembly file (like [HelloWorld.asm](https://github.com/paulmooreparks/Tortilla/blob/master/HelloWorld.asm)), 
compile and run the [mazm](https://github.com/paulmooreparks/Tortilla/tree/master/mazm) project, providing the path to the 
assembly file as the command-line parameter.
    
    mazm /path/to/HelloWorld.asm
    
The above command will output HelloWorld.bin into the same path as HelloWorld.asm.
    
To run a Maize binary, compile and run the [maize](https://github.com/paulmooreparks/Tortilla/tree/master/Maize) binary, providing 
the path to the binary in the -img parameter:

    maize -img /path/to/HelloWorld.bin

## Project Status

It's very early days for Maize, so don't expect too much in the way of application usability. So far, I've enabled a basic text-
mode console for input and output. Next, I'll start creating a file-system device. In the future I plan to port Clang or GCC to 
work with Maize binaries so that I can eventually port Linux to the virtual CPU.

In the short term, I'm implementing a very basic OS over a simple BIOS. It will provide a basic character-mode CLI to allow 
building and running simple Maize programs from within the virtual CPU environment. 

I'll eventually port Maize to a much lower-level language, likely either C++ or Rust. I started out with .NET and C# because I wanted 
to play with .NET Core, and that bought me immediate multi-platform support and a lot fewer headaches for writing code quickly and 
for implementing devices.

## Hello, World!

Here is a simple "Hello, World!" application written in Maize assembly.

    INCLUDE "core.asm"
    INCLUDE "stdlib.asm"

    ; The CPU starts execution at segment $00000000, address $00001000, 
    ; so we'll put our code there.
    LABEL hw_start          $00001000

    ; The AUTO parameter lets the assembler auto-calculate the address.
    LABEL hw_string         AUTO
    LABEL hw_string_end     AUTO

    ; Startup routine
    hw_start:

    ; Set stack pointer. The back-tick (`)  is used as a number separator. 
    ; Underscore (_) and comma (,) may also be used as separators.
    LD $0000`2000 S.H0

    ; Set base pointer
    LD S.H0 S.H1

    ; The basic ABI is for function arguments to be placed, from left to 
    ; right, into the G, H, J, K, L, and M registers. Any additional parameters 
    ; are pushed onto the stack.

    ; Get string length.
    LD hw_string G          ; Load the local pointer (current segment) to the string into G register
    CALL stdlib_strlen      ; Call stdlib function
    LD A J                  ; Return value (string length) is in A. Copy this to J for the write call

    ; Write string

    ; The kernel also implements a subset of Linux syscalls. 
    ; The syscall number is placed into the A register, and the first 
    ; six syscall arguments are placed, from left to right, into the 
    ; G, H, J, K, L, and M registers. Any remaining arguments are  
    ; pushed onto the stack. 

    LD G H.H0               ; Load the local pointer (current segment) to the string into H.H0 register
    CLR H.H1                ; We're running in segment zero
    LD $01 A                ; Load syscall opcode $01 (write) into A register
    LD $01 G                ; Load file descriptor $01 (STDOUT) into G register
    INT $80                 ; Call interrupt $80 to execute write syscall

    ; "Power down" the system, which actually means to exit the Maize CPU loop and return to the host OS.

    LD $A9 A                ; Load syscall opcode $A9 (169, sys_reboot) into A register
    LD $4321FEDC J          ; Load "magic" code for power down ($4321FEDC) into J register
    INT $80

    ; This label points to the start of the string we want to output.
    hw_string: 
    STRING "Hello, world!\0"
