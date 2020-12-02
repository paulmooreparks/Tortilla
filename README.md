# The Maize Virtual CPU 
## Implemented on the Tortilla Emulation Platform

This project implements a virtual CPU (called "Maize") on a library that enables the creation of virtual CPUs (called "Tortilla"). 
See the file [Maize.txt](https://github.com/paulmooreparks/Tortilla/blob/master/Maize.txt) for more details on the CPU assembly 
language and the system that runs on it.

The goal is to create a "BIOS" layer above the virtual devices, a simple OS, a subset of Unix/Linux system calls (interrupt $80), 
and finally an implementation of a C/C++ compiler (likely Clang or GCC) that will compile and link to Maize machine code.

## Hello, World

Here is a simple "Hello, World!" application written in Maize assembly.

    INCLUDE "core.asm"

    ; The CPU starts execution at segment $00000000, address $00001000, 
    ; so we'll put our code there.
    LABEL _start            $00001000   

    ; The AUTO parameter lets the assembler auto-calculate the address.
    LABEL hello_world       AUTO
    LABEL hello_world_end   AUTO

    ; Startup routine
    _start:

    ; Set stack pointer. The comma is used as a number separator. 
    ; Underscore (_) and back-tick (`) may also be used as separators.
    LD $0000,2000 S.H0

    ; Set base pointer
    LD S.H0 S.H1

    ; Write string
    LD $01 A                ; Load syscall opcode $01 (write) into A register
    LD $01 G                ; Load file descriptor $01 (STDOUT) into G register
    LD hello_world H.H0     ; Load the pointer to the string into H.H0 register
    LD hello_world_end J    ; Load the pointer to byte past end of string into J register
    SUB hello_world J       ; Subtract pointer to start of string from pointer to end of string,
                            ; which gives the string length in J register
    INT $80                 ; Call interrupt $80 to execute write syscall

    ; "Power down" the system
    LD $A9 A                ; Load syscall opcode $A9 (169, sys_reboot) into A register
    LD $4321FEDC J          ; Load "magic" code for power down ($4321FEDC) into J register
    INT $80

    ; This label points to the start of the string we want to output.
    hello_world: 
    STRING "Hello, world!"
    ; This label automatically points to the the memory location just past the end of the 
    ; string above. It's used to calculate the string length.
    hello_world_end:
