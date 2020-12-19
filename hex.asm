INCLUDE "core.asm"

; The CPU starts execution at segment $00000000, address $00001000, 
; so we'll put our code there.
LABEL hex_start            $00001000   

; The AUTO parameter lets the assembler auto-calculate the address.
LABEL hex_string            AUTO
LABEL hex_print_b0          AUTO
LABEL hex_print_b1          AUTO
LABEL hex_print_b2          AUTO
LABEL hex_print_b3          AUTO
LABEL hex_print_b4          AUTO
LABEL hex_print_b5          AUTO
LABEL hex_print_b6          AUTO
LABEL hex_print_b7          AUTO
LABEL hex_print_q0          AUTO
LABEL hex_print_q1          AUTO
LABEL hex_print_q2          AUTO
LABEL hex_print_q3          AUTO
LABEL hex_print_h0          AUTO
LABEL hex_print_h1          AUTO
LABEL hex_print_w0          AUTO
LABEL hex_print_low_nybble  AUTO

; Startup routine
hex_start:

LD $0000,2000 S.H0
LD S.H0 S.H1

LD $012345674321FEDC G
CALL hex_print_w0
CALL hex_print_w0
CALL hex_print_w0
CALL hex_print_w0
CALL hex_print_w0
CALL hex_print_w0
CALL hex_print_w0
CALL hex_print_w0
CALL hex_print_w0
CALL hex_print_w0
CALL hex_print_w0
CALL hex_print_w0
CALL hex_print_w0
CALL hex_print_w0
CALL hex_print_w0
CALL hex_print_w0
CALL hex_print_w0
CALL hex_print_w0
CALL hex_print_w0
CALL hex_print_w0
CALL hex_print_w0
CALL hex_print_w0
CALL hex_print_w0
CALL hex_print_w0
CALL hex_print_w0
CALL hex_print_w0
CALL hex_print_w0
CALL hex_print_w0
CALL hex_print_w0
CALL hex_print_w0
CALL hex_print_w0
CALL hex_print_w0
CALL hex_print_w0
CALL hex_print_w0
CALL hex_print_w0
CALL hex_print_w0
CALL hex_print_w0
CALL hex_print_w0
CALL hex_print_w0
CALL hex_print_w0
CALL hex_print_w0
CALL hex_print_w0
CALL hex_print_w0
CALL hex_print_w0
CALL hex_print_w0
CALL hex_print_w0
CALL hex_print_w0
CALL hex_print_w0
CALL hex_print_w0
CALL hex_print_w0
CALL hex_print_w0
CALL hex_print_w0
CALL hex_print_w0
CALL hex_print_w0
CALL hex_print_w0
CALL hex_print_w0
CALL hex_print_w0
CALL hex_print_w0
CALL hex_print_w0
CALL hex_print_w0
CALL hex_print_w0
CALL hex_print_w0
CALL hex_print_w0
CALL hex_print_w0
CALL hex_print_w0
CALL hex_print_w0
CALL hex_print_w0
CALL hex_print_w0

; "Power down" the system
LD $A9 A                ; Load syscall opcode $A9 (169, sys_reboot) into A register
LD $4321FEDC J          ; Load "magic" code for power down ($4321FEDC) into J register
INT $80

hex_print_w0:
CALL hex_print_h1
CALL hex_print_h0
RET

hex_print_h1:
CALL hex_print_q3
CALL hex_print_q2
RET

hex_print_h0:
CALL hex_print_q1
CALL hex_print_q0
RET

hex_print_q3:
CALL hex_print_b7
CALL hex_print_b6
RET

hex_print_q2:
CALL hex_print_b5
CALL hex_print_b4
RET

hex_print_q1:
CALL hex_print_b3
CALL hex_print_b2
RET

hex_print_q0:
CALL hex_print_b1
CALL hex_print_b0
RET

hex_print_b7:
PUSH G.B0
LD G.B7 G.B0
CALL hex_print_b0
POP G.B0
RET

hex_print_b6:
PUSH G.B0
LD G.B6 G.B0
CALL hex_print_b0
POP G.B0
RET

hex_print_b5:
PUSH G.B0
LD G.B5 G.B0
CALL hex_print_b0
POP G.B0
RET

hex_print_b4:
PUSH G.B0
LD G.B4 G.B0
CALL hex_print_b0
POP G.B0
RET

hex_print_b3:
PUSH G.B0
LD G.B3 G.B0
CALL hex_print_b0
POP G.B0
RET

hex_print_b2:
PUSH G.B0
LD G.B2 G.B0
CALL hex_print_b0
POP G.B0
RET

hex_print_b1:
PUSH G.B0
LD G.B1 G.B0
CALL hex_print_b0
POP G.B0
RET

; Convert byte
hex_print_b0:
PUSH G.B0
SHR $04 G.B0
CALL hex_print_low_nybble
POP G.B0
AND $0F G.B0
CALL hex_print_low_nybble
RET

hex_print_low_nybble:
PUSH G
PUSH B
LD G.B0 B.B0
AND $0F B.B0

LD $01 A                ; Load syscall opcode $01 (write) into A register
LD $01 G                ; Load file descriptor $01 (STDOUT) into G register

LD hex_string H.H0      ; Load the pointer to the string into H.H0 register
ADD B.B0 H.H0

LD $0001 J              ; Length of 1
INT $80                 ; Call interrupt $80 to execute write syscall
POP B
POP G
RET

; The output functions index into this string to perform the output
hex_string: 
STRING "0123456789ABCDEF"
