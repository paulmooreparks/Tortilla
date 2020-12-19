INCLUDE "core.asm"

; The CPU starts execution at segment $00000000, address $00001000, 
; so we'll put our code there.
LABEL hw_start            $00001000   

; The AUTO parameter lets the assembler auto-calculate the address.
LABEL hw_string         AUTO
LABEL hw_string_end     AUTO

; Startup routine
hw_start:

; Set stack pointer. The comma is used as a number separator. 
; Underscore (_) and back-tick (`) may also be used as separators.
LD $0000,2000 S.H0

; Set base pointer
LD S.H0 S.H1

; Write string
LD $01 A                ; Load syscall opcode $01 (write) into A register
LD $01 G                ; Load file descriptor $01 (STDOUT) into G register
LD hw_string H.H0       ; Load the pointer to the string into H.H0 register
LD hw_string_end J      ; Load the pointer to byte past end of string into J register
SUB hw_string J         ; Subtract pointer to start of string from pointer to end of string,
                        ; which gives the string length in J register
INT $80                 ; Call interrupt $80 to execute write syscall

; "Power down" the system
LD $A9 A                ; Load syscall opcode $A9 (169, sys_reboot) into A register
LD $4321FEDC J          ; Load "magic" code for power down ($4321FEDC) into J register
INT $80

; This label points to the start of the string we want to output.
hw_string: 
STRING "H" ; ello, world!
; This label automatically points to the the memory location just past the end of the 
; string above. It's used to calculate the string length.
hw_string_end:
