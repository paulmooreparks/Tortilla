00000000: LD $00FF, A                                        ; 01 02 10 FF 00 
00000005: ADD $0002, A.Q0                                      ; A3 02 00
00000008: LD $0100, B                                        ; 01 02 12 00 01 
0000000D: LD [B], A                                          ; 01 13 10 
00000010: ST A.Q0, [$0080]                                     ; 02 40 03 80 00 
00000015: ST A.Q1, [$0082]                                     ; 02 50 03 82 00 
0000001A: ST A.Q2, [$0084]                                     ; 02 60 03 84 00 
0000001F: ST A.Q3, [$0086]                                     ; 02 70 03 86 00 
00000024: ADD $0008, B                                         ; 03 02 12 08 00 
00000029: LD [B], A                                          ; 01 13 10 
0000002C: ST A.Q0, [$0088]                                     ; A2 88 00
0000002F: ST A.Q1, [$008A]                                     ; 02 50 03 8A 00 
00000034: ST A.Q2, [$008C]                                     ; 02 60 03 8C 00 
00000039: ST A.Q3, [$008E]                                     ; 02 70 03 8E 00 
0000003E: ADD $0008, B                                         ; 03 02 12 08 00 
00000043: LD [B], A                                          ; 01 13 10 
00000046: ST A.Q0, [$0090]                                     ; A2 90 00
00000049: ST A.Q1, [$0092]                                     ; 02 50 03 92 00 
0000004E: ST A.Q2, [$0094]                                     ; 02 60 03 94 00 
00000053: ST A.Q3, [$0096]                                     ; 02 70 03 96 00 
00000058: HALT                                                 ; 00 
00000100: DATA 50 0E 61 0E 64 0E 6D 0E 61 0E 20 07 4A 0B 61 0B ; 50 0E 61 0E 64 0E 6D 0E 61 0E 20 07 4A 0B 61 0B 
00000110: DATA 69 0B 72 0B 61 0B 6D 0B                         ; 69 0B 72 0B 61 0B 6D 0B

// Old code

LD $009E, A               ; 01 02 10 9E 00
ADD $0002, A.Q0            ; A3 02 00

; "Padm"
LD $0E6D0E640E610E50, A    ; E1 50 0E 61 0E 64 0E 6D 0E
ST A.Q0, [$0080]           ; A2 80 00
ST A.Q1, [$0082]           ; 02 50 03 82 00
ST A.Q2, [$0084]           ; 02 60 03 84 00
ST A.Q3, [$0086]           ; 02 70 03 86 00

; "a Ja"
LD $0B610B4A07200E61, A    ; E1 61 0E 20 07 4A 0B 61 0B
ST A.Q0, [$0088]           ; A2 88 00
ST A.Q1, [$008A]           ; 02 50 03 8A 00
ST A.Q2, [$008C]           ; 02 60 03 8C 00
ST A.Q3, [$008E]           ; 02 70 03 8E 00

; "iram"
LD $0B6D0B610B720B69, A    ; E1 69 0B 72 0B 61 0B 6D 0B
ST A.Q0, [$0090]           ; A2 90 00
ST A.Q1, [$0092]           ; 02 50 03 92 00
ST A.Q2, [$0094]           ; 02 60 03 94 00
ST A.Q3, [$0096]           ; 02 70 03 96 00

