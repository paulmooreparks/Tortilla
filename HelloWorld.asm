INCLUDE "core.asm"

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

; The bare-bones kernel provides a set of OS API functions via interrupt $40. 
; The function ID is placed into the A register, and function arguments are 
; placed, from left to right, into the G, H, J, K, L, and M registers. Any 
; additional parameters are pushed onto the stack.

; Get string length.
LD $0003 A              ; OS call for strlen
LD hw_string G          ; Load the local pointer (current segment) to the string into G register
INT $40                 ; Call OS function
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

; "Power down" the system
LD $A9 A                ; Load syscall opcode $A9 (169, sys_reboot) into A register
LD $4321FEDC J          ; Load "magic" code for power down ($4321FEDC) into J register
INT $80

; This label points to the start of the string we want to output.
hw_string: 
STRING "Hello, world!\0"
