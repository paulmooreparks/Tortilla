INCLUDE "core.asm"

LABEL _start            $00001000
LABEL output            AUTO
LABEL first_name        AUTO
LABEL first_name_end    AUTO
LABEL surname           AUTO
LABEL surname_end       AUTO
LABEL color_1           AUTO
LABEL color_2           AUTO

; Startup routine
_start:
LD $0000,2000,0000,2000 S

; Set background color
CLR A
LD $0B A.B1
LD $00 A.B3
LD $00 A.B2 ; Black
INT $10

; Clear screen
CLR A
LD $04 A.B1
INT $10

; Set foreground color
CLR A
LD $0B A.B1
LD $01 A.B3
LD @color_1 A.B2
INT $10

; Write first name
CLR A
LD $01 A
LD $01 G
LD first_name H.H0 
LD first_name_end J
SUB first_name J
INT $80

; Change foreground color
CLR A
LD $0B A.B1
LD $01 A.B3
LD @color_2 A.B2 
INT $10

; Write surname
CLR A
LD $01 A
LD $01 G
LD surname H.H0 
LD surname_end J
SUB surname J
INT $80

; Set foreground color
CLR A
LD $0B A.B1
LD $01 A.B3
LD $07 A.B2 ; Gray
INT $10

; Move cursor
CLR A
LD $02 A.B1   
LD $00 A.B6   
LD $05 A.B7   
INT $10

; Write '>' character
CLR A
LD $0A A.B1   
LD $3E A.B0 ; '>'
INT $10

; We're done doing setup, so go halt the CPU and wait for interrupts
LD $01 A
; A.Q0 = $00 -> yield idle time to OS
INT $40

color_1:
DATA $0E ; Yellow

color_2:
DATA $0B ; Cyan

first_name: 
STRING "Padma "
first_name_end:

surname:
STRING "Jairam"
surname_end:

