INCLUDE "core.asm"

LABEL _start     AUTO
LABEL name        AUTO

; Startup routine
_start:
LD $0000,2000,0000,2000 S
CLR A         
OUT A $01
LD $02 A.B0   
LD name B.H0 
LD @B.H0 C    
CALL output   
ADD $08 B.H0  
LD @B.H0 C    
CALL output   
ADD $08 B.H0  
LD @B.H0 C    
CALL output   
LD $03 A.B0   
LD $3E A.B2   
LD $00 A.B4   
LD $05 A.B5   
OUT A $01    
INT $01 ; We're done doing setup, so go halt the CPU and wait for interrupts

name: 
DATA $50 $0E $61 $0E $64 $0E $6D $0E $61 $0E $20 $07 ; "Padma" in yellow ($0E)
DATA $4A $0B $61 $0B $69 $0B $72 $0B $61 $0B $6D $0B ; "Jairam" in cyan ($0B)

