LABEL startup     $0000_0080
LABEL idle        $0000_01A0
LABEL echo        $0000_0200

LABEL zero        AUTO
LABEL x           AUTO
LABEL sum         AUTO


; INT 0 (Reset)
$00000000: ADDRESS startup
; INT 1 (idle)
$00000004: ADDRESS idle
; INT 2 (keyboard)
$00000008: ADDRESS echo

; Idle routine is executed when the OS isn't doing any work. HALT stops the clock,
; and any interrupt will wake the CPU, jump to the interrupt handler, and return 
; to the JMP instruction, which goes right back to HALT.
idle:
HALT
JMP idle

; Echo routine
echo:
PUSH A
PUSH B
PUSH D
CLR B
IN $01 B
CLR A
LD $01 D.B0
LD $01 A.B0
LD B.B0 A.B2
OUT A D.B0
POP D
POP B
POP A
RET

; Startup routine
LABEL top         AUTO
LABEL done        AUTO

startup:

; Set up stack
LD $8000 S.H1
LD S.H1 S.H0  

; Clear screen
LD $01 D.B0   
CLR A         
OUT A D.B0    

LD $03 A.B0
CLR B
LD x C.B0

top:
ADD @C.B0 B.B0
INC C.B0
DEC A.B0
JNZ top

done:
ADD @zero B.B0
ST B.B0 @sum
JMP idle

zero:
STRING "0"

x:
DATA $02
DATA $04
DATA $03

sum:
DATA $00

