00000000  00  HLT
10000000  80  INT 0
10100000  A0  INT 1
11000000  C0  INT 2
11100000  E0  INT 3
00000001  01  CPY sreg/[sreg]/imm/[imm] -> dreg
10000001  81  CPY imm08b val -> A.B0
10100001  A1  CPY imm16b val -> A.Q0
11000001  C1  CPY imm32b val -> A.H0
11100001  E1  CPY imm64b val -> A
00000010  02  ST sreg -> [dreg]/[imm]
10000010  82  ST A.B0 -> [imm08b]
10100010  A2  ST A.Q0 -> [imm16b]
11000010  C2  ST A.H0 -> [imm32b]
11100010  E2  ST A -> [imm64b]
00000011  03  ADD sreg/[sreg]/imm/[imm] + dreg -> dreg
10000011  83  ADD 08b val + A.B0 -> A.B0
10100011  A3  ADD 16b val + A.Q0 -> A.Q0
11000011  C3  ADD 32b val + A.H0 -> A.H0
11100011  E3  ADD 64b + A -> A
00000100  04  SUB op1 - op2 -> op1
10000100  84  SUB A.B0 - 08b -> A.B0
10100100  A4  SUB A.Q0 - 16b -> A.Q0
11000100  C4  SUB A.H0 - 32b -> A.H0
11100100  E4  SUB A - 64b -> A
00000101  05  MUL op1 * op2 -> op1
10000101  85  MUL A.B0 * 08b -> A.B0
10100101  A5  MUL A.Q0 * 16b -> A.Q0
11000101  C5  MUL A.H0 * 32b -> A.H0
11100101  E5  MUL A * 64b -> A
00000110  06  DIV op1 / op2 -> op1
10000110  86  DIV A.B0 / 08b -> A.B0
10100110  A6  DIV A.Q0 / 16b -> A.Q0
11000110  C6  DIV A.H0 / 32b -> A.H0
11100110  E6  DIV A / 64b -> A
00000111  07  MOD op1 % op2 -> op1
10000111  07  MOD A.B0 % 08b -> A.B0
10100111  A7  MOD A.Q0 % 16b -> A.Q0
11000111  C7  MOD A.H0 % 32b -> A.H0
11100111  E7  MOD A % 64b -> A
00001000  08  INC reg
10001000  88  INT byte
10101000  A8  res 
11001000  C8  res 
11101000  E8  res 
00001001  09  DEC reg
10001001  89  res 
10101001  A9  res 
11001001  C9  res 
11101001  E9  res 
00001010  0A  AND op1 & op2 -> op1
10001010  8A  AND A.B0 & 08b -> A.B0
10101010  AA  AND A.Q0 & 16b -> A.Q0
11001010  CA  AND A.H0 & 32b -> A.H0
11101010  EA  AND A & 64b -> A
00001011  0B  OR op1 | op2 -> op1
10001011  8B  OR A.B0 | 08b -> A.B0
10101011  AB  OR A.Q0 | 16b -> A.Q0
11001011  CB  OR A.H0 | 32b -> A.H0
11101011  EB  OR A | 64b -> A
00001100  0C  NOR op1 ~| op2 -> op1
10001100  8C  NOR A.B0 ~| 08b -> A.B0
10101100  AC  NOR A.Q0 ~| 16b -> A.Q0
11001100  CC  NOR A.H0 ~| 32b -> A.H0
11101100  EC  NOR A - 64b -> A
00001101  0D  NOT op1 ~ op2 -> op1
10001101  8D  NOT A.B0 ~ 08b -> A.B0
10101101  AD  NOT A.Q0 ~ 16b -> A.Q0
11001101  CD  NOT A.H0 ~ 32b -> A.H0
11101101  ED  NOT A ~ 64b -> A
00001110  0E  NAND op1 ~& op2 -> op1
10001110  8E  NAND A.B0 ~& 08b -> A.B0
10101110  AE  NAND A.Q0 ~& 16b -> A.Q0
11001110  CE  NAND A.H0 ~& 32b -> A.H0
11101110  EE  NAND A ~& 64b -> A
00001111  0F  XOR op1 ^ op2 -> op1
10001111  8F  XOR A.B0 ^ 08b -> A.B0
10101111  AF  XOR A.Q0 ^ 16b -> A.Q0
11001111  CF  XOR A.H0 ^ 32b -> A.H0
11101111  EF  XOR A ^ 64b -> A
00010000  10  SHL op1 << op2 -> op1
10010000  90  SHL A.B0 << 08b -> A.B0
10110000  B0  SHL A.Q0 << 16b -> A.Q0
11010000  D0  SHL A.H0 << 32b -> A.H0
11110000  F0  SHL A << 64b -> A
00010001  11  SHR op1 >> op2 -> op1
10010001  91  SHR A.B0 >> 08b -> A.B0
10110001  B1  SHR A.Q0 >> 16b -> A.Q0
11010001  D1  SHR A.H0 >> 32b -> A.H0
11110001  F1  SHR A >> 64b -> A
00010010  12  CMP op1, op2
10010010  92  CMP A.B0, op2
10110010  B2  CMP A.Q0, op2
11010010  D2  CMP A.H0, op2
11110010  F2  CMP A, op2
00010011  13  JMP CS:op1
10010011  93  reserved
10110011  B3  reserved
11010011  D3  JMP CS:A.H0
11110011  F3  JMP CS:32b
00010100  14  JL CS:op1
10010100  94  reserved
10110100  B4  reserved
11010100  D4  JL CS:A.H0
11110100  F4  JL CS:32b
00010101  15  JLE CS:op1
10010101  95  reserved
10110101  B5  reserved
11010101  D5  JLE CS:A.H0
11110101  F5  JLE CS:32b
00010110  16  JG CS:op1
10010110  96  reserved
10110110  B6  reserved
11010110  D6  JG CS:A.H0
11110110  F6  JG CS:32b
00010111  17  JGE CS:op1
10010111  97  reserved
10110111  B7  reserved
11010111  D7  JGE CS:A.H0
11110111  F7  JGE CS:32b
00011000  18  JZ CS:op1
10011000  98  reserved
10111000  B8  reserved
11011000  D8  JZ CS:A.H0
11111000  F8  JZ CS:32b
00011001  19  JNZ CS:op1
10011001  99  NOP
10111001  B9  reserved
11011001  D9  JNZ CS:A.H0
11111001  F9  JNZ CS:32b
00011010  1A  OUT op1
10011010  9A  reserved
10111010  BA  reserved
11011010  DA  OUT A.H0
11111010  FA  OUT 32b
00011011  1B  IN op1
10011011  9B  reserved
10111011  BB  reserved
11011011  DB  IN A.H0
11111011  FB  IN 32b
00011100  1C  PUSH op1
10011100  9C  PUSH A.B0
10111100  BC  PUSH A.Q0
11011100  DC  PUSH A.H0
11111100  FC  PUSH A
00011101  1D  POP op1
10011101  9D  POP A.B0
10111101  BD  POP A.Q0
11011101  DD  POP A.H0
11111101  FD  POP A
00011110  1E  LEA op1, op2
10011110  9E  reserved
10111110  BE  reserved
11011110  DE  reserved
11111110  FE  reserved
00011111  1F  CALL CS:op1
10011111  9F  reserved
10111111  BF  reserved
11011111  DF  CALL CS:A.H0
11111111  FF  CALL 32b

              
              

Operand 1
100nnnnn             immediate 8-bit value/address as operand (1 byte) 
101nnnnn             immediate 16-bit value/address as operand (2 bytes) 
110nnnnn             immediate 32-bit value/address as operand (4 bytes) 
111nnnnn             immediate 64-bit value/address as operand (8 bytes) 
000nnnnn  0000`000x  immediate 8-bit value/address (1 byte) 
000nnnnn  0000`001x  immediate 16-bit value/address (2 bytes) 
000nnnnn  0000`010x  immediate 32-bit value/address (4 bytes) 
000nnnnn  0000`011x  immediate 64-bit value/address (8 bytes) 
000nnnnn  0000`100x  RESERVED
000nnnnn  0000`101x  RESERVED
000nnnnn  0000`110x  RESERVED
000nnnnn  0000`111x  RESERVED
000nnnnn  0001`FFFx  r
000nnnnn  0010`FFFx  r.H0
000nnnnn  0011`FFFx  r.H1
000nnnnn  0100`FFFx  r.Q0
000nnnnn  0101`FFFx  r.Q1
000nnnnn  0110`FFFx  r.Q2
000nnnnn  0111`FFFx  r.Q3
000nnnnn  1000`FFFx  r.B0
000nnnnn  1001`FFFx  r.B1
000nnnnn  1010`FFFx  r.B2
000nnnnn  1011`FFFx  r.B3
000nnnnn  1100`FFFx  r.B4
000nnnnn  1101`FFFx  r.B5
000nnnnn  1110`FFFx  r.B6
000nnnnn  1111`FFFx  r.B7
000nnnnn  xxxx`xxx0  value flag
000nnnnn  xxxx`xxx1  address flag

Operand 2
000nnnnn  0000`000x  immediate 8-bit value/pointer (1 byte) 
000nnnnn  0000`001x  immediate 16-bit value/pointer (2 bytes) 
000nnnnn  0000`010x  immediate 32-bit value/pointer (4 bytes) 
000nnnnn  0000`011x  immediate 64-bit value/pointer (8 bytes) 
000nnnnn  0000`100x  RESERVED
000nnnnn  0000`101x  RESERVED
000nnnnn  0000`110x  RESERVED
000nnnnn  0000`111x  RESERVED
000nnnnn  0001`FFFx  r
000nnnnn  0010`FFFx  r.H0
000nnnnn  0011`FFFx  r.H1
000nnnnn  0100`FFFx  r.Q0
000nnnnn  0101`FFFx  r.Q1
000nnnnn  0110`FFFx  r.Q2
000nnnnn  0111`FFFx  r.Q3
000nnnnn  1000`FFFx  r.B0
000nnnnn  1001`FFFx  r.B1
000nnnnn  1010`FFFx  r.B2
000nnnnn  1011`FFFx  r.B3
000nnnnn  1100`FFFx  r.B4
000nnnnn  1101`FFFx  r.B5
000nnnnn  1110`FFFx  r.B6
000nnnnn  1111`FFFx  r.B7
000nnnnn  xxxx`xxx0  value flag
000nnnnn  xxxx`xxx1  address flag


Register flags in operand
FFFF`000x 000  A
FFFF`001x 001  B
FFFF`010x 010  C
FFFF`011x 011  D
FFFF`100x 100  E
FFFF`101x 101  I
FFFF`110x 110  S
FFFF`111x 111  F



CPY
Load register from register, value, or memory location

<CPY>     ::= <opcode> <src> <dest> <imm>
<opcode  ::= 00000001 
<src>    ::= 

CPY sreg/[reg]/imm/[imm], dreg
01 src08b dest08b [imm08b|imm16b|imm32b|imm64b]
00000001 xxxx`xxxx xxxx`xxxx

81 01 CPY $01, A.B0
10000001 xxxx`xxxx

A1 01 02 CPY $0201, A.Q0
10100001 xxxx`xxxx xxxx`xxxx

C1 01 02 03 04 CPY $04030201, A.H0
11000001 xxxx`xxxx xxxx`xxxx xxxx`xxxx xxxx`xxxx

E1 01 02 03 04 05 06 07 08 CPY $0807060504030201, A
11100001 xxxx`xxxx xxxx`xxxx xxxx`xxxx xxxx`xxxx xxxx`xxxx xxxx`xxxx xxxx`xxxx xxxx`xxxx




ST
Store register or value to memory

ST sreg, [reg]|[imm08b]|[imm16b]|[imm32b]|[imm64b]
01 src08b dest08b (imm08b|imm16b|imm32b|imm64b)
00000010 xxxx`xxxx xxxx`xxxx

ST A.B0, [imm08b]
82 xx 
10000010 xxxx`xxxx

ST A.Q0, [imm16b]
A2 xx xx 
10100010 xxxx`xxxx`xxxx`xxxx

ST A.H0, [imm32b]
C2 xx xx xx xx 
11000010 xxxx`xxxx`xxxx`xxxx`xxxx`xxxx`xxxx`xxxx

ST A, [imm64b]
E2 xx xx xx xx xx xx xx xx
11100010 xxxx`xxxx`xxxx`xxxx`xxxx`xxxx`xxxx`xxxx`xxxx`xxxx`xxxx`xxxx`xxxx`xxxx`xxxx`xxxx


ADD
Add reg2 or imm to reg1, store in reg1

ADD sreg/val + dreg -> dreg
03 xx xx ADD src, dest
00000011 xxxx`xxxx`xxxx`xxxx

ADD A.B0, (embedded 8-bit operand as memory location)
83 ADD 08b val + A.B0 -> A.B0
83 xx 
10000011 xxxx`xxxx

ADD A.Q0, (embedded 16-bit operand)
A3 ADD 16b val + A.Q0 -> A.Q0
A3 xx xx 
10100011 xxxx`xxxx`xxxx`xxxx

ADD A.H0, (embedded 32-bit operand)
C3 ADD 32b val + A.H0 -> A.H0
C3 xx xx xx xx 
11000011 xxxx`xxxx`xxxx`xxxx`xxxx`xxxx`xxxx`xxxx

ADD A, (embedded 64-bit operand)
E3 ADD 64b + A -> A
E3 xx xx xx xx xx xx xx xx
11100011 xxxx`xxxx`xxxx`xxxx`xxxx`xxxx`xxxx`xxxx`xxxx`xxxx`xxxx`xxxx`xxxx`xxxx`xxxx`xxxx



;-------------------------------------------
; Current

0000001E  E1 50 07 61 07 64 07 6D 07   CPY $076D076407610750, A ; "Padm"
0000002A  A2 80 00                     ST A.Q0, [$0080]
00000027  02 50 05 82 00               ST A.Q1, [$0082]
00000024  02 60 05 84 00               ST A.Q2, [$0084]
00000021  02 70 05 86 00               ST A.Q3, [$0086]
0000001E  E1 61 07 20 07 4A 07 61 07   CPY $0761074A07200761, A ; "a Ja"
0000002A  A2 88 00                     ST A.Q0, [$0088]
00000027  02 50 05 8A 00               ST A.Q1, [$008A]
00000024  02 60 05 8C 00               ST A.Q2, [$008C]
00000021  02 70 05 8E 00               ST A.Q3, [$008E]
0000001E  E1 69 07 72 07 61 07 6D 07   CPY $076D076107720769, A ; "iram"
0000002A  A2 90 00                     ST A.Q0, [$0090]
00000027  02 50 05 92 00               ST A.Q1, [$0092]
00000024  02 60 05 94 00               ST A.Q2, [$0094]
00000021  02 70 05 96 00               ST A.Q3, [$0096]


;-------------------------------------------



00000000  81 AA                        CPY $AA, A.B0
          82 60                        ST A.B0, [$60]
00000002  A1 BB CC                     CPY $BBCC, A.Q0
          A2 62 00                     ST A.Q0, [$0062]
00000005  C1 DD EE FF 11               CPY $DDEEFF11, A.H0
          C2 68 00 00 00               ST A.Q0, [$00000068]
0000000A  E1 22 33 44 55 66 77 88 99   CPY $2233445566778899, A
          E2 70 00 00 00 00 00 00 00   ST A.Q0, [$0000000000000070]


; Test, not in use, etc.

00000000 0C CC    CPY $CC, A.B0
00000002 14 51    ST A.B0, [$51]


00000000  81 50     CPY $50, A.B0
00000002  82 80     ST A.B0, [$80]
00000004  81 07     CPY $07, A.B0
00000006  82 81     ST A.B0, [$81]
00000008  81 61     CPY $61, A.B0
0000000A  82 82     ST A.B0, [$82]
0000000C  81 07     CPY $07, A.B0
0000000E  82 83     ST A.B0, [$83]
00000010  81 64     CPY $64, A.B0
00000012  82 84     ST A.B0, [$84]
00000014  81 07     CPY $07, A.B0
00000016  82 85     ST A.B0, [$85]
00000018  81 6D     CPY $6D, A.B0
0000001A  82 86     ST A.B0, [$86]
0000001C  81 07     CPY $07, A.B0
0000001E  82 87     ST A.B0, [$87]
00000020  81 61     CPY $61, A.B0
00000022  82 88     ST A.B0, [$88]
00000024  81 07     CPY $07, A.B0
00000026  82 89     ST A.B0, [$89]

00000000  A1 50 07    CPY $0750, A.Q0
00000003  A2 80 00    ST A.Q0, [$0080]
00000006  A1 61 07    CPY $0761, A.Q0
00000009  A2 82 00    ST A.Q0, [$0082]
0000000C  A1 64 07    CPY $0764, A.Q0
0000000F  A2 84 00    ST A.Q0, [$0084]
00000013  A1 6D 07    CPY $076D, A.Q0
00000016  A2 86 00    ST A.Q0, [$0086]
00000019  A1 61 07    CPY $0761, A.Q0
0000001C  A2 88 00    ST A.Q0, [$0088]

00000000  E1 50 07 61 07 64 07 6D 07   CPY $076D076407610750, A ; "Padm"
00000009  A2 80 00                     ST A.Q0, [$0080]
0000000C  02 50 05 82 00               ST A.Q1, [$0082]
00000011  02 60 05 84 00               ST A.Q2, [$0084]
00000016  02 70 05 86 00               ST A.Q3, [$0086]
0000001B  A1 61 07                     CPY $0761, A.Q0
0000001E  A2 88 00                     ST A.Q0, [$0088]
