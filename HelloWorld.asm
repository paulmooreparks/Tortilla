INCLUDE "core.asm"

LABEL _start            AUTO
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
CLR A                   ; Clear the A register (set to 0)
LD $01 A                ; Load syscall opcode 1 (write) into A register
LD $01 G                ; Load file descriptor 1 (STDOUT) into G register
LD hello_world H.H0     ; Load the pointer to the string into H.H0 register
LD hello_world_end J    ; Load the pointer to byte past end of string into J register
SUB hello_world J       ; Subtract pointer to start of string from pointer to end of string,
                        ; which gives the string length in J register
INT $80                 ; Call interrupt $80 to execute write syscall

; "Power down" the system
; A.Q0 = $0000 -> shut down
CLR A
INT $40

hello_world: 
STRING "Hello, world!"
hello_world_end:
