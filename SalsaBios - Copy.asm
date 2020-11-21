LABEL startup  $00000080
LABEL name     $00000100
LABEL idle     $000001A0
LABEL echo     $00000200
LABEL output   AUTO

; INT 0 (Reset)
$00000000: ADDRESS $00000080                             ; 80 00 00 00
; INT 1 (keyboard)
$00000004: ADDRESS $00000200                             ; 00 20 00 00 

; Startup routine
startup:
LD $1000 S.H1                                         ; 41 01 6D 00 10 
LD S.H1 S.H0                                          ; 01 6D 6C 
LD $0100 B.H0                                         ; 41 01 1C 00 01 
LD $01 D.B0                                           ; 41 00 30 01
CLR A                                                 ; 22 00 
OUT A D.B0                                            ; 1E 0E 30
LD $01 A.B0                                           ; 41 00 00 01
LD @B.H0 C                                            ; 81 1C 2E 
CALL OUTPUT                                           ; 5D 01 XX XX
ADD $08 B.H0                                          ; 43 00 1C 08 
LD @B.H0 C                                            ; 81 1C 2E 
CALL OUTPUT                                           ; 5D 01 FF FF
ADD $08 B.H0                                          ; 43 00 1C 08 
LD @B.H0 C                                            ; 81 1C 2E 
CALL OUTPUT                                           ; 5D 01 XX XX
LD $03 A.B0                                           ; 41 00 00 03
LD $41 A.B2                                           ; 41 00 02 3E
LD $00 A.B4                                           ; 41 00 04 00
LD $05 A.B5                                           ; 41 00 05 05
OUT A D.B0                                            ; 1E 0E 30
JMP $01A0                                             ; 56 01 A0 01 
HALT                                                  ; 00

output:
LD C.Q0 A.Q1                                          ; 01 28 09
OUT A D.B0                                            ; 1E 0E 30
LD C.Q1 A.Q1                                          ; 01 29 09
OUT A D.B0                                            ; 1E 0E 30
LD C.Q2 A.Q1                                          ; 01 2A 09
OUT A D.B0                                            ; 1E 0E 30
LD C.Q3 A.Q1                                          ; 01 2B 09
OUT A D.B0                                            ; 1E 0E 30
RET                                                   ; 27

name: 
DATA $50 $0E $61 $0E $64 $0E $6D $0E $61 $0E $20 $07  ; 50 0E 61 0E 64 0E 6D 0E 61 0E 20 07 
DATA $4A $0B $61 $0B $69 $0B $72 $0B $61 $0B $6D $0B  ; 4A 0B 61 0B 69 0B 72 0B 61 0B 6D 0B

; Idle routine
idle:
HALT                                                  ; 00
JMP $000001A0                                         ; 56 02 A0 01 00 00 

; Echo routine
echo:
CLR B                                                 ; 22 1E
IN $01 B                                              ; 5F 00 1E 01
CLR A                                                 ; 22 0E 
LD $01 D.B0                                           ; 41 00 30 01
LD $01 A.B0                                           ; 41 00 00 01
LD B.B0 A.B2                                          ; 01 10 02 
OUT A D.B0                                            ; 1E 0E 30
RET                                                   ; 27












// Old function

PUSH S.H1                                             ; 20 6D 
LD S.H0 S.H1                                          ; 01 6C 6D
PUSH C                                                ; 20 2E
PUSH D.H0                                             ; 20 3C
LD S.H1 D.H0                                          ; 01 6D 3C
ADD $08 D.H0                                          ; 43 00 3C 0C
LD @D.H0 C.Q2                                         ; 81 3C 2A
OUT C B.Q1                                            ; 1E 2E 19
POP D.H0                                              ; 26 3C
POP C                                                 ; 26 2E
LD S.H1 S.H0                                          ; 01 6D 6C
POP S.H1                                              ; 26 6D
RET                                                   ; 27





NOP                                                   ; AA 
LD @B.H0 A                                            ; 81 1C 0E 
STI A.Q0 @B.H1                                        ; 13 08 9D 
INC B.H1                                              ; 23 1D
INC B.H1                                              ; 23 1D
STI A.Q1 @B.H1                                        ; 13 09 9D 
ST A.Q2 @$0084                                        ; 02 0A 81 84 00 
ST A.Q3 @$0086                                        ; 02 0B 81 86 00 
ADD $08 B.H0                                          ; 43 00 1C 08 
LD @B.H0 A                                            ; 81 1C 0E 
ST A.Q0 @$0088                                        ; 02 08 81 88 00 
ST A.Q1 @$008A                                        ; 02 09 81 8A 00 
ST A.Q2 @$008C                                        ; 02 0A 81 8C 00 
ST A.Q3 @$008E                                        ; 02 0B 81 8E 00 
ADD $08 B.H0                                          ; 43 00 1C 08 
LD @B.H0 A                                            ; 81 1C 0E 
ST A.Q0 @$0090                                        ; 02 08 81 90 00 
ST A.Q1 @$0092                                        ; 02 09 81 92 00 
ST A.Q2 @$0094                                        ; 02 0A 81 94 00 
ST A.Q3 @$0096                                        ; 02 0B 81 96 00 
HALT                                                  ; 00 
LD @B.H0 A                                            ; 81 1C 0E 
ST A.Q0 @$0090                                        ; 02 08 81 90 00 
ST A.Q1 @$0092                                        ; 02 09 81 92 00 
ST A.Q2 @$0094                                        ; 02 0A 81 94 00 
ST A.Q3 @$0096                                        ; 02 0B 81 96 00 
RET                                                   ; 27

$00000100: DATA $50 $0E $61 $0E $64 $0E $6D $0E $61 $0E $20 $07 $4A $0B $61 $0B ; 50 0E 61 0E 64 0E 6D 0E 61 0E 20 07 4A 0B 61 0B 
$00000110: DATA $69 $0B $72 $0B $61 $0B $6D $0B                         ; 69 0B 72 0B 61 0B 6D 0B


// Other stuff

LD @$0100 A          ; C1 81 0E 00 01 
ADD $02 A.Q1         ; 43 00 09 02
LD A B               ; 01 0E 1E

