INCLUDE "core.asm"

; Startup routine
; The CPU starts execution at segment $00000000, address $00001000, 
; so we'll put our code there.
$00001000:

LD $0000`2000 S.H0
LD S.H0 S.H1

LD $01 A
LD $02 B
LD $03 C
LD $04 D
LD $05 E
LD $06 G
LD $07 H
LD $08 J
LD $09 K
LD $0A L
LD $0B M
LD $0C Z

CALL core_os_dump_registers

; "Power down" the system
LD $A9 A                ; Load syscall opcode $A9 (169, sys_reboot) into A register
LD $4321FEDC J          ; Load "magic" code for power down ($4321FEDC) into J register
INT $80

;;;;;;;;;; END ;;;;;;;;;;;;;

