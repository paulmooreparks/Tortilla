INCLUDE "core.asm"

LABEL _start      AUTO
LABEL zero        AUTO
LABEL zerotext    AUTO
LABEL nonzerotext AUTO

; Startup routine

_start:

; Set up stack
LD $11 F.H1
LD $00002000,00002000 S

; Clear screen
CLR A         
OUT A $7F

CLR A         
LD $41 A.B0 ; Character to write
LD $85 A.B5 ; Count
LD $0A A.B1 ; Function to execute
INT $10     ; BIOS interrupt

LD $0001 A
INT $40 ; halt CPU and await interrupts

LD $02 A.B0   
CMP $02 A.B0
JZ zero

LABEL done AUTO

nonzero:
LD nonzerotext A.H0 
JMP done

zero:
LD zerotext A.H0 

done:
INT $03 ; output string
INT $40 ; halt CPU and await interrupts

zerotext: 
STRING "Equal\0"

nonzerotext:
STRING "Not equal\0"
