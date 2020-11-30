INCLUDE "core.asm"

LABEL _start            AUTO
LABEL hello_world       AUTO
LABEL hello_world_end   AUTO

; Startup routine
_start:
LD $0000,2000,0000,2000 S

; Write string
CLR A
LD $01 A
LD $01 G
LD hello_world H.H0 
LD hello_world_end J
SUB hello_world J
INT $80

; "Power down" the system
CLR A
; A.Q0 = $0000 -> shut down
INT $40

hello_world: 
STRING "Hello, world!"
hello_world_end:

